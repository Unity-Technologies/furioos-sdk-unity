using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Rise.App.ViewModels {
    public class ProjectViewModel : MonoBehaviour {
	    public new Text name;
	    public RawImage image;
        public AspectRatioFitter aspectRatioFitter;
	    public Button view;

        void OnDestroy() {
            Destroy(name);

            DestroyImmediate(image.texture, true);
            Destroy(image);

            Destroy(aspectRatioFitter);

            Destroy(view);
        }
    }
}