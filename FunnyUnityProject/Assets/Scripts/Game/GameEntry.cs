using UnityEngine;

namespace FGame
{
    public class GameEntry : MonoBehaviour
    {
        private Game _game;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            _game = new Game();
        }


        // Update is called once per frame
        void Update()
        {
            _game?.Update();
        }
    }
}