using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FurioosSDK.Core {
	public class FSOutputManager : FSManagerModule {

		private static FSOutput activeOutput;
		private bool clearDisplay = true;
		
		public delegate void OutputChangedEventHandler(FSOutput outputMode);
		public delegate void OnRenderSizeChangedHandler( int w, int h );
		public event OnRenderSizeChangedHandler OnRenderSizeChanged;
		public event OutputChangedEventHandler OutputChanged;

		public List<FSOutput> GetAvailableOutputs() {	
			return Manager.GetAllInstances<FSOutput>();
		}

		public FSOutput Active {
			get {
				return activeOutput;
			}
			protected set {
				if(activeOutput != (activeOutput = value) && OutputChanged != null) {
					OutputChanged(activeOutput);
				}
			}
		}

		public void Start() {
			Debug.Log("Starting outputs manager");

			List<FSOutput> outputModes = GetAvailableOutputs();

			Type outputModeType = typeof(FSOutput2D);

			bool found = false;
			for(int i = outputModes.Count-1; i >= 0; i--) {
				FSOutput o = outputModes[i];

				o.OnPreactivate += OnOutputPreactivate;
				o.OnActivated += OnOutputActivated;

				if(o.enabled) {
					o.enabled = false;
				}

				if(o.GetType() == outputModeType) {
					o.enabled = true;
					found = true;
				}
				else if(i==0 && !found) {
					o.enabled = true;
				}

				o.OnDesactivated += OnOutputModeDesactivated;
			}
		}
			
		void OnOutputPreactivate (FSOutput outputMode) {
			foreach(FSOutput om in GetAvailableOutputs()) {
				if(om != outputMode && om.enabled) {
					om.enabled = false;
				}
			}
		}

		void OnOutputActivated (FSOutput outputMode) {
			Manager.SetSetting("OutputModeType",outputMode.GetType());

			if(CamerasManager.Active != null) {
				outputMode.UpdateCamera(CamerasManager.Active);
			}

			Active = outputMode;
			clearDisplay = true;
		}

		void OnOutputModeDesactivated (FSOutput outputMode) {
			outputMode.DetachFromCamera();

			if(Active == outputMode) {
				Active = null;
			}
		}
		
		public int RenderWidth {
			get; 
			private set;
		}
		
		public int RenderHeight {
			get; 
			private set;
		}


		public void RenderImage(RenderTexture source, RenderTexture destination) {
			bool sizeChanged = false;
			sizeChanged |= (RenderWidth != (RenderWidth = (destination == null) ? Screen.width : destination.width));				
			sizeChanged |= (RenderHeight != (RenderHeight = (destination == null) ? Screen.height : destination.height));
			if(sizeChanged){
				if(OnRenderSizeChanged != null) {
					OnRenderSizeChanged(RenderWidth, RenderHeight);
				}
			}

			if(Active != null && !clearDisplay) {
				Active.RenderImage(source, destination);
			}
			else {
				RenderTexture.active = destination;
				GL.Clear(false, true, new Color(0,0,0,1));
				clearDisplay = false;
			}
		}

		public void RenderGui(RenderTexture guiTexture) {
			RenderTexture.active = null;

			if(Active!=null && !clearDisplay) {
				Active.RenderGui(guiTexture);
			}
			else {
				GL.Clear(false, true, new Color(0,0,0,1));
				clearDisplay = false;
			}
		}

		public FSOutput ActivateOutputMode(string id) {
			FSOutput newOutput = Manager.GetInstance<FSOutput>(id);
			
			if(newOutput != null) {
				newOutput.enabled = true;
			}
			
			return newOutput;
		}
		
		public void DesactivateAllOutputModes() {
			List<FSOutput> outputs = Manager.GetAllInstances<FSOutput>();

			foreach( FSOutput o in outputs) {
				if(o.enabled) {
					o.enabled=false;
				}
			}
		}
	}
}