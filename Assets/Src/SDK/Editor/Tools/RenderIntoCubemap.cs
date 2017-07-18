// Render scene from a given point into a static cube map.
// Place this script in Editor folder of your project.
// Then use the cubemap with one of Reflective shaders!
using UnityEditor;
using UnityEngine;


class RenderCubemapWizard : ScriptableWizard {
	public Transform renderFromPosition = null;
	public Cubemap cubemap = null;
    
    void OnWizardUpdate () {
        helpString = "Select transform to render from and cubemap to render into";
        isValid = (renderFromPosition != null) && (cubemap != null);
    }
    
	void OnWizardCreate () {
        // create temporary camera for rendering
		GameObject go = new GameObject( "CubemapCamera");
		go.AddComponent<Camera>();
        // place it on the object
        go.transform.position = renderFromPosition.position;
        go.transform.rotation = Quaternion.identity;

        // render into cubemap        
        go.GetComponent<Camera>().RenderToCubemap( cubemap );
        
        // destroy temporary camera
        DestroyImmediate( go );
    }
    
	[MenuItem("Rise SDK/Camera/Render into cubemap")]
    static void RenderCubemap () {
        ScriptableWizard.DisplayWizard<RenderCubemapWizard>("Render cubemap", "Render!");
    }
}