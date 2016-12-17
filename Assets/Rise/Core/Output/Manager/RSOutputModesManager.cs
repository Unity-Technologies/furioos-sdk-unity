using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Rise.Core {
	public class RSOutputModesManager : RSSceneManagerModule{

		private static RSOutputMode activeOutputMode;
		private bool clearDisplay = true;
		
		public delegate void OutputModeChangedEventHandler(RSOutputMode outputMode);
		public delegate void OnRenderSizeChangedHandler( int w, int h );
		public event OnRenderSizeChangedHandler OnRenderSizeChanged;
		public event OutputModeChangedEventHandler OutputModeChanged;


		public List<RSOutputMode> GetAvailableOutputModes()
		{	
			return RSSceneManager.GetAllInstances<RSOutputMode>();
		}
		
		
		public RSOutputMode Active {
			get {
				return activeOutputMode;
			}
			protected set {
				if(activeOutputMode != (activeOutputMode=value) && OutputModeChanged!=null){
					OutputModeChanged(activeOutputMode);
				}
			}
		}

		public void Start(){

			Debug.Log("Starting output modes manager");

			List<RSOutputMode> outputModes = GetAvailableOutputModes();


			//Type outputModeType = SceneManager.GetSetting("OutputModeType", (Type) null);
			Type outputModeType = typeof(RSOutputMode2D);

			bool found = false;
			for(int i=outputModes.Count-1;i>=0;i--){
				RSOutputMode om = outputModes[i];

				om.OnPreactivate += OnOutputModePreactivate;
				om.OnActivated += OnOutputModeActivated;

				if(om.enabled)om.enabled=false;

				if(om.GetType() == outputModeType){
					om.enabled=true;
					found = true;
				}else if(i==0 && !found){
					om.enabled=true;
				}

				om.OnDesactivated += OnOutputModeDesactivated;

			}

		}



		void OnOutputModePreactivate (RSOutputMode outputMode)
		{
			foreach(RSOutputMode om in GetAvailableOutputModes()){
				if(om!=outputMode && om.enabled)om.enabled=false;
			}

		}

		void OnOutputModeActivated (RSOutputMode outputMode)
		{
			SceneManager.SetSetting("OutputModeType",outputMode.GetType());

			if(MovingModesManager.Active!=null)outputMode.AttachToMovingMode(MovingModesManager.Active);
			Active = outputMode;
			clearDisplay = true;
		}

		void OnOutputModeDesactivated (RSOutputMode outputMode)
		{
			outputMode.DetachFromMovingMode();
			if(Active==outputMode) Active = null;
		}
		
		public int RenderWidth{
			get; private set;
		}
		
		public int RenderHeight{
			get; private set;
		}


		public void RenderImage(RenderTexture source, RenderTexture destination){
			bool sizeChanged = false;
			sizeChanged |= (RenderWidth != (RenderWidth = (destination == null) ? Screen.width : destination.width));				
			sizeChanged |= (RenderHeight != (RenderHeight = (destination == null) ? Screen.height : destination.height));
			if(sizeChanged){
				if(OnRenderSizeChanged!=null)OnRenderSizeChanged(RenderWidth,RenderHeight);
			}

			if(Active!=null && !clearDisplay){
				Active.RenderImage(source, destination);
			}else{
				RenderTexture.active = destination;
				GL.Clear(false, true, new Color(0,0,0,1)) ;
				clearDisplay = false;
			}
		}

		public void RenderGui(RenderTexture guiTexture){
			RenderTexture.active = null;
			if(Active!=null && !clearDisplay){
				Active.RenderGui(guiTexture);
			}else{
				GL.Clear(false, true, new Color(0,0,0,1)) ;
				clearDisplay = false;
			}

		}

		public RSOutputMode ActivateOutputMode(string id){
			
			RSOutputMode newOutputMode = RSSceneManager.GetInstance<RSOutputMode>(id);
			
			if(newOutputMode!=null)newOutputMode.enabled=true;
			
			return newOutputMode;
		}
		
		public void DesactivateAllOutputModes(){
			List<RSOutputMode> outputModes = RSSceneManager.GetAllInstances<RSOutputMode>();
			foreach( RSOutputMode om in outputModes){
				if(om.enabled)om.enabled=false;
			}
		}
		

	}
}