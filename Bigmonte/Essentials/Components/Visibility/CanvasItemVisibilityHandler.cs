using Godot;

namespace Bigmonte.Essentials
{
    internal class CanvasItemVisibilityHandler : VisibilityHandler
    {
        private readonly CanvasItem CanvasItem;


        public CanvasItemVisibilityHandler(CanvasItem canvasItem)
        {
            CanvasItem = canvasItem;
        }

        public override bool IsVisible
        {
            get
            {
                if (CanvasItem != null) return CanvasItem.Visible;

                return false;
            }
        }
    }
}