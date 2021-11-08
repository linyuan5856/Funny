using GFrame;
using GFrame.Service;
using GFrame.System;

namespace FFrame
{
    public static class GameUtil
    {
        public static T GetService<T>(IGameLocate gameLocate) where T : IService
        {
            GameLog.Assert(gameLocate!=null,"param is null");
            var locate = gameLocate.GetLocate(GameDefine.SERVICE_LOCATE) as ServiceLocate;
            return locate.GetService<T>();
        }
        
        public static T GetSystem<T>(IGameLocate gameLocate) where T : ISystem
        {
            GameLog.Assert(gameLocate!=null,"param is null");
            var locate = gameLocate.GetLocate(GameDefine.SYSTEM_LOCATE) as SystemFactory;
            return locate.GetSystem<T>();
        }
    }
}