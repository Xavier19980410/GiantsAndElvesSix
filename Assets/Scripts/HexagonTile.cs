using System.Collections.Generic;
using UnityEngine;

public class HexagonTile : BaseTileInterface
{
    // ��ʼ�������б�
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    public bool hasTile;

    public HexagonTile(float size, Vector3 pos, GameObject meshRoot) : base(size, pos, meshRoot)
    {

    }

    private Vector3 GetPoint(int index, float size)
    {
        float angle_deg = 60 * index;
        float angle_rad = Mathf.PI / 180 * angle_deg;

        return new Vector3(size * Mathf.Cos(angle_rad), 0, size * Mathf.Sin(angle_rad));
    }

    public override void DrawHex()
    {
        // ������ĵ�
        vertices.Add(pos);
        uvs.Add(new Vector2(0.5f, 0.5f));

        // �߽��
        List<Vector3> boundaryPoints = new List<Vector3>();

        // ��������ζ���
        for (int i = 0; i < 6; i++)
        {
            var pointPos = pos + GetPoint(i, size); // ��ȡÿ�������λ��
            vertices.Add(pointPos);
            boundaryPoints.Add(pointPos);

            // UV ӳ��
            Vector2 uv = new Vector2((pointPos.x / size + 1) * 0.5f, (pointPos.z / size + 1) * 0.5f);
            uvs.Add(uv);

            // �������������
            if (i < 5)
            {
                triangles.Add(0);        // ���ĵ�
                triangles.Add(i + 2);    // ��һ����
                triangles.Add(i + 1);    // ��ǰ��
            }
            else
            {
                // ���һ������Ҫ���ӻص�һ������
                triangles.Add(0);        // ���ĵ�
                triangles.Add(1);        // ��һ������
                triangles.Add(i + 1);    // ��ǰ��
            }
        }

        // Ӧ������
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // ���� GameObject ����ֵ����
        root.GetComponent<MeshFilter>().mesh = mesh;

        // ���ò���
        root.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        // ���Ʊ߽���
        DrawHexagonBoundary(new Vector3(0, 0.05f, 0), 0.2f);
    }

    private void DrawHexagonBoundary(Vector3 offset, float lineWidth)
    {
        // ��ʼ���߽綥�������
        List<Vector3> boundaryVertices = new List<Vector3>();
        List<int> boundaryTriangles = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            // ��ǰ�ߵ�������
            Vector3 start = pos + GetPoint(i, size) + offset;
            Vector3 end = pos + GetPoint((i + 1) % 6, size) + offset;

            // ͳһ���߷���
            Vector3 dir = (end - start).normalized;
            Vector3 normal = new Vector3(-dir.z, 0, dir.x) * lineWidth / 2;

            // ����ĸ�����
            boundaryVertices.Add(start - normal); // �ڵ�1
            boundaryVertices.Add(start + normal); // ���1
            boundaryVertices.Add(end - normal);   // �ڵ�2
            boundaryVertices.Add(end + normal);   // ���2

            // ����������������ȷ����ȱ��
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
        boundaryObject = new GameObject("HexagonBoundary", typeof(MeshFilter), typeof(MeshRenderer));
        boundaryObject.GetComponent<MeshFilter>().mesh = boundaryMesh;
        boundaryObject.transform.SetParent(root.transform);
    }
}