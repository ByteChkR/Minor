using System;

namespace Engine.Exceptions
{


    /// <summary>
    /// Occurs when a Shader Fails to compile or some openGL calls went wrong
    /// </summary>
    public class OpenGLShaderException : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inner">Inner exeption</param>
        public OpenGLShaderException(string errorMessage, Exception inner) : base(
            $"Could not Create shader from Source:\n{errorMessage}", inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public OpenGLShaderException(string errorMessage) : this(errorMessage, null)
        {
        }
    }
}