using GFrame;

namespace FFrame
{
    public class Reference
    {
        private int _count;

        public bool IsUnUsed => _count == 0;

        public void Add()
        {
            _count++;
        }

        public void Release()
        {
            if (_count <= 0)
            {
                GameLog.LogError("can't release any more");
                return;
            }
            _count--;
        }
    }
}