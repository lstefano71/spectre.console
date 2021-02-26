namespace Spectre.Console
{
    /// <summary>
    /// A console capable of writing ANSI escape sequences.
    /// </summary>
    public static partial class AnsiConsole
    {
        /// <summary>
        /// Creates a new <see cref="UpdatePanel"/> instance.
        /// </summary>
        /// <returns>A <see cref="UpdatePanel"/> instance.</returns>
        public static UpdatePanel UpdatePanel()
        {
            return Console.UpdatePanel();
        }
    }
}