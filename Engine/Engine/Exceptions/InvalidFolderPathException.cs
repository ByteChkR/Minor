using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// This exception gets thrown when the specified file was not found.
    /// </summary>
    public class InvalidFolderPathException : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="folderpath">The Folder that was not found</param>
        /// <param name="inner">Inner exeption</param>
        public InvalidFolderPathException(string folderpath, Exception inner) : base(
            "The folder " + folderpath + " could not be found.", inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="folderpath">The Folder that was not found</param>
        public InvalidFolderPathException(string folderpath) : this(folderpath, null)
        {
        }
    }
}