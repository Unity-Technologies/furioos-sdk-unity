//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    /// <summary>
    /// Captures drag events from an object and sends them to other handlers, depending on the orientation of the drag.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="UnityEngine.EventSystems.IDragHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IBeginDragHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IEndDragHandler" />
    public class DragEventSender : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// This object will recieve events if the current object is dragged horizontally.
        /// </summary>
        [SerializeField]
        private GameObject m_HorizontalTargetObject;
        /// <summary>
        /// This object will recieve events if the current object is dragged horizontally.
        /// </summary>
        public GameObject horizontalTargetObject
        {
            get { return m_HorizontalTargetObject; }
            set { m_HorizontalTargetObject = value; }
        }

        /// <summary>
        /// This object will recieve events if the current object is dragged to the left.
        /// </summary>
        [SerializeField]
        private GameObject m_LeftTargetObject;
        /// <summary>
        /// This object will recieve events if the current object is dragged to the left.
        /// </summary>
        public GameObject leftTargetObject
        {
            get { return m_LeftTargetObject; }
            set { m_LeftTargetObject = value; }
        }

        /// <summary>
        /// This object will recieve events if the current object is dragged to the right.
        /// </summary>
        [SerializeField]
        private GameObject m_RightTargetObject;
        /// <summary>
        /// This object will recieve events if the current object is dragged to the right.
        /// </summary>
        public GameObject rightTargetObject
        {
            get { return m_RightTargetObject; }
            set { m_RightTargetObject = value; }
        }

        /// <summary>
        /// This object will recieve events if the current object is dragged vertically.
        /// </summary>
        [SerializeField]
        private GameObject m_VerticalTargetObject;
        /// <summary>
        /// This object will recieve events if the current object is dragged vertically.
        /// </summary>
        /// <value>
        /// The vertical target object.
        /// </value>
        public GameObject verticalTargetObject
        {
            get { return m_VerticalTargetObject; }
            set { m_VerticalTargetObject = value; }
        }

        /// <summary>
        /// This object will recieve events if the current object is dragged up.
        /// </summary>
        [SerializeField]
        private GameObject m_UpTargetObject;
        /// <summary>
        /// This object will recieve events if the current object is dragged up.
        /// </summary>
        public GameObject upTargetObject
        {
            get { return m_UpTargetObject; }
            set { m_UpTargetObject = value; }
        }

        /// <summary>
        /// This object will recieve events if the current object is dragged down.
        /// </summary>
        [SerializeField]
        private GameObject m_DownTargetObject;
        /// <summary>
        /// This object will recieve events if the current object is dragged down.
        /// </summary>
        public GameObject downTargetObject
        {
            get { return m_DownTargetObject; }
            set { m_DownTargetObject = value; }
        }

        /// <summary>
        /// This object will recieve events regardless of whether the current object is dragged horizontally or vertically.
        /// </summary>
        [SerializeField]
        private GameObject m_AnyDirectionTargetObject;
        /// <summary>
        /// This object will recieve events regardless of whether the current object is dragged horizontally or vertically.
        /// </summary>
        /// <value>
        /// Any direction target object.
        /// </value>
        public GameObject anyDirectionTargetObject
        {
            get { return m_AnyDirectionTargetObject; }
            set { m_AnyDirectionTargetObject = value; }
        }

        /// <summary>
        /// On the last OnBeginDrag event, was the current object dragged horizontally?
        /// </summary>
        private bool m_CurrentDragIsHorizontal;

        /// <summary>
        /// On the last OnBeginDrag event, was the current object dragged to the left?
        /// </summary>
        private bool m_CurrentDragIsLeft;

        /// <summary>
        /// On the last OnBeginDrag event, was the current object dragged up?
        /// </summary>
        private bool m_CurrentDragIsUp;

        /// <summary>
        /// Should horizontal drags in either direction send to the same object?
        /// </summary>
        [SerializeField]
        private bool m_CombineLeftAndRight = true;
        /// <summary>
        /// Should horizontal drags in either direction send to the same object?
        /// </summary>
        public bool combineLeftAndRight
        {
            get { return m_CombineLeftAndRight; }
            set { m_CombineLeftAndRight = value; }
        }

        /// <summary>
        /// Should vertical drags in either direction send to the same object?
        /// </summary>
        [SerializeField]
        private bool m_CombineUpAndDown = true;
        /// <summary>
        /// Should vertical drags in either direction send to the same object?
        /// </summary>
        public bool combineUpAndDown
        {
            get { return m_CombineUpAndDown; }
            set { m_CombineUpAndDown = value; }
        }

        /// <summary>
        /// When draging is occuring this will be called every time the cursor is moved.
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (m_CurrentDragIsHorizontal)
            {
                if (m_CombineLeftAndRight)
                {
                    if (m_HorizontalTargetObject != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(m_HorizontalTargetObject, eventData, ExecuteEvents.dragHandler);
                    }
                }
                else
                {
                    m_CurrentDragIsLeft = eventData.delta.x < 0;

                    if (m_CurrentDragIsLeft)
                    {
                        if (m_LeftTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_LeftTargetObject, eventData, ExecuteEvents.dragHandler);
                        }
                    }
                    else
                    {
                        if (m_RightTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_RightTargetObject, eventData, ExecuteEvents.dragHandler);
                        }
                    }
                }
            }
            else
            {
                if (m_CombineUpAndDown)
                {
                    if (m_VerticalTargetObject != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(m_VerticalTargetObject, eventData, ExecuteEvents.dragHandler);
                    }
                }
                else
                {
                    m_CurrentDragIsUp = eventData.delta.y > 0;

                    if (m_CurrentDragIsUp)
                    {
                        if (m_UpTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_UpTargetObject, eventData, ExecuteEvents.dragHandler);
                        }
                    }
                    else
                    {
                        if (m_DownTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_DownTargetObject, eventData, ExecuteEvents.dragHandler);
                        }
                    }
                }
            }

            if (m_AnyDirectionTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_AnyDirectionTargetObject, eventData, ExecuteEvents.dragHandler);
            }
        }

        /// <summary>
        /// Called by a BaseInputModule before a drag is started.
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_CurrentDragIsHorizontal = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

            if (m_CurrentDragIsHorizontal)
            {
                if (m_CombineLeftAndRight)
                {
                    if (m_HorizontalTargetObject != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(m_HorizontalTargetObject, eventData, ExecuteEvents.beginDragHandler);
                    }
                }
                else
                {
                    m_CurrentDragIsLeft = eventData.delta.x < 0;

                    if (m_CurrentDragIsLeft)
                    {
                        if (m_LeftTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_LeftTargetObject, eventData, ExecuteEvents.beginDragHandler);
                        }
                    }
                    else
                    {
                        if (m_RightTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_RightTargetObject, eventData, ExecuteEvents.beginDragHandler);
                        }
                    }
                }
            }
            else
            {
                if (m_CombineUpAndDown)
                {
                    if (m_VerticalTargetObject != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(m_VerticalTargetObject, eventData, ExecuteEvents.beginDragHandler);
                    }
                }
                else
                {
                    m_CurrentDragIsUp = eventData.delta.y > 0;

                    if (m_CurrentDragIsUp)
                    {
                        if (m_UpTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_UpTargetObject, eventData, ExecuteEvents.beginDragHandler);
                        }
                    }
                    else
                    {
                        if (m_DownTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_DownTargetObject, eventData, ExecuteEvents.beginDragHandler);
                        }
                    }
                }
            }

            if (m_AnyDirectionTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_AnyDirectionTargetObject, eventData, ExecuteEvents.beginDragHandler);
            }
        }

        /// <summary>
        /// Called by a BaseInputModule when a drag is ended.
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (m_CurrentDragIsHorizontal)
            {
                if (m_CombineLeftAndRight)
                {
                    if (m_HorizontalTargetObject != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(m_HorizontalTargetObject, eventData, ExecuteEvents.endDragHandler);
                    }
                }
                else
                {
                    m_CurrentDragIsLeft = eventData.delta.x < 0;

                    if (m_CurrentDragIsLeft)
                    {
                        if (m_LeftTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_LeftTargetObject, eventData, ExecuteEvents.endDragHandler);
                        }
                    }
                    else
                    {
                        if (m_RightTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_RightTargetObject, eventData, ExecuteEvents.endDragHandler);
                        }
                    }
                }
            }
            else
            {
                if (m_CombineUpAndDown)
                {
                    if (m_VerticalTargetObject != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(m_VerticalTargetObject, eventData, ExecuteEvents.endDragHandler);
                    }
                }
                else
                {
                    m_CurrentDragIsUp = eventData.delta.y > 0;

                    if (m_CurrentDragIsUp)
                    {
                        if (m_UpTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_UpTargetObject, eventData, ExecuteEvents.endDragHandler);
                        }
                    }
                    else
                    {
                        if (m_DownTargetObject != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(m_DownTargetObject, eventData, ExecuteEvents.endDragHandler);
                        }
                    }
                }
            }

            if (m_AnyDirectionTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_AnyDirectionTargetObject, eventData, ExecuteEvents.endDragHandler);
            }
        }
    }
}