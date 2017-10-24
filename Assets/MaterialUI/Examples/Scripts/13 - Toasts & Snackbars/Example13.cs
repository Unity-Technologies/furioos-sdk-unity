//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using MaterialUI;

public class Example13 : MonoBehaviour
{
	public void OnSimpleToastButtonClicked()
	{
		ToastManager.Show("Simple toast!");
	}

	public void OnCustomToastButtonClicked()
	{
		ToastManager.Show("Custom toast", 2.0f, GetRandomColor(), GetRandomColor(), Random.Range(12, 36));
	}

	public void OnSimpleSnackbarButtonClicked()
	{
		SnackbarManager.Show("Simple snackbar", "Action", () => { Debug.Log("Action clicked"); });
	}

	public void OnCustomSnackbarButtonClicked()
	{
		SnackbarManager.Show("Simple snackbar", 2.0f, GetRandomColor(), GetRandomColor(), Random.Range(12, 36), "Custom", () => { Debug.Log("Action clicked"); });
	}

	private Color GetRandomColor()
	{
		return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
}
