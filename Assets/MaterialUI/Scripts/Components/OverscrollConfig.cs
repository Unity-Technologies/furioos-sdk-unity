//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Component that handles overscroll effects for a ScrollRect.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="UnityEngine.EventSystems.IInitializePotentialDragHandler" />
    [AddComponentMenu("MaterialUI/Overscroll Config", 50)]
    public class OverscrollConfig : MonoBehaviour, IInitializePotentialDragHandler
    {
        //  0 = Left
        //  1 = Right
        //  2 = Top
        //  3 = Bottom

        /// <summary>
        /// The overscroll color.
        /// </summary>
        [SerializeField]
        private Color m_OverscrollColor = new Color(0f, 0f, 0f, 0.25f);
        /// <summary>
        /// The overscroll color.
        /// </summary>
        public Color overscrollColor
		{
			get { return m_OverscrollColor; }
			set
			{
				m_OverscrollColor = value;
				for (int i = 0; i < m_OverscrollObjects.Length; i++)
				{
					if (m_OverscrollObjects[i] != null)
					{
						if (m_OverscrollObjects[i].GetComponent<Image>() != null)
						{
							m_OverscrollObjects[i].GetComponent<Image>().color = m_OverscrollColor;
						}
					}
				}
			}
		}

        /// <summary>
        /// The overscroll scale.
        /// </summary>
        [SerializeField]
        private float m_OverscrollScale = 1;
        /// <summary>
        /// The overscroll scale.
        /// </summary>
        public float overscrollScale
		{
			get { return m_OverscrollScale; }
			set { m_OverscrollScale = value; }
		}

        /// <summary>
        /// The overscroll sprite.
        /// </summary>
        private static Sprite overscrollSprite;
        /// <summary>
        /// The overscroll objects.
        /// </summary>
        private readonly GameObject[] m_OverscrollObjects = new GameObject[4];
        /// <summary>
        /// The overscroll rect transforms.
        /// </summary>
        private readonly RectTransform[] m_OverscrollRectTransforms = new RectTransform[4];
        /// <summary>
        /// The overscroll images.
        /// </summary>
        private readonly Image[] m_OverscrollImages = new Image[4];
        /// <summary>
        /// The rect transform.
        /// </summary>
        private RectTransform m_RectTransform;
        /// <summary>
        /// The scroll rect.
        /// </summary>
        private ScrollRect m_ScrollRect;

        /// <summary>
        /// The pointer down.
        /// </summary>
        private readonly bool[] m_PointerDown = new bool[4];

        /// <summary>
        /// The scroll position.
        /// </summary>
        private Vector2 m_ScrollPosition;
        /// <summary>
        /// The last scroll position.
        /// </summary>
        private Vector2 m_LastScrollPosition;
        /// <summary>
        /// The size.
        /// </summary>
        private Vector2 m_Size;

        /// <summary>
        /// The scroll vertical.
        /// </summary>
        private bool m_ScrollVertical;
        /// <summary>
        /// The scroll horizontal.
        /// </summary>
        private bool m_ScrollHorizontal;

        /// <summary>
        /// The edge anims.
        /// </summary>
        private readonly int[] m_EdgeAnims = new int[4];
        /// <summary>
        /// The edge anims current.
        /// </summary>
        private readonly float[] m_EdgeAnimsCurrent = new float[4];
        /// <summary>
        /// The edge anims power.
        /// </summary>
        private readonly float[] m_EdgeAnimsPower = new float[4];
        /// <summary>
        /// The edge anims start time.
        /// </summary>
        private readonly float[] m_EdgeAnimsStartTime = new float[4];
        /// <summary>
        /// The edge anims delta time.
        /// </summary>
        private readonly float[] m_EdgeAnimsDeltaTime = new float[4];
        /// <summary>
        /// The edge anim duration.
        /// </summary>
        private float m_EdgeAnimDuration = 0.3f;

        /// <summary>
        /// The overscroll amounts.
        /// </summary>
        private readonly float[] m_OverscrollAmounts = new float[4];
        /// <summary>
        /// The overscroll positions.
        /// </summary>
        private readonly float[] m_OverscrollPositions = new float[4];

        /// <summary>
        /// The pointer down position.
        /// </summary>
        private Vector2 m_PointerDownPosition;
        /// <summary>
        /// The pointer base amounts.
        /// </summary>
        private readonly float[] m_PointerBaseAmounts = new float[4];

        /// <summary>
        /// The pointer count.
        /// </summary>
        private int m_PointerCount;

        /// <summary>
        /// See Monobehaviour.Start.
        /// </summary>
        void Start()
        {
            Setup();
        }

        /// <summary>
        /// Creates an overscroll object.
        /// </summary>
        /// <param name="i">The index/direction of the overscroll object.</param>
        private void CreateOverscroll(int i)
        {
            if (!overscrollSprite)
            {
                overscrollSprite = Resources.Load<Sprite>("Overscroll");
            }

            m_OverscrollObjects[i] = new GameObject { name = "Overscroll" };
            m_OverscrollImages[i] = m_OverscrollObjects[i].AddComponent<Image>();
            m_OverscrollImages[i].sprite = overscrollSprite;
            m_OverscrollImages[i].color = m_OverscrollColor;
            m_OverscrollRectTransforms[i] = m_OverscrollObjects[i].GetComponent<RectTransform>();
            m_OverscrollRectTransforms[i].SetParent(transform);

            Vector2 tempSize = new Vector2();
            Vector2 anchorMin = new Vector2();
            Vector2 anchorMax = new Vector2();
            Vector3 rotation = new Vector3();

            switch (i)
            {
                case 0:
                    tempSize = new Vector2(0f, m_OverscrollRectTransforms[i].sizeDelta.y);
                    anchorMin = new Vector2(0f, 0.5f);
                    anchorMax = new Vector2(0f, 0.5f);
                    rotation = new Vector3(0f, 0f, 270f);
                    break;
                case 1:
                    tempSize = new Vector2(0f, m_OverscrollRectTransforms[i].sizeDelta.y);
                    anchorMin = new Vector2(1f, 0.5f);
                    anchorMax = new Vector2(1f, 0.5f);
                    rotation = new Vector3(0f, 0f, 90f);
                    break;
                case 2:
                    tempSize = new Vector2(0f, m_OverscrollRectTransforms[i].sizeDelta.y);
                    anchorMin = new Vector2(0.5f, 1f);
                    anchorMax = new Vector2(0.5f, 1f);
                    rotation = new Vector3(0f, 0f, 180f);
                    break;
                case 3:
                    tempSize = new Vector2(0f, m_OverscrollRectTransforms[i].sizeDelta.y);
                    anchorMin = new Vector2(0.5f, 0f);
                    anchorMax = new Vector2(0.5f, 0f);
                    rotation = new Vector3(0f, 0f, 0f);
                    break;
            }

            m_OverscrollRectTransforms[i].position = Vector3.zero;
            m_OverscrollRectTransforms[i].sizeDelta = tempSize;
            m_OverscrollRectTransforms[i].anchorMin = anchorMin;
            m_OverscrollRectTransforms[i].anchorMax = anchorMax;
            m_OverscrollRectTransforms[i].pivot = new Vector2(0.5f, 0f);
            m_OverscrollRectTransforms[i].localEulerAngles = rotation;
            m_OverscrollRectTransforms[i].localScale = Vector3.one;

            CanvasGroup overscrollCanvasGroup = m_OverscrollRectTransforms[i].gameObject.GetAddComponent<CanvasGroup>();
            overscrollCanvasGroup.alpha = 1;
            overscrollCanvasGroup.blocksRaycasts = false;
            overscrollCanvasGroup.interactable = false;
            overscrollCanvasGroup.ignoreParentGroups = true;
        }

        /// <summary>
        /// Calculates which/if overscrolls are needed and generates them accordingly.
        /// This should be called if the ScrollRect or content sizes change. 
        /// </summary>
        public void Setup()
        {
            for (int i = 0; i < m_OverscrollObjects.Length; i++)
            {
                Destroy(m_OverscrollObjects[i]);
            }

            m_RectTransform = gameObject.GetAddComponent<RectTransform>();
            m_ScrollRect = GetComponent<ScrollRect>();

            if (m_ScrollRect.movementType != ScrollRect.MovementType.Clamped)
            {
                enabled = false;
                return;
            }

            Canvas.ForceUpdateCanvases();

            m_ScrollVertical = m_ScrollRect.vertical && m_RectTransform.GetProperSize().y < m_ScrollRect.content.GetProperSize().y;
            m_ScrollHorizontal = m_ScrollRect.horizontal && m_RectTransform.GetProperSize().x < m_ScrollRect.content.GetProperSize().x;

            if (GetComponent<VerticalScrollLayoutElement>())
            {
                StartCoroutine(GetDelayedSize());
            }
            else
            {
                m_Size = m_RectTransform.GetProperSize();
            }

            if (m_ScrollHorizontal)
            {
                CreateOverscroll(0);
                CreateOverscroll(1);
            }
            if (m_ScrollVertical)
            {
                CreateOverscroll(2);
                CreateOverscroll(3);
            }
        }

        /// <summary>
        /// See MonoBehaviour.Update.
        /// </summary>
        void Update()
        {
            m_ScrollPosition = m_ScrollRect.normalizedPosition;

            #region EdgeDetection

            if (m_ScrollPosition.x == 0f)
            {
                if (m_LastScrollPosition.x != 0f)
                {
                    OnEdgeHit(0);
                }
            }
            else if (m_ScrollPosition.x == 1f)
            {
                if (m_LastScrollPosition.x != 1f)
                {
                    OnEdgeHit(1);
                }
            }
            else
            {
                if (m_LastScrollPosition.x == 1f || m_LastScrollPosition.x == 0f)
                {
                    OnEdgeStray(true);
                }
            }

            if (m_ScrollPosition.y == 0f)
            {
                if (m_LastScrollPosition.y != 0f)
                {
                    OnEdgeHit(3);
                }
            }
            else if (m_ScrollPosition.y == 1f)
            {
                if (m_LastScrollPosition.y != 1f)
                {
                    OnEdgeHit(2);
                }
            }
            else
            {
                if (m_LastScrollPosition.y == 1f || m_LastScrollPosition.y == 0f)
                {
                    OnEdgeStray(false);
                }
            }

            #endregion

            #region MouseDetection

            if (Input.GetMouseButtonUp(0))
            {
                if (m_PointerCount >= 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if ((m_ScrollHorizontal && i <= 1) || (m_ScrollVertical && i >= 2))
                        {
                            float tempPosition = (i <= 1) ? m_ScrollPosition.x : m_ScrollPosition.y;
                            float targetPosition = (i == 0 || i == 3) ? 0f : 1f;

                            if (m_PointerDown[i] && tempPosition == targetPosition)
                            {
                                m_EdgeAnimsCurrent[i] = m_OverscrollAmounts[i];
                                m_EdgeAnimsPower[i] = 0;
                                m_EdgeAnimsStartTime[i] = Time.realtimeSinceStartup;
                                m_EdgeAnims[i] = 3;
                            }
                        }
                    }

                    for (int i = 0; i < m_PointerDown.Length; i++)
                    {
                        m_PointerDown[i] = false;
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        m_PointerDown[i] = false;
                    }

                    m_PointerCount = 1;
                }

                m_PointerCount--;
            }

            #endregion

            #region MouseMovement

            bool[] anims = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                if ((m_ScrollHorizontal && i <= 1) || (m_ScrollVertical && i >= 2))
                {
                    float tempPosition = (i <= 1) ? m_ScrollPosition.x : m_ScrollPosition.y;
                    float targetPosition = (i == 0 || i == 3) ? 0f : 1f;

                    if (m_PointerDown[i] && tempPosition == targetPosition)
                    {
                        float fingerOffset = (i <= 1) ? Input.mousePosition.y - m_RectTransform.position.y : Input.mousePosition.x - m_RectTransform.position.x;
                        float tempSize = (i <= 1) ? m_Size.y : m_Size.x;
                        float tempMouseDifference = (i <= 1) ? (Input.mousePosition.x - m_PointerDownPosition.x) / Screen.width : (Input.mousePosition.y - m_PointerDownPosition.y) / Screen.height;

                        fingerOffset = (fingerOffset >= 0f) ? Tween.Linear(0, tempSize / 4f, fingerOffset, tempSize / 2f) : -Tween.Linear(0, tempSize / 4f, -fingerOffset, tempSize / 2f);

                        m_OverscrollPositions[i] = Mathf.Lerp(m_OverscrollPositions[i], fingerOffset, Time.deltaTime * 10f);

                        if (i == 1 || i == 2)
                        {
                            tempMouseDifference *= -1;
                        }


                        m_OverscrollAmounts[i] = Mathf.Lerp(m_PointerBaseAmounts[i], tempSize / 4f, tempMouseDifference);

                        anims[i] = false;
                    }
                    else
                    {
                        anims[i] = true;
                    }


                }
            }

            #endregion

            #region Animations

            for (int i = 0; i < m_OverscrollRectTransforms.Length; i++)
            {
                if (anims[i])
                {
                    if (m_EdgeAnims[i] == 1)
                    {
                        m_EdgeAnimsDeltaTime[i] = Time.realtimeSinceStartup - m_EdgeAnimsStartTime[i];

                        if (m_EdgeAnimsDeltaTime[i] < m_EdgeAnimDuration)
                        {
                            m_OverscrollAmounts[i] = Tween.CubeOut(m_EdgeAnimsCurrent[i], m_EdgeAnimsPower[i], m_EdgeAnimsDeltaTime[i], m_EdgeAnimDuration);
                        }
                        else
                        {
                            float tempPosition = (i <= 1) ? m_ScrollPosition.x : m_ScrollPosition.y;
                            float targetPosition = (i == 0 || i == 3) ? 0f : 1f;

                            if (tempPosition == targetPosition)
                            {
                                m_EdgeAnimsCurrent[i] = m_OverscrollAmounts[i];
                                m_EdgeAnimsPower[i] = 0;
                                m_EdgeAnimsStartTime[i] = Time.realtimeSinceStartup;
                                m_EdgeAnims[i] = 2;
                            }
                            else
                            {
                                m_OverscrollAmounts[i] = m_EdgeAnimsPower[i];
                                m_EdgeAnims[i] = 0;
                            }
                        }
                    }
                    else if (m_EdgeAnims[i] == 2)
                    {
                        m_EdgeAnimsDeltaTime[i] = Time.realtimeSinceStartup - m_EdgeAnimsStartTime[i];

                        if (m_EdgeAnimsDeltaTime[i] < m_EdgeAnimDuration)
                        {
                            m_OverscrollAmounts[i] = Tween.CubeIn(m_EdgeAnimsCurrent[i], m_EdgeAnimsPower[i], m_EdgeAnimsDeltaTime[i], m_EdgeAnimDuration);
                        }
                        else
                        {
                            m_OverscrollAmounts[i] = m_EdgeAnimsPower[i];
                            m_EdgeAnims[i] = 0;
                        }
                    }
                    else if (m_EdgeAnims[i] == 3)
                    {
                        m_EdgeAnimsDeltaTime[i] = Time.realtimeSinceStartup - m_EdgeAnimsStartTime[i];

                        if (m_EdgeAnimsDeltaTime[i] < m_EdgeAnimDuration * 2f)
                        {
                            m_OverscrollAmounts[i] = Tween.CubeIn(m_EdgeAnimsCurrent[i], m_EdgeAnimsPower[i], m_EdgeAnimsDeltaTime[i], m_EdgeAnimDuration * 2f);
                        }
                        else
                        {
                            m_OverscrollAmounts[i] = m_EdgeAnimsPower[i];
                            m_EdgeAnims[i] = 0;
                        }
                    }

                    m_OverscrollPositions[i] = Mathf.Lerp(m_OverscrollPositions[i], 0f, Time.deltaTime);
                }
            }

            #endregion

            for (int i = 0; i < m_OverscrollRectTransforms.Length; i++)
            {
                if ((m_ScrollHorizontal && i <= 1) || (m_ScrollVertical && i >= 2))
                {
                    Vector2 tempAnchoredPos = (i <= 1) ? new Vector2(0f, m_OverscrollPositions[i]) : new Vector2(m_OverscrollPositions[i], 0f);
                    float tempSize = (i <= 1) ? m_Size.y : m_Size.x;

                    m_OverscrollRectTransforms[i].anchoredPosition = tempAnchoredPos;
                    m_OverscrollRectTransforms[i].sizeDelta = new Vector2(tempSize * 1.5f, m_OverscrollAmounts[i] * m_OverscrollScale);
                    m_LastScrollPosition = m_ScrollPosition;
					m_OverscrollImages[i].color = m_OverscrollColor.WithAlpha((m_OverscrollAmounts[i] / (tempSize / 4f)) * m_OverscrollColor.a);
                }
            }
        }

        /// <summary>
        /// Called when [edge hit].
        /// </summary>
        /// <param name="edge">The edge.</param>
        private void OnEdgeHit(int edge)
        {
            if (m_PointerDown[edge])
            {
                SetupPointer(edge);
            }
            else
            {
                float tempSize = (edge <= 1) ? m_Size.y : m_Size.x;
                float tempVelocity = (edge <= 1) ? m_ScrollRect.velocity.x : m_ScrollRect.velocity.y;

                if (edge == 1 || edge == 2)
                {
                    tempVelocity *= -1f;
                }

                m_EdgeAnimsCurrent[edge] = m_OverscrollAmounts[edge];
                m_EdgeAnimsPower[edge] = Mathf.Lerp(0f, tempSize / 4f, Mathf.Sqrt(tempVelocity) / 65f);
                m_EdgeAnimsStartTime[edge] = Time.realtimeSinceStartup;
                m_EdgeAnims[edge] = 1;
            }
        }

        /// <summary>
        /// Called when [edge stray].
        /// </summary>
        /// <param name="horizontal">if set to <c>true</c> [horizontal].</param>
        private void OnEdgeStray(bool horizontal)
        {
            for (int i = 0; i < m_OverscrollRectTransforms.Length; i++)
            {
                if ((horizontal && i <= 1) || (!horizontal && i >= 2))
                {
                    m_EdgeAnimsCurrent[i] = m_OverscrollAmounts[i];
                    m_EdgeAnimsPower[i] = 0f;
                    m_EdgeAnimsStartTime[i] = Time.realtimeSinceStartup;
                    m_EdgeAnims[i] = 2;
                }
            }
        }

        /// <summary>
        /// Setups the pointer.
        /// </summary>
        /// <param name="edge">The edge.</param>
        private void SetupPointer(int edge)
        {
            if (edge == -1)
            {
                for (int i = 0; i < m_OverscrollRectTransforms.Length; i++)
                {
                    m_PointerDown[i] = true;
                    m_EdgeAnims[i] = 0;
                    m_PointerDownPosition = Input.mousePosition;
                    m_PointerBaseAmounts[i] = m_OverscrollAmounts[i];
                }
            }
            else
            {
                m_PointerDown[edge] = true;
                m_EdgeAnims[edge] = 0;
                m_PointerDownPosition = Input.mousePosition;
                m_PointerBaseAmounts[edge] = m_OverscrollAmounts[edge];
            }
        }

        /// <summary>
        /// Gets the size of the ScrollRect, with a small delay.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetDelayedSize()
        {
            VerticalScrollLayoutElement verticalScrollLayoutElement = GetComponent<VerticalScrollLayoutElement>();
            verticalScrollLayoutElement.CalculateLayoutInputVertical();
            yield return new WaitForEndOfFrame();
            m_Size = m_RectTransform.GetProperSize();
            if (!verticalScrollLayoutElement.scrollEnabled)
            {
                enabled = false;
            }
        }

        /// <summary>
        /// Called by a BaseInputModule when a drag has been found but before it is valid to begin the drag.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (m_PointerCount <= 0)
            {
                SetupPointer(-1);
            }

            m_PointerCount++;
        }
    }
}