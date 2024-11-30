using System.Collections.Generic;
using UnityEngine;

public class HexagonTile : BaseTileInterface
{
    // 初始化顶点列表
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
        // 添加中心点
        vertices.Add(pos);
        uvs.Add(new Vector2(0.5f, 0.5f));

        // 边界点
        List<Vector3> boundaryPoints = new List<Vector3>();

        // 添加六边形顶点
        for (int i = 0; i < 6; i++)
        {
            var pointPos = pos + GetPoint(i, size); // 获取每个顶点的位置
            vertices.Add(pointPos);
            boundaryPoints.Add(pointPos);

            // UV 映射
            Vector2 uv = new Vector2((pointPos.x / size + 1) * 0.5f, (pointPos.z / size + 1) * 0.5f);
            uvs.Add(uv);

            // 添加三角形索引
            if (i < 5)
            {
                triangles.Add(0);        // 中心点
                triangles.Add(i + 2);    // 下一个点
                triangles.Add(i + 1);    // 当前点
            }
            else
            {
                // 最后一个点需要连接回第一个顶点
                triangles.Add(0);        // 中心点
                triangles.Add(1);        // 第一个顶点
                triangles.Add(i + 1);    // 当前点
            }
        }

        // 应用网格
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // 创建 GameObject 并赋值网格
        root.GetComponent<MeshFilter>().mesh = mesh;

        // 设置材质
        root.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        // 绘制边界线
        DrawHexagonBoundary(new Vector3(0, 0.05f, 0), 0.2f);
    }

    private void DrawHexagonBoundary(Vector3 offset, float lineWidth)
    {
        // 初始化边界顶点和索引
        List<Vector3> boundaryVertices = new List<Vector3>();
        List<int> boundaryTriangles = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            // 当前边的两个点
            Vector3 start = pos + GetPoint(i, size) + offset;
            Vector3 end = pos + GetPoint((i + 1) % 6, size) + offset;

            // 统一法线方向
            Vector3 dir = (end - start).normalized;
            Vector3 normal = new Vector3(-dir.z, 0, dir.x) * lineWidth / 2;

            // 添加四个顶点
            boundaryVertices.Add(start - normal); // 内点1
            boundaryVertices.Add(start + normal); // 外点1
            boundaryVertices.Add(end - normal);   // 内点2
            boundaryVertices.Add(end + normal);   // 外点2

            // 构造三角形索引，确保无缺口
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
        boundaryObject = new GameObject("HexagonBoundary", typeof(MeshFilter), typeof(MeshRenderer));
        boundaryObject.GetComponent<MeshFilter>().mesh = boundaryMesh;
        boundaryObject.transform.SetParent(root.transform);
    }
}