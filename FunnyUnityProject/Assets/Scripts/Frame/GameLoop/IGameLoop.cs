using FFrame;

namespace GFrame
{
    public interface IGameLoop
    {
        void Create(IGameLocate locate,GameContext context);
        void OnUpdate();
    }
}