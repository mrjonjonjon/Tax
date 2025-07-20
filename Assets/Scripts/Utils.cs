using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    /// <summary>
    /// Casts a ray from the camera to the mouse position and returns the hit point on the "Paper" layer.
    public static Vector3 RaycastToPaperWorld()
    {

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Paper")))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    public static bool RaycastToPaperWorld(out Vector3 hitPoint)
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Paper")))
        {
            hitPoint = hit.point;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }

// public static void UpdateHighlightQuad(Vector3 worldStart, Vector3 worldEnd)
// {
//     Vector3 center = (worldStart + worldEnd) / 2f;
//     currentHighlight.transform.position = center;

//     // Calculate local positions relative to the paper
//     Vector3 localStart = transform.InverseTransformPoint(worldStart);
//     Vector3 localEnd = transform.InverseTransformPoint(worldEnd);

//     Vector3 localCenter = (localStart + localEnd) / 2f;
//     Vector3 localSize = localEnd - localStart;

//     // Move quad to local center, then transform to world
//     currentHighlight.transform.localPosition = localCenter - Vector3.forward * 0.01f; // Slightly above paper

//     // Adjust rotation to match paper exactly
//     currentHighlight.transform.localRotation = Quaternion.identity;

//     // Because Unity Quad is XY plane, adjust scale.x = paper local x, scale.y = paper local z (if paper is XZ)
//     currentHighlight.transform.localScale = new Vector3(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y), 1f);
// }
    
    
}
