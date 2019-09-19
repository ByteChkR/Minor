using System;

namespace Common.Exceptions
{
    /// <summary>
    /// This exception gets thrown when the specified file was not found.
    /// </summary>
    public class InvalidFolderPathException : ApplicationException
    {

        public InvalidFolderPathException(string folderpath, Exception inner) : base("The folder " + folderpath + " could not be found.", inner)
        {

        }

        public InvalidFolderPathException(string folderpath) : this(folderpath, null)
        {

        }



    }
}