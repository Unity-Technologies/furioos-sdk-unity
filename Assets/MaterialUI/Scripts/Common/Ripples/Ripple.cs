//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Component that controls the lifecycle of a ripple and animates it.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class Ripple : Image
    {
        #region Coefficients

        private static readonly double[] m_Tier1Coefficients =
        {
            6.29601480149216,
            0.790101551206291,
            -0.262948117301137,
            0.00512416320905045,
            -0.0000380506669282087
        };

        private static readonly double[] m_Tier2Coefficients =
        {
            215687.256290898,
            -4.19735422974633,
            0.0951623635658089,
            -0.000309361330425578,
            -0.0000000681012204738887
        };

        private static readonly double[] m_Tier3Coefficients =
        {
            1354896728614720,
            -10.6998485226718,
            0.190322732770353,
            -0.0006188525075537,
            0.000000894213533926328
        };

        private static readonly double[] m_Tier4Coefficients =
        {
            97322952474305100,
            -10.104765382165,
            0.0891692727750869,
            -0.000138147859622898,
            0.0000000906760843587359
        };

        private static readonly double[] m_Tier5Coefficients =
        {
            0.00164932975085508,
            0.984071298443192,
            -0.0055147376298876,
            0.00000384368847512861,
            -0.00000000171500898423944
        };

        private static readonly double[] m_Tier6Coefficients =
        {
        0.000416750897989708,
        0.984141847866171,
        -0.00275751774737513,
        0.000000960973461097143,
        -0.000000000214384084088226
        };

        private static readonly float m_Tier7Value = 0.01f;

        #endregion

        /// <summary>
        /// The global identifier of the ripple.
        /// </summary>
        private int m_Id;
        /// <summary>
        /// The global identifier of the ripple.
        /// </summary>
        public int id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        /// <summary>
        /// The ripple's data.
        /// </summary>
        private RippleData m_RippleData;
        /// <summary>
        /// The ripple's data.
        /// </summary>
        public RippleData rippleData
        {
            get { return m_RippleData; }
        }


        /// <summary>
        /// The MaterialUIScaler used to get scaling values.
        /// </summary>
        private MaterialUIScaler m_Scaler;

        /// <summary>
        /// The MaterialUIScaler used to get scaling values.
        /// </summary>
        public MaterialUIScaler scaler
        {
            get
            {
                if (m_Scaler == null)
                {
                    m_Scaler = MaterialUIScaler.GetParentScaler(rectTransform);
                }
                return m_Scaler;
            }
        }

        /// <summary>
        /// The CanvasGroup of the ripple.
        /// </summary>
        private CanvasGroup m_CanvasGroup;
        /// <summary>
        /// The CanvasGroup of the ripple.
        /// </summary>
        public CanvasGroup canvasGroup
        {
            get { return m_CanvasGroup; }
        }

        /// <summary>
        /// The object that created the ripple. Callbacks will be sent to that object.
        /// </summary>
        private IRippleCreator m_RippleCreator;
        /// <summary>
        /// The parent RectTransform of the ripple.
        /// </summary>
        private RectTransform m_RippleParent;

        /// <summary>
        /// The current alpha of the ripple's graphic.
        /// </summary>
        private float m_CurrentAlpha;
        /// <summary>
        /// The current size of the ripple's RectTransform.
        /// </summary>
        private float m_CurrentSize;
        /// <summary>
        /// The current (world) position of the ripple's RectTransform.
        /// </summary>
        private Vector3 m_CurrentPosition;

        /// <summary>
        /// Should the ripple oscillate between slightly bigger and slightly smaller in size?
        /// </summary>
        private bool m_Oscillate;

        /// <summary>
        /// The duration of the ripple expand/contract animations.
        /// </summary>
        private float m_AnimationDuration;
        /// <summary>
        /// When the animation (expand/contract) was started.
        /// </summary>
        private float m_AnimStartTime;
        /// <summary>
        /// The time since the animation (expand/contract) was last updated.
        /// </summary>
        private float m_AnimDeltaTime;
        /// <summary>
        /// The state of the animation.
        /// 0 = nothing, 1 = expand, 2 = contract, 3 = oscillate.
        /// </summary>
        private int m_State;


        /// <summary>
        /// The shared material used for all ripples.
        /// </summary>
        private static Material m_RippleMaterial;

        /// <summary>
        /// Has the ripple material been reset to its default value?
        /// </summary>
        private static bool m_MaterialIsReset = false;

        /// <summary>
        /// Used to keep track of the sizes of all ripples in a scene, for softening calculation.
        /// </summary>
        private static List<int> m_AverageSizes = new List<int>();

        /// <summary>
        /// See MonoBehaviour.OnApplicationQuit.
        /// </summary>
        void OnApplicationQuit()
        {
            ResetMaterial();
        }


        /// <summary>
        /// Resets the ripple material to its default value.
        /// </summary>
        public static void ResetMaterial()
        {
            if (!m_MaterialIsReset)
            {
                m_MaterialIsReset = true;
                if (m_RippleMaterial != null)
                {
                    m_RippleMaterial.SetFloat("_Softening", 0.25f);
                }
            }
        }

        /// <summary>
        /// Creates the ripple. Only called when a ripple is actually created and not needed when reused.
        /// </summary>
        /// <param name="id">The global identifier of the ripple.</param>
        /// <param name="imageData">The image data to use for the ripple.</param>
        public void Create(int id, VectorImageData imageData)
        {
            if (m_Id != 0)
            {
                Debug.Log("Cannot Setup a Ripple more than once");
                return;
            }

            m_Id = id;
            m_CanvasGroup = GetComponent<CanvasGroup>();

            if (m_RippleMaterial == null)
            {
                m_RippleMaterial = m_Material;
            }
        }

        /// <summary>
        /// Clears the rippleData, call when resetting the ripple.
        /// </summary>
        public void ClearData()
        {
            m_RippleData = null;
        }

        /// <summary>
        /// Sets the ripple's transform and rippleData, and prepares it to start expanding.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="positon">The positon.</param>
        /// <param name="creator">The creator to send callbacks to.</param>
        /// <param name="oscillate">If true, the ripple will oscillate when expanded.</param>
        public void Setup(RippleData data, Vector2 positon, IRippleCreator creator, bool oscillate = false)
        {
            m_RippleData = data;
            m_RippleParent = (RectTransform)data.RippleParent;
            rectTransform.SetParent(m_RippleParent);
            rectTransform.SetSiblingIndex(0);
            m_Oscillate = oscillate;
            rectTransform.sizeDelta = Vector2.zero;

            Camera camera = FindObjectOfType<Camera>();

            switch (GetComponentInParent<Canvas>().renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    rectTransform.position = new Vector3(positon.x, positon.y, m_RippleParent.position.z);
                    break;
                case RenderMode.ScreenSpaceCamera:
                    rectTransform.position = new Ray(camera.transform.position, camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_RippleParent.position.z))).GetPoint(Vector3.Distance(camera.transform.position, m_RippleParent.position));
                    break;
                case RenderMode.WorldSpace:
                    rectTransform.position = new Ray(camera.transform.position, camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_RippleParent.position.z))).GetPoint(Vector3.Distance(camera.transform.position, m_RippleParent.position));
                    break;
            }

            rectTransform.localEulerAngles = Vector3.zero;
            color = data.Color;
            canvasGroup.alpha = data.StartAlpha;
            m_AnimationDuration = 4f / data.Speed;
            m_RippleCreator = creator;

            if (data.PlaceRippleBehind)
            {
                int index = m_RippleParent.GetSiblingIndex();
                rectTransform.SetParent(m_RippleParent.parent);
                rectTransform.SetSiblingIndex(index);
            }

        }

        /// <summary>
        /// Calculates the fully-expanded size of a ripple based on the parent and the rippleData's size settings.
        /// </summary>
        /// <returns>The calculated fully-expanded size.</returns>
        private float CalculateSize()
        {
            float size;

            if (rippleData.AutoSize)
            {
                float x = rectTransform.parent.GetComponent<RectTransform>().GetProperSize().x;
                float y = rectTransform.parent.GetComponent<RectTransform>().GetProperSize().y;

                if (rippleData.SizeMode == RippleData.SizeModeType.FillRect)
                {
                    x *= x;
                    y *= y;

                    x *= (rippleData.SizePercent / 100f);
                    y *= (rippleData.SizePercent / 100f);

                    size = Mathf.Sqrt(x + y);

                }
                else
                {
                    size = Mathf.Max(x, y);
                }
            }
            else
            {
                size = rippleData.ManualSize;
            }

            if (m_Oscillate)
            {
                size *= 0.75f;
            }

            return size;
        }

        /// <summary>
        /// Starts the expand animation.
        /// </summary>
        public void Expand()
        {
            rectTransform.localScale = Vector3.one;
            m_RippleCreator.OnCreateRipple();
            m_CurrentAlpha = canvasGroup.alpha;
            m_CurrentSize = rectTransform.rect.width;
            m_CurrentPosition = rectTransform.position;
            SubmitSizeForSoftening();

            m_AnimStartTime = Time.realtimeSinceStartup;
            m_State = 1;
        }

        /// <summary>
        /// Starts the contract animation.
        /// </summary>
        public void Contract()
        {
            m_CurrentAlpha = canvasGroup.alpha;
            m_CurrentSize = rectTransform.rect.width;
            m_CurrentPosition = rectTransform.position;
            SubmitSizeForSoftening();

            m_AnimStartTime = Time.realtimeSinceStartup;
            m_State = 2;
        }

        /// <summary>
        /// See MonoBehaviour.Update.
        /// </summary>
        void Update()
        {
            m_AnimDeltaTime = Time.realtimeSinceStartup - m_AnimStartTime;

            if (m_State == 1)
            {
                if (m_AnimDeltaTime <= m_AnimationDuration)
                {
                    canvasGroup.alpha = Tween.QuintOut(m_CurrentAlpha, rippleData.EndAlpha, m_AnimDeltaTime, m_AnimationDuration);
                    float size = Tween.QuintOut(m_CurrentSize, CalculateSize(), m_AnimDeltaTime, m_AnimationDuration);
                    rectTransform.sizeDelta = new Vector2(size, size);
                    SubmitSizeForSoftening();

                    if (rippleData.MoveTowardCenter)
                    {
                        Vector3 parentPosition = m_RippleParent.GetPositionRegardlessOfPivot();
                        rectTransform.position = Tween.QuintOut(m_CurrentPosition, new Vector3(parentPosition.x, parentPosition.y, m_RippleParent.position.z), m_AnimDeltaTime, m_AnimationDuration);
                    }

                }
                else
                {
                    if (m_Oscillate)
                    {
                        m_State = 3;
                        m_AnimStartTime = Time.realtimeSinceStartup;
                        m_CurrentSize = rectTransform.rect.width;
                        m_CurrentSize *= 0.95f;
                    }
                    else
                    {
                        m_State = 0;
                    }
                }
            }
            else if (m_State == 2)
            {
                if (m_AnimDeltaTime <= m_AnimationDuration * 2f)
                {
                    canvasGroup.alpha = Tween.QuintOut(m_CurrentAlpha, 0f, m_AnimDeltaTime, m_AnimationDuration * 2f);
                    float size = Tween.QuintOut(m_CurrentSize, CalculateSize(), m_AnimDeltaTime, m_AnimationDuration);
                    rectTransform.sizeDelta = new Vector2(size, size);
                    SubmitSizeForSoftening();

                    if (rippleData.MoveTowardCenter)
                    {
                        Vector3 parentPosition = m_RippleParent.GetPositionRegardlessOfPivot();
                        rectTransform.position = Tween.QuintOut(m_CurrentPosition, new Vector3(parentPosition.x, parentPosition.y, m_RippleParent.position.z), m_AnimDeltaTime, m_AnimationDuration);
                    }
                }
                else
                {
                    m_State = 0;
                    m_RippleCreator.OnDestroyRipple();
                    RippleManager.instance.ReleaseRipple(this);
                }
            }
            else if (m_State == 3)
            {
                float size = Tween.Sin(m_CurrentSize, m_CurrentSize * 0.05f, m_AnimDeltaTime * 4);

                rectTransform.sizeDelta = new Vector2(size, size);
                SubmitSizeForSoftening();
            }
        }


        /// <summary>
        /// See MonoBehaviour.LateUpdate.
        /// </summary>
        void LateUpdate()
        {
            if (m_AverageSizes.Count > 0)
            {
                CalculateAverageSoftening();
            }
        }


        /// <summary>
        /// Submits the size of this ripple to the list, for softening calculations.
        /// </summary>
        public void SubmitSizeForSoftening()
        {
            float size = (rectTransform.GetProperSize().x + rectTransform.GetProperSize().y) / 2f;

            if (scaler != null)
            {
                size *= scaler.scaler.scaleFactor;
            }

            m_AverageSizes.Add(Mathf.FloorToInt(size));
        }


        /// <summary>
        /// Calculates the best amount of softening to use for all ripples based on their sizes, and applies it to the material.
        /// </summary>
        private static void CalculateAverageSoftening()
        {
            float averageSizeInital = 0;
            float averageSizeFinal = 0;
            int numberOfSizes = 0;
            float autoSoftenValue;

            if (m_AverageSizes.Count == 1)
            {
                averageSizeFinal = m_AverageSizes[0];
            }
            else
            {
                for (int i = 0; i < m_AverageSizes.Count; i++)
                {
                    averageSizeInital += m_AverageSizes[i];
                }

                for (int i = 0; i < m_AverageSizes.Count; i++)
                {
                    if (m_AverageSizes[i] > 5)
                    {
                        if (Mathf.Abs(m_AverageSizes[i] - averageSizeInital) < 1000)
                        {
                            averageSizeFinal += m_AverageSizes[i];
                            numberOfSizes++;
                        }
                    }
                }
                averageSizeFinal /= numberOfSizes;
            }

            if (averageSizeFinal >= 2000)
            {
                autoSoftenValue = m_Tier7Value;
            }
            else if (averageSizeFinal >= 1000)
            {
                autoSoftenValue = PowerExp(averageSizeFinal, m_Tier6Coefficients);
            }
            else if (averageSizeFinal >= 500)
            {
                autoSoftenValue = PowerExp(averageSizeFinal, m_Tier5Coefficients);
            }
            else if (averageSizeFinal >= 200)
            {
                autoSoftenValue = PowerExp(averageSizeFinal, m_Tier4Coefficients);
            }
            else if (averageSizeFinal >= 100)
            {
                autoSoftenValue = PowerExp(averageSizeFinal, m_Tier3Coefficients);
            }
            else if (averageSizeFinal >= 50)
            {
                autoSoftenValue = PowerExp(averageSizeFinal, m_Tier2Coefficients);
            }
            else
            {
                autoSoftenValue = PowerExp(averageSizeFinal, m_Tier1Coefficients);
            }

            m_RippleMaterial.SetFloat("_Softening", 0.5f * (float.IsNaN(autoSoftenValue) ? 0.25f : autoSoftenValue));

            m_AverageSizes = new List<int>();
        }


        /// <summary>
        /// A 5-coefficient power-exponential function, used to calculate softening values.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="c">The set of coefficients to use.</param>
        /// <returns></returns>
        private static float PowerExp(float x, double[] c)
        {
            return (float)(c[0] * Math.Pow(x, c[1]) * Math.Pow(Math.E, c[2] * x + c[3] * Mathf.Pow(x, 2) + c[4] * Mathf.Pow(x, 3)));
        }


        /// <summary>
        /// Callback function when a UI element needs to generate vertices.
        /// </summary>
        /// <param name="vh">The VertexHelper used.</param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            List<UIVertex> verts = new List<UIVertex>();
            List<int> indicies = new List<int>();

            vh.GetUIVertexStream(verts);
            vh.Clear();

            for (int i = 0; i < verts.Count; i++)
            {
                var vert = verts[i];

                vert.position *= 1.332f;
                vert.position += new Vector3(2.209f, 0f, 0f) * (vert.position.x < 0 ? 1 : -1);
                vert.position += new Vector3(0f, 2.209f, 0f) * (vert.position.y < 0 ? 1 : -1);

                verts[i] = vert;
                indicies.Add(i);
            }

            vh.AddUIVertexStream(verts, indicies);
        }
    }

    /// <summary>
    /// Container for a ripple's settings.
    /// </summary>
    [Serializable]
    public class RippleData
    {
        /// <summary>
        /// The modes to use to automatically calculate a size based on a ripple's parent.
        /// </summary>
        public enum SizeModeType
        {
            /// <summary>
            /// Make sure that the ripple completely covers the parent's rect, including corners.
            /// </summary>
            FillRect,
            /// <summary>
            /// Make sure the the ripple's width and height match that of the parent.
            /// </summary>
            MatchSize
        }

        /// <summary>
        /// Should the ripple's size be automatically calculated?
        /// </summary>
        public bool AutoSize = true;
        /// <summary>
        /// Amount to modify the calculated size by.
        /// </summary>
        public float SizePercent = 105f;
        /// <summary>
        /// The (absolute) size to make the ripple.
        /// </summary>
        public float ManualSize;
        /// <summary>
        /// The mode to use to automatically calculate a size based on a ripple's parent.
        /// </summary>
        public SizeModeType SizeMode = SizeModeType.FillRect;
        /// <summary>
        /// The speed of the ripple's animations.
        /// (Animation duration in seconds = 4 / Speed).
        /// </summary>
        public float Speed = 8f;
        /// <summary>
        /// The color of the ripple. Alpha is ignored.
        /// </summary>
        public Color Color = Color.black;
        /// <summary>
        /// The alpha of the ripple when first expanded.
        /// </summary>
        public float StartAlpha = 0.3f;
        /// <summary>
        /// The alpha of the ripple when fully expanded.
        /// </summary>
        public float EndAlpha = 0.1f;
        /// <summary>
        /// Should the ripple move toward the center of the parent's transform over its life?
        /// </summary>
        public bool MoveTowardCenter = true;
        /// <summary>
        /// The ripple's parent Transform.
        /// </summary>
        public Transform RippleParent;
        /// <summary>
        /// Should the ripple be placed 'behind' (above in hierarchy) the parent?
        /// </summary>
        public bool PlaceRippleBehind;

        /// <summary>
        /// Copies this RippleData.
        /// </summary>
        /// <returns>A copy of this RippleData.</returns>
        public RippleData Copy()
        {
            RippleData data = new RippleData();
            data.AutoSize = AutoSize;
            data.SizePercent = SizePercent;
            data.ManualSize = ManualSize;
            data.SizeMode = SizeMode;
            data.Speed = Speed;
            data.Color = Color;
            data.StartAlpha = StartAlpha;
            data.EndAlpha = EndAlpha;
            data.MoveTowardCenter = MoveTowardCenter;
            data.RippleParent = RippleParent;
            data.PlaceRippleBehind = PlaceRippleBehind;
            return data;
        }
    }
}