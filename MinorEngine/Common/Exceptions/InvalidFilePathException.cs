using System;

namespace Common.Exceptions
{
    public class InvalidFilePathException : ApplicationException
    {

        public InvalidFilePathException(string filePath, Exception inner = null) : base("The file " + filePath + " could not be found.", inner)
        {

        }

    }
}