using UnityEngine;
using UnityEngine;
using UnityEngine;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public class DocumentStamper : MonoBehaviour
{
    [Header("References")]
    public Camera cam; // Assign your main or UI camera
    public GameObject stampPrefab; // Assign your stamp prefab (e.g. stamp sprite or quad)

    [Header("Stamp Settings")]
    public Vector3 stampRotation = new Vector3(90f, 0f, 0f); // Default rotation for stamp to face down onto paper
    public Vector3 stampScale = Vector3.one * 0.1f; // Default stamp size

    [Header("Layer Mask")]
    public LayerMask documentLayerMask; // Assign to only raycast documents

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click to stamp
        {
            if (RaycastToDocument(out RaycastHit hit))
            {
                StampDocument(hit);
            }
        }
    }

    bool RaycastToDocument(out RaycastHit hit)
    {

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Paper")|LayerMask.GetMask("UI")))
        {
            return true;
        }
        return false;

    }

    void StampDocument(RaycastHit hit)
    {
        // Instantiate stamp at hit point with document as parent
        GameObject stamp = Instantiate(stampPrefab, hit.point, Quaternion.identity, transform.Find("Canvas/Mask"));

        Vector3 localPosition = transform.InverseTransformPoint(hit.point);
        stamp.transform.localPosition = localPosition - Vector3.forward * 0.01f;

        // Align rotation relative to document normal
        stamp.transform.localRotation = //random z rotation
            Quaternion.Euler(0,0, Random.Range(-60f, 60f));
        // Apply desired scale
        //stamp.transform.localScale = stampScale;
    }
}
