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
        // 返回正方形的四个顶点位置
        switch (index)
        {
            case 0: return new Vector3(-size / 2, 0, size / 2); // 左上
            case 1: return new Vector3(size / 2, 0, size / 2);  // 右上
            case 2: return new Vector3(size / 2, 0, -size / 2); // 右下
            case 3: return new Vector3(-size / 2, 0, -size / 2); // 左下
            default: return Vector3.zero;
        }
    }

    public override void DrawHex()
    {
        // 正方形的主体绘制
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // 添加四个顶点
        for (int i = 0; i < 4; i++)
        {
            vertices.Add(pos + GetPoint(i, size));
            uvs.Add(new Vector2((vertices[i].x / size) + 0.5f, (vertices[i].z / size) + 0.5f));
        }

        // 添加中心点
        vertices.Add(pos);
        uvs.Add(new Vector2(0.5f, 0.5f));

        // 生成三角形索引
        for (int i = 0; i < 4; i++)
        {
            triangles.Add(4); // 中心点
            triangles.Add(i);
            triangles.Add((i + 1) % 4);
        }

        // 应用到网格
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
        // 正方形边界绘制
        List<Vector3> boundaryVertices = new List<Vector3>();
        List<int> boundaryTriangles = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            // 当前边的两个点
            Vector3 start = pos + GetPoint(i, size) + offset;
            Vector3 end = pos + GetPoint((i + 1) % 4, size) + offset;

            // 计算法线并偏移
            Vector3 dir = (end - start).normalized;
            Vector3 normal = new Vector3(-dir.z, 0, dir.x) * lineWidth;

            // 添加四个顶点
            boundaryVertices.Add(start - normal); // 内点1
            boundaryVertices.Add(start + normal); // 外点1
            boundaryVertices.Add(end - normal);   // 内点2
            boundaryVertices.Add(end + normal);   // 外点2

            // 构造三角形索引
            int vertexIndex = i * 4;
            boundaryTriangles.Add(vertexIndex);
            boundaryTriangles.Add(vertexIndex + 1);
            boundaryTriangles.Add(vertexIndex + 2);

            boundaryTriangles.Add(vertexIndex + 1);
            boundaryTriangles.Add(vertexIndex + 3);
            boundaryTriangles.Add(vertexIndex + 2);
        }

        // 创建边界网格
        Mesh boundaryMesh = new Mesh();
        boundaryMesh.vertices = boundaryVertices.ToArray();
        boundaryMesh.triangles = boundaryTriangles.ToArray();
        boundaryMesh.RecalculateNormals();

        // 创建边界 GameObject
        boundaryObject = new GameObject("RectangleBoundary", typeof(MeshFilter), typeof(MeshRenderer));
        boundaryObject.GetComponent<MeshFilter>().mesh = boundaryMesh;
        boundaryObject.GetComponent<MeshRenderer>().material = normal;

        boundaryObject.transform.SetParent(root.transform);
    }
}