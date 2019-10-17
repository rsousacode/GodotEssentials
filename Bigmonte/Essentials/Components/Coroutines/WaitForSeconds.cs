namespace Bigmonte.Essentials
{
    public class WaitForSeconds : CustomYieldInstruction
    {
        public float finishTime;


        public WaitForSeconds(float seconds)
        {
            finishTime = Time.time + seconds;
        }


        public override bool keepWaiting => finishTime > Time.time;
    }
}