//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using System.Collections.Generic;

namespace MaterialUI
{
    /// <summary>
    /// Singleton component that creates and handles Toast messages.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [AddComponentMenu("MaterialUI/Managers/Toast Manager")]
    public class ToastManager : MonoBehaviour
    {
        /// <summary>
        /// The singelton in-scene instance.
        /// </summary>
        private static ToastManager m_Instance;
        /// <summary>
        /// The singelton in-scene instance.
        /// If null, creates and sets the in-scene instance.
        /// </summary>
        private static ToastManager instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = "ToastManager";

                    m_Instance = go.AddComponent<ToastManager>();
                }

                return m_Instance;
            }
        }

        /// <summary>
        /// Should the Manager persist between scenes?
        /// </summary>
        [SerializeField]
        private bool m_KeepBetweenScenes = true;

        /// <summary>
        /// The parent canvas for the toast objects.
        /// </summary>
        [SerializeField]
        private Canvas m_ParentCanvas;

        /// <summary>
        /// The default duration to show Toast messages.
        /// </summary>
        [Header("Default Toasts parameters")]
        [SerializeField]
        private float m_DefaultDuration = 2f;

        /// <summary>
        /// The default panel color.
        /// </summary>
        [SerializeField]
        private Color m_DefaultPanelColor = Color.white;

        /// <summary>
        /// The default text color.
        /// </summary>
        [SerializeField]
        private Color m_DefaultTextColor = Color.black;

        /// <summary>
        /// The default font size.
        /// </summary>
        [SerializeField]
        private int m_DefaultFontSize = 16;

        /// <summary>
        /// The queue of toasts. To avoid multiple toasts being shown at once, each toast is added to the queue when Show is called.
        /// Each queued toast will automatically be shown once the preceding toast has finished being shown.
        /// </summary>
        private Queue<KeyValuePair<Toast, Canvas>> m_ToastQueue;
        /// <summary>
        /// Are toasts actively/currently being shown?
        /// </summary>
        private bool m_IsActive;
        /// <summary>
        /// The ToastAnimator for the currently shown toast.
        /// </summary>
        private ToastAnimator m_CurrentAnimator;

        /// <summary>
        /// See MonoBehaviour.Awake.
        /// </summary>
        void Awake()
        {
            if (!m_Instance)
            {
                m_Instance = this;

                if (m_KeepBetweenScenes)
                {
                    DontDestroyOnLoad(this);
                }
            }
            else
            {
                Debug.LogWarning("More than one ToastManager exist in the scene, destroying one.");
                Destroy(gameObject);
                return;
            }

            InitSystem();
        }

        /// <summary>
        /// Initializes the toast system.
        /// </summary>
        private void InitSystem()
        {
            SetCanvas(null);

            if (m_CurrentAnimator == null)
            {
                m_CurrentAnimator = PrefabManager.InstantiateGameObject(PrefabManager.ResourcePrefabs.toast, transform).GetComponent<ToastAnimator>();
            }

            m_ToastQueue = new Queue<KeyValuePair<Toast, Canvas>>();
        }

        /// <summary>
        /// See MonoBehaviour.OnDestroy.
        /// </summary>
        void OnDestroy()
        {
            m_Instance = null;
        }

        /// <summary>
        /// See MonoBehaviour.OnApplicationQuit.
        /// </summary>
        void OnApplicationQuit()
        {
            m_Instance = null;
		}

		/// <summary>
		/// Shows a toast message. If a toast message is already being shown, then the manager will instead add this toast to the queue.
		/// </summary>
		/// <param name="content">The message to show.</param>
		/// <param name="canvasHierarchy">The toast will be shown under the same canvas as the specified transform. Uses <see cref="m_ParentCanvas"/> if null.</param>
		public static void Show(string content, Transform canvasHierarchy = null)
		{
			Show(content, instance.m_DefaultDuration, instance.m_DefaultPanelColor, instance.m_DefaultTextColor, instance.m_DefaultFontSize, canvasHierarchy);
		}

		/// <summary>
		/// Shows a toast message. If a toast message is already being shown, then the manager will instead add this toast to the queue.
		/// </summary>
		/// <param name="content">The message to show.</param>
		/// <param name="duration">The duration to show the toast.</param>
		/// <param name="canvasHierarchy">The toast will be shown under the same canvas as the specified transform. Uses <see cref="m_ParentCanvas"/> if null.</param>
		public static void Show(string content, float duration, Transform canvasHierarchy = null)
		{
			Show(content, duration, instance.m_DefaultPanelColor, instance.m_DefaultTextColor, instance.m_DefaultFontSize, canvasHierarchy);
		}

        /// <summary>
        /// Shows a toast message. If a toast message is already being shown, then the manager will instead add this toast to the queue.
        /// </summary>
        /// <param name="content">The message to show.</param>
        /// <param name="duration">The duration to show the toast.</param>
        /// <param name="panelColor">Color of the background panel.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="canvasHierarchy">The toast will be shown under the same canvas as the specified transform. Uses <see cref="m_ParentCanvas"/> if null.</param>
        public static void Show(string content, float duration, Color panelColor, Color textColor, int fontSize, Transform canvasHierarchy = null)
        {
            Canvas canvas = null;
            if (canvasHierarchy != null)
            {
                canvas = MaterialUIScaler.GetParentScaler(canvasHierarchy).targetCanvas;
                if (canvas != null)
                {
                    instance.m_ParentCanvas = canvas;
                }
            }

            instance.m_ToastQueue.Enqueue(new KeyValuePair<Toast, Canvas>(new Toast(content, duration, panelColor, textColor, fontSize), canvas));
            instance.StartQueue();
        }

        /// <summary>
        /// If <see cref="m_IsActive"/> is false, start showing each toast in the queue, one after another.
        /// Otherwise, do nothing.
        /// </summary>
        private void StartQueue()
        {
            if (m_ToastQueue.Count <= 0 || m_IsActive) return;

            if (m_ToastQueue.Count > 0)
            {
                KeyValuePair<Toast, Canvas> pair = m_ToastQueue.Dequeue();
                SetCanvas(pair.Value);
                m_CurrentAnimator.Show(pair.Key);
            }
            m_IsActive = true;
        }

        /// <summary>
        /// Called by <see cref="ToastAnimator"/> when a toast has finished showing.
        /// Calls <see cref="StartQueue"/>.
        /// </summary>
        /// <returns>True. I don't know why I put that return line below, and I'm too tired to figure out why now.</returns>
        public static bool OnToastCompleted()
        {
            instance.m_IsActive = false;
            instance.StartQueue();
            return instance.m_ToastQueue.Count > -1;
        }

        /// <summary>
        /// Sets a specified Canvas as the new <see cref="m_ParentCanvas"/>.
        /// </summary>
        /// <param name="canvas">The canvas to set. If null, the first object of type Canvas in the scene will be used.</param>
        private void SetCanvas(Canvas canvas)
        {
            if (canvas != null)
            {
                m_ParentCanvas = canvas;
            }

            if (m_ParentCanvas == null)
            {
                m_ParentCanvas = FindObjectOfType<Canvas>();
            }

            transform.SetParent(m_ParentCanvas.transform, false);
            transform.localPosition = Vector3.zero;
        }
    }
}