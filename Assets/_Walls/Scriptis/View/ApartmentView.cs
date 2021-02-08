using UnityEngine;

public class ApartmentView : MonoBehaviour
{
    private ApartmentModel _model;
    private WallEditor _editor;

    public void Init(ApartmentModel apartmentModel, WallEditor editor)
    {
        _model = apartmentModel;
        _editor = editor;
        _model.Change += OnChange;
        DrawAll();
    }

    private void OnChange()
    {
        ClearAll();
        DrawAll();
    }
    
    private void DrawAll()
    {
        foreach (var wall in _model.Walls)
        {
            WallView wallView = new GameObject("Wall"+_model.Walls.IndexOf(wall)).AddComponent<WallView>();
            wallView.transform.SetParent(transform);
            wallView.Init(wall, _editor);
        }
    }

    private void ClearAll()
    {
        var childCount = transform.childCount;
        while (childCount > 0)
        {
            childCount--;
            Destroy(transform.GetChild(childCount).gameObject);
        }
        transform.DetachChildren();
    }
}