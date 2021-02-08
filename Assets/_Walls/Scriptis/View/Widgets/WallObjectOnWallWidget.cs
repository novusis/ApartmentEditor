using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WallObjectOnWallWidget : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TMP_InputField X;
    public TMP_InputField Y;
    public Button CloseButton;
    private ObjectModel _objectModel;
    private Action<ObjectModel> _onRemoveObject;
    private bool _init;
    private bool _edit;

    public void Init(ObjectModel objectModel, Action<ObjectModel> onRemoveObject)
    {
        _objectModel = objectModel;
        _objectModel.ChangePosition += OnChangePosition;
        _onRemoveObject = onRemoveObject;
        Title.text = objectModel.GetName();
        X.onEndEdit.AddListener(OnEndEdit);
        X.onValueChanged.AddListener(OnChangeValue);
        Y.onEndEdit.AddListener(OnEndEdit);
        Y.onValueChanged.AddListener(OnChangeValue);
        Y.gameObject.SetActive(!objectModel.IsDoor());
        CloseButton.onClick.AddListener(OnClick);
        ShowPosition();
    }

    private void OnChangePosition()
    {
        if (!_edit)
        {
            ShowPosition();
        }
    }

    private void OnDestroy()
    {
        X.onEndEdit.RemoveListener(OnEndEdit);
        X.onValueChanged.RemoveListener(OnChangeValue);
        Y.onEndEdit.RemoveListener(OnEndEdit);
        Y.onValueChanged.RemoveListener(OnChangeValue);
        CloseButton.onClick.RemoveListener(OnClick);
    }

    private void OnEndEdit(string value)
    {
        _edit = false;
        ShowPosition();
    }

    private void OnChangeValue(string value)
    {
        if (_init)
        {
            _edit = true;
            var x = _objectModel.GetWallPosition().x;
            try
            {
                x = float.Parse(X.text) / 10f;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            var y = _objectModel.GetWallPosition().y;
            try
            {
                y = float.Parse(Y.text) / 10f;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            _objectModel.SetWallPosition(x, y);
        }
    }

    private void ShowPosition()
    {
        X.text = (_objectModel.GetWallPosition().x * 10).ToString();
        Y.text = (_objectModel.GetWallPosition().y * 10).ToString();
        _init = true;
    }

    private void OnClick()
    {
        _onRemoveObject(_objectModel);
    }
}