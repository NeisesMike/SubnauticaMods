using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using UWE;

namespace DebugScene
{
    public class SceneController : MonoBehaviour
	{
		private bool isStartingNewGame;

		public IEnumerator StartNewGame(GameMode gameMode)
		{
			if (this.isStartingNewGame)
			{
				yield break;
			}
			//isStartingNewGame = true;
			Guid.NewGuid().ToString();
			PlatformUtils.main.GetServices().ShowUGCRestrictionMessageIfNecessary();
			global::Utils.SetContinueMode(false);
			global::Utils.SetLegacyGameMode(gameMode);

			CoroutineTask<SaveLoadManager.CreateResult> createSlotTask;
			if (PlatformUtils.isPS4Platform)
			{
				createSlotTask = SaveLoadManager.main.SetupSlotPS4Async();
			}
			else
			{
				createSlotTask = SaveLoadManager.main.CreateSlotAsync();
			}
			yield return createSlotTask;
			SaveLoadManager.CreateResult result = createSlotTask.GetResult();
			if (!result.success)
			{
				if (result.slotName == SaveLoadManager.Error.OutOfSpace.ToString())
				{
					string descriptionText = Language.main.Get("SaveFailedSpace");
					uGUI.main.confirmation.Show(descriptionText, null);
				}
				else if (result.slotName == SaveLoadManager.Error.OutOfSlots.ToString())
				{
					string descriptionText2 = Language.main.Get("SaveFailedSlot");
					uGUI.main.confirmation.Show(descriptionText2, null);
				}
				this.isStartingNewGame = false;
				yield break;
			}
			SaveLoadManager.main.SetCurrentSlot(result.slotName);
			VRLoadingOverlay.Show();
			if (!PlatformUtils.isPS4Platform)
			{
				UserStorageUtils.AsyncOperation clearSlotTask = SaveLoadManager.main.ClearSlotAsync(result.slotName);
				yield return clearSlotTask;
				if (!clearSlotTask.GetSuccessful())
				{
					Debug.LogError("Clearing save data failed. But we ignore it.");
				}
				clearSlotTask = null;
			}

			GamepadInputModule.current.SetCurrentGrid(null);
			uGUI.main.loading.BeginAsyncSceneLoad("Main");

			yield return new WaitForSecondsRealtime(1f);
			isStartingNewGame = false;
			uGUI.main.loading.End(false);
			//uGUI.main.loading.loadingBackground.FadeOut(0f, null);

			MainMenuMusic.Stop();

			VRLoadingOverlay.Hide();

			yield break;
		}
	}
}
