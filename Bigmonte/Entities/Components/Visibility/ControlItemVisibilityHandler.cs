using Godot;

namespace Bigmonte.Entities
{
    internal class ControlItemVisibilityHandler : VisibilityHandler
    {
        private readonly Control ControlItem;


        public ControlItemVisibilityHandler(Control controlItem)
        {
            ControlItem = controlItem;
        }

        public override bool IsVisible
        {
            get
            {
                if (ControlItem != null) return ControlItem.Visible;

                return false;
            }
        }

        public override void SetVisibility(bool status)
        {
            _visibility = status;
            ControlItem.Visible = status;
        }
    }
}