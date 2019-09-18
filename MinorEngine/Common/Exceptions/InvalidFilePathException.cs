using System;

namespace Common.Exceptions
{
    public class InvalidFilePathException : ApplicationException
    {

        public InvalidFilePathException(string filePath, Exception inner) : base("The file " + filePath + " could not be found.", inner)
        {

        }

        public InvalidFilePathException(string filePath) : this(filePath, null)
        {

        }

    }
}