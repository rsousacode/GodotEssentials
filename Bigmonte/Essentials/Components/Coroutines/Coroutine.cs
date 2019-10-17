using System.Collections;
using System.Runtime.InteropServices;

namespace Bigmonte.Essentials
{
    [RequiredByNativeCode]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Coroutine : CustomYieldInstruction
    {
        private readonly IEnumerator routine;


        public Coroutine(IEnumerator routine)
        {
            this.routine = routine;
        }


        public override bool keepWaiting => routine.MoveNext();
    }
}