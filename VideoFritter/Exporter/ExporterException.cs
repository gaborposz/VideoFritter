using System;

namespace VideoFritter.Exporter
{
    internal class ExporterException : Exception
    {
        public ExporterException(string commandLineIn, string consoleOutIn)
        {
            this.commandLine = commandLineIn;
            this.consoleOut = consoleOutIn;
        }

        public override string ToString()
        {
            return $"Error during export.\n\nCommand line:\n{this.commandLine}\n\nConsole out:\n{this.consoleOut}\n\nCallStack:\n{this.StackTrace}";
        }

        private readonly string commandLine;
        private readonly string consoleOut;
    }
}
