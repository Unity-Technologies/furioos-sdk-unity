using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;

public class RSInputAxisBuilder {
	private const string INPUT_TYPE_KEY_OR_MOUSE = "0";
	private const string INPUT_TYPE_MOUSE_MOVEMENT = "1";
	private const string INPUT_TYPE_JOYSTICK_AXIS = "2";

	[MenuItem("Rise SDK/Input/Build Axis")]
	private static void BuildInputManager() {
		StreamWriter sr = File.CreateText("ProjectSettings" + Path.DirectorySeparatorChar + "InputManager.asset");

		sr.WriteLine("%YAML 1.1");
		sr.WriteLine("%TAG !u! tag:unity3d.com,2011:");
		sr.WriteLine("--- !u!13 &1");
		sr.WriteLine("InputManager:");
		sr.WriteLine("  m_ObjectHideFlags: 0");
		sr.WriteLine("  m_Axes:");

		WriteInput(sr, "mouse scrollwheel", "2", INPUT_TYPE_MOUSE_MOVEMENT);

		for(int i = 0; i < 20; i++) {
			WriteInput(sr, "joystick button " + i.ToString(), "joystick button " + i.ToString(), INPUT_TYPE_KEY_OR_MOUSE);
		}

		for(int i = 0; i < 20; i++) {
			WriteInput(sr, "joystick axis " + i.ToString(), i.ToString(), INPUT_TYPE_JOYSTICK_AXIS);	
		}

		WriteInput(sr, "Horizontal", "right", INPUT_TYPE_KEY_OR_MOUSE, "left");
		WriteInput(sr, "Vertical", "down", INPUT_TYPE_KEY_OR_MOUSE, "up");

		WriteInput(sr, "Mouse X", "", INPUT_TYPE_MOUSE_MOVEMENT);
		WriteInput(sr, "Mouse Y", "", INPUT_TYPE_MOUSE_MOVEMENT);

		sr.Close();
	}

	private static void WriteInput(StreamWriter sr, string name, string action, string type, string negativeAction = "") {
		sr.WriteLine("  - serializedVersion: 3");
		sr.WriteLine("    m_Name: "+name);
		sr.WriteLine("    descriptiveName: ");
		sr.WriteLine("    descriptiveNegativeName: ");

		if(type == INPUT_TYPE_KEY_OR_MOUSE) {
			sr.WriteLine("    negativeButton: "+negativeAction);
		}
		else sr.WriteLine("    negativeButton: ");

		if(type == INPUT_TYPE_KEY_OR_MOUSE) {
			sr.WriteLine("    positiveButton: "+action);
		}
		else {
			sr.WriteLine("    positiveButton: ");
		}

		sr.WriteLine("    altNegativeButton: ");
		sr.WriteLine("    altPositiveButton: ");
		sr.WriteLine("    gravity: 0");

		if(type == INPUT_TYPE_MOUSE_MOVEMENT) {
			sr.WriteLine("    dead: 0");
		}
		else {
			sr.WriteLine("    dead: .35");
		}

		if (type == INPUT_TYPE_KEY_OR_MOUSE) {
			sr.WriteLine ("    sensitivity: 1000");
		}
		else {
			sr.WriteLine ("    sensitivity: 1");
		}

		sr.WriteLine("    snap: 0");
		sr.WriteLine("    invert: 0");
		sr.WriteLine("    type: "+type);

		if(type==INPUT_TYPE_MOUSE_MOVEMENT || type==INPUT_TYPE_JOYSTICK_AXIS) {
			sr.WriteLine("    axis: "+action);
		}
		else {
			sr.WriteLine("    axis: 0");
		}

		sr.WriteLine("    joyNum: 0");
	}
}