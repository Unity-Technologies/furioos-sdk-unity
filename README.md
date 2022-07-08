# Furioos Unity Packages

The Furioos Unity package is composed of different tools that allow you to:

- Communicate between your application and your website
- Export your Unity application to Furioos directly from the Unity Editor
- Use [Unity Render Streaming](https://github.com/Unity-Technologies/UnityRenderStreaming) with the Furioos platform.

It includes the following packages:
|**Location**|**Description**|
|---|---|
|_com.unity.furioos-connection-kit_|The basic communication layer (not intended to be used alone)|
|_com.unity.furioos-sdk_|The official Furioos SDK for Unity with debug tools and template|
|_com.unity.furioos-exporter_|Build and deploy your application from Unity Editor to your Furioos account|
|_com.unity.furioos-renderstreaming-bridge_|A package that allow a Unity Render Streaming application to be hosted on Furioos platform|

## Requirements

- Unity Editor 2020.3+
- A Furioos Account on [Furioos.com](https://portal.furioos.com)
- The Furioos SDK JS in your Web site [GitHub Repository](https://github.com/Unity-Technologies/furioos-sdk-js)

## Installation

> ***Important**: Furioos Unity Package uses the UPM (Unity Package Manager) but it's still an experimental package. Which means that you don't find it directly from the Unity Registry. Follow the steps below to install it*

1. Download and unzip the last release of the Furioos Unity Packages[here](https://github.com/Unity-Technologies/furioos-sdk-unity/releases)
2. Open your Unity project
3. Import the three packages from the package manager. "Window > Package Manager > Add package from disk.."

> ***Note**: You need to follow the installation order*
>
> - _com.unity.furioos-exporter/package.json_
> - _com.unity.furioos-connection-kit/package.json_
> - _com.unity.furioos-sdk/package.json_

4. **Restart the Unity Editor**

> ***Note**: If your project uses the Furioos SDK Unity v1.x remove it before using the new packages*

## Documentations

- [Furioos SDK](com.unity.furioos-sdk/README.md) About the Furioos SDK with Unity
- [Furioos Exporter](com.unity.furioos-exporter/README.md) Learn how to export your application from Unity Editor to Furioos
