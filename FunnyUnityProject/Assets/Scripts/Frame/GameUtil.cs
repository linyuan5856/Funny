using GFrame;
using GFrame.Service;

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
    }
}