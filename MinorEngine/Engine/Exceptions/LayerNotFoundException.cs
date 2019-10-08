using System;

namespace Engine.Exceptions
{
    public class LayerNotFoundException : EngineException
    {
        public LayerNotFoundException(int index, Exception inner) : base(
            "The Layer at index " + index + " could not be found.", inner)
        {
        }

        public LayerNotFoundException(int index) : this(index, null)
        {
        }
    }
}