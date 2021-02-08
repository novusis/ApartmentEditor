using System;
using UnityEngine;

public class ObjectModel
{
    //Event
    public event Action ChangePosition;

    private Vector2 _position;
    private readonly ObjectConfig _data;
    private Vector2 _wallSize;

    public ObjectModel(WallObjectConfig objectConfig, ApartmentConfig config, Vector2 wallSize)
    {
        _position = objectConfig.Position;
        _data = config.GetObject(objectConfig.Id);
        _wallSize = wallSize;
    }

    public GameObject GetHole(Material material)
    {
        _data.SetMaterial(material);
        return _data.Hole;
    }

    public GameObject GetObject()
    {
        return _data.View;
    }

    public Vector3 GetPosition()
    {
        var dataSize = _position + _data.Size / 2f;
        return new Vector3(dataSize.x, dataSize.y, 0);
    }

    public bool HasView()
    {
        return _data.View != null;
    }

    public string GetName()
    {
        return _data.Name;
    }

    public Vector2 GetWallPosition()
    {
        return _position;
    }

    public bool IsDoor()
    {
        return _data.Door;
    }

    public void SetWallPosition(float x, float y)
    {
        _position.x = Mathf.Clamp(x, 0, _wallSize.x - _data.Size.x);
        _position.y = IsDoor() ? 0 : Mathf.Clamp(y, 0, _wallSize.y - _data.Size.y);
        ChangePosition?.Invoke();
    }
}