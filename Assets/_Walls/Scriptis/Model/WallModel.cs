using System;
using System.Collections.Generic;
using UnityEngine;

public class WallModel
{
    public Vector3 Position;
    public Vector2 Size;
    public readonly float RotationY;
    public readonly float Thickness;
    public List<ObjectModel> Objects => _objects;
    public Material MaterialFront => _config.GetWallMaterialById(_wall.MaterialFrontId);
    public Material MaterialRight => _config.GetWallMaterialById(_wall.MaterialRightId);
    public Material MaterialBack => _config.GetWallMaterialById(_wall.MaterialBackId);
    public Material MaterialLeft => _config.GetWallMaterialById(_wall.MaterialLeftId);
    public Material MaterialTopBottom => _config.MaterialTopBottom;

    //Events
    public event Action Change;
    public event Action ChangeObjects;

    //Data
    private WallConfig _wall;
    private ApartmentConfig _config;
    private List<ObjectModel> _objects;

    public WallModel(WallConfig wall, ApartmentConfig config)
    {
        _wall = wall;
        _config = config;
        Position = new Vector3(wall.StartPosition.x, 0, wall.StartPosition.y);
        Size = new Vector2(Vector2.Distance(wall.StartPosition, wall.EndPosition), _config.GetHeight(wall.Height));
        var angle = Mathf.Atan2(wall.StartPosition.y - wall.EndPosition.y, wall.EndPosition.x - wall.StartPosition.x);
        var startOffset = _config.GetThickness(_wall.StartOffset);
        var endOffset = _config.GetThickness(_wall.EndOffset);
        Position.x += Mathf.Cos(angle) * startOffset;
        Position.z -= Mathf.Sin(angle) * startOffset;
        Size.x -= startOffset + endOffset;
        RotationY = angle * Mathf.Rad2Deg;
        Thickness = _config.GetThickness(wall.Thickness);
        _objects = new List<ObjectModel>();
        foreach (var objectConfig in wall.Objects)
        {
            var objectModel = MakeObject(objectConfig);
            _objects.Add(objectModel);
        }
    }

    private ObjectModel MakeObject(WallObjectConfig objectConfig)
    {
        var objectModel = new ObjectModel(objectConfig, _config, Size);
        objectModel.ChangePosition += OnObjectChange;
        return objectModel;
    }

    public void RemoveObject(ObjectModel objectModel)
    {
        objectModel.ChangePosition -= OnObjectChange;
        _objects.RemoveAt(_objects.IndexOf(objectModel));
        Change?.Invoke();
    }

    private void OnObjectChange()
    {
        ChangeObjects?.Invoke();
    }

    public void AddObject(ObjectConfig objectConfig)
    {
        var wallObjectConfig = new WallObjectConfig
        {
            Id = objectConfig.Id,
            Position = new Vector2(Size.x / 2 - objectConfig.Size.x / 2, objectConfig.Door ? 0 : Size.y / 2 - objectConfig.Size.y / 2)
        };
        var newObject = MakeObject(wallObjectConfig);
        _objects.Add(newObject);
        Change?.Invoke();
    }
}