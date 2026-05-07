using System.Collections.Generic;
using UnityEngine;

namespace Ecos
{
    public class FishSource : MonoBehaviour
    {
        [SerializeField] private List<FishDef> fishesInSource;

        public List<FishDef> FishesInSource { get => fishesInSource; set => fishesInSource = value; }
    }
}
