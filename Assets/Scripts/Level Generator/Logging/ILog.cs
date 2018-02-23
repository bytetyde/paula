namespace Assets.Scripts.Level_Generator.Logging
{
    public interface ILog
    {
        void LogLine(params object[] data);
        void SaveLog();
        void InsertSeparator();
        void InsertNewSection(params object[] data);
    }
}