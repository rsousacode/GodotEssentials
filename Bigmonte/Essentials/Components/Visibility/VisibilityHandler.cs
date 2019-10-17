namespace Bigmonte.Essentials
{
    internal class VisibilityHandler
    {
        public virtual bool IsVisible
        {
            get { return _visibility; }
            
        }

        protected bool _visibility = true;

        public virtual void SetVisibility(bool status)
        {
            _visibility = status;
        }
    }
}