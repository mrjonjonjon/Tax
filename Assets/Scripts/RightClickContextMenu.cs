using UnityEngine;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RightClickContextMenu : MonoBehaviour
{
    public GameObject contextMenuPanel;
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            ShowContextMenu(Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(0) && contextMenuPanel.activeSelf) // Left-click to hide
        {
            contextMenuPanel.SetActive(false);
        }
    }
    public Vector3 GetProjectedMousePosition()
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Paper")))
        {
            return hit.point;
        }
            return Vector3.zero; // Return zero if no hit

        
    }
    void ShowContextMenu(Vector3 position)
    {
        contextMenuPanel.SetActive(true);
        contextMenuPanel.transform.position = GetProjectedMousePosition() + Vector3.up * 0.01f;
    }
}
