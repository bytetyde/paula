using System;
using System.Collections.Generic;
using Assets.Scripts.Level_Generator.Populator;

namespace Assets.Scripts.Level_Generator.Infinite
{
    [Serializable]
    public class IntListStringCurveIntPair
    {
        public int Key;
        public List<KeyValueCurveGroup> Value;

        public IntListStringCurveIntPair()
        {
            Key = 0;
            Value = new List<KeyValueCurveGroup>();
        }

        public IntListStringCurveIntPair(int key, List<KeyValueCurveGroup> value)
        {
            Key = key;
            Value = value;
        }
    }
}