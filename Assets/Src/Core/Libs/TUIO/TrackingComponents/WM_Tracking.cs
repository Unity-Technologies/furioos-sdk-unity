/*
Unity3d-TUIO connects touch tracking from a TUIO to objects in Unity3d.

Copyright 2011 - Mindstorm Limited (reg. 05071596)

Author - Simon Lerpiniere

This file is part of Unity3d-TUIO.

Unity3d-TUIO is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Unity3d-TUIO is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser Public License for more details.

You should have received a copy of the GNU Lesser Public License
along with Unity3d-TUIO.  If not, see <http://www.gnu.org/licenses/>.

If you have any questions regarding this library, or would like to purchase 
a commercial licence, please contact Mindstorm via www.mindstorm.com.
*/

using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Mindstorm.WM;

public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
public delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, IntPtr lParam);

public class WM_Tracking 
{
	public Dictionary<int, TOUCHINPUT> current = new Dictionary<int, TOUCHINPUT>();	

    IntPtr hMainWindow;
    IntPtr oldWndProcPtr;
    IntPtr newWndProcPtr;

    WndProcDelegate newWndProc;
	
	object m_lock = new object();

    bool isrunning = false;
	
	void FindWindow(string name)
    {
	    EnumDesktopWindowsDelegate filter = delegate(IntPtr hWnd, IntPtr lParam)
	    {
	        StringBuilder strbTitle = new StringBuilder(255);
			GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
	        string strTitle = strbTitle.ToString();
	
			if (!string.IsNullOrEmpty(strTitle))
			{
				Debug.Log(strTitle);
				
	        	if (IsWindowVisible(hWnd))
		        {
		        	if (String.Equals(strTitle, name, StringComparison.CurrentCultureIgnoreCase))
					{
						hMainWindow = hWnd;
						Debug.Log("Found");
						return false;
						
					}
		        }
			}
	        return true;
	    };
	
	    EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
    }

    public void Start()
    {
        if (isrunning) return;

        touchInputSize = Marshal.SizeOf(typeof(TOUCHINPUT));

        hMainWindow = GetForegroundWindow();
		//FindWindow("Unity - Main.unity - effects-unity-RPSEU00X_Starter - PC, Mac & Linux Standalone");
		//FindWindow("RPSEU00X_Starter");
		
        RegisterTouchWindow(hMainWindow, 0);

        newWndProc = new WndProcDelegate(wndProc);
        newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newWndProc);
        oldWndProcPtr = SetWindowLong(hMainWindow, -4, newWndProcPtr);

        isrunning = true;
    }
	
	public void Close()
    {
        if (!isrunning) return;
		
        SetWindowLong(hMainWindow, -4, oldWndProcPtr);
        UnregisterTouchWindow(hMainWindow);

        hMainWindow = IntPtr.Zero;
        oldWndProcPtr = IntPtr.Zero;
        newWndProcPtr = IntPtr.Zero;

        newWndProc = null;
		
		isrunning = false;
    }

    IntPtr wndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_TOUCH) processTouches(wParam, lParam);
        return CallWindowProc(oldWndProcPtr, hWnd, msg, wParam, lParam);
    }

    void processTouches(IntPtr wParam, IntPtr lParam)
    {
		int inputCount = LOWORD(wParam.ToInt32());
        TOUCHINPUT[] inputs = new TOUCHINPUT[inputCount];

        if (!GetTouchInputInfo(lParam, inputCount, inputs, touchInputSize))
        {
            return;		
        }

        for (int i = 0; i < inputCount; i++)
        {
            TOUCHINPUT touch = inputs[i];
			updateTouchFromWM(touch);
        }

        CloseTouchInputHandle(lParam);
    }
	
	void updateTouchFromWM(TOUCHINPUT WM_T)
	{
		lock(m_lock)
		{
			if ((WM_T.dwFlags & (int)TouchEvent.TOUCHEVENTF_MOVE) != 0)
			{
				current[WM_T.dwID] = WM_T;
			}
			if ((WM_T.dwFlags & (int)TouchEvent.TOUCHEVENTF_DOWN) != 0)
			{
				current.Add(WM_T.dwID, WM_T);
			}
			if ((WM_T.dwFlags & (int)TouchEvent.TOUCHEVENTF_UP) != 0)
			{
				current.Remove(WM_T.dwID);
			}
		}
	}
	
	public POINT getClientPoint(TOUCHINPUT WM_T)
	{
		POINT p = new POINT();
        p.X = WM_T.x / 100;
        p.Y = WM_T.y / 100;
        ScreenToClient(hMainWindow, ref p);
		return p;
	}	
	
	public TOUCHINPUT[] GetTouchArray()
    {
        lock (m_lock)
        {
            TOUCHINPUT[] ts = this.current.Values.ToArray();
			return ts;
        }
    }
	
    #region p/invoke

    // Touch event window message constants [winuser.h]
    const int WM_TOUCH = 0x0240;

    // Touch API defined structures [winuser.h]
    [StructLayout(LayoutKind.Sequential)]
    public struct TOUCHINPUT
    {
        public int x;
        public int y;
        public IntPtr hSource;
        public int dwID;
        public int dwFlags;
        public int dwMask;
        public int dwTime;
        public IntPtr dwExtraInfo;
        public int cxContact;
        public int cyContact;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }
	
	[StructLayout(LayoutKind.Sequential)]
    public struct WINDOWDATA
    {
        public string pName;
		public IntPtr hWnd;
    }

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
	
    [DllImport("user32.dll")]
    static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool RegisterTouchWindow(IntPtr hWnd, uint ulFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool UnregisterTouchWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [Out] TOUCHINPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern void CloseTouchInputHandle(IntPtr lParam);

    [DllImport("user32.dll")]
    static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);
	
	[DllImport("user32.dll")]
	static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsDelegate lpfn, IntPtr lParam);
	
	[DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);
	
	[DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWindowVisible(IntPtr hWnd);

    int touchInputSize;

    int HIWORD(int value)
    {
        return (int)(value >> 16);
    }

    int LOWORD(int value)
    {
        return (int)(value & 0xffff);
    }

    #endregion
}
