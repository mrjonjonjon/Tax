using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperStack : MonoBehaviour
{
    public List<GameObject> stack = new List<GameObject>();
    public float yOffset = 0.5f; // Adjust this value to set the spacing between papers

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < stack.Count; i++)
        {
            var go = Instantiate(stack[i], transform);
            go.transform.position = new Vector3(transform.position.x, transform.position.y - (i * yOffset), transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
