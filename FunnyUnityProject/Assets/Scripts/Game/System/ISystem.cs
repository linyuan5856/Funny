using GFrame.Service;

namespace GFrame.System
{
    public interface ISystem
    {
        void Create(IServiceLocate locate);
        void Update();
    }
}