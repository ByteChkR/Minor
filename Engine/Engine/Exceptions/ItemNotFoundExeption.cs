using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// Occurs when a file is not found.
    /// </summary>
    public class ItemNotFoundExeption : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inner">Inner exeption</param>
        public ItemNotFoundExeption(string itemType, string desc, Exception inner) : base(
            $"The Item {itemType} could not be Found.\n Description: {desc}", inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ItemNotFoundExeption(string itemType, string desc) : this(itemType, desc, null)
        {
        }
    }
}