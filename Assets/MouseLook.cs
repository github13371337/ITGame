using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

[System.Flags]
public enum Axis
{
    X = 0b10,
    Y = 0b01,
}

public class MouseLook : MonoBehaviour
{    
    public Axis axis;

    [SerializeField] float sens = 6;
    [SerializeField] float vertMin = -90;
    [SerializeField] float vertMax = 90;
    [SerializeField] bool controlDistance = true;
    [SerializeField][HideUnless("controlDistance", true)] float maxDistance = -30;
    [SerializeField][HideUnless("controlDistance", true)] float defaultCameraDistance = -5;
    [SerializeField][HideUnless("controlDistance", true)] float minDistance = 0;
    [SerializeField][HideUnless("controlDistance", true)] float distanceChangeSpeed = 1;

    private bool mouseLocked;

    private GameObject camera_;
    public float angleY;
    public float angleX;


	private void Awake()
    { 
        camera_ = GetComponentInChildren<Camera>().gameObject;
        SetCursorLock(true);
	}
	
	private void Update() 
    {
        if (mouseLocked == true)
        {                     
            if (axis.HasFlag(Axis.Y))
            {
                float mouseY = (Input.GetAxis("Mouse Y") * sens);
                angleY -= mouseY;
                angleY = Mathf.Clamp(angleY, vertMin, vertMax);
            }
            else angleY = transform.eulerAngles.x;

            if (axis.HasFlag(Axis.X))
            {
                float mouseX = (Input.GetAxis("Mouse X") * sens);
                angleX = transform.eulerAngles.y + mouseX;
            }
            else angleX = transform.eulerAngles.y;

            transform.eulerAngles = new Vector3(angleY, angleX, transform.eulerAngles.z);

            if (controlDistance)
            {
                float distance = camera_.transform.localPosition.z;
                if (Input.GetAxis("Mouse ScrollWheel") > 0) distance += distanceChangeSpeed;
                else if (Input.GetAxis("Mouse ScrollWheel") < 0) distance -= distanceChangeSpeed;
                if (distance > minDistance) distance = minDistance;
                if (distance < maxDistance) distance = maxDistance;
                camera_.transform.localPosition = new Vector3(0, 0, distance);
            }
        }
    }

    public void SetCursorLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
        mouseLocked = locked;
    }

    public void ResetCameraDistance() => camera_.transform.localPosition = new Vector3(0, 0, defaultCameraDistance);
}
