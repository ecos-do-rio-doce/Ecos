using UnityEngine;

namespace Ecos
{
    public abstract class State
    {
        public WorldFishing worldFishing;

        public abstract void OnEnterState();
        public abstract void Update();
        public abstract void OnExitState();
    }
}
