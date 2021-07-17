namespace GFrame
{
    public class ClientGameLoop : IGameLoop
    {
        private IGameLocate _gameLocate;

        void IGameLoop.Create(IGameLocate locate)
        {
            _gameLocate = locate;
        }

        void IGameLoop.OnUpdate()
        {
            _gameLocate?.OnUpdate();
        }
    }
}