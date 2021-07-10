using GFrame.Service;
using GFrame.System;
using UnityEngine;

namespace FGame
{
    public class Game
    {
        private ServiceLocate _serviceLocate;
        private SystemFactory _systemFactory;

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
            _serviceLocate = ServiceLocate.Get();
            _serviceLocate.RegisterService<LoaderService>();
            _serviceLocate.RegisterService<AudioService>();

            _systemFactory = SystemFactory.Get(_serviceLocate);
            _systemFactory.CreateSystem<LoginSystem>();
            _systemFactory.CreateSystem<UiSystem>();
            _systemFactory.CreateSystem<BaseSystem>();
        }

        private void OnUpdate()
        {
            _serviceLocate?.Update();
            _systemFactory?.Update();
        }

        void SetGameSetting()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}