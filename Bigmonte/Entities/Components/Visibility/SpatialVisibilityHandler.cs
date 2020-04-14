using Godot;

namespace Bigmonte.Entities
{
    internal class SpatialVisibilityHandler : VisibilityHandler
    {
        private readonly Spatial spatial;


        public SpatialVisibilityHandler(Spatial spatial)
        {
            this.spatial = spatial;
        }

        public override bool IsVisible
        {
            get
            {
                if (spatial != null) return spatial.Visible;

                return false;
            }
        }

        public override void SetVisibility(bool status)
        {
            _visibility = status;
            spatial.Visible = status;
        }
    }
}