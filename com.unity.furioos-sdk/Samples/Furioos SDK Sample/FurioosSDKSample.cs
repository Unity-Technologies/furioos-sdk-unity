using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Furioos.SDK {

    

    public class FurioosSDKSample : MonoBehaviour {

        [SerializeField]
        public FurioosSDK sdk;

        public TMP_InputField input;
        public TMP_InputField output;
        public Text info;
        public Dropdown peersDropdown;


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

            this.peersDropdown.ClearOptions();
            List<string> options = new List<string> { "Select recipient : ", "All" };
            this.peersDropdown.AddOptions(options);

            this.fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;

            this.sdk.OnSDKMessage += this.OnSDKMessage;
            this.sdk.OnSDKSessionStart += this.OnSDKSessionStart;
            this.sdk.OnSDKSessionStop += this.OnSDKSessionStop;

            Application.logMessageReceived += this.OnLog;
        }





        void OnDisable() {

            this.sdk.OnSDKMessage -= this.OnSDKMessage;
            this.sdk.OnSDKSessionStart -= this.OnSDKSessionStart;
            this.sdk.OnSDKSessionStop -= this.OnSDKSessionStop;

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

        private void OnSDKSessionStart(string from){
            Debug.Log("SDK session started : \"" + from);
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = from;
            this.peersDropdown.options.Add(option);
            this.peersDropdown.value = this.peersDropdown.options.Count - 1;
        }

        private void OnSDKSessionStop(string from){
            Debug.Log("SDK session stopped : \"" + from);
            int index = this.peersDropdown.options.FindIndex(option => option.text == from);
            if (this.peersDropdown.value == index) this.peersDropdown.value = 0;
            this.peersDropdown.options.RemoveAt(index);
        }

        public void OnSDKMessage(JToken data, string from) {
            Debug.Log("SDK data from \"" + from + "\" : \n" + JsonConvert.SerializeObject(data));
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

        public void OnSendButtonClicked() {

            int tiIndex = this.peersDropdown.value;

            if (this.peersDropdown.value > 0) {

                string to = this.peersDropdown.value > 1 ? this.peersDropdown.options[this.peersDropdown.value].text : "";

                try {
                    JObject data = JObject.Parse(this.input.text);
                    Debug.Log("Sending json to " + to + " : " + JsonConvert.SerializeObject(data));
                    sdk.send(data, to);
                } catch {
                    Debug.Log("Sending string to " + to + " : " + this.input.text);
                    sdk.send(this.input.text, to);
                }
            } else {
                Debug.Log("Please select a recipient");
            }
        }
    }
}