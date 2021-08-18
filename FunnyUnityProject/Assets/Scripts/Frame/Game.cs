using GFrame;
using GFrame.Service;
using GFrame.StateMachine;
using GFrame.System;
using UnityEngine;

namespace FGame
{
    public class Game
    {
        private IGameLoop _gameLoop;
        private GameLocate _gameLocate;
        private StateMachine _stateMachine;

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
            systemFactory.CreateSystem<BagSystem>();

            _stateMachine = new StateMachine();
            _stateMachine.RegisterState(GameDefine.HotUpdateState, new HotUpdateState());
            _stateMachine.RegisterState(GameDefine.LoginState, new LoginState());
            _stateMachine.RegisterState(GameDefine.GameState, new GameState());
            _stateMachine.RegisterState(GameDefine.BattleState, new BattleState());

            _gameLoop = new ClientGameLoop();
            _gameLoop.Create(_gameLocate, new GameContext(this));
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

        public void ChangeState(int state)
        {
            _stateMachine.ChangeState(state);
        }
    }
}