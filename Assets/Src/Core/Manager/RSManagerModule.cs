using UnityEngine;
using System.Collections;

namespace Rise.Core {
	public abstract class RSManagerModule {
		public RSManager Manager {
			get { 
				return RSManager.Manager; 
			}
		}
		
		public RSInputManager InputController {
			get { 
				return Manager!= null ? Manager.InputManager : null; 
			}
		}

		public RSOutputManager OutputModesManager {
			get { 
				return Manager!= null ? Manager.OutputsManager : null; 
			}
		}

		public RSCamerasManager MovingModesManager {
			get { 
				return Manager!= null ? Manager.CamerasManager : null; 
			}
		}
	}
}