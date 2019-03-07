namespace Client.Console
{
    /// <summary>
    /// Default implementation of <see cref="IConsole"/>.
    /// Redirects everything to the <see cref="System.Console"/>.
    /// </summary>
    public class DefaultConsole : IConsole
    {
        public void WriteLine(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }
    }
}
