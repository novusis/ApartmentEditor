using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ApartmentConfig", menuName = "Scriptable Objects/ApartmentConfig", order = 0)]
public class ApartmentConfig : ScriptableObject
{
    public MaterialConfig[] WallsMaterials;
    public ObjectConfig[] Objects;
    public WallConfig[] Walls;
    public float WallHeight;
    public float WallThickness;
    public Material MaterialTopBottom;

    public float GetHeight(float wallHeight)
    {
        if (wallHeight < 0)
            return WallHeight;

        return wallHeight;
    }

    public float GetThickness(float wallThickness)
    {
        if (wallThickness < 0)
            return WallThickness;

        return wallThickness;
    }

    public Material GetWallMaterialById(int materialId)
    {
        return WallsMaterials.ToList().Find(material => material.Id == materialId).Material;
    }

    public ObjectConfig GetObject(int objectId)
    {
        return Objects.ToList().Find(obj => obj.Id == objectId);
    }
}

[Serializable]
public class WallConfig
{
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public float Thickness = -1f;
    public float Height = 3f;
    public int MaterialFrontId;
    public int MaterialRightId;
    public int MaterialBackId;
    public int MaterialLeftId;
    public float StartOffset = 0;
    public float EndOffset = 0;
    public List<WallObjectConfig> Objects;
}

[Serializable]
public class WallObjectConfig
{
    public int Id;
    public Vector2 Position;
}

[Serializable]
public class ObjectConfig
{
    public int Id;
    public string Name;
    public GameObject View;
    public GameObject Hole;
    public Vector2 Size;
    public bool MakeHole = true;
    public bool Door;

    public void SetMaterial(Material material)
    {
        Hole.GetComponent<Renderer>().sharedMaterial = material;
    }
}

[Serializable]
public class MaterialConfig
{
    public int Id;
    public Material Material;
}