using UnityEngine;

public class App : MonoBehaviour
{
    public ApartmentConfig ApartmentConfig;
    private ApartmentModel _apartmentModel;
    public ApartmentView ApartmentView;
    public WallEditor WallEditor;

    void Start()
    {
        _apartmentModel = new ApartmentModel(ApartmentConfig);
        ApartmentView.Init(_apartmentModel, WallEditor);
        WallEditor.Init(ApartmentConfig);
    }
}