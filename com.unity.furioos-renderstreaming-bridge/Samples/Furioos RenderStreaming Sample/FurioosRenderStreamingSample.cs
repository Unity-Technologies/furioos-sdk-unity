using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Furioos.SDK {

    

    public class FurioosRenderStreamingSample : MonoBehaviour {

        public TMP_InputField output;
        public Text info;


        const float fpsMeasurePeriod = 0.5f;
        private int fpsAccumulator = 0;
        private float fpsNextPeriod = 0;
        private float fps = 0;

        private bool fullScreen = false;
        private FullScreenMode fullScreenMode;
        private int width = 0;
        private int height = 0;
        private int windowWidth = 0;
        private int windowHeight = 0;



        void OnEnable() {

            

            Application.logMessageReceived += this.OnLog;
        }





        void OnDisable() {


            Application.logMessageReceived -= OnLog;
        
        }

        void Start() {

            this.width = Screen.width;
            this.height = Screen.height;
            this.windowWidth = Screen.width;
            this.windowHeight = Screen.height;
            this.fullScreen = Screen.fullScreen;
            this.fullScreenMode = Screen.fullScreenMode;

        }


        void Update() {

            if (this.fullScreen && (this.width != Screen.currentResolution.width || this.height != Screen.currentResolution.height)) {

                this.width = Screen.currentResolution.width;
                this.height = Screen.currentResolution.height;

                Debug.Log("Update SetResolution : " + this.width + " x " + this.height + " " + this.fullScreen);

                Screen.SetResolution(this.width, this.height, this.fullScreenMode);
            }

            if (this.fullScreen && (Screen.width != Screen.currentResolution.width || Screen.height != Screen.currentResolution.height)) {

                this.width = Screen.currentResolution.width;
                this.height = Screen.currentResolution.height;

                Debug.Log("Go back to windowed : " + this.width + " x " + this.height + " " + this.fullScreen);

                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
            }

            if (this.fullScreen && !Screen.fullScreen){

                this.width = Screen.currentResolution.width;
                this.height = Screen.currentResolution.height;

                Debug.Log("Go back to fullscreen : " + this.width + " x " + this.height + " " + this.fullScreen);

                Screen.SetResolution(this.width, this.height, this.fullScreenMode);
            }



            this.fpsAccumulator++;
            if (Time.realtimeSinceStartup > this.fpsNextPeriod) {
                this.fps = ((int)(10 * this.fpsAccumulator / fpsMeasurePeriod))/10;
                this.fpsAccumulator = 0;
                this.fpsNextPeriod += fpsMeasurePeriod;
            }


            info.text = "Info :\n\nWidth : " + Screen.width
                + "\nHeight : " + Screen.height
                + "\nWidth : " + Screen.currentResolution.width
                + "\nHeight : " + Screen.currentResolution.height
                + "\nDpi : " + Screen.dpi
                + "\nFullScreen : " + Screen.fullScreen
                + "\nFullScreen mode : " + Screen.fullScreenMode
                + "\nFPS : " + this.fps
                + "\nTime : " + DateTime.Now.ToString()
            ;

        }

        public void OnLog(string logString, string stackTrace, LogType type) {
           this.output.text += logString + "\n\n";
            this.output.verticalScrollbar.value = 1;
        }

        private void toggleFullScreen(FullScreenMode mode) {

            this.fullScreen = !this.fullScreen;

            if (this.fullScreen) {

                this.windowWidth = Screen.width;
                this.windowHeight = Screen.height;
                this.fullScreenMode = mode;

                Screen.fullScreenMode = this.fullScreenMode;

                Resolution currentResolution = Screen.currentResolution;
                Debug.Log("toggleFullScreen SetResolution fullscreen : " + currentResolution.width + " x " + currentResolution.height);

                Screen.SetResolution(currentResolution.width, currentResolution.height, this.fullScreenMode);

            } else {

                this.fullScreenMode = FullScreenMode.Windowed;

                Debug.Log("toggleFullScreen SetResolution window : " + this.windowWidth + " x " + this.windowHeight);

                Screen.SetResolution(this.windowWidth, this.windowHeight, false);
            }
        }

        public void OnFullScreenButtonClicked() {
            this.toggleFullScreen(FullScreenMode.FullScreenWindow);
        }

        public void OnExFullScreenButtonClicked() {
            this.toggleFullScreen(FullScreenMode.ExclusiveFullScreen);
        }

    }
}