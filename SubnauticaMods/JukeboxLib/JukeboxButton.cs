namespace JukeboxLib
{
    public class JukeboxButton : HandTarget, IHandTarget
    {
        public System.Action hoverAction = null;
        public System.Action clickAction = null;
        void IHandTarget.OnHandClick(GUIHand hand)
        {
            clickAction?.Invoke();
        }
        void IHandTarget.OnHandHover(GUIHand hand)
        {
            hoverAction?.Invoke();
        }
    }
}
