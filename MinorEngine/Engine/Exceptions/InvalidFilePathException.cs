using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// This exception gets thrown when the specified file was not found.
    /// </summary>
    public class InvalidFilePathException : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">The File that was not found</param>
        /// <param name="inner">Inner exeption</param>
        public InvalidFilePathException(string filePath, Exception inner) : base(
            "The file " + filePath + " could not be found.", inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Filepath">The File that was not found</param>
        public InvalidFilePathException(string filePath) : this(filePath, null)
        {
        }
    }
}