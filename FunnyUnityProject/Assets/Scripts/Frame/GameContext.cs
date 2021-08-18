namespace FGame
{
    public class GameContext
    {
        private Game _game;

        public GameContext(Game game)
        {
            _game = game;
        }

        public void ChangeState(int state)
        {
            _game.ChangeState(state);
        }
    }
}