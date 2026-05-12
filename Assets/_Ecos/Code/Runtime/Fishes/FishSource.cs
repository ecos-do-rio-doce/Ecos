using System.Collections.Generic;
using UnityEngine;

namespace Ecos
{
    public class FishSource : MonoBehaviour
    {
        [SerializeField] private List<FishDef> fishesInSource;
        [SerializeField] private List<FishDef> allowedFishes;

        public List<FishDef> FishesInSource { get => fishesInSource; set => fishesInSource = value; }

        public bool IsAllowed(FishDef def)
        {
            return allowedFishes.Contains(def);
        }
    }
}
