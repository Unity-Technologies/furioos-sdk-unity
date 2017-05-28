using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraTarget
	: MonoBehaviour
{

	public Transform target;
	
	void LateUpdate()
	{
		if (target)
			transform.LookAt(target);
	}
	
	void Start()
	{
		//appManager.register(this);
		
	   	if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
}
