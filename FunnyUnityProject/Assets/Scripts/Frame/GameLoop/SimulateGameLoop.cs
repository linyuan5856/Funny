using FFrame;
using GFrame.Service;
using UnityEngine;

namespace GFrame
{
    public class SimulateGameLoop : IGameLoop
    {
        private IGameLocate _gameLocate;
        private GameContext _context;

        void IGameLoop.Create(IGameLocate locate, GameContext context)
        {
            _gameLocate = locate;
            _context = context;
            BeginSimulate();
        }

        void IGameLoop.OnUpdate()
        {
            _gameLocate?.OnUpdate();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                var loader = GameUtil.GetService<LoaderService>(_gameLocate);
                loader.UnLoadAssetBundle(testAssetName, true);
            }
        }


        private bool bAsync;
        private const string testAssetName = @"Assets/~Test/~AbFolder/Capsule.prefab";

        void BeginSimulate()
        {
            var loader = GameUtil.GetService<LoaderService>(_gameLocate);
            loader.SetLoader(ELoadType.AssetBundle);
            if (bAsync)
                loader.InstantiateAsync<GameObject>(testAssetName, null);
            else
                loader.Instantiate<GameObject>(testAssetName);
        }
    }
}