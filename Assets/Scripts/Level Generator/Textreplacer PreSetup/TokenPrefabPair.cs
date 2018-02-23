using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Textreplacer_PreSetup
{
    [Serializable]
    public class TokenPrefabPair : IEquatable<TokenPrefabPair>
    {
        [SerializeField] public readonly string Token;

        [SerializeField] public List<string> PrefabPath;

        public TokenPrefabPair(string tok, List<string> pref)
        {
            Token = tok;
            PrefabPath = pref;
        }

        public bool Equals(TokenPrefabPair obj)
        {
            var other = obj;
            return Token == other.Token;
        }

        public override int GetHashCode()
        {
            return Token.GetHashCode();
        }

        public override string ToString()
        {
            return "Token: " + Token + "\t" + "Prefab Path: " + PrefabPath;
        }
    }
}