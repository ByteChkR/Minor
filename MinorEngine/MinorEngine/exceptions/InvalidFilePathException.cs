using System;

namespace MinorEngine.exceptions
{
    /// <summary>
    /// This exception gets thrown when the specified file was not found.
    /// </summary>
    public class InvalidFilePathException : EngineException
    {
        public InvalidFilePathException(string filePath, Exception inner) : base(
            "The file " + filePath + " could not be found.", inner)
        {
        }

        public InvalidFilePathException(string filePath) : this(filePath, null)
        {
        }
    }
}