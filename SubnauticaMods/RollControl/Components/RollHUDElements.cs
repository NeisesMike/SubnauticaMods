using UnityEngine;
using System.Reflection;
using System.IO;

namespace RollControl.Components
{
    public class RollHUDElements : MonoBehaviour
    {
        private GameObject playerHUD = null;
        private GameObject subHUD = null;
        private ScubaRollController ScubaCon => Player.main.gameObject.EnsureComponent<ScubaRollController>();
        private VehicleRollController VehicleCon => Player.main.GetVehicle()?.gameObject.EnsureComponent<VehicleRollController>();

        public void Start()
        {
            if (UnityEngine.XR.XRSettings.enabled)
            {
                Component.DestroyImmediate(this);
            }
            SetupPlayerElement();
            SetupSubmersibleElement();
            playerHUD.SetActive(false);
            subHUD.SetActive(false);
        }
        public void Update()
        {
            HandleScuba();
            HandleVehicle();
        }
        private void HandleScuba()
        {
            if(ScubaCon != null && playerHUD != null)
            {
                playerHUD.SetActive(Player.main.gameObject.EnsureComponent<ScubaRollController>().IsActuallyScubaRolling);
            }
        }
        private void HandleVehicle()
        {
            if(VehicleCon == null || subHUD == null)
            {
                subHUD.SetActive(false);
            }
            if (VehicleCon != null && subHUD != null)
            {
                subHUD.SetActive(VehicleCon.IsActuallyRolling);
            }
        }

        public void SetupSubmersibleElement()
        {
            Transform compassHUDElementsRoot = uGUI.main.transform.Find("ScreenCanvas/HUD/Content/DepthCompass/SubmersibleDepth");
            GameObject background = compassHUDElementsRoot.Find("Background").gameObject;
            subHUD = GameObject.Instantiate(background, compassHUDElementsRoot);
            subHUD.name = "VehicleRoll";
            subHUD.transform.localPosition = new Vector3(130, -9, 0);
            subHUD.transform.localEulerAngles = Vector3.zero;
            subHUD.GetComponent<UnityEngine.UI.Image>().sprite = GetSpriteRaw("VehicleRollElement.png");
            subHUD.GetComponent<UnityEngine.UI.Image>().enabled = true;
            subHUD.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);

        }
        public void SetupPlayerElement()
        {
            Transform compassHUDElementsRoot = uGUI.main.transform.Find("ScreenCanvas/HUD/Content/DepthCompass/PlayerDepth");
            GameObject halfmoon = compassHUDElementsRoot.Find("HalfMoon").gameObject;
            playerHUD = GameObject.Instantiate(halfmoon, compassHUDElementsRoot);
            playerHUD.name = "PlayerRoll";
            playerHUD.transform.localPosition = new Vector3(125, -25, 0);
            playerHUD.transform.localEulerAngles = Vector3.zero;
            playerHUD.GetComponent<UnityEngine.UI.Image>().sprite = GetSpriteRaw("PlayerRollElement.png");
            playerHUD.GetComponent<UnityEngine.UI.Image>().enabled = true;
            playerHUD.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
        }

        public static Sprite GetSpriteRaw(string relativePath)
        {
            string modPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            string fullPath = Path.Combine(modPath, relativePath);
            return GetSpriteGenericRaw(fullPath);
        }
        private static Sprite GetSpriteGenericRaw(string fullPath)
        {
            byte[] spriteBytes = System.IO.File.ReadAllBytes(fullPath);
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            return Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

    }
}
