using UnityEngine;
using System.Collections;

using Rise.Core;
using Rise.Features.MovingMode;
using Rise.Features.InteractionPoint;
using Rise.UI.Effects;

using UnityStandardAssets.ImageEffects;

namespace Rise.UI {
	public class RSUI : RSBehaviour {
		public GameObject ui;

		public Material blendMaterial;

		public int blurTextureResolution = 256;
		public float blurRefreshTime = 1;
		private float lastBlurRefreshTime = 0;

		public RenderTexture uiTexture;
		public RenderTexture UiTexture {
			get { return uiTexture; }
			set { uiTexture = value; }
		}

		private RenderTexture blurTexture;

		private RSUICamera uiCamera;
		private RenderTexture uiTex;

		void Start() {
			uiCamera = RSSceneManager.GetInstance<RSUICamera> ();

			uiTexture = new RenderTexture (Screen.width, Screen.height, 0);
			uiTexture.filterMode = FilterMode.Point;
			uiCamera.GetComponent<Camera> ().targetTexture = uiTexture;
		}

		void OnGUI() {
			blendMaterial.SetTexture ("_BlurTex", ui.GetComponent<UIBlur>().blurTexture);
			Graphics.Blit (uiTexture, uiTexture, blendMaterial);
		}

		void RefreshBlur() {
			if (blurTexture == null) {
				blurTexture = new RenderTexture(blurTextureResolution, blurTextureResolution, 16);
			}

			RenderTexture lastRenderTexture = RenderTexture.active;

			RenderTexture.active = blurTexture;
			MovingModesManager.ActiveCamera.Render ();

			RenderTexture.active = lastRenderTexture;
		}
	}
}