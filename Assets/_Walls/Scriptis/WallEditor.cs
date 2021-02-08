using System.Collections.Generic;
using UnityEngine;

public class WallEditor : MonoBehaviour
{
    public GameObject ObjectWidget;
    public GameObject ObjectOnWallWidget;
    public Transform ObjectsContainer;
    public Transform ObjectsOnWallContainer;
    public GameObject ObjectsOnWallPanel;
    public GameObject AddObjectsPanel;
    private ApartmentConfig _config;
    private List<WallObjectOnWallWidget> _wallObjects = new List<WallObjectOnWallWidget>();
    private WallModel _targetWall;

    public void Init(ApartmentConfig config)
    {
        _config = config;
        for (int i = 0; i < _config.Objects.Length; i++)
        {
            var widget = Instantiate(ObjectWidget).GetComponent<WallObjectWidget>();
            widget.Init(_config.Objects[i], OnAddObject);
            widget.transform.SetParent(ObjectsContainer);
        }
        ObjectsOnWallPanel.SetActive(false);
        AddObjectsPanel.SetActive(false);
    }

    public void SelectWall(WallModel wall)
    {
        if (_targetWall == wall)
        {
            return;
        }
        if (_targetWall != null)
        {
            _targetWall.Change -= OnObjects;
            _targetWall = null;
        }
        _targetWall = wall;
        _targetWall.Change += OnObjects;
        DrawWallObjects();
        AddObjectsPanel.SetActive(true);
    }

    private void OnAddObject(ObjectConfig objectConfig)
    {
        _targetWall?.AddObject(objectConfig);
    }

    private void OnObjects()
    {
        DrawWallObjects();
    }

    private void DrawWallObjects()
    {
        ClearSelectWallObjects();
        //TODO : Надо сделать пул для виджетов объектов на стене.
        for (int i = 0; i < _targetWall.Objects.Count; i++)
        {
            var widget = Instantiate(ObjectOnWallWidget).GetComponent<WallObjectOnWallWidget>();
            widget.Init(_targetWall.Objects[i], OnRemoveObject);
            widget.transform.SetParent(ObjectsOnWallContainer);
            _wallObjects.Add(widget);
        }

        ObjectsOnWallPanel.SetActive(_wallObjects.Count > 0);
    }

    private void ClearSelectWallObjects()
    {
        foreach (var wallObject in _wallObjects)
        {
            Destroy(wallObject.gameObject);
        }
        ObjectsOnWallContainer.DetachChildren();
        _wallObjects.Clear();
    }

    private void OnRemoveObject(ObjectModel obj)
    {
        _targetWall.RemoveObject(obj);
    }
}