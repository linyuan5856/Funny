using System.Collections.Generic;

namespace GFrame
{
    public class GameLocate : IGameLocate
    {
        private readonly Dictionary<string, ILocate> _locations = new Dictionary<string, ILocate>();

        public void RegisterLocate(string name, ILocate location)
        {
            _locations[name] = location;
        }

        public ILocate GetLocate(string name)
        {
            return _locations.TryGetValue(name, out var locate) ? locate : null;
        }

        public void OnUpdate()
        {
            foreach (var kv in _locations)
                kv.Value.OnUpdate();
        }
    }
}