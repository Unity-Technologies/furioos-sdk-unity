//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace MaterialUI
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [AddComponentMenu("MaterialUI/Managers/Snackbar Manager")]
    public class SnackbarManager : MonoBehaviour
    {
        /// <summary>
        /// The singelton in-scene instance.
        /// </summary>
        private static SnackbarManager m_Instance;
        /// <summary>
        /// The singelton in-scene instance.
        /// If null, creates and sets the in-scene instance.
        /// </summary>
        private static SnackbarManager instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = "SnackbarManager";
                    go.AddComponent<CanvasRenderer>();
                    RectTransform rectTransform = go.AddComponent<RectTransform>();
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                    m_Instance = go.AddComponent<SnackbarManager>();
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
        /// The parent canvas for the snackbar objects.
        /// </summary>
        [SerializeField]
        private Canvas m_ParentCanvas;

        /// <summary>
        /// The default duration to show snackbar messages.
        /// </summary>
        [Header("Default Snackbar parameters")]
        [SerializeField]
        private float m_DefaultDuration = 5f;

        /// <summary>
        /// The default panel color.
        /// </summary>
        [SerializeField]
        private Color m_DefaultPanelColor = MaterialColor.HexToColor("323232");

        /// <summary>
        /// The default text color.
        /// </summary>
        [SerializeField]
        private Color m_DefaultTextColor = MaterialColor.textLight;

        /// <summary>
        /// The default font size.
        /// </summary>
        [SerializeField]
        private int m_DefaultFontSize = 16;

        /// <summary>
        /// The queue of snackbars. To avoid multiple snackbars being shown at once, each snackbar is added to the queue when Show is called.
        /// Each queued snackbar will automatically be shown once the preceding snackbar has finished being shown.
        /// </summary>
        private Queue<Snackbar> m_Snackbars;
        /// <summary>
        /// Are snackbars actively/currently being shown?
        /// </summary>
        private bool m_IsActive;
        /// <summary>
        /// The SnackbarAnimator for the currently shown snackbar.
        /// </summary>
        private SnackbarAnimator m_CurrentAnimator;

        /// <summary>
        /// Has the system been initialized?
        /// </summary>
        private bool m_InitDone = false;

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
                Debug.LogWarning("More than one SnackbarManager exist in the scene, destroying one.");
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initializes the snackbar system.
        /// </summary>
        private void InitSystem()
        {
            if (m_ParentCanvas == null)
            {
                m_ParentCanvas = FindObjectOfType<Canvas>();
            }

            transform.SetParent(m_ParentCanvas.transform, false);
            transform.localPosition = Vector3.zero;
            
            m_CurrentAnimator = PrefabManager.InstantiateGameObject(PrefabManager.ResourcePrefabs.snackbar, transform).GetComponent<SnackbarAnimator>();

            m_Snackbars = new Queue<Snackbar>();

            m_InitDone = true;
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
        /// Shows a snackbar message. If a snackbar message is already being shown, then the manager will instead add this snackbar to the queue.
        /// The action button has text "Okay" and simply clears the snackbar when clicked.
        /// </summary>
        /// <param name="content">The message to show.</param>
        public static void Show(string content)
        {
            Show(content, "Okay", null);
        }

        /// <summary>
        /// Shows a snackbar message. If a snackbar message is already being shown, then the manager will instead add this snackbar to the queue.
        /// </summary>
        /// <param name="content">The message to show.</param>
        /// <param name="actionName">The text of the action button.</param>
        /// <param name="onActionButtonClicked">Called when the action button is clicked.</param>
        public static void Show(string content, string actionName, Action onActionButtonClicked)
        {
            Show(content, instance.m_DefaultDuration, instance.m_DefaultPanelColor, instance.m_DefaultTextColor, instance.m_DefaultFontSize, actionName, onActionButtonClicked);
        }

        /// <summary>
        /// Shows a snackbar message. If a snackbar message is already being shown, then the manager will instead add this snackbar to the queue.
        /// </summary>
        /// <param name="content">The message to show.</param>
        /// <param name="duration">The duration to show the snackbar.</param>
        /// <param name="actionName">The text of the action button.</param>
        /// <param name="onActionButtonClicked">Called when the action button is clicked.</param>
        public static void Show(string content, float duration, string actionName, Action onActionButtonClicked)
		{
			Show(content, duration, instance.m_DefaultPanelColor, instance.m_DefaultTextColor, instance.m_DefaultFontSize, actionName, onActionButtonClicked);
		}

        /// <summary>
        /// Shows a snackbar message. If a snackbar message is already being shown, then the manager will instead add this snackbar to the queue.
        /// </summary>
        /// <param name="content">The message to show.</param>
        /// <param name="duration">The duration to show the snackbar.</param>
        /// <param name="panelColor">Color of the background panel.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="actionName">The text of the action button.</param>
        /// <param name="onActionButtonClicked">Called when the action button is clicked.</param>
        public static void Show(string content, float duration, Color panelColor, Color textColor, int fontSize, string actionName, Action onActionButtonClicked)
        {
            if (!instance.m_InitDone)
            {
                instance.InitSystem();
            }

            instance.m_Snackbars.Enqueue(new Snackbar(content, duration, panelColor, textColor, fontSize, actionName, onActionButtonClicked));
            instance.StartQueue();
        }

        /// <summary>
        /// If <see cref="m_IsActive"/> is false, start showing each snackbar in the queue, one after another.
        /// Otherwise, do nothing.
        /// </summary>
        private void StartQueue()
        {
            if (m_Snackbars.Count > 0 && !m_IsActive)
            {
                m_CurrentAnimator.Show(m_Snackbars.Dequeue());
                m_IsActive = true;
            }
        }

        /// <summary>
        /// Called by <see cref="SnackbarAnimator"/> when a snackbar has finished showing.
        /// Calls <see cref="StartQueue"/>.
        /// </summary>
        /// <returns>True. I don't know why I put that return line below, and I'm too tired to figure out why now.</returns>
        public static bool OnSnackbarCompleted()
        {
            instance.m_IsActive = false;
            instance.StartQueue();
            return (instance.m_Snackbars.Count > -1);
        }
    }
}