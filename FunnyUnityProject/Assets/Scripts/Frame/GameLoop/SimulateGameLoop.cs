using FGame;
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
        }


        private bool bAsync;

        void BeginSimulate()
        {
            var locate = _gameLocate.GetLocate(GameDefine.SERVICE_LOCATE) as ServiceLocate;
            var loader = locate.GetService<LoaderService>();
            loader.SetLoader(ELoadType.AssetBundle);
            var assetPath = @"Assets/~Test/~AbFolder/Capsule.prefab";
            if (bAsync)
                loader.InstantiateAsync<GameObject>(assetPath,null);
            else
                loader.Instantiate<GameObject>(assetPath);
        }
    }
}