using System;

namespace Common.Exceptions
{
    public class InvalidFolderPathException : ApplicationException
    {

        public InvalidFolderPathException(string folderpath, Exception inner = null) : base("The folder " + folderpath + " could not be found.", inner)
        {

        }

    }
}