using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework.Assets;

namespace VFScannerArm
{
    public static class FragmentUtils
    {
        const string classID = "ScannerArmFragment";
        const string displayName = "Scanner Arm";
        const string encykey = "ScannerArmVF";
        const string description = "A Scannable fragment of the Scanner Arm";
        public static string GetEncyDescription()
        {
            return "An arm-mounted version of the essential science and survival tool, the scanner arm can be used to add new blueprints to memory, and analyze unknown entities.\n\nIt emits electromagnetic radiation in the specified direction, which is reflected by the environment and then analyzed to determine the physical make-up of the targeted object. It has three primary functions.\n\nBlueprint Adquisition:\nRecord the physical parameters of scanned technologies to add their blueprints to the PDA databank. These blueprints may then be constructed at the appropriate fabricator. The scanner is also equipped to break down damaged and otherwise useless devices into their base metals for salvage purposes.\n\nOrganism Analysis:\nThe scanner will attempt to match scanned organisms against the onboard database. If no match is found then the species will be assigned an easy-to-remember name, and a new databank entry will be created. Your PDA\'s AI will also attempt to synthesize theories on behavioral tendencies and evolutionary origins where possible, as well as deliver assessments on how best to approach them.\n\nMedical Analysis:\nScanning any living organism will display basic information on their state of health on the scanner\'s HUD. This information will be limited without access to a network database.\n\n\"The Alterra Spectroscope Scanner Arm, for when you are afraid to understand the world around you.\"";
        }
        public static void RegisterEncyEntry()
        {
            Sprite template = SpriteHelper.GetSprite("ScannerArmTemplated.png");
            Nautilus.Handlers.PDAHandler.AddEncyclopediaEntry(encykey, "Tech/Equipment", displayName, GetEncyDescription(), template.texture, SpriteHelper.GetSprite("ScannerArmPopUp.png"));
        }
        public static void EnsureColliders(GameObject armPrefab)
        {
            foreach(Transform tr in armPrefab.transform.Find("offset/model/Bone.002/Bone/Bone.001"))
            {
                tr.gameObject.AddComponent<BoxCollider>().size = 0.01f * Vector3.one;
            }
        }
        public static void RegisterScannerArmFragment(TechType scannerArmTT, GameObject armPrefab)
        {
            RegisterEncyEntry();
            EnsureColliders(armPrefab);
            Component.DestroyImmediate(armPrefab.GetComponent<ScannerArm>());
            armPrefab.EnsureComponent<Rigidbody>().isKinematic = true;
            DisableVisualEffects(armPrefab);
            List<Vector3> spawnLocations = new List<Vector3>
            {
                new Vector3 (1005.95f, -0.20f, 7.88f),
                new Vector3 (1023.4f, -9.71f, -31.06f),
                new Vector3 (1030.09f, -1.57f, -10.57f),
                new Vector3 (1007.22f, -7.67f, -6.59f),
                new Vector3 (992.84f, -7.008f, -20.31f)
            };
            List<Vector3> spawnRotations = new List<Vector3>
            {
                new Vector3 (0, 180f, 265.7f),
                new Vector3 (0, 0, 265.94f),
                new Vector3 (0, 188.5f, 265f),
                new Vector3 (1.226f, 357.2f, 34.28f),
                new Vector3 (0, 68.57f, 265.6f)
            };
            FragmentData fragData = new FragmentData
            {
                fragment = armPrefab,
                toUnlock = scannerArmTT,
                fragmentsToScan = 3,
                scanTime = 3f,
                classID = classID,
                displayName = displayName,
                description = description,
                spawnLocations = spawnLocations,
                spawnRotations = spawnRotations,
                encyKey = encykey
            };
            FragmentManager.RegisterFragment(fragData);
        }
        public static void DisableVisualEffects(GameObject arm)
        {
            void DisableSimpleEmission(Material mat, bool isGlow = true)
            {
                mat.EnableKeyword("MARMO_EMISSION");
                mat.EnableKeyword("MARMO_SPECMAP");
                if (isGlow)
                {
                    mat.SetFloat("_GlowStrength", 0);
                    mat.SetFloat("_GlowStrengthNight", 0);
                    mat.SetColor("_GlowColor", new Color(0, 1, 1, 1));
                    mat.SetFloat("_EmissionLM", 0);
                    mat.SetFloat("_EmissionLMNight", 0);
                }
                else
                {
                    mat.SetFloat("_GlowStrength", 0);
                    mat.SetFloat("_GlowStrengthNight", 0);
                    mat.SetFloat("_EmissionLM", 0);
                    mat.SetFloat("_EmissionLMNight", 0);
                }
            }
            DisableSimpleEmission(arm.transform.Find("offset/model/Bone.002/Bone/Bone.001/ProgressBar").GetComponent<MeshRenderer>().material, false);
            List<Material> mats = new List<Material>
            {
                arm.transform.Find("offset/model/Bone.002/Bone/Bone.001/Forearm.001").GetComponent<MeshRenderer>().material,
                arm.transform.Find("offset/model/Bone.002/Bone/Bone.001/Forearm.002").GetComponent<MeshRenderer>().material,
                arm.transform.Find("offset/model/Bone.002/Bone/Bone.001/Forearm.003").GetComponent<MeshRenderer>().material,
                arm.transform.Find("offset/model/Bone.002/Bone/Bone.001/Forearm.004").GetComponent<MeshRenderer>().material
            };
            mats.ForEach(x => DisableSimpleEmission(x));
            arm.transform.Find("offset/model/Bone.002/Bone/Bone.001/ScreenOuter").localEulerAngles = new Vector3(90, 0, 0);
        }
    }
}
