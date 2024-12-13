using UnityEngine;

using UnityEngine;

public class Signable : MonoBehaviour
{
    public Color penColor = Color.black;
    public float penSize = 0.1f;

    private CustomLineRenderer customLineRenderer;
    private Camera uiCamera;
    public bool isSigning = false;

    private void Start()
    {
        uiCamera = Camera.main; // Assuming the main camera is used for UI.

        // Initialize CustomLineRenderer
        customLineRenderer = gameObject.AddComponent<CustomLineRenderer>();
        customLineRenderer.lineColor = penColor;
        customLineRenderer.lineWidth = penSize;
    }

    public void BeginSignature()
    {
        isSigning = true;
        customLineRenderer.ClearLine(); // Reset the line when starting a new signature
    }

    private void Update()
    {
        if (isSigning)
        {
            AddPointToSignature(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndSignature();
        }
    }

    public void AddPointToSignature(Vector2 screenPosition)
    {
        Ray ray = uiCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("SignBox")))
        {
            Vector3 worldPosition = hit.point;

            // Add the new point to the CustomLineRenderer
            customLineRenderer.AddPoint(transform.InverseTransformPoint(worldPosition));
        }
    }

    public void EndSignature()
    {
        isSigning = false;
        Debug.Log("Signature completed!");
    }

    public void ClearSignature()
    {
        customLineRenderer.ClearLine(); // Clear all points in the CustomLineRenderer
        Debug.Log("Signature cleared!");
    }
}
