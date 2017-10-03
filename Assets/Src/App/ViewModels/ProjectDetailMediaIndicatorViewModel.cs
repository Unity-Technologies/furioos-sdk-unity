using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Rise.App.ViewModels {
    public class ProjectDetailMediaIndicatorViewModel : MonoBehaviour {
	    public Toggle toggle;
	    public RawImage image;
        public AspectRatioFitter aspectRatioFitter;
	    public GameObject videoIcon;
	    public GameObject threeDIcon;

        void OnDestroy() {
            DestroyImmediate(image.texture, true);
            Destroy(image);
        }
    }
}