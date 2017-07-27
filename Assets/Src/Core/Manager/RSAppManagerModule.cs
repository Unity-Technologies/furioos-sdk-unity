using UnityEngine;
using System.Collections;

namespace Rise.Core {
	public abstract class RSAppManagerModule{
		public RSManager AppManager {
			get { return RSManager.AppManager; }
		}
		
		public RSInputManager InputController {
			get { return AppManager!= null ? AppManager.InputManager : null; }
		}

		public RSOutputModesManager OutputModesManager {
			get { return AppManager!= null ? AppManager.OutputModesManager : null; }
		}

		public RSCamerasManager MovingModesManager {
			get { return AppManager!= null ? AppManager.CamerasManager : null; }
		}
	}
}