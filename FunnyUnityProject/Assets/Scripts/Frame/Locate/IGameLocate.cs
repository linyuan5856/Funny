namespace GFrame
{
    public interface IGameLocate
    {
        void RegisterLocate(string name, ILocate location);


        ILocate GetLocate(string name);


        void OnUpdate();
    }
}