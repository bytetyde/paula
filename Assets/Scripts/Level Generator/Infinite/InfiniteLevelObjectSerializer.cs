using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Infinite
{
    public class InfiniteLevelObjectSerializer : MonoBehaviour
    {
        private int _hash;

        private List<int> _modifiedObjects;
        public bool SectionRemoved;

        public void Setup(List<int> list, int hash)
        {
            _modifiedObjects = list;
            _hash = hash;
            SectionRemoved = false;
        }

        private void OnDestroy()
        {
            if (_modifiedObjects != null && !SectionRemoved)
            {
                _modifiedObjects.Add(_hash);
            }
        }
    }
}