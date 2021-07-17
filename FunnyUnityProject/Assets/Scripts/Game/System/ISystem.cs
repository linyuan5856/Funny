namespace GFrame.System
{
    public interface ISystem :ILocation
    {
        void Create(IGameLocate gameLocate);
    }
}