using System;

namespace Engine.Exceptions
{
    public class ItemNotFoundExeption : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inner">Inner exeption</param>
        public ItemNotFoundExeption(string itemType, string desc, Exception inner) : base($"The Item {itemType} could not be Found.\n Description: {desc}", inner)
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