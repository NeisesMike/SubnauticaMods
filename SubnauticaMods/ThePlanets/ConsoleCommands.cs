using UnityEngine;

namespace ThePlanets
{
	class ConsoleCommands : MonoBehaviour
	{
		private void Awake()
		{
			DevConsole.RegisterConsoleCommand(this, "planetmars", false, false);
			DevConsole.RegisterConsoleCommand(this, "planeturanus", false, false);
			DevConsole.RegisterConsoleCommand(this, "planetvenus", false, false);
			DevConsole.RegisterConsoleCommand(this, "planetjupiter", false, false);
			DevConsole.RegisterConsoleCommand(this, "planetmercury", false, false);
		}
		public void OnConsoleCommand_planetmars(NotificationCenter.Notification _)
		{
			GotoConsoleCommand.main.GotoPosition(MainPatcher.marsLocation + Vector3.up, false);
		}
		public void OnConsoleCommand_planeturanus(NotificationCenter.Notification _)
		{
			GotoConsoleCommand.main.GotoPosition(MainPatcher.uranusLocation + Vector3.up, false);
		}
		public void OnConsoleCommand_planetvenus(NotificationCenter.Notification _)
		{
			GotoConsoleCommand.main.GotoPosition(MainPatcher.venusLocation + Vector3.up, false);
		}
		public void OnConsoleCommand_planetjupiter(NotificationCenter.Notification _)
		{
			GotoConsoleCommand.main.GotoPosition(MainPatcher.jupiterLocation + Vector3.up, false);
		}
		public void OnConsoleCommand_planetmercury(NotificationCenter.Notification _)
		{
			GotoConsoleCommand.main.GotoPosition(MainPatcher.mercuryLocation + Vector3.up, false);
		}
	}
}
