namespace Assets.Scripts.Level_Generator.Stats
{
    public static class StatsLocator
    {
        private static IPopulationStatsInterface _levelPopulationService;
        private static IGenerationStatsInterface _levelGenerationService;

        public static IPopulationStatsInterface GetLevelPopulationLog()
        {
            return _levelPopulationService;
        }

        public static void ProvideLevelPopulationStats(IPopulationStatsInterface levelPopulationLog)
        {
            _levelPopulationService = levelPopulationLog;
        }

        public static IGenerationStatsInterface GetLevelGenerationLog()
        {
            return _levelGenerationService;
        }

        public static void ProvideLevelGenerationLog(IGenerationStatsInterface levelGenerationLog)
        {
            _levelGenerationService = levelGenerationLog;
        }
    }
}