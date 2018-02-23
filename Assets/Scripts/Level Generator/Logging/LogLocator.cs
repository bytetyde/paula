namespace Assets.Scripts.Level_Generator.Logging
{
    public static class LogLocator
    {
        private static ILog _service;

        public static void ProvideLog(ILog log)
        {
            _service = log;
        }

        public static ILog GetLogger()
        {
            return _service ?? new NullLog();
        }
    }
}