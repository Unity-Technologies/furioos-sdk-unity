using UnityEngine;
using System.Collections;

namespace Rise.Core {
	public abstract class RSAppManagerModule{
		public RSAppManager SceneManager {
			get { return RSAppManager.SceneManagerInstance; }
		}
		
		public RSInputManager InputController {
			get { return SceneManager!= null ? SceneManager.InputManager : null; }
		}

		public RSOutputModesManager OutputModesManager {
			get { return SceneManager!= null ? SceneManager.OutputModesManager : null; }
		}

		public RSMovingModesManager MovingModesManager {
			get { return SceneManager!= null ? SceneManager.MovingModesManager : null; }
		}
	}
}