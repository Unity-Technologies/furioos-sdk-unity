#pragma warning disable 0414
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rise.Core;

namespace Rise.Features.PathFinding {
	public class RandomNav : RSBehaviour {

		public GameObject navMesh;

		private float timeBeforeCheck = 5;
		private float lastTimeCheck;
		
		private Animation animationComponent;
		private UnityEngine.AI.NavMeshAgent agent;

		private List<Vector3> listPoints;

		public void Start() {
			animationComponent = gameObject.GetComponent<Animation> ();
			agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

			listPoints = new List<Vector3> ();

			foreach (Vector3 point in navMesh.GetComponent<MeshFilter>().mesh.vertices) 
				listPoints.Add (point);

			lastTimeCheck = Time.time;

			SetNewDestination ();
		}

		public void Update() {
			if (Time.time - lastTimeCheck > timeBeforeCheck) {
				if(agent.velocity.x == 0 && agent.velocity.z == 0) {
					SetNewDestination();
				}

				lastTimeCheck = Time.time;
			}

		}

		void SetNewDestination() {
			int randomIndex = Random.Range (1, listPoints.Count);

			Vector3 destinationPoint = navMesh.transform.TransformPoint (listPoints [randomIndex]);

			agent.SetDestination (destinationPoint);
		}
	}
}