using FGame;

namespace GFrame
{
    public class ClientGameLoop : IGameLoop
    {
        private IGameLocate _gameLocate;
        private GameContext _context;

        public void Create(IGameLocate locate, GameContext context)
        {
            _gameLocate = locate;
            _context = context;
            _context.ChangeState(GameDefine.HotUpdateState);
        }

        void IGameLoop.OnUpdate()
        {
            _gameLocate?.OnUpdate();
        }
    }
}