using System.Collections.Generic;

namespace GFrame
{
    public class GameLocate : IGameLocate
    {
        private Dictionary<string, ILocate> _locations = new Dictionary<string, ILocate>();

        public void RegisterLocate(string name, ILocate location)
        {
            _locations[name] = location;
        }

        public ILocate GetLocate(string name)
        {
            if (_locations.TryGetValue(name, out var locate))
                return locate;
            return null;
        }

        public void OnUpdate()
        {
            foreach (var kv in _locations)
                kv.Value.OnUpdate();
        }
    }
}