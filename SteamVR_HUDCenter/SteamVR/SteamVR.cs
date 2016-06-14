//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Access to SteamVR system (hmd) and compositor (distort) interfaces.
//
//=============================================================================

using System;
using SteamVR_HUDCenter;
using Valve.VR;

public class SteamVR : System.IDisposable
{
	// Use this to check if SteamVR is currently active without attempting
	// to activate it in the process.
	public static bool active { get { return _instance != null; } }

	// Set this to false to keep from auto-initializing when calling SteamVR.instance.
	private static bool _enabled = true;
	public static bool enabled
	{
		get { return _enabled; }
		set
		{
			_enabled = value;
			if (!_enabled)
				SafeDispose();
		}
	}

	private static SteamVR _instance;
	public static SteamVR instance
	{
		get
		{
			if (!enabled)
				return null;

			if (_instance == null)
			{
				_instance = CreateInstance(EVRApplicationType.VRApplication_Overlay);

				// If init failed, then auto-disable so scripts don't continue trying to re-initialize things.
				if (_instance == null)
					_enabled = false;
			}

			return _instance;
		}
	}

	public static bool usingNativeSupport
	{
		get
		{
			return true;
		}
	}

	static SteamVR CreateInstance(EVRApplicationType apptype)
	{
		try
		{
			var error = EVRInitError.None;
			if (!SteamVR.usingNativeSupport)
			{
				OpenVR.Init(ref error, apptype);
				if (error != EVRInitError.None)
				{
					ReportError(error);
					ShutdownSystems();
					return null;
				}
			}

			// Verify common interfaces are valid.

			OpenVR.GetGenericInterface(OpenVR.IVRCompositor_Version, ref error);
			if (error != EVRInitError.None)
			{
				ReportError(error);
				ShutdownSystems();
				return null;
			}

			OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);
			if (error != EVRInitError.None)
			{
				ReportError(error);
				ShutdownSystems();
				return null;
			}
		}
		catch (System.Exception e)
		{
            Console.WriteLine("[Stream_VR] " + e);
			UDebug.LogError(e);
			return null;
		}

		return new SteamVR();
	}

	static void ReportError(EVRInitError error)
	{
		switch (error)
		{
			case EVRInitError.None:
				break;
			case EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime:
				UDebug.Log("SteamVR Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
				break;
			case EVRInitError.Init_VRClientDLLNotFound:
				UDebug.Log("SteamVR drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
				break;
			case EVRInitError.Driver_RuntimeOutOfDate:
				UDebug.Log("SteamVR Initialization Failed!  Make sure device's runtime is up to date.");
				break;
			default:
				UDebug.Log(OpenVR.GetStringForHmdError(error));
				break;
		}
	}

	// native interfaces
	public CVRSystem hmd { get; private set; }
	public CVRCompositor compositor { get; private set; }
	public CVROverlay overlay { get; private set; }

	// tracking status
	static public bool initializing { get; private set; }
	static public bool calibrating { get; private set; }
	static public bool outOfRange { get; private set; }

	static public bool[] connected = new bool[OpenVR.k_unMaxTrackedDeviceCount];

	// render values
	public float sceneWidth { get; private set; }
	public float sceneHeight { get; private set; }
	public float aspect { get; private set; }
	public float fieldOfView { get; private set; }
	//public Vector2 tanHalfFov { get; private set; }
	public VRTextureBounds_t[] textureBounds { get; private set; }
	//public SteamVR_Utils.RigidTransform[] eyes { get; private set; }
	public EGraphicsAPIConvention graphicsAPI;

	// hmd properties
	public string hmd_TrackingSystemName { get { return GetStringProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String); } }
	public string hmd_ModelNumber { get { return GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String); } }
	public string hmd_SerialNumber { get { return GetStringProperty(ETrackedDeviceProperty.Prop_SerialNumber_String); } }

	public float hmd_SecondsFromVsyncToPhotons { get { return GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float); } }
	public float hmd_DisplayFrequency { get { return GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float); } }

	public string GetTrackedDeviceString(uint deviceId)
	{
		var error = ETrackedPropertyError.TrackedProp_Success;
		var capacity = hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, null, 0, ref error);
		if (capacity > 1)
		{
			var result = new System.Text.StringBuilder((int)capacity);
			hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, result, capacity, ref error);
			return result.ToString();
		}
		return null;
	}

	string GetStringProperty(ETrackedDeviceProperty prop)
	{
		var error = ETrackedPropertyError.TrackedProp_Success;
		var capactiy = hmd.GetStringTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, prop, null, 0, ref error);
		if (capactiy > 1)
		{
			var result = new System.Text.StringBuilder((int)capactiy);
			hmd.GetStringTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, prop, result, capactiy, ref error);
			return result.ToString();
		}
		return (error != ETrackedPropertyError.TrackedProp_Success) ? error.ToString() : "<unknown>";
	}

	float GetFloatProperty(ETrackedDeviceProperty prop)
	{
		var error = ETrackedPropertyError.TrackedProp_Success;
		return hmd.GetFloatTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, prop, ref error);
	}

	#region Event callbacks

	private void OnInitializing(params object[] args)
	{
		initializing = (bool)args[0];
	}

	private void OnCalibrating(params object[] args)
	{
		calibrating = (bool)args[0];
	}

	private void OnOutOfRange(params object[] args)
	{
		outOfRange = (bool)args[0];
	}

	private void OnDeviceConnected(params object[] args)
	{
		var i = (int)args[0];
		connected[i] = (bool)args[1];
	}

	#endregion

	private SteamVR()
	{
		hmd = OpenVR.System;
		UDebug.Log("Connected to " + hmd_TrackingSystemName + ":" + hmd_SerialNumber);

		compositor = OpenVR.Compositor;
		overlay = OpenVR.Overlay;

		// Setup render values
		uint w = 0, h = 0;
		hmd.GetRecommendedRenderTargetSize(ref w, ref h);
		sceneWidth = (float)w;
		sceneHeight = (float)h;

		float l_left = 0.0f, l_right = 0.0f, l_top = 0.0f, l_bottom = 0.0f;
		hmd.GetProjectionRaw(EVREye.Eye_Left, ref l_left, ref l_right, ref l_top, ref l_bottom);

		float r_left = 0.0f, r_right = 0.0f, r_top = 0.0f, r_bottom = 0.0f;
		hmd.GetProjectionRaw(EVREye.Eye_Right, ref r_left, ref r_right, ref r_top, ref r_bottom);
	}

	~SteamVR()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		System.GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		ShutdownSystems();
		_instance = null;
	}

	private static void ShutdownSystems()
	{
		OpenVR.Shutdown();
	}

	// Use this interface to avoid accidentally creating the instance in the process of attempting to dispose of it.
	public static void SafeDispose()
	{
		if (_instance != null)
			_instance.Dispose();
	}
}

