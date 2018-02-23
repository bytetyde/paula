using System.Diagnostics;

namespace Assets.Scripts.Level_Generator.Stats
{
    public interface IGenerationStatsInterface
    {
        long GenerationDuration { get; set; }
        Stopwatch Stopwatch { get; set; }

        void Setup();
        void StartGenerationStopwatch();
        void StopGenerationStopwatch();
        void Reset();
    }
}