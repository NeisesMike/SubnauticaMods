/*
using SMLHelper.V2.Handlers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UWE;

namespace SlotExtenderZero.API
{
    public class SeaTruckHelper
    {
        private class SeaTruckSlotListener : MonoBehaviour
        {            
            public SeaTruckHelper helper = null;

            private void Update()
            {
                if (helper == null || !helper.IsPiloted())
                    return;

                helper.ActiveSlot = helper.GetActiveSlotID();
            }
        }

        private class SeaTruckDockingListener : MonoBehaviour
        {
            public SeaTruckHelper helper = null;

            private void Update()
            {
                if (helper == null || !helper.IsPiloted())
                {
                    return;
                }

                helper.IsDocked = helper.dockable.isDocked;
            }
        }

        private class SeaTruckDamageListener : MonoBehaviour
        {
            public SeaTruckHelper helper = null;

            private void Update()
            {
                if (helper == null || !helper.IsPiloted())
                    return;

                helper.Damage = helper.damageInfo.damage;
            }
        }

        private class SeaTruckPilotingListener : MonoBehaviour
        {
            public SeaTruckHelper helper = null;

            public void OnPilotBegin()
            {
                helper.onPilotingBegin?.Invoke();
            }

            public void OnPilotEnd()
            {
                helper.onPilotingEnd?.Invoke();
            }
        }

        public GameObject MainCab { get; private set; }

        public SeaTruckAnimation animation;
        public SeaTruckAquarium aquarium;
        public SeaTruckConnection connection;
        public SeaTruckDockingBay dockingBay;
        public SeaTruckEffects effects;
        public SeaTruckLights lights;
        public SeaTruckSegment segment;
        public SeaTruckConnectingDoor connectingDoor;
        public SeaTruckMotor motor;
        public SeaTruckTeleporter teleporter;
        public PingInstance pingInstance;
        public Dockable dockable;
        public ColorNameControl colorNameControl;
        public LiveMixin thisLiveMixin;
        private DamageInfo damageInfo;
        public DealDamageOnImpact dealDamageOnImpact;
        
        public WorldForces worldForces;
        public GameObject inputStackDummy;
        public Int2 leverDirection;
        public float animAccel;

        public SeaTruckUpgrades upgrades;
        public IQuickSlots quickSlots;

        public float[] quickSlotTimeUsed;
        public float[] quickSlotCooldown;
        public float[] quickSlotCharge;

        public string[] slotIDs;
        public Dictionary<string, int> slotIndexes;
        public Dictionary<TechType, float> crushDepths;
        public Equipment modules;

        public Event<int> OnActiveSlotChanged;
        public Event<bool> OnDockedChanged;
        public Event<float> OnDamageReceived;

        public PowerRelay powerRelay;

        private List<IItemsContainer> containers = new List<IItemsContainer>();
        private List<GameObject> handTargets = new List<GameObject>();

        public bool isReady = false;

        private int _activeSlot;

        private bool _isDocked;

        private float _damage;

        private readonly bool[] listenersEnabled = new bool[3];

        public uGUI_SeaTruckHUD thisHUD;

        public delegate void OnPilotingBegin();
        public delegate void OnPilotingEnd();
        public OnPilotingBegin onPilotingBegin;
        public OnPilotingEnd onPilotingEnd;

        public int ActiveSlot
        {
            get => _activeSlot;

            set
            {
                if (_activeSlot != value)
                {
                    _activeSlot = value;
                    OnActiveSlotChanged.Trigger(_activeSlot);
                }
            }
        }

        public bool IsDocked
        {
            get => _isDocked;

            set
            {
                if (_isDocked != value)
                {
                    _isDocked = value;
                    OnDockedChanged.Trigger(_isDocked);
                }
            }
        }

        public float Damage
        {
            get => _damage;

            set
            {
                if (_damage != value)
                {
                    _damage = value;
                    OnDamageReceived.Trigger(_damage);
                }
            }
        }

        public string TruckName
        {
            get => pingInstance.GetLabel();
        }

        public SeaTruckHelper
            (
            GameObject Seatruck,
            bool slotListener,
            bool dockListener,
            bool damageListener
            )
        {

            BZLogger.Debug("SeatruckHelper", $"constructor on this Seatruck has started. ID: [{Seatruck.GetInstanceID()}]");

            MainCab = Seatruck;
            listenersEnabled[0] = slotListener;
            listenersEnabled[1] = dockListener;
            listenersEnabled[2] = damageListener;

            Init();

            BZLogger.Debug("SeatruckHelper", $"constructor on this Seatruck has finished. ID: [{Seatruck.GetInstanceID()}]");
        }

        private void Init()
        {
            upgrades = MainCab.GetComponent<SeaTruckUpgrades>();
            animation = MainCab.GetComponent<SeaTruckAnimation>();
            aquarium = MainCab.GetComponent<SeaTruckAquarium>();
            connection = MainCab.GetComponent<SeaTruckConnection>();
            dockingBay = MainCab.GetComponent<SeaTruckDockingBay>();
            effects = MainCab.GetComponent<SeaTruckEffects>();
            lights = MainCab.GetComponent<SeaTruckLights>();
            segment = MainCab.GetComponent<SeaTruckSegment>();
            connectingDoor = MainCab.GetComponent<SeaTruckConnectingDoor>();
            motor = MainCab.GetComponent<SeaTruckMotor>();
            teleporter = MainCab.GetComponent<SeaTruckTeleporter>();
            pingInstance = MainCab.GetComponent<PingInstance>();
            dockable = MainCab.GetComponent<Dockable>();
            colorNameControl = MainCab.GetComponent<ColorNameControl>();
            thisLiveMixin = segment.liveMixin;
            damageInfo = thisLiveMixin.GetPrivateField("damageInfo") as DamageInfo;
            dealDamageOnImpact = MainCab.GetComponent<DealDamageOnImpact>();

            worldForces = MainCab.GetComponent<WorldForces>();
            inputStackDummy = motor.GetPrivateField("inputStackDummy") as GameObject;
            leverDirection = (Int2)motor.GetPrivateProperty("leverDirection", BindingFlags.SetProperty);
            animAccel = (float)motor.GetPrivateField("animAccel", BindingFlags.SetField);

            slotIDs = upgrades.GetPrivateField("slotIDs", BindingFlags.Static) as string[];
            slotIndexes = upgrades.GetPrivateField("slotIndexes") as Dictionary<string, int>;
            crushDepths = upgrades.GetPrivateField("crushDepths", BindingFlags.Static) as Dictionary<TechType, float>;

            quickSlotTimeUsed = upgrades.GetPrivateField("quickSlotTimeUsed", BindingFlags.SetField) as float[];
            quickSlotCooldown = upgrades.GetPrivateField("quickSlotCooldown", BindingFlags.SetField) as float[];
            quickSlotCharge = upgrades.GetPrivateField("quickSlotCharge", BindingFlags.SetField) as float[];

            quickSlots = MainCab.GetComponent<IQuickSlots>();

            powerRelay = upgrades.relay;

            modules = upgrades.modules;

            thisHUD = uGUI.main.GetComponentInChildren<uGUI_SeaTruckHUD>();

            if (listenersEnabled[0])
            {
                OnActiveSlotChanged = new Event<int>();
                SeaTruckSlotListener thisSlotListener = MainCab.AddComponent<SeaTruckSlotListener>();
                thisSlotListener.helper = this;

                BZLogger.Debug("SeatruckHelper", $"Slot Listener component added to this Seatruck. ID: [{MainCab.GetInstanceID()}]");                
            }

            if (listenersEnabled[1])
            {
                OnDockedChanged = new Event<bool>();
                SeaTruckDockingListener thisDockListener = MainCab.AddComponent<SeaTruckDockingListener>();
                thisDockListener.helper = this;

                BZLogger.Debug("SeatruckHelper", $"Docking Listener component added to this Seatruck. ID: [{MainCab.GetInstanceID()}]");
            }

            if (listenersEnabled[2])
            {
                OnDamageReceived = new Event<float>();
                SeaTruckDamageListener thisDamageListener = MainCab.AddComponent<SeaTruckDamageListener>();
                thisDamageListener.helper = this;

                BZLogger.Debug("SeatruckHelper", $"Damage Listener component added to this Seatruck. ID: [{MainCab.GetInstanceID()}]");
            }

            isReady = true;

            SeaTruckPilotingListener truckPilotingListener = MainCab.AddComponent<SeaTruckPilotingListener>();
            truckPilotingListener.helper = this;

            DebugSlots();
        }
        
        public bool IsPowered()
        {
            return !motor.requiresPower || (motor.relay && motor.relay.IsPowered());
        }

        public int GetActiveSlotID()
        {
            return quickSlots.GetActiveSlotID();
        }

        public float GetSlotProgress(int slotID)
        {
            return quickSlots.GetSlotProgress(slotID);
        }

        public int GetSlotIndex(string slot)
        {
            if (slotIndexes.TryGetValue(slot, out int result))
            {
                return result;
            }

            return -1;
        }

        public bool IsPiloted()
        {
            return motor.IsPiloted();
        }        

        public float GetWeight()
        {
            return segment.GetWeight() + segment.GetAttachedWeight() * (motor.horsePowerUpgrade ? 0.65f : 0.8f);
        }

        public InventoryItem GetSlotItem(int slotID)
        {
            return quickSlots.GetSlotItem(slotID);
        }

        public TechType GetSlotBinding(int slotID)
        {
            return quickSlots.GetSlotBinding(slotID);
        }

        public int GetSlotCount()
        {
            return quickSlots.GetSlotCount();
        }

        float GetSlotCharge(int slotID)
        {
            return quickSlots.GetSlotCharge(slotID);
        }

        public QuickSlotType GetQuickSlotType(int slotID, out TechType techType)
        {
            if (slotID >= 0 && slotID < slotIDs.Length)
            {
                techType = modules.GetTechTypeInSlot(slotIDs[slotID]);

                if (techType != TechType.None)
                {
                    return TechData.GetSlotType(techType);
                }
            }

            techType = TechType.None;

            return QuickSlotType.None;
        }

        public ItemsContainer GetSeamothStorageInSlot(int slotID, TechType techType)
        {
            InventoryItem slotItem = GetSlotItem(slotID);

            if (slotItem == null)
            {
                return null;
            }

            Pickupable item = slotItem.item;

            if (item.GetTechType() != techType)
            {
                return null;
            }

            if (item.TryGetComponent(out SeamothStorageContainer component))
            {
                DebugStorageContainer(slotID, component);

                return component.container;
            }

            return null;            
        }

        public ItemsContainer GetSeaTruckStorageInSlot(int slotID)
        {
            InventoryItem slotItem = GetSlotItem(slotID);

            if (slotItem == null)
            {
                return null;
            }
            
            if (slotItem.item.TryGetComponent(out SeamothStorageContainer component))
            {
                DebugStorageContainer(slotID, component);

                return component.container;
            }

            return null;
        }

        public bool TryOpenSeaTruckStorageContainer(int slotID)
        {
            ItemsContainer container = GetSeaTruckStorageInSlot(slotID);

            if (container != null)
            {
                PDA pda = Player.main.GetPDA();
                Inventory.main.SetUsedStorage(container, false);
                pda.Open(PDATab.Inventory, null, null);
                return true;
            }

            return false;
        }

        private void GetAllStorages()
        {
            containers.Clear();

            if (!TechTypeHandler.TryGetModdedTechType("SeaTruckStorage", out TechType techType))
                return;

            foreach (string slot in slotIDs)
            {
                if (modules.GetTechTypeInSlot(slot) == techType)
                {
                    InventoryItem item = modules.GetItemInSlot(slot);                    

                    if (item.item.TryGetComponent(out SeamothStorageContainer component))
                    {
                        containers.Add(component.container);
                    }
                }
            }
        }

        public bool HasRoomForItem(Pickupable pickupable)
        {
            GetAllStorages();

            foreach (ItemsContainer container in containers)
            {
                if (container.HasRoomFor(pickupable))
                {
                    return true;
                }
            }

            return false;
        }

        public ItemsContainer GetRoomForItem(Pickupable pickupable)
        {
            GetAllStorages();

            foreach (ItemsContainer container in containers)
            {
                if (container.HasRoomFor(pickupable))
                {
                    return container;
                }
            }

            return null;
        }
        
        public bool IsValidSeaTruckStorageContainer(int slotID)
        {
            try
            {
                GameObject storageLeft = MainCab.transform.Find("StorageRoot/StorageLeft").gameObject;

                if (storageLeft)
                {
                    Component component = storageLeft.GetComponent("SeaTruckStorage.SeaTruckStorageInput");

                    int leftSlotID = (int)component.GetPublicField("slotID");

                    if (leftSlotID == slotID)
                        return true;
                }
            }
            catch
            {
                return false;
            }

            try
            {
                GameObject storageRight = MainCab.transform.Find("StorageRoot/StorageRight").gameObject;

                if (storageRight)
                {
                    Component component = storageRight.GetComponent("SeaTruckStorage.SeaTruckStorageInput");

                    int rightSlotID = (int)component.GetPublicField("slotID");

                    if (rightSlotID == slotID)
                        return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public bool IsSeatruckChained()
        {
            return (segment.rearConnection != null && segment.rearConnection.occupied) ? true : false;            
        }

        public bool IsDockingModulePresent()
        {
            if (!IsSeatruckChained())
            {
                return false;
            }

            List<SeaTruckSegment> chain = new List<SeaTruckSegment>();

            segment.GetTruckChain(chain);
                        
            foreach (SeaTruckSegment segment in chain)
            {
                if (segment.name.Equals("SeaTruckDockingModule(Clone)"))
                {
                    return true;
                }                   
            }

            return false;
        }


        public float GetSeatruckZShift()
        {
            float zShift = 0;

            if (!IsSeatruckChained())
            {
                return zShift;
            }

            List<SeaTruckSegment> chain = new List<SeaTruckSegment>();

            segment.GetTruckChain(chain);

            SeaTruckSegment lastSegment = chain.GetLast();                      

            if (lastSegment != segment)
            {
                foreach (SeaTruckSegment segment in chain)
                {
                    if (segment == this.segment)
                    {
                        continue;
                    }

                    float shift = Mathf.Abs(segment.transform.localPosition.z);

                    if (segment.name.Equals("SeaTruckDockingModule(Clone)"))
                    {
                        zShift += shift - 1.32f;                        
                    }
                    else
                    {
                        zShift += shift;
                    }
                }                

                zShift = zShift * -1f;                
            }            

            return zShift;
        }

        public List<GameObject> GetWheelTriggers()
        {
            handTargets.Clear();

            if (!IsSeatruckChained())
            {
                return handTargets;
            }

            List<SeaTruckSegment> chain = new List<SeaTruckSegment>();

            segment.GetTruckChain(chain);
                        
            foreach (SeaTruckSegment segment in chain)
            {
                if (segment == this.segment)
                {
                    continue;
                }

                GenericHandTarget handtarget = segment.GetComponentInChildren<GenericHandTarget>(true);

                if (handtarget != null)
                {
                    handTargets.Add(handtarget.gameObject);
                }                    
            }            

            DebugTriggers();

            return handTargets;
        }


        [Conditional("DEBUG")]
        void DebugSlots()
        {
            BZLogger.Debug("SeatruckHelper", $"Upgrade slots check started on this Seatruck. ID: [{MainCab.GetInstanceID()}]");

            foreach (string slot in slotIDs)
            {
                BZLogger.Debug("SeatruckHelper", $"Found slot: [{slot}]");
            }

            BZLogger.Debug("SeatruckHelper", $"Upgrade slots check finished on this Seatruck. ID: [{MainCab.GetInstanceID()}]");
        }

        [Conditional("DEBUG")]
        void DebugStorageContainer(int slotID, SeamothStorageContainer container)
        {
            BZLogger.Debug("SeatruckHelper", $"Seamoth storage container found on slot [{slotID}], name [{container.name}]");

            foreach (TechType techtype in container.allowedTech)
            {
                BZLogger.Debug("SeatruckHelper", $"allowedTech: {techtype}");
            }
        }


        [Conditional("DEBUG")]
        void DebugTriggers()
        {
            BZLogger.Debug("SeatruckHelper", "Debug handTargets:");

            foreach (GameObject trigger in handTargets)
            {
                BZLogger.Log($"handtarget name: {trigger.name}");
            }
        }

    }
}
*/