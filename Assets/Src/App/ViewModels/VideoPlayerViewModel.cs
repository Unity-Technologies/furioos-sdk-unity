using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Rise.App.ViewModels {
    public class VideoPlayerViewModel : MonoBehaviour {
        public RawImage image;

        public GameObject header;
        public GameObject footer;

        public GameObject play;
        public GameObject pause;

        public Slider progress;
    }
}
