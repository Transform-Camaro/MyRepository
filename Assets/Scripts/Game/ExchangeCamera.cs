using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ToggleCamera()
    {
        // turn off one camera
        Vuforia.CameraDevice.Instance.Stop();
        Vuforia.CameraDevice.Instance.Deinit();

        // turn on another camera
        Vuforia.CameraDevice.Instance.Init(getNextCamera());
        Vuforia.CameraDevice.Instance.Start();
    }

    private Vuforia.CameraDevice.CameraDirection getNextCamera()
    {
        // decide which camera to turn on
        switch (Vuforia.CameraDevice.Instance.GetCameraDirection())
        {
            case Vuforia.CameraDevice.CameraDirection.CAMERA_BACK:
            case Vuforia.CameraDevice.CameraDirection.CAMERA_DEFAULT:
            default:
                return Vuforia.CameraDevice.CameraDirection.CAMERA_FRONT;
            case Vuforia.CameraDevice.CameraDirection.CAMERA_FRONT:
                return Vuforia.CameraDevice.CameraDirection.CAMERA_BACK;
        }
    }


}
