using System.Collections.Generic;
using System.Collections;

namespace JukeboxLib
{
    internal class JukeboxDisk : Pickupable, ISecondaryTooltip
    {
        internal static readonly Dictionary<TechType, string> displayNames = new Dictionary<TechType, string>();
        string ISecondaryTooltip.GetSecondaryTooltip()
        {
            return displayNames[GetTechType()];
        }
        public override void Awake()
        {
            base.Awake();
            pickedUpEvent.AddHandler(gameObject, new UWE.Event<Pickupable>.HandleFunction(OnPickedUp));
        }
        private void OnPickedUp(Pickupable pickupable)
        {
            TechType thisDiskTT = GetTechType();
            IEnumerator DestroyMeInAMoment()
            {
                yield return null;
                Inventory.main.DestroyItem(thisDiskTT, false);
            }
            UWE.CoroutineHost.StartCoroutine(DestroyMeInAMoment());
            if (Story.StoryGoalManager.main.OnGoalComplete(BuildStoryGoalString(thisDiskTT)))
            {
                JukeboxLibrary.UnlockSong(thisDiskTT);
            }
        }
        internal static string BuildStoryGoalString(TechType tt)
        {
            return $"JukeboxLib{tt.AsString()}Goal";
        }
    }
}
