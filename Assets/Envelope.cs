using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Envelope : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private Vector3 velocity;
    private Camera mainCamera;

    public TextMeshProUGUI tmpro;
    public string text;

    [SerializeField] private float planeY = 0.0f; // Set the Y-coordinate of the plane
    [SerializeField] private float dragSmoothing = 10.0f; // Smoothing for drag motion

    void Start()
    {
        tmpro.text=text;
        // Cache the main camera for screen-to-world calculations
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleDrag();
    }

    void HandleDrag()
    {

        //Vector3 mouseDelta = GetMouseWorldPositionOnPlane()-lastMousePosition;
        if (Input.GetMouseButtonDown(0)) // Detect mouse click
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit,Mathf.Infinity,LayerMask.GetMask("Paper")))
            {
                if (hit.collider.gameObject == this.gameObject) // Check if this paper is clicked
                {
                    isDragging = true;
                    lastMousePosition = GetMouseWorldPositionOnPlane();
                    velocity = Vector3.zero;
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // Release mouse button
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mousePosition = GetMouseWorldPositionOnPlane();
            velocity = (mousePosition - lastMousePosition)/ Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            lastMousePosition = mousePosition;
        }
        else
        {
            // Apply deceleration when not dragging
            transform.position += velocity * Time.deltaTime;
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * dragSmoothing);
        }
    }

    Vector3 GetMouseWorldPositionOnPlane()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, planeY, 0)); // Plane at specified Y-coordinate

        if (dragPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position; // Fallback in case raycast fails
    }
}
