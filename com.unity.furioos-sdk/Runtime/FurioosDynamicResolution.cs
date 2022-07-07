using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurioosDynamicResolution : MonoBehaviour
{
    private int width;
    private int height;

    void Start()
    {
        width = Screen.width;
        height = Screen.height;
    }

    void Update()
    {
        if (width != Screen.width || height != Screen.height)
        {
            width = Screen.width;
            height = Screen.height;
            Screen.SetResolution(width, height, true);
        }
    }
}