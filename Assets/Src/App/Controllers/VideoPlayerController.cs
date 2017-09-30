using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Video;

using Rise.Core;
using Rise.App.ViewModels;

namespace Rise.App.Controllers {
    public class VideoPlayerController : RSBehaviour {
        private static VideoPlayerController _instance;

        public VideoPlayer videoPlayer;
        public VideoPlayerViewModel videoPlayerViewModel;

        private RenderTexture rt;

        public static void Play(string uri) {
            if(_instance == null) {
                return;
            }

            _instance.StartPlay(uri);
        }

        public void Start() {
            Vector2 size = videoPlayerViewModel.image.GetComponent<RectTransform>().sizeDelta;

            rt = new RenderTexture(1920, 1080, 24);

            _instance = this;
        }

        public void StartPlay(string uri) {
            videoPlayer.url = uri;
            videoPlayer.targetTexture = rt;
            videoPlayer.Play();

            videoPlayerViewModel.image.texture = rt;

            videoPlayerViewModel.gameObject.SetActive(true);
            videoPlayerViewModel.play.SetActive(false);
            videoPlayerViewModel.pause.SetActive(true);
        }

        public void Exit() {
            videoPlayer.Stop();

            videoPlayerViewModel.gameObject.SetActive(false);

            videoPlayerViewModel.play.SetActive(true);
            videoPlayerViewModel.pause.SetActive(false);
        }

        public void Resume() {
            videoPlayerViewModel.play.SetActive(false);
            videoPlayerViewModel.pause.SetActive(true);

            videoPlayer.Play();
        }

        public void Pause() {
            videoPlayerViewModel.play.SetActive(true);
            videoPlayerViewModel.pause.SetActive(false);

            videoPlayer.Pause();
        }

        public void Update() {
            videoPlayerViewModel.progress.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        }
    }
}
