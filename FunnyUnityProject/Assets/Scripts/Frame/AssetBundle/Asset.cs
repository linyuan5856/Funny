namespace FFrame
{
    public class Asset
    {
        private readonly Reference _reference = new Reference();

        protected bool IsUnUsed()
        {
            return _reference.IsUnUsed;
        }

        protected void OnLoad()
        {
            _reference.Add();
        }

        protected void UnLoad()
        {
            _reference.Release();
        }
    }
}