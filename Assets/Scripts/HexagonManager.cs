using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonManager : MonoBehaviour
{
    private static HexagonManager instance;
    public static HexagonManager Instance => instance;

    public int width = 10;
    public int height = 10;
    public float size = 5;
    public int preparationSeatNum = 10;
    List<HexagonTile> allTiles = new List<HexagonTile>();
    List<RectangleTile> allPreparationSeat = new List<RectangleTile>();
    GameObject mapRoot;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mapRoot = new GameObject("Map");
        CreateMap();
        CreatePreparationSeat();
    }

    private void CreatePreparationSeat()
    {
        float centerX = (width / 2) * (2 * size * Mathf.Cos(Mathf.PI / 180 * 30));

        Debug.Log(centerX);

        if (preparationSeatNum % 2 == 0)
        {

        }
        else
        {
            GameObject tileObj = new GameObject("Center", typeof(MeshFilter), typeof(MeshRenderer));
            RectangleTile tile = new RectangleTile(size, new Vector3(0, 0, centerX), tileObj);
            tile.DrawHex();
            allPreparationSeat.Add(tile);

            int num = 1;

            while (num < preparationSeatNum / 2)
            {
                GameObject newObj = new GameObject("Seat_Right" + num, typeof(MeshFilter), typeof(MeshRenderer));
                RectangleTile newTile = new RectangleTile(size, new Vector3(0, 0, centerX + size * num), newObj);
                newTile.DrawHex();
                allPreparationSeat.Add(tile);

                GameObject newObjLeft = new GameObject("Seat_Left" + num, typeof(MeshFilter), typeof(MeshRenderer));
                RectangleTile newTileLeft = new RectangleTile(size, new Vector3(0, 0, centerX - size * num), newObjLeft);
                newTileLeft.DrawHex();
                allPreparationSeat.Add(newTileLeft);

                num++;
            }
        }
    }

    private void CreateMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float offect = 0;

                if (j % 2 == 0)
                {
                    offect = size * Mathf.Cos(Mathf.PI / 180 * 30);
                }

                GameObject tileObj = new GameObject("HexagonTile" + i + "_" + j, typeof(MeshFilter), typeof(MeshRenderer));

                tileObj.transform.SetParent(mapRoot.transform);

                HexagonTile tile = new HexagonTile(size, new Vector3(j * 3 * size * Mathf.Sin(Mathf.PI / 180 * 30), 0, offect + i * (2 * size * Mathf.Cos(Mathf.PI / 180 * 30))), tileObj);
                allTiles.Add(tile);

                tile.DrawHex();
            }
        }

        // 获取当前物体及其子物体的所有 MeshFilter
        List<MeshFilter> meshFilters = new List<MeshFilter>();

        for (int i = 0; i < mapRoot.transform.childCount; i++)
        {
            meshFilters.Add(mapRoot.transform.GetChild(i).GetComponent<MeshFilter>());
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Count];

        for (int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].GetComponent<MeshRenderer>().enabled = false;
        }

        // 创建合并后的网格
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        // 为当前物体赋值合并的网格
        MeshFilter meshFilter = mapRoot.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = mapRoot.AddComponent<MeshRenderer>();

        meshFilter.mesh = combinedMesh;
        meshRenderer.material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        mapRoot.transform.localPosition = Vector3.zero;
    }

    public Vector2Int GetHexagonTileByPos(Vector3 pos)
    {
        int j = Mathf.RoundToInt(pos.x / (3 * size * Mathf.Sin(Mathf.PI / 180 * 30)));

        float offect = 0;

        if (j % 2 == 0)
        {
            offect = size * Mathf.Cos(Mathf.PI / 180 * 30);
        }

        int i = Mathf.RoundToInt((pos.z - offect) / (2 * size * Mathf.Cos(Mathf.PI / 180 * 30)));

        return new Vector2Int(i, j);
    }

    public HexagonTile this[int x, int y]
    {
        get
        {
            if (x * height + y < allTiles.Count && x >= 0 && y >= 0)
                return allTiles[x * height + y];
            else
                return null;
        }
        set
        {
            allTiles[x * height + y] = value;
        }
    }

    private void Update()
    {
        foreach (var key in allTiles)
        {
            key.ShowBoundary(Input.GetKey(KeyCode.X));
        }
    }
}
