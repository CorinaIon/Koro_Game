using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_script : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float height = 2f; // eraa pitch in codul celuilalt
    public float zoomSpeed = 4f;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float speed_rotatie = 100f;
    public float speed_rotatie_mouse = 10000f;

    private float zoom = 10f;
    private float rotatie = 0f;
    public bool roteste_sens_mouse = true;
    
    void Update()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        rotatie += Input.GetAxis("Horizontal") * speed_rotatie * Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            if(roteste_sens_mouse)
                rotatie -= Input.GetAxis("Mouse X") * speed_rotatie_mouse * Time.deltaTime;
            else
                rotatie += Input.GetAxis("Mouse X") * speed_rotatie_mouse * Time.deltaTime;
        }

    }

    void LateUpdate()
    {
        transform.position = target.position - offset * zoom;   //actualizeaza pozitia
        transform.LookAt(target.position + Vector3.up * height);  //seteaza directia camerei inspre player
        transform.RotateAround(target.position, Vector3.up, rotatie);
    }
}
