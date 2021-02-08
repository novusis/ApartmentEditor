using UnityEngine;
using UnityEngine.Rendering;

public class Helper
{
    public static GameObject CreateWallThickness(Vector2 size, float thickness, Material[] material)
    {
        float zuv = size.y / size.y;
        float xuv = size.x / size.y;
        
        float sideX = thickness / size.y;

        GameObject go = new GameObject("Wall");
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        mr.receiveShadows = true;
        mr.shadowCastingMode = ShadowCastingMode.On;

        Mesh m = new Mesh();

        m.vertices = new Vector3[]
        {
            new Vector3(0, 0),
            new Vector3(0, size.y),
            new Vector3(size.x, 0),
            new Vector3(size.x, size.y),
            //thickness
            new Vector3(0, 0, thickness),
            new Vector3(0, size.y, thickness),
            new Vector3(size.x, 0, thickness),
            new Vector3(size.x, size.y, thickness),
        };
        
        m.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, zuv),
            new Vector2(xuv, 0),
            new Vector2(xuv, zuv),
            new Vector2(sideX, 0),
            new Vector2(sideX, zuv),
            new Vector2(xuv+sideX, 0),
            new Vector2(xuv+sideX, zuv),
        };

        int[] front =
        {
            0, 1, 2,
            1, 3, 2,
        };

        int[] right =
        {
            2, 3, 6,
            3, 7, 6,
        };

        int[] back =
        {
            6, 7, 4,
            7, 5, 4,
        };

        int[] left =
        {
            4, 5, 0,
            5, 1, 0,
        };

        int[] topBottom = 
        {
            //top
            5, 7, 1,
            7, 3, 1,
            //bottom
            4, 0, 2,
            6, 4, 2,
        };

        m.subMeshCount = 5;
        m.SetTriangles(front, 0);
        m.SetTriangles(right, 1);
        m.SetTriangles(back, 2);
        m.SetTriangles(left, 3);
        m.SetTriangles(topBottom, 4);

        mf.mesh = m;
        m.RecalculateBounds();

        var materials = new Material[5];
        for (int i = 0; i < 5; i++)
            materials[i] = material[i];
        mr.sharedMaterials = materials;

        m.RecalculateNormals();
        var collider = go.AddComponent<MeshCollider>();
        collider.sharedMesh = m;
        
        return go;
    }
}