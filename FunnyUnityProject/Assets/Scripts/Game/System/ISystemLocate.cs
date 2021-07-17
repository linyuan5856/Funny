namespace GFrame.System
{
    public interface ISystemLocate : ILocate
    {
        T GetSystem<T>() where T : ISystem;
    }
}