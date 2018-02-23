using System;

namespace Assets.Scripts.Level_Generator.Textreplacer_PreSetup
{
    [Serializable]
    public class KeyValueTokenString : IEquatable<KeyValueTokenString>
    {
        public int Difficulty;
        public bool IsCell;
        public bool IsFinalString;
        public bool IsManuallyMapped;
        public bool IsMapped;
        public string KeyString;
        public string ValueString;

        public KeyValueTokenString()
        {
        }

        public KeyValueTokenString(string key, string value, bool cell = false, bool finalString = false,
            bool isMapped = false,
            bool manMapped = false,
            int dif = 50)
        {
            KeyString = key;
            ValueString = value;
            IsCell = cell;
            IsFinalString = finalString;
            IsMapped = isMapped;
            IsManuallyMapped = manMapped;
            Difficulty = dif;
        }

        public bool Equals(KeyValueTokenString other)
        {
            return KeyString == other.KeyString;
        }

        public override int GetHashCode()
        {
            return KeyString.GetHashCode();
        }

        public override string ToString()
        {
            return "Key: " + KeyString + "  " + "Value: " + ValueString + "  " + "Is Cell: " + IsCell + "   " +
                   "Final-string: " + IsFinalString + "  " + "Was generated: " + IsMapped + "  " + "Manually Mapped: " +
                   IsManuallyMapped + "  " + "Difficulty: " + Difficulty;
        }
    }
}