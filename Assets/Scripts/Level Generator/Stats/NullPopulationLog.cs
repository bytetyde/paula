using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Scripts.Level_Generator.Stats
{
    public class NullPopulationLog : IPopulationStatsInterface
    {
        public NullPopulationLog()
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
        }

        public void AddCountToDictionary(string key, int value, int targetCount)
        {
        }

        public void AddConflict(int number)
        {
        }

        public void StartPopulationStopwatch()
        {
        }

        public void StopPopulationStopwatch()
        {
        }

        public int GetOverallObjectCount()
        {
            return 0;
        }

        public void Reset()
        {
        }
    }
}