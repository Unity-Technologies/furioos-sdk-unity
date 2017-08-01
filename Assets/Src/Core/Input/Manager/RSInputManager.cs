#pragma warning disable 0067
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tuio;

namespace Rise.Core {
	public class RSInputManager  : RSManagerModule {

		public const int mouseToucheId = -150000;

		public delegate void FingerTouchEventHandler(RSInputTouch sender);
		
		
		public static event FingerTouchEventHandler Clicked;
		public static event FingerTouchEventHandler SimpleClicked;
		public static event FingerTouchEventHandler GoneActive;
		public static event FingerTouchEventHandler GoneInactive;
		public static event FingerTouchEventHandler DragStarted;
		public static event FingerTouchEventHandler DragStopped;
		
		public float clickTime = 0.3f;
		public float clickDistanceTolerance = 25.0f;

		public bool useJoypadAsMouse = true;
		public float joypadCursorSpeed = 0.5f;

		private bool joypadIsConnected = false;

		private bool isUpdated;
		
		private List<RSInputTouch> aliveTouches;
		private List<int> aliveTouchesIds;
		
		private float desktopZoom;
		private float deltaZoom;
		private float desktopLastZoom;
		private float desktopZoomSpeed;

		private bool isZooming;
		
		private int numTouches;
		
		private RSInputTouch defaultTouch;
		private float lastGoneActiveTime;
		private float lastGoneInactiveTime;
		private int numClick;
		private bool hasBeenClicked;
		private bool hasBeenSimpleClicked;
		private bool hasBeenSimpleClickedDone;
		
		private Vector3 savedMousePos;
		private float savedZoomDist;
		
		private float dpiFactor;
		
		private Dictionary<string,RSInputBinding> inputBindings = new Dictionary<string, RSInputBinding>();
		
		
		public Vector2 Position {
			get {
				UpdateControlState();
				return defaultTouch.Position;
			}
		}
		
		public Vector2 NormalizedPosition {
			get {
				UpdateControlState();
				return defaultTouch.NormalizedPosition;
			}
		}
		
		public Vector2 Delta {
			get {
				UpdateControlState();
				return defaultTouch.Delta;
			}
		}
		
		public int NumClick {
			get {
				UpdateControlState();
				return numClick;
			}
		}
		
		public bool HasBeenClicked {
			get {
				UpdateControlState();
				return hasBeenClicked;
			}
		}
		
		public bool HasBeenSimpleClicked {
			get {
				UpdateControlState();
				return hasBeenSimpleClicked;
			}
		}
		
		public Vector2 NormalizedDelta {
			get {
				UpdateControlState();
				return defaultTouch.NormalizedDelta;
			}
		}
		
		public bool IsMoving {
			get {
				UpdateControlState();
				return defaultTouch.IsMoving;
			}

		}
		
		public bool HasGoneActive {
			get {
				UpdateControlState();
				return defaultTouch.HasGoneActive;
			}

		}
		
		public bool HasGoneInactive {
			get {
				UpdateControlState();
				return defaultTouch.HasGoneInactive;
			}

		}
		
		public float Distance {
			get {
				UpdateControlState();
				return defaultTouch.Distance;
			}
		}
		
		public bool IsZooming {
			get {
				UpdateControlState();
				return isZooming;
			}
		}
		
		public float DeltaZoom {
			get {
				UpdateControlState();
				return deltaZoom;
			}
		}
		
		public int NumTouch {
			get {
				UpdateControlState();
				return numTouches;
			}
		}
		
		public List<RSInputTouch> Touches {
			get {
				UpdateControlState();
				return aliveTouches;
			}
		}
		
		
		public void Start() { 
			savedMousePos = Input.mousePosition;

			desktopZoom = 1.0f;
			desktopLastZoom = 1.0f;
			desktopZoomSpeed = 0.0f;
			
			numTouches = 0;
			
			defaultTouch = new RSInputTouch();
			aliveTouches = new List<RSInputTouch>();
			aliveTouchesIds = new List<int>();
			lastGoneActiveTime = Time.time;
			lastGoneInactiveTime = Time.time;
			numClick = 0;

			joypadIsConnected = false;

			dpiFactor = Mathf.Max( Screen.dpi != 0 ? Screen.dpi / 130.0f : 1.0f , 1.0f);

			RSInputBinding (true);
		}

		public void RSInputBinding(bool init = false) {
			if (init) {
				inputBindings.Add ("Move X", new RSInputBinding ());
				inputBindings.Add ("Move Y", new RSInputBinding ());
				inputBindings.Add ("Look X", new RSInputBinding ());
				inputBindings.Add ("Look Y", new RSInputBinding ());
				inputBindings.Add ("Cursor X", new RSInputBinding ());
				inputBindings.Add ("Cursor Y", new RSInputBinding ());
				inputBindings.Add ("Click", new RSInputBinding ());
				inputBindings.Add ("Jet Pack", new RSInputBinding ());
				inputBindings.Add ("Zoom", new RSInputBinding ());
			}
			
			if(Application.platform == RuntimePlatform.OSXEditor ||
			   Application.platform == RuntimePlatform.OSXPlayer ||
			   Application.platform == RuntimePlatform.WebGLPlayer ||
			   Application.platform == RuntimePlatform.OSXDashboardPlayer){
				
				inputBindings["Move X"].SetJoyStick("joystick axis 0");
				inputBindings["Move Y"].SetJoyStick("joystick axis 1");
				inputBindings["Look X"].SetJoyStick("joystick axis 2");
				inputBindings["Look Y"].SetJoyStick("joystick axis 3");
				inputBindings["Cursor X"].SetJoyStick("joystick button 8","joystick button 7");
				inputBindings["Cursor Y"].SetJoyStick("joystick button 5","joystick button 6");
				inputBindings["Click"].SetJoyStick("joystick button 16");
				inputBindings["Jet Pack"].SetJoyStick("joystick button 17");
				inputBindings["Zoom"].SetJoyStick("joystick axis 1");
				
			} 
			else {
				inputBindings["Move X"].SetJoyStick("joystick axis 0");
				inputBindings["Move Y"].SetJoyStick("joystick axis 1");
				inputBindings["Look X"].SetJoyStick("joystick axis 3");
				inputBindings["Look Y"].SetJoyStick("joystick axis 4");
				inputBindings["Cursor X"].SetJoyStick("joystick axis 5");
				inputBindings["Cursor Y"].SetJoyStick("joystick axis 6");
				inputBindings["Click"].SetJoyStick("joystick button 0");
				inputBindings["Jet Pack"].SetJoyStick("joystick button 1");
				inputBindings["Zoom"].SetJoyStick("joystick axis 1");
			}
			
			inputBindings["Move X"].SetKeyboard(KeyCode.RightArrow,KeyCode.LeftArrow);
			inputBindings["Move Y"].SetKeyboard(KeyCode.DownArrow,KeyCode.UpArrow);
			inputBindings["Cursor X"].SetKeyboard(KeyCode.Keypad6,KeyCode.Keypad4);
			inputBindings["Cursor Y"].SetKeyboard(KeyCode.Keypad8,KeyCode.Keypad2);
			inputBindings["Click"].SetKeyboard(KeyCode.Return);
			inputBindings["Jet Pack"].SetKeyboard(KeyCode.Space);
			
			if (!joypadIsConnected) {
				inputBindings ["Zoom"].SetJoyStick ("mouse scrollwheel");
			}
			
			inputBindings["Zoom"].SetKeyboard(KeyCode.KeypadPlus,KeyCode.KeypadMinus);
		}

		public void Update(){
			UpdateControlState();

			UpdateRSInputBinding ();
		}

		public void LateUpdate(){
			isUpdated = false;
		}

		public float GetAxisRaw(string axis){
			return inputBindings[axis].GetAxisRaw();
		}

		public bool GetKeyUp(string key) {
			return inputBindings [key].GetKeyUp ();
		}


		public Ray GetRay(Camera camera) {
			if(camera!=null) {
				UpdateControlState();

				return camera.ScreenPointToRay(new Vector3(defaultTouch.Position.x,Screen.height - defaultTouch.Position.y));
			}
			return new Ray();
		}

		public RSInputTouch findOrCreateTouch(int touchId) {
			RSInputTouch fingerTouch = null;
			bool found = false;
			
			foreach(RSInputTouch testTouch in aliveTouches) {
				if(testTouch.Id == touchId){
					fingerTouch = testTouch;
					found = true;
					break;
				}
			}

			if(!found){
				fingerTouch = new RSInputTouch();
				fingerTouch.Id = touchId;
				aliveTouches.Add(fingerTouch);
			}

			aliveTouchesIds.Add(fingerTouch.Id);

			return fingerTouch;
		}


		private void UpdateControlState() {
			if(!isUpdated){
				Vector2 defaultTouchPosition = new Vector2(0,0);
				bool defaultTouchActive = false;

				//Reset touches
				if(aliveTouches != null) {
					foreach(RSInputTouch testTouch in aliveTouches) {
						testTouch.ResetLastState();
					}
				}
				
				if(defaultTouch!=null) {
					defaultTouchPosition = defaultTouch.Position;
					defaultTouch.ResetLastState();
				}

				if(aliveTouchesIds != null) {
					aliveTouchesIds.Clear();
				}
					
				List<RSInputController> touchInputs = Manager.GetAllInstances<RSInputController>();


				foreach(RSInputController ti in touchInputs) {
					ti.UpdateTouches(this);
				}

				/*
				if(multitouchEnabled){
				

				}*/

				if(aliveTouchesIds.Count==0) {
					RSInputTouch fingerTouch  = findOrCreateTouch(mouseToucheId);

					if(Input.mousePosition != savedMousePos) {
						fingerTouch.UpdatePosition(Input.mousePosition.x,Screen.height - Input.mousePosition.y);
						savedMousePos = Input.mousePosition;
					}

					fingerTouch.IsActive = Input.GetMouseButton(0);
				}
				//Joypad
				
				if(useJoypadAsMouse && joypadIsConnected) {
					RSInputTouch fingerTouch  = findOrCreateTouch(mouseToucheId);

					if(GetAxisRaw("Click")==1) {
						fingerTouch.IsActive = true;
					}

					fingerTouch.UpdatePosition(fingerTouch.Position.x+GetAxisRaw("Cursor X")*joypadCursorSpeed*Screen.width*Time.deltaTime, 
					                           fingerTouch.Position.y-GetAxisRaw("Cursor Y")*joypadCursorSpeed*Screen.width*Time.deltaTime);

				}

				//Remove not active touches
				for(int i = aliveTouches.Count-1 ; i >= 0 ; i--) {
					if(!aliveTouchesIds.Contains(aliveTouches[i].Id)) {
						aliveTouches[i].IsActive = false;
						aliveTouches.RemoveAt(i);
					}
				}
				
				int numTouches = aliveTouches.Count;

				if (numTouches == 1) {
					defaultTouchActive = aliveTouches[0].IsActive;
					defaultTouchPosition = aliveTouches[0].Position;
				}
				
				defaultTouch.IsActive = defaultTouchActive;
				defaultTouch.UpdatePosition(defaultTouchPosition.x, defaultTouchPosition.y);

				if (numTouches == 2) {
					RSInputTouch touch1 = aliveTouches[0];
			        RSInputTouch touch2 = aliveTouches[1];
			        float curDist = Vector2.Distance(touch1.Position, touch2.Position);
					if(touch1.IsMoving && touch2.IsMoving && savedZoomDist > 0) {
						deltaZoom = curDist / savedZoomDist;
						isZooming = true;
					}
					
					//Debug.Log("distance " + deltaZoom);
					savedZoomDist = curDist;
				} 
				else {
					float scroll = GetAxisRaw("Zoom");

					if(scroll!=0) {
						isZooming = true;
						desktopZoomSpeed += scroll / 10.0f;
					}
					else if(desktopZoomSpeed == 0) {
						isZooming = false;
					}

					deltaZoom = desktopZoom / desktopLastZoom;
					desktopZoom = 1.0f;
				}

				desktopLastZoom = desktopZoom;
				
				if(desktopZoomSpeed < - 0.01f ||  desktopZoomSpeed > 0.01f) {
					desktopZoomSpeed -= desktopZoomSpeed  * Time.deltaTime * 3;
					desktopZoom += desktopZoomSpeed * Time.deltaTime * 3;
				}
				else {
					desktopZoomSpeed = 0;
				}

				//Click processing
				hasBeenClicked = false;
				hasBeenSimpleClicked = false;
				
				if(defaultTouch.HasGoneActive) {
				
					lastGoneActiveTime = Time.time;
				}
				
				if(defaultTouch.HasGoneInactive){
					if(((numClick == 0 && (Time.time - lastGoneActiveTime < clickTime)) || (Time.time - lastGoneInactiveTime < clickTime)) && defaultTouch.Distance < clickDistanceTolerance * dpiFactor){
						hasBeenClicked = true;
						numClick++;
						if(Clicked!=null) {
							Clicked(defaultTouch);
						}
					}

					lastGoneInactiveTime = Time.time;
				}
				else if(numClick>0 && (Time.time - lastGoneInactiveTime > clickTime)) {
					numClick = 0;
					hasBeenSimpleClickedDone = false;
				}
				
				if(!hasBeenSimpleClickedDone && numClick == 1 && (Time.time - lastGoneActiveTime > clickTime)) {
					hasBeenSimpleClicked =true;
					hasBeenSimpleClickedDone = true;
					if(SimpleClicked!=null) {
						SimpleClicked(defaultTouch);
					}
					
				}
				
				isUpdated = true;
			}
		}
			
		private void UpdateRSInputBinding() {
			bool joypadConnected = (Input.GetJoystickNames().Length > 0) ? true : false;

			if (joypadConnected == joypadIsConnected) return;

			joypadIsConnected = joypadConnected;
			RSInputBinding ();
		}
	}
}