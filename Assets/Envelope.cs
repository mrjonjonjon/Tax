using System.Collections;
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

    [SerializeField] public float planeY = 0.0f; // Set the Y-coordinate of the plane
    [SerializeField] public float dragSmoothing = 10.0f; // Smoothing for drag motion
    [SerializeField] public float moveSpeed = 2.0f; // Speed for smooth movement
    [SerializeField] private Transform zoomTarget; // Target transform for zoom position and rotation

    private Coroutine moveCoroutine;
    private bool isZoomedIn = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        if(tmpro!=null){
             //tmpro.text = text;
        }
       
        mainCamera = Camera.main;

        // Store the original position and rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }


    public void ToggleZoom(){

         if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine); // Interrupt ongoing coroutine
            }
            if(isZoomedIn){
                moveCoroutine = StartCoroutine(MoveToTransform(originalPosition, originalRotation));
                isZoomedIn = false;
            }else{
                moveCoroutine = StartCoroutine(MoveToTransform(zoomTarget.position, zoomTarget.rotation));
                isZoomedIn = true;
            }
    }
 
    void Update()
    {
        HandleDrag();

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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Paper")|LayerMask.GetMask("Calculator")))
            {
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

        if (isDragging)
        {
            Vector3 mousePosition = GetMouseWorldPositionOnPlane();
            velocity = (mousePosition - lastMousePosition) / Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            lastMousePosition = mousePosition;
        }
        else
        {
            // Deceleration when not dragging
            transform.position += velocity * Time.deltaTime;
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * dragSmoothing);
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
