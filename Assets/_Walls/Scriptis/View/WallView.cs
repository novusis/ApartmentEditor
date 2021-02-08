using System.Collections.Generic;
using Parabox.CSG;
using UnityEngine;

public class WallView : MonoBehaviour
{
    private WallModel _wall;
    private Camera _camera;
    private Transform _originalWall;
    private GameObject _currentView;
    private WallEditor _editor;
    private MeshCollider _collider;
    private readonly List<GameObject> _allObjects = new List<GameObject>();

    public void Init(WallModel wall, WallEditor editor)
    {
        _wall = wall;
        _editor = editor;
        _wall.Change += OnChange;
        _wall.ChangeObjects += OnChange;
        _camera = Camera.main;

        _originalWall = Helper.CreateWallThickness(wall.Size, wall.Thickness,
            new[]
            {
                wall.MaterialFront, wall.MaterialRight, wall.MaterialBack, wall.MaterialLeft, wall.MaterialTopBottom
            }).transform;
        _originalWall.SetParent(transform);
        _collider = gameObject.AddComponent<MeshCollider>();
        _collider.sharedMesh = _originalWall.gameObject.GetComponent<MeshFilter>().mesh;
        _collider.convex = true;
        _currentView = _originalWall.gameObject;
        SetupObjects();

        transform.localPosition = wall.Position;
        transform.Rotate(0, wall.RotationY, 0);
    }

    private void SetupObjects()
    {
        foreach (var objectModel in _wall.Objects)
        {
            //TODO : Надо сделать пул для объектов и дыр.
            if (objectModel.HasView())
            {
                GameObject objectView = Instantiate(objectModel.GetObject(), transform);
                objectView.transform.localPosition = objectModel.GetPosition();
                objectView.transform.localRotation = new Quaternion();
                _allObjects.Add(objectView);
            }

            GameObject hole = Instantiate(objectModel.GetHole(_wall.MaterialFront), transform);
            hole.transform.localPosition = objectModel.GetPosition();
            hole.transform.localRotation = new Quaternion();
            MakeHole(hole, _currentView);
            Destroy(hole);
        }
    }

    private void MakeHole(GameObject hole, GameObject target)
    {
        CSG_Model result = Boolean.Subtract(target, hole);
        if (_currentView != _originalWall.gameObject)
        {
            Destroy(_currentView);
        }

        _currentView = new GameObject();
        _currentView.AddComponent<MeshFilter>().sharedMesh = result.mesh;
        _currentView.AddComponent<MeshRenderer>().sharedMaterials = MakeMaterials(result.materials);
        _currentView.transform.SetParent(transform);
        hole.SetActive(false);
        target.SetActive(false);
    }

    private Material[] MakeMaterials(List<Material> resultMaterials)
    {
        Material[] result = new Material[resultMaterials.Count];
        for (int i = 0; i < resultMaterials.Count; i++)
        {
            result[i] = resultMaterials[i];
        }

        return result;
    }

    private void OnChange()
    {
        ClearObjects();
        SetupObjects();
    }

    private void ClearObjects()
    {
        foreach (var objectView in _allObjects)
        {
            Destroy(objectView);
        }

        _allObjects.Clear();
        if (_currentView != _originalWall.gameObject)
        {
            Destroy(_currentView);
            _currentView = _originalWall.gameObject;
        }

        _currentView.SetActive(true);
    }

    private void Update()
    {
        if (StreetViewCamera.IsPointerOverUIObject())
        {
            return;
        }

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && StreetViewCamera.GetMouseButtonUp(0))
        {
            if (hit.transform == transform || hit.transform.parent == transform)
            {
                _editor.SelectWall(_wall);
                var transformPosition = transform.position;
                var eulerAngles = transform.rotation.eulerAngles;
                eulerAngles.x += 20;
                var angle = eulerAngles.y * Mathf.Deg2Rad;
                transformPosition.x += Mathf.Cos(angle) * _wall.Size.x;
                transformPosition.z -= Mathf.Sin(angle) * _wall.Size.x;
                angle -= Mathf.PI / 2;
                transformPosition.x -= Mathf.Cos(angle) * 6.5f;
                transformPosition.z += Mathf.Sin(angle) * 6.5f;
                StreetViewCamera.MoveAndRotate(transformPosition, eulerAngles);
            }
        }
    }

    private void OnDestroy()
    {
        _wall = null;
    }
}