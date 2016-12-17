using UnityEngine;
using System.Collections;
using System;
using Rise.Core;

namespace Rise.Features.MovingMode {
	[RequireComponent (typeof(Camera))]
	public class MovingModeTransitionCamera : RSMovingMode {
		public float transitionTime = 3;
		public float transitionWait = 2.5f;
		public float transitionAnimationY = 50;
		public LeanTweenType animationType;

		public bool forceDirectAnimation = false;

		public delegate void TransitionHasBeenStarted();
		public static event TransitionHasBeenStarted onTransitionStarted;

		public delegate void TransitionHasBeenCompleted();
		public event TransitionHasBeenCompleted onTransitionCompleted;

		private Vector3 destinationPosition;
		private Vector3 destinationRotation;

		private float destinationFov;
		private float destinationOrthographicSize;

		public void GoTo(RSMovingMode movingMode, bool forceDeviatedAnimation = false) {
			GameObject movingModeCamera = movingMode.GetCameraGameObject ();

			destinationPosition = movingModeCamera.transform.position;
			destinationRotation = movingModeCamera.transform.eulerAngles;

			Vector3 fwd = movingModeCamera.transform.position - transform.position;
			RaycastHit hit;
			Physics.Raycast (transform.position, fwd, out hit);

			destinationFov = movingModeCamera.GetComponent<Camera>().fieldOfView;
			destinationOrthographicSize = movingModeCamera.GetComponent<Camera>().orthographicSize;

			CameraPropertiesAnimation (movingModeCamera.GetComponent<Camera>().orthographic);

			if (forceDeviatedAnimation) {
				DeviatedAnimation ();
				return;
			}

			float distanceToDestination = Vector3.Distance(transform.position, destinationPosition);
			float distanceToHit = Vector3.Distance(transform.position, hit.point);

			if (distanceToDestination > distanceToHit && !forceDirectAnimation) {
				DeviatedAnimation ();
			} else {
				DirectAnimation();
			}

			if (onTransitionStarted != null)
				onTransitionStarted ();
		}

		private void DeviatedAnimation() {
			LeanTween.move (
				gameObject,
			    new Vector3 (transform.position.x, transitionAnimationY, transform.position.z),
			    transitionTime
			).setEase(animationType);

			LeanTween.rotate (
				gameObject,
				new Vector3 (90, 0, 0),
				transitionTime
			).setEase(animationType);

			LeanTween.move (
				gameObject,
				new Vector3 (destinationPosition.x, transitionAnimationY, destinationPosition.z),
				transitionTime
			).setDelay (transitionWait).setEase(animationType);

			LeanTween.move (
				gameObject,
				new Vector3 (destinationPosition.x, destinationPosition.y, destinationPosition.z),
				transitionTime
			).setDelay (transitionWait * 2).setEase(animationType);

			LeanTween.rotate (
				gameObject,
				new Vector3 (destinationRotation.x, destinationRotation.y, destinationRotation.z),
				transitionTime
			).setDelay (transitionWait * 2).setOnComplete (TransitionCompleted).setEase(animationType);
		}

		private void DirectAnimation() {
			LeanTween.move (
				gameObject,
				destinationPosition,
				transitionTime
			).setEase(animationType);

			LeanTween.rotate (
				gameObject,
				destinationRotation,
				transitionTime
			).setOnComplete (TransitionCompleted).setEase(animationType);
		}

		private void CameraPropertiesAnimation(bool isOrthographic) {
			if (isOrthographic) {
				GetComponent<Camera>().orthographic = true;

				LeanTween.value(
					gameObject,
					1,
					destinationOrthographicSize,
					transitionTime
				).setOnUpdate(
					(Action<float>)(newVal => {
						GetComponent<Camera>().orthographicSize = (float)newVal;
					})
				).setEase(animationType);
			} else {
				GetComponent<Camera>().orthographic = false;

				LeanTween.value(
					gameObject,
					1,
					destinationFov,
					transitionTime
				).setOnUpdate(
					(Action<float>)(newVal => {
						GetComponent<Camera>().fieldOfView = (float)destinationFov;
					})
				).setEase(animationType);
			}
		}

		private void TransitionCompleted() {
			if (onTransitionCompleted == null)
				return;
			onTransitionCompleted ();
		}

		public void ResetTransform(Transform activeCameraTransform = null) {
			if (activeCameraTransform == null) {
				transform.position = Vector3.zero;
				transform.eulerAngles = Vector3.zero;

				return;
			}

			transform.position = activeCameraTransform.position;
			transform.eulerAngles = activeCameraTransform.eulerAngles;
		}

		public override void Activate ()
		{
			base.Activate ();
			transform.gameObject.SetActive(true);
		}
		
		public override void Desactivate ()
		{
			base.Desactivate ();
			transform.gameObject.SetActive(false);
		}
	}
}