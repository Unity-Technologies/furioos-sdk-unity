//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Helper component to display the current FPS in play mode, on a <see cref="Text"/> component.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [AddComponentMenu("MaterialUI/FPSCounter", 100)]
    public class FPSCounter : MonoBehaviour
    {
        /// <summary>
        /// The time (in seconds) between each update of the text.
        /// </summary>
        [SerializeField]
        private float m_UpdateInterval = 0.5f;
        /// <summary>
        /// The time (in seconds) between each update of the text.
        /// </summary>
        public float updateInterval
        {
            get { return m_UpdateInterval; }
            set { m_UpdateInterval = value; }
        }

        /// <summary>
        /// The Text component on which to display the FPS value.
        /// </summary>
        [SerializeField]
        private Text m_Text;
        /// <summary>
        /// The Text component on which to display the FPS value.
        /// If null, the attached Text will be added if it exists. If not, one will be created.
        /// </summary>
        public Text text
        {
            get
            {
                if (m_Text == null)
                {
                    m_Text = gameObject.GetAddComponent<Text>();
                }
                return m_Text;
            }
            set { m_Text = value; }
        }

        /// <summary>
        /// The FPS accumulated over the interval.
        /// </summary>
        private float m_DeltaFps;
        /// <summary>
        /// The Frames drawn over the interval.
        /// </summary>
        private int m_Frames;
        /// <summary>
        /// The time left for current interval.
        /// </summary>
        private float m_Timeleft;

        /// <summary>
        /// See MonoBehaviour.Start.
        /// </summary>
        void Start()
        {
            m_Timeleft = updateInterval;
        }

        /// <summary>
        /// See MonoBehaviour.Update.
        /// </summary>
        void Update()
        {
            m_Timeleft -= Time.deltaTime;
            m_DeltaFps += Time.timeScale / Time.deltaTime;
            ++m_Frames;

            // Interval ended - update GUI text and start new interval
            if (m_Timeleft <= 0f)
            {
                // display two fractional digits (f2 format)
                text.text = "" + (m_DeltaFps / m_Frames).ToString("f2") + " FPS";
                if ((m_DeltaFps / m_Frames) < 1)
                {
                    text.text = "";
                }
                m_Timeleft = updateInterval;
                m_DeltaFps = 0f;
                m_Frames = 0;
            }
        }
    }
}