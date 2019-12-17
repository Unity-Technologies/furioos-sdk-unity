using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FurioosSDK.Editor {
	public class FurioosInspector : EditorWindow {
		[MenuItem("FurioosSDK/Inspector")]
		static void Init() {
			FurioosInspector inspector = (FurioosInspector)EditorWindow.GetWindow(typeof(FurioosInspector), false, "Furioos");
			inspector.Show();
		}
	}
}