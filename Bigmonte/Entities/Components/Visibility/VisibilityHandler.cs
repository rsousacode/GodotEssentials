namespace Bigmonte.Entities
{
    internal class VisibilityHandler
    {
        protected bool _visibility = true;

        public virtual bool IsVisible => _visibility;

        public virtual void SetVisibility(bool status)
        {
            _visibility = status;
        }
    }
}