using UnityEngine;
using System.Collections;

namespace FurioosSDK.Core {
	public abstract class FSManagerModule {
		public FSManager Manager {
			get { 
				return FSManager.Manager; 
			}
		}
		
		public FSInputManager InputManager {
			get { 
				return Manager!= null ? Manager.InputManager : null; 
			}
		}

		public FSOutputManager OutputsManager {
			get { 
				return Manager!= null ? Manager.OutputsManager : null; 
			}
		}

		public FSCamerasManager CamerasManager {
			get { 
				return Manager!= null ? Manager.CamerasManager : null; 
			}
		}
	}
}