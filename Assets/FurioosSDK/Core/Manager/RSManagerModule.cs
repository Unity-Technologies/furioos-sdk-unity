using UnityEngine;
using System.Collections;

namespace Rise.Core {
	public abstract class RSManagerModule {
		public RSManager Manager {
			get { 
				return RSManager.Manager; 
			}
		}
		
		public RSInputManager InputManager {
			get { 
				return Manager!= null ? Manager.InputManager : null; 
			}
		}

		public RSOutputManager OutputsManager {
			get { 
				return Manager!= null ? Manager.OutputsManager : null; 
			}
		}

		public RSCamerasManager CamerasManager {
			get { 
				return Manager!= null ? Manager.CamerasManager : null; 
			}
		}
	}
}