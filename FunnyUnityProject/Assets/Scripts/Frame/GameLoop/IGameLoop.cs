namespace GFrame
{
    public interface IGameLoop
    {
        void Create(IGameLocate locate);
        void OnUpdate();
    }
}