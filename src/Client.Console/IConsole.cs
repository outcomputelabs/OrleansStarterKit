namespace Client.Console
{
    /// <summary>
    /// Abstracts the console for easier testing.
    /// </summary>
    public interface IConsole
    {
        void WriteLine(string format, params object[] args);
    }
}
