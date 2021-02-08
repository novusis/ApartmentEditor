using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WallObjectWidget : MonoBehaviour
{
    public TextMeshProUGUI Label;
    public Button Button;
    private ObjectConfig _configObject;
    private Action<ObjectConfig> _onSelect;

    public void Init(ObjectConfig configObject, Action<ObjectConfig> onSelect)
    {
        _configObject = configObject;
        _onSelect = onSelect;
        Label.text = configObject.Name;
        Button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        _onSelect(_configObject);
    }
}