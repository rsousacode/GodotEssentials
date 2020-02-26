using System;
using System.Runtime.CompilerServices;
using Godot;

namespace Bigmonte.Essentials
{
    public struct MathTools
    {
        //
        // Static Fields
        //
        public const float PI = 3.14159274f;

        public const float Infinity = float.PositiveInfinity;

        public const float NegativeInfinity = float.NegativeInfinity;

        public const float Deg2Rad = 0.0174532924f;

        public const float Rad2Deg = 57.29578f;

        public static readonly float Epsilon = !MathfInternal.IsFlushToZeroEnabled
            ? MathfInternal.FloatMinDenormal
            : MathfInternal.FloatMinNormal;

        //
        // Static Methods
        //
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        public static float Abs(float f)
        {
            return Math.Abs(f);
        }

        public static float MapJava(float value, float istart, float istop, float ostart, float ostop)
        {
            var _a = ostart + (ostop - ostart);
            var _b = value - istart;
            var _c = istop - istart;
            var _d = _a * _b;
            var _r = _d / _c;
            return _r;
        }

        public static float Map(float value, float leftMin, float leftMax, float rightMin, float rightMax)
        {
            // Figure out how 'wide' each range is
            var leftSpan = leftMax - leftMin;
            var rightSpan = rightMax - rightMin;

            // Convert the left range into a 0-1 range (float)
            var valueScaled = (value - leftMin) / leftSpan;

            return rightMin + valueScaled * rightSpan;
        }


        public static float Acos(float f)
        {
            return (float) Math.Acos(f);
        }

        public static bool Approximately(float a, float b)
        {
            return Abs(b - a) < Max(1E-06f * Max(Abs(a), Abs(b)), Epsilon * 8f);
        }

        public static float Asin(float f)
        {
            return (float) Math.Asin(f);
        }

        public static float Ceil(float f)
        {
            return (float) Math.Ceiling(f);
        }

        public static int CeilToInt(float f)
        {
            return (int) Math.Ceiling(f);
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max) value = max;

            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max) value = max;

            return value;
        }

        public static float Clamp01(float value)
        {
            float result;
            if (value < 0f)
                result = 0f;
            else if (value > 1f)
                result = 1f;
            else
                result = value;

            return result;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int ClosestPowerOfTwo(int value);

        public static Color CorrelatedColorTemperatureToRgb(float kelvin)
        {
            Color result;
            INTERNAL_CALL_CorrelatedColorTemperatureToRGB(kelvin, out result);
            return result;
        }

        public static float Cos(float f)
        {
            return (float) Math.Cos(f);
        }

        public static float DeltaAngle(float current, float target)
        {
            var num = Repeat(target - current, 360f);
            if (num > 180f) num -= 360f;

            return num;
        }

        public static float Exp(float power)
        {
            return (float) Math.Exp(power);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern ushort FloatToHalf(float val);

        public static float Floor(float f)
        {
            return (float) Math.Floor(f);
        }

        public static int FloorToInt(float f)
        {
            return (int) Math.Floor(f);
        }

        public static float Gamma(float value, float absmax, float gamma)
        {
            var flag = value < 0f;

            var num = Abs(value);
            float result;
            if (num > absmax)
            {
                result = !flag ? num : -num;
            }
            else
            {
                var num2 = Pow(num / absmax, gamma) * absmax;
                result = !flag ? num2 : -num2;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float GammaToLinearSpace(float value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float HalfToFloat(ushort val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_CorrelatedColorTemperatureToRGB(float kelvin, out Color value);

        public static float InverseLerp(float a, float b, float value)
        {
            float result;
            if (a != b)
                result = Clamp01((value - a) / (b - a));
            else
                result = 0f;

            return result;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsPowerOfTwo(int value);

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }

        public static float LerpAngle(float a, float b, float t)
        {
            var num = Repeat(b - a, 360f);
            if (num > 180f) num -= 360f;

            return a + num * Clamp01(t);
        }

        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float LinearToGammaSpace(float value);

        internal static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        {
            var num = p2.x - p1.x;
            var num2 = p2.y - p1.y;
            var num3 = p4.x - p3.x;
            var num4 = p4.y - p3.y;
            var num5 = num * num4 - num2 * num3;
            bool result2;
            if (num5 == 0f)
            {
                result2 = false;
            }
            else
            {
                var num6 = p3.x - p1.x;
                var num7 = p3.y - p1.y;
                var num8 = (num6 * num4 - num7 * num3) / num5;
                result = new Vector2(p1.x + num8 * num, p1.y + num8 * num2);
                result2 = true;
            }

            return result2;
        }

        internal static bool LineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        {
            var num = p2.x - p1.x;
            var num2 = p2.y - p1.y;
            var num3 = p4.x - p3.x;
            var num4 = p4.y - p3.y;
            var num5 = num * num4 - num2 * num3;
            bool result2;
            if (num5 == 0f)
            {
                result2 = false;
            }
            else
            {
                var num6 = p3.x - p1.x;
                var num7 = p3.y - p1.y;
                var num8 = (num6 * num4 - num7 * num3) / num5;
                if (num8 < 0f || num8 > 1f)
                {
                    result2 = false;
                }
                else
                {
                    var num9 = (num6 * num2 - num7 * num) / num5;
                    if (num9 < 0f || num9 > 1f)
                    {
                        result2 = false;
                    }
                    else
                    {
                        result = new Vector2(p1.x + num8 * num, p1.y + num8 * num2);
                        result2 = true;
                    }
                }
            }

            return result2;
        }

        public static float Log(float f, float p)
        {
            return (float) Math.Log(f, p);
        }

        public static float Log(float f)
        {
            return (float) Math.Log(f);
        }

        public static float Log10(float f)
        {
            return (float) Math.Log10(f);
        }

        public static float Max(float a, float b)
        {
            return a <= b ? b : a;
        }

        public static int Max(int a, int b)
        {
            return a <= b ? b : a;
        }

        public static int Max(params int[] values)
        {
            var num = values.Length;
            int result;
            if (num == 0)
            {
                result = 0;
            }
            else
            {
                var num2 = values[0];
                for (var i = 1; i < num; i++)
                    if (values[i] > num2)
                        num2 = values[i];

                result = num2;
            }

            return result;
        }

        public static float Max(params float[] values)
        {
            var num = values.Length;
            float result;
            if (num == 0)
            {
                result = 0f;
            }
            else
            {
                var num2 = values[0];
                for (var i = 1; i < num; i++)
                    if (values[i] > num2)
                        num2 = values[i];

                result = num2;
            }

            return result;
        }

        public static float Min(float a, float b)
        {
            return a >= b ? b : a;
        }

        public static int Min(params int[] values)
        {
            var num = values.Length;
            int result;
            if (num == 0)
            {
                result = 0;
            }
            else
            {
                var num2 = values[0];
                for (var i = 1; i < num; i++)
                    if (values[i] < num2)
                        num2 = values[i];

                result = num2;
            }

            return result;
        }

        public static int Min(int a, int b)
        {
            return a >= b ? b : a;
        }

        public static float Min(params float[] values)
        {
            var num = values.Length;
            float result;
            if (num == 0)
            {
                result = 0f;
            }
            else
            {
                var num2 = values[0];
                for (var i = 1; i < num; i++)
                    if (values[i] < num2)
                        num2 = values[i];

                result = num2;
            }

            return result;
        }

        public static float MoveTowards(float current, float target, float maxDelta)
        {
            float result;
            if (Abs(target - current) <= maxDelta)
                result = target;
            else
                result = current + Sign(target - current) * maxDelta;

            return result;
        }

        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            var num = DeltaAngle(current, target);
            float result;
            if (-maxDelta < num && num < maxDelta)
            {
                result = target;
            }
            else
            {
                target = current + num;
                result = MoveTowards(current, target, maxDelta);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int NextPowerOfTwo(int value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float PerlinNoise(float x, float y);

        public static float PingPong(float t, float length)
        {
            t = Repeat(t, length * 2f);
            return length - Abs(t - length);
        }

        public static float Pow(float f, float p)
        {
            return (float) Math.Pow(f, p);
        }

        internal static long RandomToLong(Random r)
        {
            var array = new byte[8];
            r.NextBytes(array);
            return (long) (BitConverter.ToUInt64(array, 0) & 9223372036854775807uL);
        }

        public static float Repeat(float t, float length)
        {
            return Clamp(t - Floor(t / length) * length, 0f, length);
        }

        public static float Round(float f)
        {
            return (float) Math.Round(f);
        }

        public static int RoundToInt(float f)
        {
            return (int) Math.Round(f);
        }

        public static float Sign(float f)
        {
            return f < 0f ? -1f : 1f;
        }

        public static float Sin(float f)
        {
            return (float) Math.Sin(f);
        }

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime,
            float maxSpeed)
        {
            var deltaTime = Time.fixedDeltaTime;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime)
        {
            var deltaTime = Time.fixedDeltaTime;
            var maxSpeed = float.PositiveInfinity;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime,
            [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
        {
            smoothTime = Max(0.0001f, smoothTime);
            var num = 2f / smoothTime;
            var num2 = num * deltaTime;
            var num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            var num4 = current - target;
            var num5 = target;
            var num6 = maxSpeed * smoothTime;
            num4 = Clamp(num4, -num6, num6);
            target = current - num4;
            var num7 = (currentVelocity + num * num4) * deltaTime;
            currentVelocity = (currentVelocity - num * num7) * num3;
            var num8 = target + (num4 + num7) * num3;
            if (num5 - current > 0f == num8 > num5)
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }

            return num8;
        }

        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime,
            float maxSpeed)
        {
            var deltaTime = Time.fixedDeltaTime;
            return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime)
        {
            var deltaTime = Time.fixedDeltaTime;
            var maxSpeed = float.PositiveInfinity;
            return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime,
            [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
        {
            target = current + DeltaAngle(current, target);
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static float SmoothStep(float from, float to, float t)
        {
            t = Clamp01(t);
            t = -2f * t * t * t + 3f * t * t;
            return to * t + from * (1f - t);
        }

        public static Vector3 Vector3Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Clamp01(t);
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        public static Vector2 Vector2Lerp(Vector2 a, Vector2 b, float t)
        {
            t = Clamp01(t);
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }


        public static float SqrMagnitudeVector2(Vector2 v)
        {
            return v.x * v.x + v.y * v.y;
        }


        public static float Sqrt(float f)
        {
            return (float) Math.Sqrt(f);
        }

        public static float Tan(float f)
        {
            return (float) Math.Tan(f);
        }


        public static float Wrap(float value, float minVal, float maxVal)
        {
            var f1 = value - minVal;
            var f2 = maxVal - minVal;
            return f1 % f2 + minVal;
        }

        public static Vector3 GetTransformForward(Spatial v)
        {
            var q = new Quat(v.Transform.basis);
            var t = QuatVector3Multiplication(q, Vector3.Forward);
            return t;
        }

        public static void SetTransformForward(Spatial v, Vector3 toSet)
        {
            var t = new Transform();
            t.basis.z = toSet;
            v.Transform = t;
        }


        public static Vector3 GetEulerAngles(Spatial v)
        {
            var q = new Quat(v.Transform.basis);
            var e = q.GetEuler();
            return e;
        }

        public static Vector3 QuatVector3Multiplication(Quat rotation, Vector3 point)
        {
            var num1 = rotation.x * 2f;
            var num2 = rotation.y * 2f;
            var num3 = rotation.z * 2f;
            var num4 = rotation.x * num1;
            var num5 = rotation.y * num2;
            var num6 = rotation.z * num3;
            var num7 = rotation.x * num2;
            var num8 = rotation.x * num3;
            var num9 = rotation.y * num3;
            var num10 = rotation.w * num1;
            var num11 = rotation.w * num2;
            var num12 = rotation.w * num3;
            Vector3 vector3;
            vector3.x = (float) ((1.0 - (num5 + (double) num6)) * point.x + (num7 - (double) num12) * point.y +
                                 (num8 + (double) num11) * point.z);
            vector3.y = (float) ((num7 + (double) num12) * point.x + (1.0 - (num4 + (double) num6)) * point.y +
                                 (num9 - (double) num10) * point.z);
            vector3.z = (float) ((num8 - (double) num11) * point.x + (num9 + (double) num10) * point.y +
                                 (1.0 - (num4 + (double) num5)) * point.z);
            return vector3;
        }
    }
}