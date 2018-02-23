using System;

namespace Assets.Scripts.Level_Generator.Infinite
{
    [Serializable]
    public class IntStringPair
    {
        public int Key;
        public string Value;

        public IntStringPair(int key = 0, string value = "")
        {
            Key = key;
            Value = value;
        }
    }
}