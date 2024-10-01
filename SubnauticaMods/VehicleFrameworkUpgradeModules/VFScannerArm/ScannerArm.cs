using System.Collections.Generic;
using UnityEngine;
using VehicleFramework;

namespace VFScannerArm
{
	public class ScannerArm : MonoBehaviour, VehicleFramework.IPlayerListener
	{
		Transform GObjectsRoot => transform.Find("offset/model/Bone.002/Bone/Bone.001");
		Transform ScreenOuter => GObjectsRoot.Find("ScreenOuter");
		Vector3 ScreenOpen = new Vector3(180, 0, 0);
		Vector3 ScreenClosed = new Vector3(90, 0, 0);
		readonly float fullOrangeOffset = 0.86f;
		readonly float fullBlueOffset = 1.25f;
		private float lastProgressValue = 0.86f;
		private ModVehicle mv => GetComponentInParent<ModVehicle>();
		private bool IsInUse = false;
		private float timeSwitched = 0;
		public void Awake()
		{
			SetupScannerTool();
			UpdateScreen(ScannerTool.ScreenState.Default, 0f);
		}
		public void Start()
		{
			SetFXActive(false);
			Shader scannerToolScanning = ShaderManager.preloadedShaders.scannerToolScanning;
			if (scannerToolScanning != null)
			{
				scanMaterialCircuitFX = new Material(scannerToolScanning)
				{
					hideFlags = HideFlags.HideAndDontSave,
				};
				scanMaterialCircuitFX.SetTexture(ShaderPropertyID._MainTex, scanCircuitTex);
				scanMaterialCircuitFX.SetColor(ShaderPropertyID._Color, scanCircuitColor);
				scanMaterialOrganicFX = new Material(scannerToolScanning)
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				scanMaterialOrganicFX.SetTexture(ShaderPropertyID._MainTex, scanOrganicTex);
				scanMaterialOrganicFX.SetColor(ShaderPropertyID._Color, scanOrganicColor);
			}

			void EnableSimpleEmission(Material mat, bool isGlow = true)
			{
				mat.EnableKeyword("MARMO_EMISSION");
				mat.EnableKeyword("MARMO_SPECMAP");
				if (isGlow)
				{
					mat.SetFloat("_GlowStrength", 1);
					mat.SetFloat("_GlowStrengthNight", 1);
					mat.SetColor("_GlowColor", new Color(0, 1, 1, 1));
					mat.SetFloat("_EmissionLM", 0);
					mat.SetFloat("_EmissionLMNight", 0);
				}
				else
				{
					mat.SetFloat("_GlowStrength", 0);
					mat.SetFloat("_GlowStrengthNight", 0);
					mat.SetFloat("_EmissionLM", 1);
					mat.SetFloat("_EmissionLMNight", 1);
				}
			}
			EnableSimpleEmission(GObjectsRoot.Find("ProgressBar").GetComponent<MeshRenderer>().material, false);
			List<Material> mats = new List<Material>
			{
				GObjectsRoot.Find("Forearm.001").GetComponent<MeshRenderer>().material,
				GObjectsRoot.Find("Forearm.002").GetComponent<MeshRenderer>().material,
				GObjectsRoot.Find("Forearm.003").GetComponent<MeshRenderer>().material,
				GObjectsRoot.Find("Forearm.004").GetComponent<MeshRenderer>().material
			};
			mats.ForEach(x => EnableSimpleEmission(x));


			GameObject screenInner = ScreenOuter.Find("ScreenInner").gameObject;
			screenInner.GetComponent<UnityEngine.UI.Image>().sprite = MainPatcher.originalScannerTool.transform.Find("UI/Content/Background").GetComponent<UnityEngine.UI.Image>().sprite;

			GameObject textCanvas = ScreenOuter.Find("ScreenInner/Text (TMP)").gameObject;
			var tmprougui = textCanvas.GetComponent<TMPro.TextMeshProUGUI>();
			tmprougui.font = MainPatcher.originalScannerTool.transform.Find("UI/Content/Default").GetComponent<TMPro.TextMeshProUGUI>().font;
			tmprougui.text = "SEARCHING...";
			tmprougui.fontSize = 32;
			tmprougui.alignment = TMPro.TextAlignmentOptions.Center;
			tmprougui.enableAutoSizing = true;
			if (gameObject == mv.GetComponent<VehicleFramework.VehicleComponents.VFArmsManager>()?.rightArm)
			{
				Transform rightArmDisplay = ScreenOuter.Find("ScreenInner/Text (TMP)");
				rightArmDisplay.transform.localScale = new Vector3(
					-rightArmDisplay.transform.localScale.x,
					rightArmDisplay.transform.localScale.y,
					rightArmDisplay.transform.localScale.z
				);
			}
		}
		public void Update()
		{
			if (idleTimer > 0f)
			{
				idleTimer = Mathf.Max(0f, idleTimer - Time.deltaTime);
			}
			if (IsInUse && mv.IsPlayerControlling())
			{
				DoScan();
			}
		}
		public void ToggleSelect()
		{
			IsInUse = !IsInUse;
			timeSwitched = Time.time;
		}
		public void OnDisable()
		{
			scanSound.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		public void OnDestroy()
		{
			if (scanFX != null)
			{
				StopScanFX();
			}
		}
		public void DoScan()
		{
			PDAScanner.Result result = Scan();
			if (result != PDAScanner.Result.Processed)
			{
				if (result == PDAScanner.Result.Known)
				{
					ErrorMessage.AddDebug(Language.main.GetFormat<string>("ScannerEntityKnown", Language.main.Get(PDAScanner.scanTarget.techType.AsString(false))));
				}
			}
			else
			{
				ErrorMessage.AddDebug(Language.main.Get("ScannerInstanceKnown"));
			}
		}
		public void HandleScreenRotation()
		{
			float lerpAmount = (Time.time - timeSwitched) * 3;
			if (IsInUse)
			{
				ScreenOuter.localEulerAngles = Vector3.Lerp(ScreenClosed, ScreenOpen, lerpAmount);
			}
			else
			{
				ScreenOuter.localEulerAngles = Vector3.Lerp(ScreenOpen, ScreenClosed, lerpAmount);
			}
		}
		public void HandleProgressBar()
		{
			float progressLerp; // = Mathf.Lerp(fullOrangeOffset, fullBlueOffset, PDAScanner.scanTarget.gameObject == null ? lastProgressValue : PDAScanner.scanTarget.progress);
			if (PDAScanner.CanScan() == PDAScanner.Result.Scan)
			{
				progressLerp = Mathf.Lerp(fullOrangeOffset, fullBlueOffset, PDAScanner.scanTarget.progress);
				lastProgressValue = PDAScanner.scanTarget.progress;

			}
			else
			{
				progressLerp = Mathf.Lerp(fullOrangeOffset, fullBlueOffset, lastProgressValue);
			}
			float uvRange = fullBlueOffset - fullOrangeOffset;
			float pixelHeightUV = uvRange / 1500.0f; // UV space per pixel
													 // Snap to 3-pixel increments
			float snappedScrollY = Mathf.Round(progressLerp / (3 * pixelHeightUV)) * (3 * pixelHeightUV);
			GObjectsRoot.Find("ProgressBar").GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0, snappedScrollY);
		}
		public void DoAnimations()
		{
			GetComponentInChildren<Animator>().SetBool("IsArmRaised", IsInUse);
			HandleScreenRotation();
			HandleProgressBar();
		}
		public void LateUpdate()
		{
			bool flag = stateCurrent == ScannerTool.ScanState.Scan;
			if (mv.IsPlayerControlling() && idleTimer <= 0f)
			{
				OnHover();
			}
			SetFXActive(flag);
			DoAnimations();
			if (flag)
			{
				scanSound.Play();
			}
			else
			{
				scanSound.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			stateCurrent = ScannerTool.ScanState.None;
		}
		private PDAScanner.Result Scan()
		{
			if (stateCurrent != ScannerTool.ScanState.None)
			{
				return PDAScanner.Result.None;
			}
			if (idleTimer > 0f)
			{
				return PDAScanner.Result.None;
			}
			PDAScanner.Result result = PDAScanner.Result.None;
			PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
			if (scanTarget.isValid && mv.energyInterface.hasCharge)
			{
				result = PDAScanner.Scan();
				if (result == PDAScanner.Result.Scan)
				{
					float amount = powerConsumption * Time.deltaTime;
					mv.energyInterface.ConsumeEnergy(amount);
					stateCurrent = ScannerTool.ScanState.Scan;
				}
				else if (result == PDAScanner.Result.Done || result == PDAScanner.Result.Researched)
				{
					UpdateScreen(ScannerTool.ScreenState.Default, 0f);
					idleTimer = 0.5f;
					PDASounds.queue.PlayIfFree(completeSound);
					//PlayScanCompleteVisualEffect();
				}
				else if (result == PDAScanner.Result.NotInfected || result == PDAScanner.Result.Infected)
				{
					UpdateScreen((result == PDAScanner.Result.Infected) ? ScannerTool.ScreenState.Infected : ScannerTool.ScreenState.NotInfected, 0f);
					idleTimer = 3f;
					PDASounds.queue.PlayIfFree(completeSound);
					//PlayScanCompleteVisualEffect();
				}
			}
			return result;
		}
		private void OnHover()
		{
			if (!mv.energyInterface.hasCharge)
			{
				UpdateScreen(ScannerTool.ScreenState.Unpowered, 0f);
				return;
			}
			PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
			if (!scanTarget.isValid)
			{
				UpdateScreen(ScannerTool.ScreenState.Default, 0f);
				return;
			}
			if (PDAScanner.CanScan() != PDAScanner.Result.Scan)
			{
				UpdateScreen(ScannerTool.ScreenState.Default, 0f);
				return;
			}
			HandReticle main = HandReticle.main;
			main.SetText(HandReticle.TextType.Hand, scanTarget.techType.AsString(false), true, GameInput.Button.RightHand);
			main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
			if (stateCurrent == ScannerTool.ScanState.Scan)
			{
				UpdateScreen(ScannerTool.ScreenState.Scanning, scanTarget.progress);
				return;
			}
			main.SetIcon(HandReticle.IconType.Scan, 1.5f);
			UpdateScreen(ScannerTool.ScreenState.Ready, 0f);
		}
		private void SetFXActive(bool state)
		{
			scanBeam.gameObject.SetActive(state);
			if (state && PDAScanner.scanTarget.isValid)
			{
				PlayScanFX();
				return;
			}
			StopScanFX();
		}
		private void PlayScanFX()
		{
			PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
			if (scanTarget.isValid)
			{
				if (scanFX != null)
				{
					if (scanFX.gameObject != scanTarget.gameObject)
					{
						StopScanFX();
						scanFX = scanTarget.gameObject.AddComponent<VFXOverlayMaterial>();
						if (scanTarget.gameObject.GetComponent<Creature>() != null)
						{
							scanFX.ApplyOverlay(scanMaterialOrganicFX, "VFXOverlay: Scanning", false, null);
						}
						else
						{
							scanFX.ApplyOverlay(scanMaterialCircuitFX, "VFXOverlay: Scanning", false, null);
						}
					}
				}
				else
				{
					scanFX = scanTarget.gameObject.AddComponent<VFXOverlayMaterial>();
					if (scanTarget.gameObject.GetComponent<Creature>() != null)
					{
						scanFX.ApplyOverlay(scanMaterialOrganicFX, "VFXOverlay: Scanning", false, null);
					}
					else
					{
						scanFX.ApplyOverlay(scanMaterialCircuitFX, "VFXOverlay: Scanning", false, null);
					}
				}
				float value = 1f;
				if (!MiscSettings.flashes)
				{
					value = 0.1f;
				}
				scanMaterialCircuitFX.SetFloat(ShaderPropertyID._TimeScale, value);
				scanMaterialOrganicFX.SetFloat(ShaderPropertyID._TimeScale, value);
			}
		}
		private void StopScanFX()
		{
			if (scanFX != null)
			{
				scanFX.RemoveOverlay();
			}
		}
		private void UpdateScreen(ScannerTool.ScreenState state, float progress = 0f)
		{
			TMPro.TextMeshProUGUI textCanvas = ScreenOuter.Find("ScreenInner/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>();
			switch (state)
			{
				case ScannerTool.ScreenState.Default:
					textCanvas.text = Language.main.Get("ScannerScreenDefault");
					textCanvas.color = new Color32(159, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					return;
				case ScannerTool.ScreenState.Ready:
					textCanvas.text = Language.main.Get("ScannerScreenReady");
					textCanvas.color = new Color32(159, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					return;
				case ScannerTool.ScreenState.Scanning:
					textCanvas.text = Language.main.Get("ScannerScreenScanning");
					textCanvas.color = new Color32(159, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					return;
				case ScannerTool.ScreenState.Unpowered:
					textCanvas.text = Language.main.Get("ScannerScreenUnpowered");
					textCanvas.color = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
					return;
				case ScannerTool.ScreenState.NotInfected:
					textCanvas.text = Language.main.Get("ScannerScreenNotInfected");
					textCanvas.color = new Color(0f, 255f, 0f, 255f);
					return;
				case ScannerTool.ScreenState.Infected:
					textCanvas.text = Language.main.Get("ScannerScreenInfected");
					textCanvas.color = new Color(255f, 0f, 0f, 255f);
					return;
				default:
					return;
			}
		}

		public void SetupScannerTool()
		{
			powerConsumption = 0.2f;
			scanBeam = GameObject.Instantiate(MainPatcher.originalScannerTool.scanBeam);
			scanBeam.transform.SetParent(transform);
			scanSound = gameObject.EnsureComponent<FMOD_CustomLoopingEmitter>();
			scanSound.asset = MainPatcher.originalScannerTool.scanSound.asset;
			completeSound = Instantiate(MainPatcher.originalScannerTool.completeSound);
			scanOrganicTex = new Texture2D(MainPatcher.originalScannerTool.scanOrganicTex.width,
				MainPatcher.originalScannerTool.scanOrganicTex.height,
				MainPatcher.originalScannerTool.scanOrganicTex.format,
				mipCount: 11,
				false);
			Graphics.CopyTexture(MainPatcher.originalScannerTool.scanOrganicTex, scanOrganicTex);
			scanCircuitTex = new Texture2D(MainPatcher.originalScannerTool.scanCircuitTex.width,
				MainPatcher.originalScannerTool.scanCircuitTex.height,
				MainPatcher.originalScannerTool.scanCircuitTex.format,
				mipCount: 11,
				false);
			Graphics.CopyTexture(MainPatcher.originalScannerTool.scanCircuitTex, scanCircuitTex);
		}

		void IPlayerListener.OnPlayerEntry()
		{
		}

		void IPlayerListener.OnPlayerExit()
		{
			IsInUse = false;
		}

		void IPlayerListener.OnPilotBegin()
		{
		}

		void IPlayerListener.OnPilotEnd()
		{
			IsInUse = false;
		}

		public GameObject scanBeam;
		public FMOD_CustomLoopingEmitter scanSound;
		public FMODAsset completeSound;
		private ScannerTool.ScanState stateCurrent;
		private float idleTimer;
		private Material scanMaterialCircuitFX;
		private Material scanMaterialOrganicFX;
		private VFXOverlayMaterial scanFX;
		public Texture2D scanCircuitTex;
		public Texture2D scanOrganicTex;
		public Color scanCircuitColor = Color.white;
		public Color scanOrganicColor = Color.white;
		public float powerConsumption = 0.2f;
	}
}
