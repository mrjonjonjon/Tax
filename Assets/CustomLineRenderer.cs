using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CustomLineRenderer : MonoBehaviour
{
    public Color lineColor = Color.black;
    public float lineWidth = 0.1f;
    public int smoothness = 10; // Number of segments for rounding

    private Mesh mesh;
    public List<Vector3> points = new List<Vector3>();

    public IReadOnlyList<Vector3> Points => points; // Expose points list

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Assign material
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.color = lineColor;
        GetComponent<MeshRenderer>().material = material;
    }

    public void AddPoint(Vector3 position)
    {
        if (points.Count > 0 && Vector3.Distance(points[points.Count - 1], position) < 0.01f)
            return; // Avoid adding very close points

        points.Add(position);
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        if (points.Count < 2)
        {
            mesh.Clear();
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 current = points[i];
            Vector3 previous = i > 0 ? points[i - 1] : current;
            Vector3 next = i < points.Count - 1 ? points[i + 1] : current;

            // Calculate the average direction to smooth normals
            Vector3 directionToPrev = (current - previous).normalized;
            Vector3 directionToNext = (next - current).normalized;
            Vector3 averageNormal = Vector3.Cross((directionToPrev + directionToNext).normalized, Vector3.forward) * lineWidth / 2;

            Vector3 left = current - averageNormal;
            Vector3 right = current + averageNormal;

            vertices.Add(left);
            vertices.Add(right);

            colors.Add(lineColor);
            colors.Add(lineColor);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));

            if (i < points.Count - 1)
            {
                int vertIndex = i * 2;

                triangles.Add(vertIndex);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 1);

                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 3);
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void ClearLine()
    {
        points.Clear();
        mesh.Clear();
    }
}
