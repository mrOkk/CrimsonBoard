using System.Collections.Generic;

namespace CrimsonBoard
{
    /// <summary>
    /// Owns and drives a list of IGameSystem objects.
    /// Called by GameplayState each frame.
    /// Add systems via RegisterSystem() before Initialize().
    /// </summary>
    public class GameplaySystemRunner
    {
        private readonly List<IGameSystem> _systems = new();

        public void RegisterSystem(IGameSystem system)
        {
            _systems.Add(system);
        }

        public void Initialize()
        {
            for (var index = 0; index < _systems.Count; index++)
            {
                var system = _systems[index];
                system.Initialize();
            }
        }

        public void Tick(float deltaTime)
        {
            for (var index = 0; index < _systems.Count; index++)
            {
                var system = _systems[index];
                system.Tick(deltaTime);
            }
        }

        public void Dispose()
        {
            for (var index = 0; index < _systems.Count; index++)
            {
                var system = _systems[index];
                system.Dispose();
            }

            _systems.Clear();
        }
    }
}
