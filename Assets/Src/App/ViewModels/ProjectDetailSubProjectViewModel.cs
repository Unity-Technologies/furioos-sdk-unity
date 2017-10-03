using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


namespace Rise.App.ViewModels {
    public class ProjectDetailSubProjectViewModel : MonoBehaviour {
	    public new Text name;
	    public RawImage image;
	    public Button view;

        void OnDestroy() {
            Destroy(name);

            DestroyImmediate(image.texture, true);
            Destroy(image);

            Destroy(view);
        }
    }
}