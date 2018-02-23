using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Common;

namespace Assets.Scripts.Level_Generator.Stats
{
    public class LevelPopulationLog : IPopulationStatsInterface
    {
        public LevelPopulationLog()
        {
            Setup();
        }

        public List<KeyValPair<string, Dictionary<string, int>>> ObjectCount { get; set; }
        public long PopulationDuration { get; set; }
        public int Conflicts { get; set; }
        public Stopwatch Stopwatch { get; set; }

        public void Setup()
        {
            ObjectCount = new List<KeyValPair<string, Dictionary<string, int>>>();
            PopulationDuration = 0;
            Conflicts = 0;
            Stopwatch = new Stopwatch();
        }

        public void AddToList(KeyValPair<string, Dictionary<string, int>> obj)
        {
            ObjectCount.Add(obj);
        }

        public void AddCountToDictionary(string tokenKey, int value, int targetCount = 0)
        {
            foreach (var objectPair in ObjectCount)
            {
                var token = StringHelper.ExceptCharsFromString(tokenKey, new[] {'<', '>'});
                var objPair = StringHelper.ExceptCharsFromString(objectPair._Key, new[] {'<', '>'});

                if (token.Contains(objPair))
                {
                    if (objectPair._Value.ContainsKey(tokenKey))
                    {
                        objectPair._Value[tokenKey] += value;
                    }
                    else
                    {
                        objectPair._Value.Add(tokenKey, value);
                    }
                }
            }
        }

        public void AddConflict(int number)
        {
            Conflicts += number;
        }

        public void StartPopulationStopwatch()
        {
            Stopwatch.Start();
        }

        public void StopPopulationStopwatch()
        {
            Stopwatch.Stop();
            PopulationDuration = Stopwatch.ElapsedMilliseconds;
        }

        public int GetOverallObjectCount()
        {
            return ObjectCount.Sum(item => item._Value.Sum(token => token.Value));
        }

        public void Reset()
        {
            ObjectCount.Clear();
            Stopwatch.Reset();
            PopulationDuration = 0;
            Conflicts = 0;
        }
    }

    [Serializable]
    public class KeyValPair<Key, Val>
    {
        public KeyValPair()
        {
        }

        public KeyValPair(Key key, Val val)
        {
            _Key = key;
            _Value = val;
        }

        public Key _Key { get; set; }
        public Val _Value { get; set; }
    }
}