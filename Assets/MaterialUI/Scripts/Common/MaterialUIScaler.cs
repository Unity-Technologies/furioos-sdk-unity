//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
#if UNITY_EDITOR
    /// <summary>
    /// Handles canvas scale change events and keeps track of the scale.
    /// Required on any root Canvas that contains MaterialUI components.
    /// </summary>
    /// <seealso cref="UnityEngine.EventSystems.UIBehaviour" />
    [InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    [RequireComponent(typeof(Canvas))]
    [AddComponentMenu("MaterialUI/MaterialUI Scaler", 50)]
    public class MaterialUIScaler : UIBehaviour
    {
        /// <summary>
        /// Called when the scale is changed.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        public delegate void ScaleFactorChangeEvent(float scaleFactor);
        /// <summary>
        /// Called when the scale is changed.
        /// </summary>
        public ScaleFactorChangeEvent OnScaleFactorChange;

        /// <summary>
        /// Called when the orientation is changed.
        /// </summary>
        /// <param name="resolution">The resolution.</param>
        public delegate void OrientationChangeEvent(Vector2 resolution);
        /// <summary>
        /// Called when the orientation is changed.
        /// </summary>
        public OrientationChangeEvent OnOrientationChange;

        /// <summary>
        /// The target canvas
        /// </summary>
        private Canvas m_TargetCanvas;
        /// <summary>
        /// Gets the target canvas.
        /// </summary>
        /// <value>
        /// The target canvas.
        /// </value>
        public Canvas targetCanvas
        {
            get
            {
                if (m_TargetCanvas == null)
                {
                    m_TargetCanvas = GetComponent<Canvas>();
                }
                return m_TargetCanvas;
            }
        }

        private CanvasScaler m_Scaler;
        public CanvasScaler scaler
        {
            get
            {
                if (m_Scaler == null)
                {
                    m_Scaler = GetComponent<CanvasScaler>();
                }
                return m_Scaler;
            }
        }


        /// <summary>
        /// Should the CanvasScaler's reference resolution be swapped between portrait and landscape when the screen is?
        /// </summary>
        [SerializeField]
        [Tooltip("Should the CanvasScaler's reference resolution be swapped between portrait and landscape when the screen is?")]
        private bool m_MatchOrientationToScreen = true;
        /// <summary>
        /// Should the CanvasScaler's reference resolution be swapped between portrait and landscape when the screen is?
        /// </summary>
        public bool matchOrientationToScreen
        {
            get { return m_MatchOrientationToScreen; }
            set
            {
                m_MatchOrientationToScreen = value;
                CheckScaleFactor();
            }
        }

        /// <summary>
        /// The last scale factor.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private float m_LastScaleFactor;

        /// <summary>
        /// The last screen resolution.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private Vector2 m_LastResolution;

#if UNITY_EDITOR
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialUIScaler"/> class.
        /// </summary>
        public MaterialUIScaler()
        {
            EditorUpdate.Init();
            EditorUpdate.onEditorUpdate += CheckScaleFactor;
        }

        /// <summary>
        /// See MonoBehaviour.OnDestroy.
        /// </summary>
        protected override void OnDestroy()
        {
            EditorUpdate.onEditorUpdate -= CheckScaleFactor;
        }
#endif

        /// <summary>
        /// Updates this instance.
        /// </summary>
        void Update()
        {
            if (Application.isPlaying)
            {
                CheckScaleFactor();
            }
        }

        /// <summary>
        /// Checks the scale factor and fires event if scale has changed.
        /// <para></para>
        /// If the screen has been rotated, swap the reference resolution's orientation and wait for next check to fire event to avoid duplicate event firing.
        /// </summary>
        private void CheckScaleFactor()
        {
#if UNITY_EDITOR
            if (IsDestroyed())
            {
                EditorUpdate.onEditorUpdate -= CheckScaleFactor;
                return;
            }
#endif

            if (targetCanvas == null) return;

            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                if (m_MatchOrientationToScreen)
                {
                    if (Screen.width > Screen.height == scaler.referenceResolution.x < scaler.referenceResolution.y && Screen.width == m_LastResolution.y && Screen.height == m_LastResolution.x)
                    {
                        Vector2 tempRes = scaler.referenceResolution;
                        float temp = tempRes.x;
                        tempRes.x = tempRes.y;
                        tempRes.y = temp;
                        scaler.referenceResolution = tempRes;

                        if (OnOrientationChange != null)
                        {
                            OnOrientationChange(new Vector2(Screen.width, Screen.height));
                        }

                        m_LastResolution = new Vector2(Screen.width, Screen.height);

                        return;
                    }
                }
            }

            if (m_LastScaleFactor != targetCanvas.scaleFactor)
            {
                m_LastScaleFactor = targetCanvas.scaleFactor;
                if (OnScaleFactorChange != null)
                {
                    OnScaleFactorChange(m_LastScaleFactor);
                }
            }

            m_LastResolution = new Vector2(Screen.width, Screen.height);
        }

        /// <summary>
        /// Gets the parent scaler from a Transform.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns></returns>
        public static MaterialUIScaler GetParentScaler(Transform transform)
        {
            if (transform == null) return null;

            Transform currentTransform = transform;
            MaterialUIScaler scaler = null;

            while (currentTransform.root != currentTransform)
            {
                currentTransform = currentTransform.parent;
                scaler = currentTransform.GetComponent<MaterialUIScaler>();
                if (scaler != null) break;
            }

            return scaler;
        }
    }
}