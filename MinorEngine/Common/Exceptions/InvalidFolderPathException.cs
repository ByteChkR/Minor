using System;

namespace Common.Exceptions
{
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