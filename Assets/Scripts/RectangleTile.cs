using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleTile : BaseTileInterface
{
    public RectangleTile(float size, Vector3 pos, GameObject meshRoot) : base(size, pos, meshRoot)
    {

    }

    private Vector3 GetPoint(int index, float size)
    {
        // ���������ε��ĸ�����λ��
        switch (index)
        {
            case 0: return new Vector3(-size / 2, 0, size / 2); // ����
            case 1: return new Vector3(size / 2, 0, size / 2);  // ����
            case 2: return new Vector3(size / 2, 0, -size / 2); // ����
            case 3: return new Vector3(-size / 2, 0, -size / 2); // ����
            default: return Vector3.zero;
        }
    }

    public override void DrawHex()
    {
        // �����ε��������
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // ����ĸ�����
        for (int i = 0; i < 4; i++)
        {
            vertices.Add(pos + GetPoint(i, size));
            uvs.Add(new Vector2((vertices[i].x / size) + 0.5f, (vertices[i].z / size) + 0.5f));
        }

        // ������ĵ�
        vertices.Add(pos);
        uvs.Add(new Vector2(0.5f, 0.5f));

        // ��������������
        for (int i = 0; i < 4; i++)
        {
            triangles.Add(4); // ���ĵ�
            triangles.Add(i);
            triangles.Add((i + 1) % 4);
        }

        // Ӧ�õ�����
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        root.GetComponent<MeshFilter>().mesh = mesh;
        root.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        DrawBoundary(new Vector3(0, 0.05f, 0), 0.2f);
    }

    private void DrawBoundary(Vector3 offset, float lineWidth)
    {
        // �����α߽����
        List<Vector3> boundaryVertices = new List<Vector3>();
        List<int> boundaryTriangles = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            // ��ǰ�ߵ�������
            Vector3 start = pos + GetPoint(i, size) + offset;
            Vector3 end = pos + GetPoint((i + 1) % 4, size) + offset;

            // ���㷨�߲�ƫ��
            Vector3 dir = (end - start).normalized;
            Vector3 normal = new Vector3(-dir.z, 0, dir.x) * lineWidth;

            // ����ĸ�����
            boundaryVertices.Add(start - normal); // �ڵ�1
            boundaryVertices.Add(start + normal); // ���1
            boundaryVertices.Add(end - normal);   // �ڵ�2
            boundaryVertices.Add(end + normal);   // ���2

            // ��������������
            int vertexIndex = i * 4;
            boundaryTriangles.Add(vertexIndex);
            boundaryTriangles.Add(vertexIndex + 1);
            boundaryTriangles.Add(vertexIndex + 2);

            boundaryTriangles.Add(vertexIndex + 1);
            boundaryTriangles.Add(vertexIndex + 3);
            boundaryTriangles.Add(vertexIndex + 2);
        }

        // �����߽�����
        Mesh boundaryMesh = new Mesh();
        boundaryMesh.vertices = boundaryVertices.ToArray();
        boundaryMesh.triangles = boundaryTriangles.ToArray();
        boundaryMesh.RecalculateNormals();

        // �����߽� GameObject
        boundaryObject = new GameObject("RectangleBoundary", typeof(MeshFilter), typeof(MeshRenderer));
        boundaryObject.GetComponent<MeshFilter>().mesh = boundaryMesh;
        boundaryObject.GetComponent<MeshRenderer>().material = normal;

        boundaryObject.transform.SetParent(root.transform);
    }
}