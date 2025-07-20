using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Envelope : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 velocity;
    private Vector3 dragOffsetLocal;

    public Camera mainCamera;
    public TextMeshProUGUI tmpro;
    public string text;

    [SerializeField] public float planeY = 0.0f;
    [SerializeField] public float dragSmoothing = 10.0f;
    [SerializeField] public float moveSpeed = 50.0f;
    [SerializeField] private Transform zoomTarget;

    private Coroutine moveCoroutine;
    private bool isZoomedIn = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public GameObject targetButton;

    void Start()
    {
        if (tmpro != null)
        {
            //tmpro.text = text;
        }

        mainCamera = Camera.main;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        eventSystem = EventSystem.current;
        zoomTarget = Camera.main.transform.Find("zoomTarget");
    }

    public void ToggleZoom()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            // Optional: hook for toggling zoom if needed
        }
    }

    void HandleDrag()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        bool isOverButton = targetButton && results.Any(r => r.gameObject == targetButton);
        if (isOverButton)
        {
            Debug.Log("Pointer is over the button, not dragging.");
            return;
        }

        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Down on Envelope");

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Paper")))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    isDragging = true;

                    // Store offset from object center to hit point
                    Vector3 localHitPoint = transform.InverseTransformPoint(new Vector3(hit.point.x, planeY, hit.point.z));
                    dragOffsetLocal = localHitPoint;

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
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Desk")))
            {
                Vector3 worldMousePosition = new Vector3(hit.point.x, planeY, hit.point.z);
                Vector3 adjustedPosition = worldMousePosition - transform.TransformVector(dragOffsetLocal);
                adjustedPosition.y = planeY; // Ensure the Y position is set to the plane height
                transform.position = adjustedPosition;
            }
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

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}
