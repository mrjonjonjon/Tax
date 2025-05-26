using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; // At top of script


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Envelope : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private Vector3 velocity;
    public Camera mainCamera;

    public TextMeshProUGUI tmpro;
    public string text;

    [SerializeField] public float planeY = 0.0f; // Set the Y-coordinate of the plane
    [SerializeField] public float dragSmoothing = 10.0f; // Smoothing for drag motion
    [SerializeField] public float moveSpeed = 2.0f; // Speed for smooth movement
    [SerializeField] private Transform zoomTarget; // Target transform for zoom position and rotation

    private Coroutine moveCoroutine;
    private bool isZoomedIn = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;



    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public GameObject targetButton; // assign your button here
    void Start()
    {
        if (tmpro != null)
        {
            //tmpro.text = text;
        }

        mainCamera = Camera.main;

        // Store the original position and rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }


    public void ToggleZoom()
    {

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine); // Interrupt ongoing coroutine
        }
        if (isZoomedIn)
        {
            moveCoroutine = StartCoroutine(MoveToTransform(originalPosition, originalRotation));
            isZoomedIn = false;
        }
        else
        {
            moveCoroutine = StartCoroutine(MoveToTransform(zoomTarget.position, zoomTarget.rotation));
            isZoomedIn = true;
        }
    }

    void Update()
    {
        if (!isZoomedIn)
        {
             HandleDrag();
        }
       

        // Toggle zoom in/out with "F" key
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (moveCoroutine != null)
            {
                //StopCoroutine(moveCoroutine); // Interrupt ongoing coroutine
            }

            if (isZoomedIn)
            {
                //
            }
            else
            {

            }
        }
    }

    void HandleDrag()
    {
        //check if pointer is over a specific button.)

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        bool isOverButton = results.Any(r => r.gameObject == targetButton);
        if (isOverButton) {
            Debug.Log("Pointer is over the button, not dragging.");
            return;
        }



        RaycastHit hit = new RaycastHit();
        if (Input.GetMouseButtonDown(0))
        {


            Debug.Log("Mouse Down on Envelope");

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Paper")))
            {
                Debug.Log("Hit Paper Layer");
                if (hit.collider.gameObject == this.gameObject)
                {
                    isDragging = true;
                    lastMousePosition = GetMouseWorldPositionOnPlane();
                    velocity = Vector3.zero;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        Debug.Log("IsDragging: " + isDragging);
        if (isDragging)
        {


            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Desk")))
            {
                Debug.Log("Hit Desk Layer");
                // Calculate the new position based on the hit point
                Vector3 worldMousePosition = new Vector3(hit.point.x, planeY, hit.point.z);
                transform.position = worldMousePosition;

            }




        }
        else
        {
            // Deceleration when not dragging
            //transform.position = Vector3.zero;// * Time.deltaTime;
            //velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * dragSmoothing);
        }
    }

    Vector3 GetMouseWorldPositionOnPlane()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, planeY, 0));

        if (dragPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position;
    }

    private IEnumerator MoveToTransform(Vector3 targetPosition, Quaternion targetRotation)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f || Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
            yield return null;
        }

        // Ensure final position and rotation are set
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}
