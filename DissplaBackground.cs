using UnityEngine;
using System.Collections.Generic;

public class WireframeCity : MonoBehaviour
{
    public Material lineMaterial;
    public int blockCount = 100;
    public Vector2 sizeRange = new Vector2(1f, 4f);

    void Start()
    {
        for (int i = 0; i < blockCount; i++)
        {
            GameObject block = new GameObject("Block_" + i);
            block.transform.parent = transform;

            float w = Random.Range(sizeRange.x, sizeRange.y);
            float h = Random.Range(sizeRange.x, sizeRange.y);
            float d = Random.Range(sizeRange.x, sizeRange.y);
            Vector3 pos = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));

            block.transform.position = pos;

            var mesh = CreateWireframeCube(w, h, d);
            var mf = block.AddComponent<MeshFilter>();
            var mr = block.AddComponent<MeshRenderer>();
            mf.mesh = mesh;
            mr.material = lineMaterial;
        }
    }

    Mesh CreateWireframeCube(float width, float height, float depth)
    {
        var mesh = new Mesh();
        var verts = new List<Vector3>();
        var indices = new List<int>();

        Vector3[] corners = new Vector3[8] {
            new Vector3(-width, 0, -depth),
            new Vector3(width, 0, -depth),
            new Vector3(width, 0, depth),
            new Vector3(-width, 0, depth),
            new Vector3(-width, height, -depth),
            new Vector3(width, height, -depth),
            new Vector3(width, height, depth),
            new Vector3(-width, height, depth)
        };

        // Bottom square
        AddLine(verts, indices, corners[0], corners[1]);
        AddLine(verts, indices, corners[1], corners[2]);
        AddLine(verts, indices, corners[2], corners[3]);
        AddLine(verts, indices, corners[3], corners[0]);

        // Top square
        AddLine(verts, indices, corners[4], corners[5]);
        AddLine(verts, indices, corners[5], corners[6]);
        AddLine(verts, indices, corners[6], corners[7]);
        AddLine(verts, indices, corners[7], corners[4]);

        // Verticals
        for (int i = 0; i < 4; i++)
            AddLine(verts, indices, corners[i], corners[i + 4]);

        mesh.SetVertices(verts);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        return mesh;
    }

    void AddLine(List<Vector3> verts, List<int> indices, Vector3 a, Vector3 b)
    {
        indices.Add(verts.Count);
        verts.Add(a);
        indices.Add(verts.Count);
        verts.Add(b);
    }
}
