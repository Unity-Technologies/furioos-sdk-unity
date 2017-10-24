//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Singleton component to create and handle <see cref="AutoTween"/> objects.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [ExecuteInEditMode]
    public class TweenManager : MonoBehaviour
    {
        #region shared

        /// <summary>
        /// A pool of AutoTweens that can be recycled.
        /// </summary>
        /// <typeparam name="T">The type of value the AutoTween in the pool will tween.</typeparam>
        [Serializable]
        private class TweenQueue<T> where T : AutoTween, new()
        {
            /// <summary>
            /// The AutoTween in the pool.
            /// </summary>
            [SerializeField]
            private Queue<T> m_Tweens = new Queue<T>();
            /// <summary>
            /// The AutoTween in the pool.
            /// </summary>
            [SerializeField]
            public Queue<T> tweens
            {
                get { return m_Tweens; }
            }

            /// <summary>
            /// Gets an AutoTween from the pool to be used. If none available, one is created.
            /// </summary>
            /// <returns>An AutoTween object of type T.</returns>
            [SerializeField]
            public T GetTween()
            {
                return m_Tweens.Count > 0 ? m_Tweens.Dequeue() : new T();
            }
        }

        /// <summary>
        /// The singleton in-scene instance.
        /// </summary>
        [SerializeField]
        private static TweenManager m_Instance;

        /// <summary>
        /// The singleton in-scene instance.
        /// If null, one is created and returned.
        /// </summary>
        private static TweenManager instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new GameObject("Tween Manager").AddComponent<TweenManager>();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// Gets the total AutoTween count.
        /// </summary>
        public int totalTweenCount
        {
            get { return activeTweenCount + dormantTweenCount; }
        }

        /// <summary>
        /// Gets the active AutoTween count.
        /// </summary>
        public int activeTweenCount
        {
            get { return m_ActiveTweens.Count; }
        }

        /// <summary>
        /// Gets the dormant/queued AutoTween count.
        /// </summary>
        public int dormantTweenCount
        {
            get
            {
                int count = 0;
                count += m_TweenIntQueue.tweens.Count;
                count += m_TweenFloatQueue.tweens.Count;
                count += m_TweenVector2Queue.tweens.Count;
                count += m_TweenVector3Queue.tweens.Count;
                count += m_TweenVector4Queue.tweens.Count;
                count += m_TweenColorQueue.tweens.Count;
                return count;
            }
        }

        /// <summary>
        /// Should the TweenManager be killed in the next Update?
        /// </summary>
        private bool m_ReadyToKill;

        /// <summary>
        /// See MonoBehaviour.OnApplicationQuit.
        /// </summary>
        public void OnApplicationQuit()
        {
            m_ReadyToKill = true;
        }

        /// <summary>
        /// The list of active auto tweens.
        /// </summary>
        [SerializeField]
        private List<AutoTween> m_ActiveTweens = new List<AutoTween>();

        /// <summary>
        /// The current highest AutoTween id.
        /// This value is used as the id for the next-created AutoTween.
        /// </summary>
        private int m_TweenIdCount = 1;

        /// <summary>
        /// Is it currently the first Update cycle?
        /// </summary>
        private bool m_FirstFrame = true;

#if UNITY_EDITOR
        /// <summary>
        /// See MonoBehaviour.Start.
        /// </summary>
        private void Start()
        {
            if (!Application.isPlaying)
            {
                DestroyImmediate(gameObject);
            }
        }
#endif

        /// <summary>
        /// See MonoBehaviour.Update.
        /// </summary>
        private void Update()
        {
            if (m_FirstFrame)
            {
                m_FirstFrame = false;
                return;
            }

            for (int i = 0; i < m_ActiveTweens.Count; i++)
            {
                m_ActiveTweens[i].UpdateTween();
            }

            if (m_ReadyToKill)
            {
                Destroy(gameObject);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(gameObject);
            }
#endif
        }

        /// <summary>
        /// Releases the specified AutoTween back into the queue.
        /// </summary>
        /// <param name="tween">The AutoTween to release.</param>
        public static void Release(AutoTween tween)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            instance.m_ActiveTweens.Remove(tween);

            if (tween.GetType() == typeof(AutoTweenFloat))
            {
                instance.m_TweenFloatQueue.tweens.Enqueue((AutoTweenFloat)tween);
            }
        }

        /// <summary>
        /// Checks if a specified AutoTween is currently active.
        /// </summary>
        /// <param name="id">The AutoTween identifier to search.</param>
        /// <returns>True if the AutoTween is currently active, otherwise false.</returns>
        public static bool TweenIsActive(int id)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return false;
#endif
            for (int i = 0; i < instance.m_ActiveTweens.Count; i++)
            {
                if (instance.m_ActiveTweens[i].tweenId == id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Ends an AutoTween specified.
        /// </summary>
        /// <param name="id">The AutoTween identifier to search.</param>
        /// <param name="callCallback">Should the AutoTween's <see cref="AutoTween.m_Callback"/> be called?</param>
        public static void EndTween(int id, bool callCallback = false)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            for (int i = 0; i < instance.m_ActiveTweens.Count; i++)
            {
                AutoTween tween = instance.m_ActiveTweens[i];

                if (tween.tweenId == id)
                {
                    tween.EndTween(callCallback);
                }
            }
        }

        #endregion

        #region generic

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable to tween.</typeparam>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The inital value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenValue<T>(Action<T> updateValue, T startValue, T targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenValue<T>(updateValue, () => startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable to tween.</typeparam>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenValue<T>(Action<T> updateValue, Func<T> startValue, T targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenValue<T>(updateValue, startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable to tween.</typeparam>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenValue<T>(Action<T> updateValue, Func<T> startValue, Func<T> targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            if (typeof(T) == typeof(float))
            {
                return TweenFloat(updateValue as Action<float>, startValue as Func<float>, targetValue as Func<float>, duration, delay, callback, scaledTime, tweenType);
            }
            else if (typeof(T) == typeof(int))
            {
                return TweenInt(updateValue as Action<int>, startValue as Func<int>, targetValue as Func<int>, duration, delay, callback, scaledTime, tweenType);
            }
            else if (typeof(T) == typeof(Vector2))
            {
                return TweenVector2(updateValue as Action<Vector2>, startValue as Func<Vector2>, targetValue as Func<Vector2>, duration, delay, callback, scaledTime, tweenType);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                return TweenVector3(updateValue as Action<Vector3>, startValue as Func<Vector3>, targetValue as Func<Vector3>, duration, delay, callback, scaledTime, tweenType);
            }
            else if (typeof(T) == typeof(Vector4))
            {
                return TweenVector4(updateValue as Action<Vector4>, startValue as Func<Vector4>, targetValue as Func<Vector4>, duration, delay, callback, scaledTime, tweenType);
            }
            else if (typeof(T) == typeof(Color))
            {
                return TweenColor(updateValue as Action<Color>, startValue as Func<Color>, targetValue as Func<Color>, duration, delay, callback, scaledTime, tweenType);
            }
            else
            {
                Debug.LogWarning("Value type not supported for tweening");
                return 0;
            }
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="T"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable to tween.</typeparam>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The initial value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenTweenValueCustom<T>(Action<T> updateValue, T startValue, T targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenValueCustom<T>(updateValue, () => startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="T"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable to tween.</typeparam>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenValueCustom<T>(Action<T> updateValue, Func<T> startValue, T targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenValueCustom<T>(updateValue, startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="T"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable to tween.</typeparam>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenValueCustom<T>(Action<T> updateValue, Func<T> startValue, Func<T> targetValue, float duration, AnimationCurve animationCurve, float delay = 0, Action callback = null, bool scaledTime = false)
        {
            if (typeof(T) == typeof(float))
            {
                return TweenFloatCustom(updateValue as Action<float>, startValue as Func<float>, targetValue as Func<float>, duration, animationCurve, delay, callback, scaledTime);
            }
            else if (typeof(T) == typeof(int))
            {
                return TweenIntCustom(updateValue as Action<int>, startValue as Func<int>, targetValue as Func<int>, duration, animationCurve, delay, callback, scaledTime);
            }
            else if (typeof(T) == typeof(Vector2))
            {
                return TweenVector2Custom(updateValue as Action<Vector2>, startValue as Func<Vector2>, targetValue as Func<Vector2>, duration, animationCurve, delay, callback, scaledTime);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                return TweenVector3Custom(updateValue as Action<Vector3>, startValue as Func<Vector3>, targetValue as Func<Vector3>, duration, animationCurve, delay, callback, scaledTime);
            }
            else if (typeof(T) == typeof(Vector4))
            {
                return TweenVector4Custom(updateValue as Action<Vector4>, startValue as Func<Vector4>, targetValue as Func<Vector4>, duration, animationCurve, delay, callback, scaledTime);
            }
            else if (typeof(T) == typeof(Color))
            {
                return TweenColorCustom(updateValue as Action<Color>, startValue as Func<Color>, targetValue as Func<Color>, duration, animationCurve, delay, callback, scaledTime);
            }
            else
            {
                Debug.LogWarning("Value type not supported for tweening");
                return 0;
            }
        }

        #endregion

        #region float

        /// <summary>
        /// The TweenQueue of float AutoTweens.
        /// </summary>
        [SerializeField]
        private TweenQueue<AutoTweenFloat> m_TweenFloatQueue = new TweenQueue<AutoTweenFloat>();

        /// <summary>
        /// Creates an AutoTween object that tweens a <see cref="float"/> variable.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The inital value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenFloat(Action<float> updateValue, float startValue, float targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenFloat(updateValue, () => startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="float"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenFloat(Action<float> updateValue, Func<float> startValue, float targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenFloat(updateValue, startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="float"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenFloat(Action<float> updateValue, Func<float> startValue, Func<float> targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenFloat tween = instance.m_TweenFloatQueue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, tweenType, callback, null, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }
        
        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="float"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The initial value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenFloatCustom(Action<float> updateValue, float startValue, float targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenFloatCustom(updateValue, () => startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="float"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenFloatCustom(Action<float> updateValue, Func<float> startValue, float targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenFloatCustom(updateValue, startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="float"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenFloatCustom(Action<float> updateValue, Func<float> startValue, Func<float> targetValue, float duration, AnimationCurve animationCurve, float delay = 0, Action callback = null, bool scaledTime = false)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenFloat tween = instance.m_TweenFloatQueue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, Tween.TweenType.Custom, callback, animationCurve, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        #endregion

        #region int
        
        /// <summary>
        /// The TweenQueue of int AutoTweens.
        /// </summary>
        [SerializeField]
        private TweenQueue<AutoTweenInt> m_TweenIntQueue = new TweenQueue<AutoTweenInt>();

        /// <summary>
        /// Creates an AutoTween object that tweens a <see cref="int"/> variable.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The inital value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenInt(Action<int> updateValue, int startValue, int targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenInt(updateValue, () => startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="int"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenInt(Action<int> updateValue, Func<int> startValue, int targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenInt(updateValue, startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="int"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenInt(Action<int> updateValue, Func<int> startValue, Func<int> targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenInt tween = instance.m_TweenIntQueue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, tweenType, callback, null, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="int"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The initial value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenIntCustom(Action<int> updateValue, int startValue, int targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenIntCustom(updateValue, () => startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="int"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenIntCustom(Action<int> updateValue, Func<int> startValue, int targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenIntCustom(updateValue, startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="int"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenIntCustom(Action<int> updateValue, Func<int> startValue, Func<int> targetValue, float duration, AnimationCurve animationCurve, float delay = 0, Action callback = null, bool scaledTime = false)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenInt tween = instance.m_TweenIntQueue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, Tween.TweenType.Custom, callback, animationCurve, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        #endregion

        #region Vector2

        /// <summary>
        /// The TweenQueue of Vector2 AutoTweens.
        /// </summary>
        [SerializeField]
        private TweenQueue<AutoTweenVector2> m_TweenVector2Queue = new TweenQueue<AutoTweenVector2>();

        /// <summary>
        /// Creates an AutoTween object that tweens a <see cref="Vector2"/> variable.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The inital value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector2(Action<Vector2> updateValue, Vector2 startValue, Vector2 targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenVector2(updateValue, () => startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector2"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector2(Action<Vector2> updateValue, Func<Vector2> startValue, Vector2 targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenVector2(updateValue, startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector2"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector2(Action<Vector2> updateValue, Func<Vector2> startValue, Func<Vector2> targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenVector2 tween = instance.m_TweenVector2Queue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, tweenType, callback, null, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector2"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The initial value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector2Custom(Action<Vector2> updateValue, Vector2 startValue, Vector2 targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenVector2Custom(updateValue, () => startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector2"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector2Custom(Action<Vector2> updateValue, Func<Vector2> startValue, Vector2 targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenVector2Custom(updateValue, startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector2"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector2Custom(Action<Vector2> updateValue, Func<Vector2> startValue, Func<Vector2> targetValue, float duration, AnimationCurve animationCurve, float delay = 0, Action callback = null, bool scaledTime = false)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenVector2 tween = instance.m_TweenVector2Queue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, Tween.TweenType.Custom, callback, animationCurve, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        #endregion

        #region Vector3

        /// <summary>
        /// The TweenQueue of Vector3 AutoTweens.
        /// </summary>
        [SerializeField]
        private TweenQueue<AutoTweenVector3> m_TweenVector3Queue = new TweenQueue<AutoTweenVector3>();

        /// <summary>
        /// Creates an AutoTween object that tweens a <see cref="Vector3"/> variable.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The inital value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector3(Action<Vector3> updateValue, Vector3 startValue, Vector3 targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenVector3(updateValue, () => startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector3"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector3(Action<Vector3> updateValue, Func<Vector3> startValue, Vector3 targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenVector3(updateValue, startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector3"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector3(Action<Vector3> updateValue, Func<Vector3> startValue, Func<Vector3> targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenVector3 tween = instance.m_TweenVector3Queue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, tweenType, callback, null, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector3"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The initial value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector3Custom(Action<Vector3> updateValue, Vector3 startValue, Vector3 targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenVector3Custom(updateValue, () => startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector3"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector3Custom(Action<Vector3> updateValue, Func<Vector3> startValue, Vector3 targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenVector3Custom(updateValue, startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector3"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector3Custom(Action<Vector3> updateValue, Func<Vector3> startValue, Func<Vector3> targetValue, float duration, AnimationCurve animationCurve, float delay = 0, Action callback = null, bool scaledTime = false)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenVector3 tween = instance.m_TweenVector3Queue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, Tween.TweenType.Custom, callback, animationCurve, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        #endregion

        #region Vector4

        /// <summary>
        /// The TweenQueue of Vector4 AutoTweens.
        /// </summary>
        [SerializeField]
        private TweenQueue<AutoTweenVector4> m_TweenVector4Queue = new TweenQueue<AutoTweenVector4>();

        /// <summary>
        /// Creates an AutoTween object that tweens a <see cref="Vector4"/> variable.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The inital value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector4(Action<Vector4> updateValue, Vector4 startValue, Vector4 targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenVector4(updateValue, () => startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector4"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector4(Action<Vector4> updateValue, Func<Vector4> startValue, Vector4 targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenVector4(updateValue, startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector4"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector4(Action<Vector4> updateValue, Func<Vector4> startValue, Func<Vector4> targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenVector4 tween = instance.m_TweenVector4Queue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, tweenType, callback, null, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector4"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The initial value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector4Custom(Action<Vector4> updateValue, Vector4 startValue, Vector4 targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenVector4Custom(updateValue, () => startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector4"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector4Custom(Action<Vector4> updateValue, Func<Vector4> startValue, Vector4 targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenVector4Custom(updateValue, startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Vector4"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenVector4Custom(Action<Vector4> updateValue, Func<Vector4> startValue, Func<Vector4> targetValue, float duration, AnimationCurve animationCurve, float delay = 0, Action callback = null, bool scaledTime = false)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenVector4 tween = instance.m_TweenVector4Queue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, Tween.TweenType.Custom, callback, animationCurve, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        #endregion

        #region Color

        /// <summary>
        /// The TweenQueue of Color AutoTweens.
        /// </summary>
        [SerializeField]
        private TweenQueue<AutoTweenColor> m_TweenColorQueue = new TweenQueue<AutoTweenColor>();

        /// <summary>
        /// Creates an AutoTween object that tweens a <see cref="Color"/> variable.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The inital value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenColor(Action<Color> updateValue, Color startValue, Color targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenColor(updateValue, () => startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Color"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenColor(Action<Color> updateValue, Func<Color> startValue, Color targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
            return TweenColor(updateValue, startValue, () => targetValue, duration, delay, callback, scaledTime, tweenType);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Color"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <param name="tweenType">Type of the tween curve.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenColor(Action<Color> updateValue, Func<Color> startValue, Func<Color> targetValue, float duration, float delay = 0f, Action callback = null, bool scaledTime = false, Tween.TweenType tweenType = Tween.TweenType.EaseOutQuint)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenColor tween = instance.m_TweenColorQueue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, tweenType, callback, null, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Color"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">The initial value of the tween.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenColorCustom(Action<Color> updateValue, Color startValue, Color targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenColorCustom(updateValue, () => startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Color"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">The end value of the tween.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenColorCustom(Action<Color> updateValue, Func<Color> startValue, Color targetValue, float duration, AnimationCurve animationCurve, float delay = 0f, Action callback = null, bool scaledTime = false)
        {
            return TweenColorCustom(updateValue, startValue, () => targetValue, duration, animationCurve, delay, callback, scaledTime);
        }

        /// <summary>
        /// Creates an AutoTween object that tweens a variable of type <see cref="Color"/> that uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="updateValue">This is called to retrieve the target variable's value at any given time.</param>
        /// <param name="startValue">This is called to retrieve a desired initial value at any given time.</param>
        /// <param name="targetValue">This is called to retrieve a desired end value at any given time.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="animationCurve">The custom AnimationCurve to use for the tween.</param>
        /// <param name="delay">The delay betwen when the AutoTween is created and when it begins the tween.</param>
        /// <param name="callback">Called when the tween has finished.</param>
        /// <param name="scaledTime">Should the tween factor in <see cref="Time.timeScale"/>?.</param>
        /// <returns>The id of the tween that was created.</returns>
        public static int TweenColorCustom(Action<Color> updateValue, Func<Color> startValue, Func<Color> targetValue, float duration, AnimationCurve animationCurve, float delay = 0, Action callback = null, bool scaledTime = false)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return -2;
            }
#endif
            AutoTweenColor tween = instance.m_TweenColorQueue.GetTween();

            int id = instance.m_TweenIdCount;
            instance.m_TweenIdCount++;

            tween.Initialize(updateValue, startValue, targetValue, duration, delay, Tween.TweenType.Custom, callback, animationCurve, scaledTime, id);

            instance.m_ActiveTweens.Add(tween);

            return id;
        }

        #endregion
    }
}