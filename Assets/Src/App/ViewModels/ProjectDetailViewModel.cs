using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Rise.App.ViewModels {
    public class ProjectDetailViewModel : MonoBehaviour {
	    [Header("Details")]
	    [Space(1)]

        public RawImage thumbnail;
        public AspectRatioFitter thumbnailAspectRatio;

	    [Space(5)]
	    public GameObject descriptionContainer;
	    public Text description;

	    [Space(5)]
	    public GameObject subProjectContainer;
	    public RectTransform subProjectContent;
	    public GameObject subProjectPrefab;

	    [Space(5)]
	    public GameObject textContainer;
	    public RectTransform textContent;
	    public GameObject textView;

	    [Space(10)]

	    [Header("Assets")]
	    [Space(1)]

		public GameObject scenePreviewTab;
		public GameObject scenePreviewContainer;
	    public RectTransform scenePreviewsContent;
	    public GameObject scenePreviewView;

	    [Space(5)]
	    public RectTransform sceneIndicatorsContent;
	    public GameObject sceneIndicatorView;

		[Space(5)]
		public GameObject imagePreviewTab;
		public GameObject imagePreviewContainer;
		public RectTransform imagePreviewsContent;
		public GameObject imagePreviewView;

		[Space(5)]
		public RectTransform imageIndicatorsContent;
		public GameObject imageIndicatorView;

		[Space(5)]
		public GameObject videoPreviewTab;
		public GameObject videoPreviewContainer;
		public RectTransform videoPreviewsContent;
		public GameObject videoPreviewView;

		[Space(5)]
		public RectTransform videoIndicatorsContent;
		public GameObject videoIndicatorView;

		[Space(5)]
		public GameObject documentPreviewTab;
		public GameObject documentPreviewContainer;
		public RectTransform documentPreviewsContent;
		public GameObject documentPreviewView;

		[Space(5)]
		public RectTransform documentIndicatorsContent;
		public GameObject documentIndicatorView;

        void OnDestroy() {
            DestroyImmediate(thumbnail.texture, true);
        }
    }
}