using UnityEngine;

public class PaperHighlighter : MonoBehaviour
{
    public Camera cam; // assign your camera
    public Material highlightMaterial; // assign a blue transparent material

    private Vector3 startWorldPos;
    private GameObject currentHighlight;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RaycastToPaper(out Vector3 hitPoint))
            {
                startWorldPos = hitPoint;

                // create quad
                currentHighlight = GameObject.CreatePrimitive(PrimitiveType.Quad);
                currentHighlight.GetComponent<Renderer>().material = highlightMaterial;
                currentHighlight.transform.parent = transform; // parent to paper
                currentHighlight.transform.localRotation = Quaternion.identity;
            }
        }

        if (Input.GetMouseButton(0) && currentHighlight != null)
        {
            if (RaycastToPaper(out Vector3 currentHit))
            {
                UpdateHighlightQuad(startWorldPos, currentHit);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentHighlight = null;
        }
    }

    bool RaycastToPaper(out Vector3 hitPoint)
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Paper")))
        {
            hitPoint = hit.point;
            return true;
        }
        // return Vector3.zero; // Return zero if no hit
        hitPoint = Vector3.zero;
        return false;
    }

void UpdateHighlightQuad(Vector3 start, Vector3 end)
{
    Vector3 center = (start + end) / 2f;
    currentHighlight.transform.position = center;

    // Calculate local positions relative to the paper
    Vector3 localStart = transform.InverseTransformPoint(start);
    Vector3 localEnd = transform.InverseTransformPoint(end);

    Vector3 localCenter = (localStart + localEnd) / 2f;
    Vector3 localSize = localEnd - localStart;

    // Move quad to local center, then transform to world
    currentHighlight.transform.localPosition = localCenter - Vector3.forward * 0.01f; // Slightly above paper

    // Adjust rotation to match paper exactly
    currentHighlight.transform.localRotation = Quaternion.identity;

    // Because Unity Quad is XY plane, adjust scale.x = paper local x, scale.y = paper local z (if paper is XZ)
    currentHighlight.transform.localScale = new Vector3(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y), 1f);
}

}
