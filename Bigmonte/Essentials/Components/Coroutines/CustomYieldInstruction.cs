using System.Collections;
using Godot;

namespace Bigmonte.Essentials
{
    public abstract class CustomYieldInstruction : Object, IEnumerator
    {
        public abstract bool keepWaiting { get; }

        //
        // Properties
        //
        public object Current => null;

        //
        // Methods
        //
        public bool MoveNext()
        {
            return keepWaiting;
        }

        public void Reset()
        {
        }
    }
}