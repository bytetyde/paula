namespace Assets.Scripts.Level_Generator.Logging
{
    public class NullLog : ILog
    {
        public void LogLine(params object[] data)
        {
        }

        public void SaveLog()
        {
        }

        public void InsertSeparator()
        {
        }

        public void InsertNewSection(params object[] data)
        {
        }
    }
}