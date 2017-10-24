//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// A class used to manually tween values (or find values based on curves).
    /// Many MaterialUI components make use of this class.
    /// </summary>
    public static class Tween
    {
        /// <summary>
        /// The different types/curves that a tween can be.
        /// Set to 'custom' to use a custom AnimationCurve.
        /// </summary>
        public enum TweenType
        {
            Custom,
            Linear,
            Overshoot,
            Bounce,
            EaseInCubed,
            EaseInQuint,
            EaseInSept,
            EaseOutCubed,
            EaseOutQuint,
            EaseOutSept,
            EaseInOutCubed,
            EaseInOutQuint,
            EaseInOutSept,
            SoftEaseOutCubed,
            SoftEaseOutQuint,
            SoftEaseOutSept
        };

        /// <summary>
        /// Generates a set of AnimationCurve keys from a tween type.
        /// Only needed if you want to create your own AnimationCurves from tween functions.
        /// </summary>
        /// <param name="tweenType">Type of the tween.</param>
        /// <returns>
        /// The generated keyset.
        /// </returns>
        public static float[][] GetAnimCurveKeys(TweenType tweenType)
        {
            float[][] keys;

            if (tweenType == TweenType.Linear)
            {
                keys = new[] { new[] { 0, 0f }, new[] { 1f, 1f } };
            }
            else
            {
                keys = new[]
                {
                    new[] { 0, 0f },
                    new[] { 0.1f, 0f },
                    new[] { 0.2f, 0f },
                    new[] { 0.3f, 0f },
                    new[] { 0.4f, 0f },
                    new[] { 0.5f, 0f },
                    new[] { 0.6f, 0f },
                    new[] { 0.7f, 0f },
                    new[] { 0.8f, 0f },
                    new[] { 0.9f, 0f },
                    new[] { 1f, 0f }
                };
            }

            const float start = 0f;
            const float end = 1f;
            float keyLength = keys.Length;

            switch (tweenType)
            {
                case TweenType.Overshoot:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = Overshoot(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.Bounce:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = Bounce(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.EaseInCubed:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = CubeIn(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.EaseOutCubed:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = CubeOut(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.SoftEaseOutCubed:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = CubeSoftOut(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.EaseInOutCubed:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = CubeInOut(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.EaseInQuint:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = QuintIn(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.EaseOutQuint:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = QuintOut(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.SoftEaseOutQuint:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = QuintSoftOut(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.EaseInOutQuint:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = QuintInOut(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.EaseInSept:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = SeptIn(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.EaseOutSept:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = SeptOut(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.SoftEaseOutSept:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = SeptSoftOut(start, end, i / keyLength, end);
                    }
                    break;
                case TweenType.EaseInOutSept:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        keys[i][1] = SeptInOut(start, end, i / keyLength, end);
                    }
                    break;
            }
            return keys;
        }

        /// <summary>
        /// Formats an AnimationCurve's parameters to be used by the 'Evaluate' method.
        /// Clamps the min/max and time to 0-1 and checks that there is a 'start' and 'end' key.
        /// </summary>
        /// <param name="curve">The curve to clamp.</param>
        /// <returns>
        /// The formatted AnimationCurve.
        /// </returns>
        public static AnimationCurve CheckCurve(AnimationCurve curve)
        {
            if (curve.keys.Length < 2)
            {
                curve = new AnimationCurve(new[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
            }
            else if (curve.keys[0].time != 0f || curve.keys[curve.keys.Length - 1].time != 1f)
            {
                curve = new AnimationCurve(new[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
            }

            return curve;
        }

        /// <summary>
        /// Returns a float evaluated from a TweenType curve or AnimationCurve.
        /// </summary>
        /// <param name="type">The TweenType to use, set to 'Custom' to use an AnimationCurve.</param>
        /// <param name="startValue">The start value of the tween.</param>
        /// <param name="endValue">Th end value of the tween.</param>
        /// <param name="time">The current time in the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="curve">The custom AnimationCurve to use, if desired.</param>
        /// <returns>
        /// The calulated value.
        /// </returns>
        public static float Evaluate(TweenType type, float startValue, float endValue, float time, float duration, AnimationCurve curve = null)
        {
            switch (type)
            {
                case TweenType.Linear:
                    return Linear(startValue, endValue, time, duration);
                case TweenType.Overshoot:
                    return Overshoot(startValue, endValue, time, duration);
                case TweenType.Bounce:
                    return Bounce(startValue, endValue, time, duration);
                case TweenType.EaseInCubed:
                    return CubeIn(startValue, endValue, time, duration);
                case TweenType.EaseInQuint:
                    return QuintIn(startValue, endValue, time, duration);
                case TweenType.EaseInSept:
                    return SeptIn(startValue, endValue, time, duration);
                case TweenType.EaseOutCubed:
                    return CubeOut(startValue, endValue, time, duration);
                case TweenType.EaseOutQuint:
                    return QuintOut(startValue, endValue, time, duration);
                case TweenType.EaseOutSept:
                    return SeptOut(startValue, endValue, time, duration);
                case TweenType.EaseInOutCubed:
                    return CubeInOut(startValue, endValue, time, duration);
                case TweenType.EaseInOutQuint:
                    return QuintInOut(startValue, endValue, time, duration);
                case TweenType.EaseInOutSept:
                    return SeptInOut(startValue, endValue, time, duration);
                case TweenType.SoftEaseOutCubed:
                    return CubeSoftOut(startValue, endValue, time, duration);
                case TweenType.SoftEaseOutQuint:
                    return QuintSoftOut(startValue, endValue, time, duration);
                case TweenType.SoftEaseOutSept:
                    return SeptSoftOut(startValue, endValue, time, duration);
                case TweenType.Custom:
                    return (endValue - startValue) * curve.Evaluate(time / duration) + startValue;
            }

            return 0f;
        }

        /// <summary>
        /// Evaluates a point on a linear curve to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float Linear(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);
            time /= duration;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            return differenceValue * time + startValue;
        }

        /// <summary>
        /// Evaluates a point on a linear curve to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 Linear(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = Linear(startValue.x, endValue.x, time, duration);
            tempVector2.y = Linear(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a linear curve to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 Linear(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = Linear(startValue.x, endValue.x, time, duration);
            tempVector3.y = Linear(startValue.y, endValue.y, time, duration);
            tempVector3.z = Linear(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a linear curve to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color Linear(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = Linear(startValue.r, endValue.r, time, duration);
            tempColor.g = Linear(startValue.g, endValue.g, time, duration);
            tempColor.b = Linear(startValue.b, endValue.b, time, duration);
            tempColor.a = Linear(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a sin wave to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="amplitude">The amplitude (min/max values) of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float Sin(float startValue, float amplitude, float time)
        {
            return Mathf.Cos(time) * amplitude + startValue;
        }

        /// <summary>
        /// Evaluates a point on a sin wave to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="amplitude">The amplitude (min/max values) of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 Sin(Vector2 startValue, Vector2 amplitude, float time)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = Sin(startValue.x, amplitude.x, time);
            tempVector2.y = Sin(startValue.y, amplitude.y, time);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a sin wave to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="amplitude">The amplitude (min/max values) of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 Sin(Vector3 startValue, Vector3 amplitude, float time)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = Sin(startValue.x, amplitude.x, time);
            tempVector3.y = Sin(startValue.y, amplitude.y, time);
            tempVector3.z = Sin(startValue.z, amplitude.z, time);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a sin wave to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="amplitude">The amplitude (min/max values) of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color Sin(Color startValue, Color amplitude, float time)
        {
            Color tempColor = startValue;
            tempColor.r = Sin(startValue.r, amplitude.r, time);
            tempColor.g = Sin(startValue.g, amplitude.g, time);
            tempColor.b = Sin(startValue.b, amplitude.b, time);
            tempColor.a = Sin(startValue.a, amplitude.a, time);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on an overshoot tween to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float Overshoot(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);
            time /= duration;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            if (time < 0.6069f)
                return differenceValue * (-(Mathf.Sin(2 * Mathf.PI * time * time)) / (2 * Mathf.PI * time * time) + 1) + startValue;
            if (time < 0.8586f)
                return differenceValue * (-(6.7f * (Mathf.Pow(time - 0.8567f, 2f))) + 1.1f) + startValue;

            return differenceValue * ((5 * Mathf.Pow(time - 1f, 2f)) + 1) + startValue;
        }

        /// <summary>
        /// Evaluates a point on an overshoot tween to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 Overshoot(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = Overshoot(startValue.x, endValue.x, time, duration);
            tempVector2.y = Overshoot(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on an overshoot tween to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 Overshoot(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = Overshoot(startValue.x, endValue.x, time, duration);
            tempVector3.y = Overshoot(startValue.y, endValue.y, time, duration);
            tempVector3.z = Overshoot(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on an overshoot tween to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color Overshoot(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = Overshoot(startValue.r, endValue.r, time, duration);
            tempColor.g = Overshoot(startValue.g, endValue.g, time, duration);
            tempColor.b = Overshoot(startValue.b, endValue.b, time, duration);
            tempColor.a = Overshoot(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a bounce tween to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float Bounce(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);
            time /= duration;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            if (time < 0.75f)
                return differenceValue * (3.16f * time * time * time * time) + startValue;

            return differenceValue * ((8 * Mathf.Pow(time - 0.875f, 2f)) + 0.875f) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a bounce tween to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 Bounce(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = Bounce(startValue.x, endValue.x, time, duration);
            tempVector2.y = Bounce(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a bounce tween to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 Bounce(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = Bounce(startValue.x, endValue.x, time, duration);
            tempVector3.y = Bounce(startValue.y, endValue.y, time, duration);
            tempVector3.z = Bounce(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a bounce tween to return a value.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color Bounce(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = Bounce(startValue.r, endValue.r, time, duration);
            tempColor.g = Bounce(startValue.g, endValue.g, time, duration);
            tempColor.b = Bounce(startValue.b, endValue.b, time, duration);
            tempColor.a = Bounce(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float CubeIn(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);
            time /= duration;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            return differenceValue * time * time * time + startValue;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 CubeIn(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = CubeIn(startValue.x, endValue.x, time, duration);
            tempVector2.y = CubeIn(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 CubeIn(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = CubeIn(startValue.x, endValue.x, time, duration);
            tempVector3.y = CubeIn(startValue.y, endValue.y, time, duration);
            tempVector3.z = CubeIn(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color CubeIn(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = CubeIn(startValue.r, endValue.r, time, duration);
            tempColor.g = CubeIn(startValue.g, endValue.g, time, duration);
            tempColor.b = CubeIn(startValue.b, endValue.b, time, duration);
            tempColor.a = CubeIn(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float CubeOut(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);
            time /= duration;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            time--;
            return differenceValue * (time * time * time + 1) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 CubeOut(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = CubeOut(startValue.x, endValue.x, time, duration);
            tempVector2.y = CubeOut(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 CubeOut(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = CubeOut(startValue.x, endValue.x, time, duration);
            tempVector3.y = CubeOut(startValue.y, endValue.y, time, duration);
            tempVector3.z = CubeOut(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color CubeOut(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = CubeOut(startValue.r, endValue.r, time, duration);
            tempColor.g = CubeOut(startValue.g, endValue.g, time, duration);
            tempColor.b = CubeOut(startValue.b, endValue.b, time, duration);
            tempColor.a = CubeOut(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float CubeInOut(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);

            time /= duration / 2f;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            if (time < 1f)
            {
                return differenceValue / 2 * time * time * time + startValue;
            }
            time -= 2f;
            return differenceValue / 2 * (time * time * time + 2) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 CubeInOut(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = CubeInOut(startValue.x, endValue.x, time, duration);
            tempVector2.y = CubeInOut(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 CubeInOut(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = CubeInOut(startValue.x, endValue.x, time, duration);
            tempVector3.y = CubeInOut(startValue.y, endValue.y, time, duration);
            tempVector3.z = CubeInOut(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color CubeInOut(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = CubeInOut(startValue.r, endValue.r, time, duration);
            tempColor.g = CubeInOut(startValue.g, endValue.g, time, duration);
            tempColor.b = CubeInOut(startValue.b, endValue.b, time, duration);
            tempColor.a = CubeInOut(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float CubeSoftOut(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);

            time /= duration / 2f;

            if (time == 0f)
                return startValue;
            if (time == 0.5f)
                return endValue;

            if (time < 0.559f)
            {
                return differenceValue / 2 * time * time * time * time * time * time * time * 16 + startValue;
            }
            time -= 2f;
            return differenceValue / 2 * (time * time * time * 0.5772f + 2) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 CubeSoftOut(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = CubeSoftOut(startValue.x, endValue.x, time, duration);
            tempVector2.y = CubeSoftOut(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 CubeSoftOut(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = CubeSoftOut(startValue.x, endValue.x, time, duration);
            tempVector3.y = CubeSoftOut(startValue.y, endValue.y, time, duration);
            tempVector3.z = CubeSoftOut(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a cubic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color CubeSoftOut(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = CubeSoftOut(startValue.r, endValue.r, time, duration);
            tempColor.g = CubeSoftOut(startValue.g, endValue.g, time, duration);
            tempColor.b = CubeSoftOut(startValue.b, endValue.b, time, duration);
            tempColor.a = CubeSoftOut(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float QuintIn(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);
            time /= duration;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            return differenceValue * time * time * time * time * time + startValue;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 QuintIn(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = QuintIn(startValue.x, endValue.x, time, duration);
            tempVector2.y = QuintIn(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 QuintIn(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = QuintIn(startValue.x, endValue.x, time, duration);
            tempVector3.y = QuintIn(startValue.y, endValue.y, time, duration);
            tempVector3.z = QuintIn(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color QuintIn(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = QuintIn(startValue.r, endValue.r, time, duration);
            tempColor.g = QuintIn(startValue.g, endValue.g, time, duration);
            tempColor.b = QuintIn(startValue.b, endValue.b, time, duration);
            tempColor.a = QuintIn(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float QuintOut(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);
            time /= duration;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            time--;
            return differenceValue * (time * time * time * time * time + 1) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 QuintOut(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = QuintOut(startValue.x, endValue.x, time, duration);
            tempVector2.y = QuintOut(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 QuintOut(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = QuintOut(startValue.x, endValue.x, time, duration);
            tempVector3.y = QuintOut(startValue.y, endValue.y, time, duration);
            tempVector3.z = QuintOut(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color QuintOut(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = QuintOut(startValue.r, endValue.r, time, duration);
            tempColor.g = QuintOut(startValue.g, endValue.g, time, duration);
            tempColor.b = QuintOut(startValue.b, endValue.b, time, duration);
            tempColor.a = QuintOut(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float QuintInOut(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);

            time /= duration / 2f;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            if (time < 1f)
            {
                return differenceValue / 2 * time * time * time * time * time + startValue;
            }
            time -= 2f;
            return differenceValue / 2 * (time * time * time * time * time + 2) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 QuintInOut(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = QuintInOut(startValue.x, endValue.x, time, duration);
            tempVector2.y = QuintInOut(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 QuintInOut(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = QuintInOut(startValue.x, endValue.x, time, duration);
            tempVector3.y = QuintInOut(startValue.y, endValue.y, time, duration);
            tempVector3.z = QuintInOut(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color QuintInOut(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = QuintInOut(startValue.r, endValue.r, time, duration);
            tempColor.g = QuintInOut(startValue.g, endValue.g, time, duration);
            tempColor.b = QuintInOut(startValue.b, endValue.b, time, duration);
            tempColor.a = QuintInOut(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float QuintSoftOut(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);

            time /= duration / 2f;

            if (time == 0f)
                return startValue;
            if (time == 0.5f)
                return endValue;

            if (time < 0.497f)
            {
                return differenceValue / 2 * time * time * time * time * time * 16 + startValue;
            }
            time -= 2f;
            return differenceValue / 2 * (time * time * time * time * time * 0.1975f + 2) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 QuintSoftOut(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = QuintSoftOut(startValue.x, endValue.x, time, duration);
            tempVector2.y = QuintSoftOut(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 QuintSoftOut(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = QuintSoftOut(startValue.x, endValue.x, time, duration);
            tempVector3.y = QuintSoftOut(startValue.y, endValue.y, time, duration);
            tempVector3.z = QuintSoftOut(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a quintic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color QuintSoftOut(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = QuintSoftOut(startValue.r, endValue.r, time, duration);
            tempColor.g = QuintSoftOut(startValue.g, endValue.g, time, duration);
            tempColor.b = QuintSoftOut(startValue.b, endValue.b, time, duration);
            tempColor.a = QuintSoftOut(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float SeptIn(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);
            time /= duration;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            return differenceValue * time * time * time * time * time * time * time + startValue;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 SeptIn(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = SeptIn(startValue.x, endValue.x, time, duration);
            tempVector2.y = SeptIn(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 SeptIn(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = SeptIn(startValue.x, endValue.x, time, duration);
            tempVector3.y = SeptIn(startValue.y, endValue.y, time, duration);
            tempVector3.z = SeptIn(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' increases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color SeptIn(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = SeptIn(startValue.r, endValue.r, time, duration);
            tempColor.g = SeptIn(startValue.g, endValue.g, time, duration);
            tempColor.b = SeptIn(startValue.b, endValue.b, time, duration);
            tempColor.a = SeptIn(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float SeptOut(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);
            time /= duration;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            time--;
            return differenceValue * (time * time * time * time * time * time * time + 1) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 SeptOut(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = SeptOut(startValue.x, endValue.x, time, duration);
            tempVector2.y = SeptOut(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 SeptOut(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = SeptOut(startValue.x, endValue.x, time, duration);
            tempVector3.y = SeptOut(startValue.y, endValue.y, time, duration);
            tempVector3.z = SeptOut(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color SeptOut(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = SeptOut(startValue.r, endValue.r, time, duration);
            tempColor.g = SeptOut(startValue.g, endValue.g, time, duration);
            tempColor.b = SeptOut(startValue.b, endValue.b, time, duration);
            tempColor.a = SeptOut(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float SeptInOut(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);

            time /= duration / 2f;

            if (time == 0f)
                return startValue;
            if (time == 1f)
                return endValue;

            if (time < 1f)
            {
                return differenceValue / 2 * time * time * time * time * time * time * time + startValue;
            }
            time -= 2f;
            return differenceValue / 2 * (time * time * time * time * time * time * time + 2) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 SeptInOut(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = SeptInOut(startValue.x, endValue.x, time, duration);
            tempVector2.y = SeptInOut(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 SeptInOut(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = SeptInOut(startValue.x, endValue.x, time, duration);
            tempVector3.y = SeptInOut(startValue.y, endValue.y, time, duration);
            tempVector3.z = SeptInOut(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' increases then decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color SeptInOut(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = SeptInOut(startValue.r, endValue.r, time, duration);
            tempColor.g = SeptInOut(startValue.g, endValue.g, time, duration);
            tempColor.b = SeptInOut(startValue.b, endValue.b, time, duration);
            tempColor.a = SeptInOut(startValue.a, endValue.a, time, duration);
            return tempColor;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static float SeptSoftOut(float startValue, float endValue, float time, float duration)
        {
            float differenceValue = endValue - startValue;
            time = Mathf.Clamp(time, 0f, duration);

            time /= duration / 2f;

            if (time == 0f)
                return startValue;
            if (time == 0.5f)
                return endValue;

            if (time < 0.341f)
            {
                return differenceValue / 2 * time * time * time * 16 + startValue;
            }
            time -= 2f;
            return differenceValue / 2 * (time * time * time * time * time * time * time * 0.03948f + 2) + startValue;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector2 SeptSoftOut(Vector2 startValue, Vector2 endValue, float time, float duration)
        {
            Vector2 tempVector2 = startValue;
            tempVector2.x = SeptSoftOut(startValue.x, endValue.x, time, duration);
            tempVector2.y = SeptSoftOut(startValue.y, endValue.y, time, duration);
            return tempVector2;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Vector3 SeptSoftOut(Vector3 startValue, Vector3 endValue, float time, float duration)
        {
            Vector3 tempVector3 = startValue;
            tempVector3.x = SeptSoftOut(startValue.x, endValue.x, time, duration);
            tempVector3.y = SeptSoftOut(startValue.y, endValue.y, time, duration);
            tempVector3.z = SeptSoftOut(startValue.z, endValue.z, time, duration);
            return tempVector3;
        }

        /// <summary>
        /// Evaluates a point on a septic tween to return a value.
        /// 'Velocity' softly decreases over duration.
        /// </summary>
        /// <param name="startValue">The value at the start of the tween.</param>
        /// <param name="endValue">The value at the end of the tween.</param>
        /// <param name="time">The current time of the tween.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <returns>
        /// The calculated value.
        /// </returns>
        public static Color SeptSoftOut(Color startValue, Color endValue, float time, float duration)
        {
            Color tempColor = startValue;
            tempColor.r = SeptSoftOut(startValue.r, endValue.r, time, duration);
            tempColor.g = SeptSoftOut(startValue.g, endValue.g, time, duration);
            tempColor.b = SeptSoftOut(startValue.b, endValue.b, time, duration);
            tempColor.a = SeptSoftOut(startValue.a, endValue.a, time, duration);
            return tempColor;
        }
    }
}