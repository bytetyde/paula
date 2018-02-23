using System;

namespace Assets.Scripts.Common
{
    [Serializable]
    public class KeyValueString : IEquatable<KeyValueString>
    {
        public string key;
        public string value;

        public KeyValueString(string k, string v)
        {
            key = k;
            value = v;
        }

        public KeyValueString()
        {
        }

        public bool Equals(KeyValueString other)
        {
            return value == other.value && key == other.key;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }
    }
}