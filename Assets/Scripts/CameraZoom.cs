using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private float zoom;
    private float minZoom = 3f;
    private float maxZoom = 6f;
    private float zoomSpeed = 0.05f;

    private Vector3 source;
    private Vector3 diff;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        zoom = cam.orthographicSize;
    }

    void LateUpdate()
    {
        Camerazoom();
    }

    private void Camerazoom()
    {
        float val = Input.GetAxis("Mouse ScrollWheel");
        zoom -= val;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, zoomSpeed);
    }


}
