using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTileInterface
{
    protected float size; 
    protected Mesh mesh;
    protected Vector3 pos; 
    protected Material normal;
    protected Material show;
    protected GameObject root;
    protected GameObject boundaryObject;

    public BaseTileInterface(float size, Vector3 pos, GameObject meshRoot)
    {
        this.size = size;
        this.pos = pos;

        mesh = new Mesh();
        root = meshRoot;

        normal = new Material(Shader.Find("Standard"))
        {
            color = Color.black // 设置为黑色边界
        };

        show = new Material(Shader.Find("Standard"))
        {
            color = Color.red // 设置为红色边界
        };
    }

    public abstract void DrawHex();
    public virtual void ShowBoundary(bool show)
    {
        if (boundaryObject != null)
        {
            boundaryObject.GetComponent<MeshRenderer>().material = show ? this.show : normal;
        }
    }

    public Vector3 GetPos()
    {
        return pos;
    }

}
