//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
	[ExecuteInEditMode]
	[AddComponentMenu("MaterialUI/Vector Image", 50)]
	public class VectorImage : Text
	{
		public enum SizeMode
		{
			Manual,
			MatchWidth,
			MatchHeight,
			MatchMin,
			MatchMax
		}

		[SerializeField]
		private float m_Size = 48;
		public float size
		{
			get { return m_Size; }
			set
			{
				m_Size = value;
				RefreshScale();
			}
		}

		[SerializeField]
		private float m_ScaledSize;
		public float scaledSize
		{
			get { return m_ScaledSize; }
		}

		[SerializeField]
		private SizeMode m_SizeMode = SizeMode.MatchMin;
		public SizeMode sizeMode
		{
			get { return m_SizeMode; }
			set
			{
				m_SizeMode = value;
				m_Tracker.Clear();
				RefreshScale();
				SetLayoutDirty();
			}
		}

		[SerializeField]
		private MaterialUIScaler m_MaterialUiScaler;
		public MaterialUIScaler materialUiScaler
		{
			get
			{
				if (m_MaterialUiScaler == null)
				{
					m_MaterialUiScaler = MaterialUIScaler.GetParentScaler(transform);
				}
				return m_MaterialUiScaler;
			}
		}

		[SerializeField]
		private VectorImageData m_VectorImageData = new VectorImageData();
		public VectorImageData vectorImageData
		{
			get { return m_VectorImageData; }
			set
			{
				m_VectorImageData = value;
				updateFontAndText();

				RefreshScale();

				#if UNITY_EDITOR
				EditorUtility.SetDirty(gameObject);
				#endif
			}
		}

		private bool m_DisableDirty;
		private float m_LocalScaleFactor;
		#if !UNITY_5_2_0 && !UNITY_5_2_1
		private readonly UIVertex[] m_TempVerts = new UIVertex[4];
		#endif
		private DrivenRectTransformTracker m_Tracker = new DrivenRectTransformTracker();

		public void Refresh()
		{
			updateFontAndText();
			RefreshScale();

			#if UNITY_EDITOR
			EditorUtility.SetDirty(gameObject);
			#endif
		}

		#if UNITY_EDITOR
		public VectorImage()
		{
			EditorUpdate.Init();
			EditorUpdate.onEditorUpdate += OnEditorUpdate;
		}
		#endif

		#region TextUnicode


		#if UNITY_5_2_0 || UNITY_5_2_1
		protected override void OnPopulateMesh(Mesh toFill)
		{
			if (fontSize == 0)
			{
				return;
			}

			string cache = text;
			m_DisableDirty = true;
			text = IconDecoder.Decode(text);
			base.OnPopulateMesh(toFill);
			text = cache;
			m_DisableDirty = false;
		}
		#else
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (fontSize == 0)
			{
				toFill.Clear();
				return;
			}

			m_DisableDirty = true;

			if (font != null)
			{
				m_DisableFontTextureRebuiltCallback = true;
				cachedTextGenerator.Populate(IconDecoder.Decode(text), GetGenerationSettings(rectTransform.rect.size));
				Rect rect = rectTransform.rect;
				Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
				Vector2 zero = Vector2.zero;
				zero.x = textAnchorPivot.x != 1.0 ? rect.xMin : rect.xMax;
				zero.y = textAnchorPivot.y != 0.0 ? rect.yMax : rect.yMin;
				Vector2 vector2 = PixelAdjustPoint(zero) - zero;
				IList<UIVertex> verts = cachedTextGenerator.verts;
				float num1 = 1f / pixelsPerUnit;
				int num2 = verts.Count - 4;
				toFill.Clear();

				if (vector2 != Vector2.zero)
				{
					for (int index1 = 0; index1 < num2; ++index1)
					{
						int index2 = index1 & 3;
						m_TempVerts[index2] = verts[index1];
						m_TempVerts[index2].position *= num1;
						m_TempVerts[index2].position.x += vector2.x;
						m_TempVerts[index2].position.y += vector2.y;

						if (index2 == 3)
						{
							toFill.AddUIVertexQuad(m_TempVerts);
						}
					}
				}
				else
				{
					for (int index1 = 0; index1 < num2; ++index1)
					{
						int index2 = index1 & 3;
						m_TempVerts[index2] = verts[index1];
						m_TempVerts[index2].position *= num1;

						if (index2 == 3)
						{
							toFill.AddUIVertexQuad(m_TempVerts);
						}
					}
				}
				m_DisableFontTextureRebuiltCallback = false;
			}

			m_DisableDirty = false;
		}
		#endif

		public override void SetLayoutDirty()
		{
			if (m_DisableDirty) return;

			base.SetLayoutDirty();
		}

		public override void SetVerticesDirty()
		{
			if (m_DisableDirty) return;

			base.SetVerticesDirty();
		}

		public override void SetMaterialDirty()
		{
			if (m_DisableDirty) return;

			base.SetMaterialDirty();
		}

		#endregion

		protected override void OnEnable()
		{
			base.OnEnable();

			alignment = TextAnchor.MiddleCenter;
			horizontalOverflow = HorizontalWrapMode.Overflow;
			verticalOverflow = VerticalWrapMode.Overflow;

			updateFontAndText();

			SetAllDirty();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			m_Tracker.Clear();
		}

		#if UNITY_EDITOR
		public void OnEditorUpdate()
		{
			if (IsDestroyed())
			{
				EditorUpdate.onEditorUpdate -= OnEditorUpdate;
				return;
			}

			if (!enabled)
			{
				EditorUpdate.onEditorUpdate -= OnEditorUpdate;
				return;
			}

			if (vectorImageData == null || vectorImageData.glyph == null) return;

			updateFontAndText();

			RefreshScale();
		}
		#endif

		protected override void Start()
		{
			alignment = TextAnchor.MiddleCenter;
			horizontalOverflow = HorizontalWrapMode.Overflow;
			verticalOverflow = VerticalWrapMode.Overflow;

			updateFontAndText();

			SetAllDirty();
			UpdateMaterial();
			UpdateGeometry();
		}

		private void updateFontAndText()
		{
			if (vectorImageData != null)
			{
				font = vectorImageData.font;
				text = vectorImageData.glyph.unicode;
			}
		}

		private void RefreshScale()
		{
			if (materialUiScaler == null) // When instantiating the icon for the first time
			{
				return;
			}

			if (!enabled) return;

			if (size == 0 && sizeMode == SizeMode.Manual)
			{
				fontSize = 0;
				return;
			}

			float tempSize = size;

			if (sizeMode == SizeMode.Manual)
			{
				m_ScaledSize = tempSize * materialUiScaler.scaler.scaleFactor;
			}
			else if (sizeMode == SizeMode.MatchWidth)
			{
				m_ScaledSize = rectTransform.rect.width;
				tempSize = m_ScaledSize;
				m_ScaledSize *= materialUiScaler.scaler.scaleFactor;
			}
			else if (sizeMode == SizeMode.MatchHeight)
			{
				m_ScaledSize = rectTransform.rect.height;
				tempSize = m_ScaledSize;
				m_ScaledSize *= materialUiScaler.scaler.scaleFactor;
			}
			else if (sizeMode == SizeMode.MatchMin)
			{
				Vector2 tempVector2 = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

				m_ScaledSize = Mathf.Min(tempVector2.x, tempVector2.y);
				tempSize = m_ScaledSize;
				m_ScaledSize *= materialUiScaler.scaler.scaleFactor;
			}
			else if (sizeMode == SizeMode.MatchMax)
			{
				Vector2 tempVector2 = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

				m_ScaledSize = Mathf.Max(tempVector2.x, tempVector2.y);
				tempSize = m_ScaledSize;
				m_ScaledSize *= materialUiScaler.scaler.scaleFactor;
			}

			if (m_ScaledSize > 500)
			{
				m_LocalScaleFactor = m_ScaledSize / 500;
			}
			else
			{
				m_LocalScaleFactor = 1f;
			}

			tempSize *= m_LocalScaleFactor;

			fontSize = Mathf.RoundToInt(tempSize);

			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				EditorUtility.SetDirty(this);
			}
			#endif

			m_LocalScaleFactor *= (size / Mathf.Max(size));

			if (m_LocalScaleFactor != float.NaN && new Vector3(m_LocalScaleFactor, m_LocalScaleFactor, m_LocalScaleFactor) != rectTransform.localScale)
			{
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
				rectTransform.localScale = new Vector3(m_LocalScaleFactor, m_LocalScaleFactor, m_LocalScaleFactor);
			}
		}

		public override void CalculateLayoutInputHorizontal()
		{
			RefreshScale();
		}

		public override void CalculateLayoutInputVertical()
		{
			RefreshScale();
		}

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			m_Tracker.Clear();
			RefreshScale();
			base.OnValidate();
			SetLayoutDirty();

			updateFontAndText();
		}
		#endif

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			RefreshScale();
		}

		public override float preferredWidth { get { return size; } }
		public override float minWidth { get { return -1; } }
		public override float flexibleWidth { get { return -1; } }
		public override float preferredHeight { get { return size; } }
		public override float minHeight { get { return -1; } }
		public override float flexibleHeight { get { return -1; } }
	}
}