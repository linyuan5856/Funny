using GFrame;
using GFrame.Service;
using GFrame.System;
using UnityEngine;

namespace FGame
{
    public class Game
    {
        private IGameLoop _gameLoop;
        private GameLocate _gameLocate;

        public Game()
        {
            OnCreate();
        }

        public void Update()
        {
            OnUpdate();
        }

        private void OnCreate()
        {
            SetGameSetting();
            _gameLocate = new GameLocate();
            var serviceLocate = ServiceLocate.Get();
            var systemFactory = SystemFactory.Get(_gameLocate);

            _gameLocate.RegisterLocate(GameDefine.SERVICE_LOCATE, serviceLocate);
            _gameLocate.RegisterLocate(GameDefine.SYSTEM_LOCATE, systemFactory);
            serviceLocate.RegisterService<LoaderService>();
            serviceLocate.RegisterService<AudioService>();

            systemFactory.CreateSystem<LoginSystem>();
            systemFactory.CreateSystem<UiSystem>();
            systemFactory.CreateSystem<BaseSystem>();

            _gameLoop = new ClientGameLoop();
            _gameLoop.Create(_gameLocate);
        }

        private void OnUpdate()
        {
            _gameLoop?.OnUpdate();
        }

        void SetGameSetting()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}