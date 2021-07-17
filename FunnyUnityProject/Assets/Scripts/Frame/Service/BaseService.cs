namespace GFrame.Service
{
    public class BaseService : IService
    {
        void ILocation.Create()
        {
            OnCreate();
        }

        void ILocation.Update()
        {
            OnUpdate();
        }

        void ILocation.Reset()
        {
            OnReset();
        }

        void ILocation.Destroy()
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