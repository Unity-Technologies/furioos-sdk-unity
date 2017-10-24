//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Tween object that modifies a value each frame, over a specified period of time.
    /// </summary>
    public abstract class AutoTween
    {
        /// <summary> Used by TweenManager to identify the tween. Unique for each and every AutoTween. </summary>
        protected int m_TweenId = -1;
        /// <summary> Used by TweenManager to identify the tween. Unique for each and every AutoTween. </summary>
        public int tweenId
        {
            get { return m_TweenId; }
        }

        /// <summary> Should the tween be updated each frame by TweenManager? </summary>
        protected bool m_Active;

        /// <summary> Should the tween wait for the delay to timeout before modifying anything? </summary>
        protected bool m_WaitingForDelay;

        /// <summary> The action to call once the tween has finished. </summary>
        protected Action m_Callback;

        /// <summary> The time that the tween started being active. </summary>
        protected float m_StartTime;

        /// <summary> The time since the tween was last updated. </summary>
        protected float m_DeltaTime;

        /// <summary> The duration the tween should be, not including the delay. </summary>
        protected float m_Duration;

        /// <summary> The delay that the tween should wait after being created, but before modifying anything. </summary>
        protected float m_Delay;

        /// <summary> Should the tween factor in Time.timeScale when running? </summary>
        protected bool m_ScaledTime;

        /// <summary> The generated AnimationCurves to use to tween the variable. The number of curves should match the number of elements in the variable. </summary>
        private AnimationCurve[] m_AnimationCurves;

        /// <summary> The curve type of the tween. </summary>
        protected Tween.TweenType m_TweenType;

        /// <summary> If m_TweenType is 'custom', the custom curve the tween should instead use. </summary>
        protected AnimationCurve m_CustomCurve;

        /// <summary> The number of elements in the value type being modified (eg. 1 for float/int, 2 for Vector2, 4 for Color). </summary>
        protected abstract int ValueLength();

        /// <summary>
        /// The method to be called on each tween update, excluding the final one.
        /// </summary>
        protected abstract void OnUpdateValue();

        /// <summary>
        /// The method to be called on the final tween update.
        /// </summary>
        protected abstract void OnFinalUpdateValue();

        /// <summary>
        /// Gets a value from an element of either the initial or target value of the tween.
        /// For example, GetValue(false, 0) would get the first element of the intial value.
        /// Used to make inherited AutoTweens for different value types easier.
        /// </summary>
        /// <param name="isEnd">Get the value from the target value of the tween? Otherwise get the value from the intial value.</param>
        /// <param name="valueIndex">The index of the value within the value being checked. Only used if the value being checked has more than 1 element (eg. Vector2).</param>
        /// <returns>The desired value within the initial or target values of the tween.</returns>
        protected abstract float GetValue(bool isEnd, int valueIndex);

        /// <summary>
        /// Called by TweenManager, initializes the tween.
        /// </summary>
        /// <param name="duration">The duration to run the tween, not including delay.</param>
        /// <param name="delay">The delay that the tween should wait after being created, but before modifying anything.</param>
        /// <param name="tweenType">The curve type of the tween.</param>
        /// <param name="callback">The action to call once the tween has finished.</param>
        /// <param name="animationCurve">TODO</param>
        /// <param name="scaledTime">Should the tween factor in Time.timeScale when running?</param>
        /// <param name="id">Used by TweenManager to identify the tween. Unique for each and every AutoTween.</param>
        public void Initialize(float duration, float delay, Tween.TweenType tweenType, Action callback, AnimationCurve animationCurve, bool scaledTime, int id)
        {
            m_Duration = duration;
            m_Delay = delay;
            m_TweenType = tweenType;
            m_Callback = callback;
            m_CustomCurve = animationCurve;
            m_ScaledTime = scaledTime;
            m_TweenId = id;

            if (m_Delay > 0)
            {
                m_WaitingForDelay = true;
            }
            else
            {
                m_WaitingForDelay = false;
                StartTween();
            }

            m_Active = true;
        }

        /// <summary>
        /// Called once the delay has reached 0, prepares the tween to start modifying the target variable.
        /// </summary>
        protected virtual void StartTween()
        {
            SetupCurves();
            m_StartTime = m_ScaledTime ? Time.time : Time.unscaledTime;
        }

        /// <summary>
        /// Called once each frame by TweenManager, modifies the target variable.
        /// </summary>
        public virtual void UpdateTween()
        {
            if (!m_Active) return;

            if (m_WaitingForDelay)
            {
                m_Delay -= m_ScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;

                if (m_Delay <= 0)
                {
                    StartTween();
                    m_WaitingForDelay = false;
                }
            }
            else
            {
                m_DeltaTime = m_ScaledTime ? Time.time : Time.unscaledTime - m_StartTime;

                if (m_DeltaTime < m_Duration)
                {
                    try
                    {
                        OnUpdateValue();
                    }
                    catch (Exception)
                    {
                        EndTween(false);
                    }
                }
                else
                {
                    try
                    {
                        OnFinalUpdateValue();
                    }
                    catch
                    {
                        //  ignored
                    }

                    m_Active = false;
                    EndTween(true);
                }
            }
        }

        /// <summary>
        /// Stops the tween from further modifying the target variable and releases it back into the AutoTween pool in TweenManager.
        /// </summary>
        /// <param name="callback">Should the callback be called?</param>
        public void EndTween(bool callback)
        {
            if (callback)
            {
                m_Callback.InvokeIfNotNull();
            }

            m_TweenId = -1;
            TweenManager.Release(this);
        }

        /// <summary>
        /// Creates a smoothed AnimationCurve from either the tween type, or custom AnimationCurve, to use to tween the variable.
        /// </summary>
        private void SetupCurves()
        {
            m_AnimationCurves = new AnimationCurve[ValueLength()];
            Keyframe[][] keyframes = new Keyframe[ValueLength()][];

            if (m_TweenType == Tween.TweenType.Custom)
            {
                for (int i = 0; i < ValueLength(); i++)
                {
                    keyframes[i] = m_CustomCurve.keys;
                }
            }
            else
            {
                float[][] source = Tween.GetAnimCurveKeys(m_TweenType);

                for (int i = 0; i < ValueLength(); i++)
                {
                    keyframes[i] = new Keyframe[source.Length];

                    for (int j = 0; j < keyframes[i].Length; j++)
                    {
                        keyframes[i][j].time = source[j][0];
                        keyframes[i][j].value = source[j][1];
                    }
                }
            }

            for (int i = 0; i < ValueLength(); i++)
            {
                for (int j = 0; j < keyframes[i].Length; j++)
                {
                    keyframes[i][j].value *= GetValue(true, i) - GetValue(false, i);
                    keyframes[i][j].value += GetValue(false, i);
                    keyframes[i][j].time *= m_Duration;
                }

                m_AnimationCurves[i] = new AnimationCurve(keyframes[i]);

                if (m_CustomCurve == null)
                {
                    for (int j = 0; j < keyframes[i].Length; j++)
                    {
                        m_AnimationCurves[i].SmoothTangents(j, 0f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tween object that modifies an int each frame, over a specified period of time.
    /// </summary>
    public class AutoTweenInt : AutoTween
    {
        /// <summary>The Func to get the tween's start int.</summary>
        private Func<int> m_GetStartValue;

        /// <summary>The Func to get the tween's end int.</summary>
        private Func<int> m_GetTargetValue;

        /// <summary>The Action to update the tween's target variable.</summary>
        private Action<int> m_UpdateValue;

        /// <summary>The tween's start int.</summary>
        private int m_StartValue;

        /// <summary>The tween's end int.</summary>
        private int m_TargetValue;

        /// <summary>
        /// Gets a value from an element of either the initial or target value of the tween.
        /// For example, GetValue(false, 0) would get the first element of the intial value.
        /// Used to make inherited AutoTweens for different value types easier.
        /// </summary>
        /// <param name="isEnd">Get the value from the target value of the tween? Otherwise get the value from the intial value.</param>
        /// <param name="valueIndex">The index of the value within the value being checked. Only used if the value being checked has more than 1 element (eg. Vector2).</param>
        /// <returns>
        /// The desired value within the initial or target values of the tween.
        /// </returns>
        protected override float GetValue(bool isEnd, int valueIndex)
        {
            return (isEnd ? m_GetTargetValue() : m_GetStartValue());
        }

        /// <summary>
        /// The number of elements in the value type being modified (eg. 1 for float/int, 2 for Vector2, 4 for Color).
        /// </summary>
        /// <returns></returns>
        protected override int ValueLength()
        {
            return 1;
        }

        /// <summary>
        /// Initializes the specified update value.
        /// </summary>
        /// <param name="updateValue">The update value.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="tweenType">Type of the tween.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="animationCurve">The animation curve.</param>
        /// <param name="scaledTime">if set to <c>true</c> [scaled time].</param>
        /// <param name="id">The identifier.</param>
        public void Initialize(Action<int> updateValue, Func<int> startValue, Func<int> targetValue, float duration, float delay, Tween.TweenType tweenType, Action callback, AnimationCurve animationCurve, bool scaledTime, int id)
        {
            m_GetStartValue = startValue;
            m_UpdateValue = updateValue;
            m_GetTargetValue = targetValue;

            Initialize(duration, delay, tweenType, callback, animationCurve, scaledTime, id);
        }

        /// <summary>
        /// Called once the delay has reached 0, prepares the tween to start modifying the target variable.
        /// </summary>
        protected override void StartTween()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            base.StartTween();

            m_StartValue = m_GetStartValue();
            m_TargetValue = m_GetTargetValue();
        }

        /// <summary>
        /// The method to be called on each tween update, excluding the final one.
        /// </summary>
        protected override void OnUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            m_UpdateValue(Mathf.RoundToInt(Tween.Evaluate(m_TweenType, m_StartValue, m_TargetValue, m_DeltaTime, m_Duration, m_CustomCurve)));
        }

        /// <summary>
        /// The method to be called on the final tween update.
        /// </summary>
        protected override void OnFinalUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                return;
            }

            m_UpdateValue(m_TargetValue);
        }
    }

    /// <summary>
    /// Tween object that modifies a float each frame, over a specified period of time.
    /// </summary>
    public class AutoTweenFloat : AutoTween
    {
        /// <summary>
        /// The Func to get the tween's start float.
        /// </summary>
        private Func<float> m_GetStartValue;

        /// <summary>
        /// The Func to get the tween's end float.
        /// </summary>
        private Func<float> m_GetTargetValue;

        /// <summary>
        /// The Action to update the tween's target variable.
        /// </summary>
        private Action<float> m_UpdateValue;

        /// <summary>
        /// The tween's start float.
        /// </summary>
        private float m_StartValue;

        /// <summary>
        /// The tween's end float.
        /// </summary>
        private float m_TargetValue;

        /// <summary>
        /// Gets a value from an element of either the initial or target value of the tween.
        /// For example, GetValue(false, 0) would get the first element of the intial value.
        /// Used to make inherited AutoTweens for different value types easier.
        /// </summary>
        /// <param name="isEnd">Get the value from the target value of the tween? Otherwise get the value from the intial value.</param>
        /// <param name="valueIndex">The index of the value within the value being checked. Only used if the value being checked has more than 1 element (eg. Vector2).</param>
        /// <returns>
        /// The desired value within the initial or target values of the tween.
        /// </returns>
        protected override float GetValue(bool isEnd, int valueIndex)
        {
            return (isEnd ? m_GetTargetValue() : m_GetStartValue());
        }

        /// <summary>
        /// The number of elements in the value type being modified (eg. 1 for float/int, 2 for Vector2, 4 for Color).
        /// </summary>
        /// <returns></returns>
        protected override int ValueLength()
        {
            return 1;
        }

        /// <summary>
        /// Initializes the specified update value.
        /// </summary>
        /// <param name="updateValue">The update value.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="tweenType">Type of the tween.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="animationCurve">The animation curve.</param>
        /// <param name="scaledTime">if set to <c>true</c> [scaledTime].</param>
        /// <param name="id">The identifier.</param>
        public void Initialize(Action<float> updateValue, Func<float> startValue, Func<float> targetValue, float duration, float delay, Tween.TweenType tweenType, Action callback, AnimationCurve animationCurve, bool scaledTime, int id)
        {
            m_GetStartValue = startValue;
            m_UpdateValue = updateValue;
            m_GetTargetValue = targetValue;

            Initialize(duration, delay, tweenType, callback, animationCurve, scaledTime, id);
        }

        /// <summary>
        /// Called once the delay has reached 0, prepares the tween to start modifying the target variable.
        /// </summary>
        protected override void StartTween()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            base.StartTween();

            try
            {
                m_StartValue = m_GetStartValue();
                m_TargetValue = m_GetTargetValue();
            }
            catch (Exception)
            {
                EndTween(false);
            }
        }

        /// <summary>
        /// The method to be called on each tween update, excluding the final one.
        /// </summary>
        protected override void OnUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            m_UpdateValue(Tween.Evaluate(m_TweenType, m_StartValue, m_TargetValue, m_DeltaTime, m_Duration, m_CustomCurve));
        }

        /// <summary>
        /// The method to be called on the final tween update.
        /// </summary>
        protected override void OnFinalUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                return;
            }

            m_UpdateValue(m_TargetValue);
        }
    }

    /// <summary>
    /// Tween object that modifies a Vector2 each frame, over a specified period of time.
    /// </summary>
    public class AutoTweenVector2 : AutoTween
    {
        /// <summary>
        /// The Func to get the tween's start Vector2.
        /// </summary>
        private Func<Vector2> m_GetStartValue;

        /// <summary>
        /// The Func to get the tween's end Vector2.
        /// </summary>
        private Func<Vector2> m_GetTargetValue;

        /// <summary>
        /// The Action to update the tween's target variable.
        /// </summary>
        private Action<Vector2> m_UpdateValue;

        /// <summary>
        /// The tween's start Vector2.
        /// </summary>
        private Vector2 m_StartValue;

        /// <summary>
        /// The tween's end Vector2.
        /// </summary>
        private Vector2 m_TargetValue;

        /// <summary>
        /// Gets a value from an element of either the initial or target value of the tween.
        /// For example, GetValue(false, 0) would get the first element of the intial value.
        /// Used to make inherited AutoTweens for different value types easier.
        /// </summary>
        /// <param name="isEnd">Get the value from the target value of the tween? Otherwise get the value from the intial value.</param>
        /// <param name="valueIndex">The index of the value within the value being checked. Only used if the value being checked has more than 1 element (eg. Vector2).</param>
        /// <returns>
        /// The desired value within the initial or target values of the tween.
        /// </returns>
        protected override float GetValue(bool isEnd, int valueIndex)
        {
            return (isEnd ? m_GetTargetValue()[valueIndex] : m_GetStartValue()[valueIndex]);
        }

        /// <summary>
        /// The number of elements in the value type being modified (eg. 1 for float/int, 2 for Vector2, 4 for Color).
        /// </summary>
        /// <returns></returns>
        protected override int ValueLength()
        {
            return 2;
        }

        /// <summary>
        /// Initializes the specified update value.
        /// </summary>
        /// <param name="updateValue">The update value.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="tweenType">Type of the tween.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="animationCurve">The animation curve.</param>
        /// <param name="scaledTime">if set to <c>true</c> [scaled time].</param>
        /// <param name="id">The identifier.</param>
        public void Initialize(Action<Vector2> updateValue, Func<Vector2> startValue, Func<Vector2> targetValue, float duration, float delay, Tween.TweenType tweenType, Action callback, AnimationCurve animationCurve, bool scaledTime, int id)
        {
            m_GetStartValue = startValue;
            m_UpdateValue = updateValue;
            m_GetTargetValue = targetValue;

            base.Initialize(duration, delay, tweenType, callback, animationCurve, scaledTime, id);
        }

        /// <summary>
        /// Called once the delay has reached 0, prepares the tween to start modifying the target variable.
        /// </summary>
        protected override void StartTween()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            base.StartTween();

            try
            {
                m_StartValue = m_GetStartValue();
                m_TargetValue = m_GetTargetValue();
            }
            catch (Exception)
            {
                EndTween(false);
            }
        }

        /// <summary>
        /// The method to be called on each tween update, excluding the final one.
        /// </summary>
        protected override void OnUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            Vector2 value = new Vector2
            {
                x = Tween.Evaluate(m_TweenType, m_StartValue.x, m_TargetValue.x, m_DeltaTime, m_Duration, m_CustomCurve),
                y = Tween.Evaluate(m_TweenType, m_StartValue.y, m_TargetValue.y, m_DeltaTime, m_Duration, m_CustomCurve)
            };
            m_UpdateValue(value);
        }

        /// <summary>
        /// The method to be called on the final tween update.
        /// </summary>
        protected override void OnFinalUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                return;
            }

            m_UpdateValue(m_TargetValue);
        }
    }

    /// <summary>
    /// Tween object that modifies a Vector3 each frame, over a specified period of time.
    /// </summary>
    public class AutoTweenVector3 : AutoTween
    {
        /// <summary>
        /// The Func to get the tween's start Vector3.
        /// </summary>
        private Func<Vector3> m_GetStartValue;

        /// <summary>
        /// The Func to get the tween's end Vector3.
        /// </summary>
        private Func<Vector3> m_GetTargetValue;

        /// <summary>
        /// The Action to update the tween's target variable.
        /// </summary>
        private Action<Vector3> m_UpdateValue;

        /// <summary>
        /// The tween's start Vector3.
        /// </summary>
        private Vector3 m_StartValue;

        /// <summary>
        /// The tween's end Vector3.
        /// </summary>
        private Vector3 m_TargetValue;

        /// <summary>
        /// Gets a value from an element of either the initial or target value of the tween.
        /// For example, GetValue(false, 0) would get the first element of the intial value.
        /// Used to make inherited AutoTweens for different value types easier.
        /// </summary>
        /// <param name="isEnd">Get the value from the target value of the tween? Otherwise get the value from the intial value.</param>
        /// <param name="valueIndex">The index of the value within the value being checked. Only used if the value being checked has more than 1 element (eg. Vector2).</param>
        /// <returns>
        /// The desired value within the initial or target values of the tween.
        /// </returns>
        protected override float GetValue(bool isEnd, int valueIndex)
        {
            return (isEnd ? m_GetTargetValue()[valueIndex] : m_GetStartValue()[valueIndex]);
        }

        /// <summary>
        /// The number of elements in the value type being modified (eg. 1 for float/int, 2 for Vector2, 4 for Color).
        /// </summary>
        /// <returns></returns>
        protected override int ValueLength()
        {
            return 3;
        }

        /// <summary>
        /// Initializes the specified update value.
        /// </summary>
        /// <param name="updateValue">The update value.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="tweenType">Type of the tween.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="animationCurve">The animation curve.</param>
        /// <param name="scaledTime">if set to <c>true</c> [scaled time].</param>
        /// <param name="id">The identifier.</param>
        public void Initialize(Action<Vector3> updateValue, Func<Vector3> startValue, Func<Vector3> targetValue, float duration, float delay, Tween.TweenType tweenType, Action callback, AnimationCurve animationCurve, bool scaledTime, int id)
        {
            m_GetStartValue = startValue;
            m_UpdateValue = updateValue;
            m_GetTargetValue = targetValue;

            Initialize(duration, delay, tweenType, callback, animationCurve, scaledTime, id);
        }

        /// <summary>
        /// Called once the delay has reached 0, prepares the tween to start modifying the target variable.
        /// </summary>
        protected override void StartTween()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            base.StartTween();

            try
            {
                m_StartValue = m_GetStartValue();
                m_TargetValue = m_GetTargetValue();
            }
            catch (Exception)
            {
                EndTween(false);
            }
        }

        /// <summary>
        /// The method to be called on each tween update, excluding the final one.
        /// </summary>
        protected override void OnUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            Vector3 value = new Vector3
            {
                x = Tween.Evaluate(m_TweenType, m_StartValue.x, m_TargetValue.x, m_DeltaTime, m_Duration, m_CustomCurve),
                y = Tween.Evaluate(m_TweenType, m_StartValue.y, m_TargetValue.y, m_DeltaTime, m_Duration, m_CustomCurve),
                z = Tween.Evaluate(m_TweenType, m_StartValue.z, m_TargetValue.z, m_DeltaTime, m_Duration, m_CustomCurve)
            };
            m_UpdateValue(value);
        }

        /// <summary>
        /// The method to be called on the final tween update.
        /// </summary>
        protected override void OnFinalUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                return;
            }

            m_UpdateValue(m_TargetValue);
        }
    }

    /// <summary>
    /// Tween object that modifies a Vector4 each frame, over a specified period of time.
    /// </summary>
    public class AutoTweenVector4 : AutoTween
    {
        /// <summary>
        /// The Func to get the tween's start Vector4.
        /// </summary>
        private Func<Vector4> m_GetStartValue;

        /// <summary>
        /// The Func to get the tween's end Vector4.
        /// </summary>
        private Func<Vector4> m_GetTargetValue;

        /// <summary>
        /// The Action to update the tween's target variable.
        /// </summary>
        private Action<Vector4> m_UpdateValue;

        /// <summary>
        /// The tween's start Vector4.
        /// </summary>
        private Vector4 m_StartValue;

        /// <summary>
        /// The tween's end Vector4.
        /// </summary>
        private Vector4 m_TargetValue;

        /// <summary>
        /// Gets a value from an element of either the initial or target value of the tween.
        /// For example, GetValue(false, 0) would get the first element of the intial value.
        /// Used to make inherited AutoTweens for different value types easier.
        /// </summary>
        /// <param name="isEnd">Get the value from the target value of the tween? Otherwise get the value from the intial value.</param>
        /// <param name="valueIndex">The index of the value within the value being checked. Only used if the value being checked has more than 1 element (eg. Vector2).</param>
        /// <returns>
        /// The desired value within the initial or target values of the tween.
        /// </returns>
        protected override float GetValue(bool isEnd, int valueIndex)
        {
            return (isEnd ? m_GetTargetValue()[valueIndex] : m_GetStartValue()[valueIndex]);
        }

        /// <summary>
        /// The number of elements in the value type being modified (eg. 1 for float/int, 2 for Vector2, 4 for Color).
        /// </summary>
        /// <returns></returns>
        protected override int ValueLength()
        {
            return 4;
        }

        /// <summary>
        /// Initializes the specified update value.
        /// </summary>
        /// <param name="updateValue">The update value.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="tweenType">Type of the tween.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="animationCurve">The animation curve.</param>
        /// <param name="scaledTime">if set to <c>true</c> [scaled time].</param>
        /// <param name="id">The identifier.</param>
        public void Initialize(Action<Vector4> updateValue, Func<Vector4> startValue, Func<Vector4> targetValue, float duration, float delay, Tween.TweenType tweenType, Action callback, AnimationCurve animationCurve, bool scaledTime, int id)
        {
            m_GetStartValue = startValue;
            m_UpdateValue = updateValue;
            m_GetTargetValue = targetValue;

            base.Initialize(duration, delay, tweenType, callback, animationCurve, scaledTime, id);
        }

        /// <summary>
        /// Called once the delay has reached 0, prepares the tween to start modifying the target variable.
        /// </summary>
        protected override void StartTween()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            base.StartTween();

            try
            {
                m_StartValue = m_GetStartValue();
                m_TargetValue = m_GetTargetValue();
            }
            catch (Exception)
            {
                EndTween(false);
            }
        }

        /// <summary>
        /// The method to be called on each tween update, excluding the final one.
        /// </summary>
        protected override void OnUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            Vector4 value = new Vector4
            {
                x = Tween.Evaluate(m_TweenType, m_StartValue.x, m_TargetValue.x, m_DeltaTime, m_Duration, m_CustomCurve),
                y = Tween.Evaluate(m_TweenType, m_StartValue.y, m_TargetValue.y, m_DeltaTime, m_Duration, m_CustomCurve),
                z = Tween.Evaluate(m_TweenType, m_StartValue.z, m_TargetValue.z, m_DeltaTime, m_Duration, m_CustomCurve),
                w = Tween.Evaluate(m_TweenType, m_StartValue.w, m_TargetValue.w, m_DeltaTime, m_Duration, m_CustomCurve)
            };
            m_UpdateValue(value);
        }

        /// <summary>
        /// The method to be called on the final tween update.
        /// </summary>
        protected override void OnFinalUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                return;
            }

            m_UpdateValue(m_TargetValue);
        }
    }

    /// <summary>
    /// Tween object that modifies a Color each frame, over a specified period of time.
    /// </summary>
    public class AutoTweenColor : AutoTween
    {
        /// <summary>
        /// The Func to get the tween's start Color.
        /// </summary>
        private Func<Color> m_GetStartValue;

        /// <summary>
        /// The Func to get the tween's end Color.
        /// </summary>
        private Func<Color> m_GetTargetValue;

        /// <summary>
        /// The Action to update the tween's target variable.
        /// </summary>
        private Action<Color> m_UpdateValue;

        /// <summary>
        /// The tween's start Color.
        /// </summary>
        private Color m_StartValue;

        /// <summary>
        /// The tween's end Color.
        /// </summary>
        private Color m_TargetValue;

        /// <summary>
        /// Gets a value from an element of either the initial or target value of the tween.
        /// For example, GetValue(false, 0) would get the first element of the intial value.
        /// Used to make inherited AutoTweens for different value types easier.
        /// </summary>
        /// <param name="isEnd">Get the value from the target value of the tween? Otherwise get the value from the intial value.</param>
        /// <param name="valueIndex">The index of the value within the value being checked. Only used if the value being checked has more than 1 element (eg. Vector2).</param>
        /// <returns>
        /// The desired value within the initial or target values of the tween.
        /// </returns>
        protected override float GetValue(bool isEnd, int valueIndex)
        {
            return (isEnd ? m_GetTargetValue()[valueIndex] : m_GetStartValue()[valueIndex]);
        }

        /// <summary>
        /// The number of elements in the value type being modified (eg. 1 for float/int, 2 for Vector2, 4 for Color).
        /// </summary>
        /// <returns></returns>
        protected override int ValueLength()
        {
            return 4;
        }

        /// <summary>
        /// Initializes the specified update value.
        /// </summary>
        /// <param name="updateValue">The update value.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="tweenType">Type of the tween.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="animationCurve">The animation curve.</param>
        /// <param name="scaledTime">if set to <c>true</c> [scaled time].</param>
        /// <param name="id">The identifier.</param>
        public void Initialize(Action<Color> updateValue, Func<Color> startValue, Func<Color> targetValue, float duration, float delay, Tween.TweenType tweenType, Action callback, AnimationCurve animationCurve, bool scaledTime, int id)
        {
            m_GetStartValue = startValue;
            m_UpdateValue = updateValue;
            m_GetTargetValue = targetValue;

            Initialize(duration, delay, tweenType, callback, animationCurve, scaledTime, id);
        }

        /// <summary>
        /// Called once the delay has reached 0, prepares the tween to start modifying the target variable.
        /// </summary>
        protected override void StartTween()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            base.StartTween();

            try
            {
                m_StartValue = m_GetStartValue();
                m_TargetValue = m_GetTargetValue();
            }
            catch (Exception)
            {
                EndTween(false);
            }
        }

        /// <summary>
        /// The method to be called on each tween update, excluding the final one.
        /// </summary>
        protected override void OnUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                EndTween(false);
                return;
            }

            Color value = new Color
            {
                r = Tween.Evaluate(m_TweenType, m_StartValue.r, m_TargetValue.r, m_DeltaTime, m_Duration, m_CustomCurve),
                g = Tween.Evaluate(m_TweenType, m_StartValue.g, m_TargetValue.g, m_DeltaTime, m_Duration, m_CustomCurve),
                b = Tween.Evaluate(m_TweenType, m_StartValue.b, m_TargetValue.b, m_DeltaTime, m_Duration, m_CustomCurve),
                a = Tween.Evaluate(m_TweenType, m_StartValue.a, m_TargetValue.a, m_DeltaTime, m_Duration, m_CustomCurve)
            };
            m_UpdateValue(value);
        }

        /// <summary>
        /// The method to be called on the final tween update.
        /// </summary>
        protected override void OnFinalUpdateValue()
        {
            if (m_UpdateValue == null)
            {
                return;
            }

            m_UpdateValue(m_TargetValue);
        }
    }
}