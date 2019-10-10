using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// This exception gets thrown when a layer was not found in the Physics System.
    /// </summary>
    public class LayerNotFoundException : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">The Layer index that was not found</param>
        /// <param name="inner">Inner exeption</param>
        public LayerNotFoundException(int index, Exception inner) : base(
            "The Layer at index " + index + " could not be found.", inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">The Layer index that was not found</param>
        public LayerNotFoundException(int index) : this(index, null)
        {
        }
    }
}