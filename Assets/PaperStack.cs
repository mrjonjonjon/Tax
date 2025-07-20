using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperStack : MonoBehaviour
{
    public List<GameObject> stack = new List<GameObject>();
    public float yOffset = 0.5f;
    public float moveDuration = 0.2f;

    public int currentIndex = 0;

    // Toggle booleans for runtime testing
    public bool runNextDocument = false;
    public bool runPreviousDocument = false;
    public bool refreshStackPositions = false;

    void Start()
    {
        InitializeStack();
    }

    void InitializeStack()
    {
        stack.Clear();
        foreach (var envelope in FindObjectsOfType<Envelope>())
        {
            if (envelope.gameObject.activeInHierarchy)
                stack.Add(envelope.gameObject);
        }
        UpdateStackPositions(true);
    }

    void Update()
    {
        if (runNextDocument)
        {
            runNextDocument = false;
            NextDocument();
        }

        if (runPreviousDocument)
        {
            runPreviousDocument = false;
            PreviousDocument();
        }

        if (refreshStackPositions)
        {
            refreshStackPositions = false;
            UpdateStackPositions();
        }
    }

    void UpdateStackPositions(bool instant = false)
    {
        for (int i = 0; i < stack.Count; i++)
        {
            var go = stack[i];
            float yPos = transform.position.y + (i * yOffset);

            if (instant)
                go.transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            else
                StartCoroutine(MoveToPosition(go, new Vector3(transform.position.x, yPos, transform.position.z), moveDuration));

            go.GetComponent<Envelope>().planeY = yPos;
        }
    }

    public void NextDocument()
    {
        if (stack.Count == 0) return;
        //currentIndex = (currentIndex + 1) % stack.Count;
        MoveToBottom(stack.Count - 1);
    }

    public void PreviousDocument()
    {
        if (stack.Count == 0) return;
        //currentIndex = (currentIndex - 1 + stack.Count) % stack.Count;
        MoveToTop(0);
    }

    void MoveToTop(int index)
    {
        var go = stack[index];
        stack.RemoveAt(index);
        stack.Add(go);
        UpdateStackPositions();
    }
    void MoveToBottom(int index)
    {
        var go = stack[index];
        stack.RemoveAt(index);
        stack.Insert(0,go);
        UpdateStackPositions();
    }
    IEnumerator MoveToPosition(GameObject obj, Vector3 target, float duration)
    {
        Vector3 start = obj.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            obj.transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = target;
    }
}
