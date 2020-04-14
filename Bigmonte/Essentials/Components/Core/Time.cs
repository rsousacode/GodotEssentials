using Godot;

namespace Bigmonte.Entities
{
    public static class Time
    {
        public static float deltaTime = 0.0f;
        public static float fixedDeltaTime = 0.0f;
        public static float time { get; internal set; }
        public static float realtimeSinceStartup => OS.GetTicksMsec() / 1000.0f;

        public static float timeScale
        {
            get => Engine.TimeScale;
            set => Engine.TimeScale = value;
        }

        public static int frameCount => Engine.GetFramesDrawn();
    }
}