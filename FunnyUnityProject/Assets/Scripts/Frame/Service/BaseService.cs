namespace GFrame.Service
{
    public class BaseService : IService
    {
        void IService.Create()
        {
            OnCreate();
        }

        void IService.Update()
        {
            OnUpdate();
        }

        void IService.Reset()
        {
            OnReset();
        }

        void IService.Destroy()
        {
            OnDestroy();
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnReset()
        {
        }

        protected virtual void OnDestroy()
        {
        }
    }
}