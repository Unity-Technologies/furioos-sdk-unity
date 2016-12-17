using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rise.Core;

namespace Rise.Features.SwitchMaterial {
	public class OBSSwitchMaterial : RSBehaviour {
		
		public List<Material> materials;

		private int materialId;
		public bool changeOnClick = true;
		
		
		void Start(){
			materialId = 0;
		}
		
		public void NextMaterial(){
			transform.GetComponent<Renderer>().material = materials[++materialId % materials.Count];
		}
		
		// Update is called once per frame
		void Update () {

			if(changeOnClick && InputManager!=null &&  MovingModesManager!=null && InputManager.HasBeenSimpleClicked){
				Ray ray = InputManager.GetRay(MovingModesManager.ActiveCamera);
				RaycastHit hit;
				if(Physics.Raycast(ray,out hit)){
					if(hit.collider.gameObject == gameObject){
						NextMaterial();
					}
				}	
			}
			
		}
	}
}