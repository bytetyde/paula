using System.Diagnostics;

namespace Assets.Scripts.Level_Generator.Stats
{
    public class LevelGenerationLog : IGenerationStatsInterface
    {
        public LevelGenerationLog()
        {
            Setup();
        }

        public long GenerationDuration { get; set; }
        public Stopwatch Stopwatch { get; set; }

        public void Setup()
        {
            GenerationDuration = 0;
            Stopwatch = new Stopwatch();
        }

        public void StartGenerationStopwatch()
        {
            Stopwatch.Start();
        }

        public void StopGenerationStopwatch()
        {
            Stopwatch.Stop();
            GenerationDuration = Stopwatch.ElapsedMilliseconds;
        }

        public void Reset()
        {
            GenerationDuration = 0;
            Stopwatch.Reset();
        }
    }
}