using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Scripts.Level_Generator.Stats
{
    public interface IPopulationStatsInterface
    {
        List<KeyValPair<string, Dictionary<string, int>>> ObjectCount { get; set; }
        long PopulationDuration { get; set; }
        int Conflicts { get; set; }
        Stopwatch Stopwatch { get; set; }

        void Setup();
        void AddToList(KeyValPair<string, Dictionary<string, int>> obj);
        void AddCountToDictionary(string key, int value, int targetCount);
        void AddConflict(int number);
        void StartPopulationStopwatch();
        void StopPopulationStopwatch();
        int GetOverallObjectCount();
        void Reset();
    }
}