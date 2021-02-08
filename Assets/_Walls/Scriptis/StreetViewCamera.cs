using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StreetViewCamera : MonoBehaviour
{
    private static bool _target;
    private static Vector3 _targetPosition;
    private static Vector3 _targetRotation;
    private static float _startMoveToTarget;
    private static Vector2 _moveDelta;
    
    public float MoveToTargetDelay = 1f;
    public float Speed = 100f;
    public float SpeedPan = 50f;
    public Vector4 Range = new Vector4(-10f, -10f, 20f, 20f);
    private bool _start;
    private Vector2 _startMovePosition;
    private bool _pan;
    private Vector3 _targetStartPosition;
    private Quaternion _targetStartRotation;

    private void Awake()
    {
        _targetPosition = transform.position;
        _targetRotation = transform.rotation.eulerAngles;
        _targetStartPosition = transform.localPosition;
        _targetStartRotation = transform.rotation;
    }

    private void Update()
    {
        if (_target)
        {
            var moveTime = Time.unscaledTime - _startMoveToTarget;
            if (moveTime < MoveToTargetDelay)
            {
                var percent = moveTime / MoveToTargetDelay;
                transform.position = Vector3.Slerp(_targetStartPosition, _targetPosition, percent);
                transform.rotation = Quaternion.Slerp(_targetStartRotation, Quaternion.Euler(_targetRotation), percent);
                return;
            }

            transform.position = _targetPosition;
            transform.rotation = Quaternion.Euler(_targetRotation);
            _targetStartPosition = transform.localPosition;
            _targetStartRotation = transform.rotation;
            _target = false;
        }
        else
        {
            if (TouchScreenKeyboard.visible && (Vector3.Distance(transform.position, _targetStartPosition) > 1f ||
                                                Vector3.Distance(transform.rotation.eulerAngles, _targetRotation) > 1f))
            {
                MoveAndRotate(_targetStartPosition, _targetRotation);
            }
        }

        if (IsPointerOverUIObject())
        {
            _start = false;
            _pan = false;
            return;
        }

        if (Input.touchCount > 1 || Input.GetMouseButton(2))
            CalculateMouseDelta(Pan);
        else if (!_pan && Input.GetMouseButton(0))
            CalculateMouseDelta(Rotate);
        else
        {
            _start = false;
            _pan = false;
        }
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return _target || EventSystem.current.IsPointerOverGameObject() || results.Count > 0;
    }

    private void CalculateMouseDelta(Action<Vector2> action)
    {
        if (_start)
        {
            var mousePosition = GetMousePosition() - _startMovePosition;
            _moveDelta += mousePosition * 10f;
            action(mousePosition);
            _startMovePosition = GetMousePosition();
        }
        else
        {
            _start = true;
            _moveDelta = new Vector2();
            _startMovePosition = GetMousePosition();
        }
    }

    private void Rotate(Vector2 mousePosition)
    {
        transform.Rotate(new Vector3(mousePosition.y * Speed, -mousePosition.x * Speed, 0));
        float clampX = transform.rotation.eulerAngles.x;
        if (clampX < 180)
            clampX = Mathf.Min(clampX, 50f);
        else
            clampX = Mathf.Max(clampX, 316f);
        transform.rotation = Quaternion.Euler(clampX, transform.rotation.eulerAngles.y, 0);
        _targetStartRotation = transform.rotation;
    }

    private void Pan(Vector2 mousePosition)
    {
        _pan = true;
        var pos = transform.localPosition;
        var angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        pos.x -= Mathf.Sin(angle) * mousePosition.y * SpeedPan;
        pos.z -= Mathf.Cos(angle) * mousePosition.y * SpeedPan;
        pos.x -= Mathf.Cos(angle) * mousePosition.x * SpeedPan;
        pos.z += Mathf.Sin(angle) * mousePosition.x * SpeedPan;
        pos.x = Mathf.Clamp(pos.x, Range.x, Range.x + Range.z);
        pos.z = Mathf.Clamp(pos.z, Range.y, Range.y + Range.w);
        transform.localPosition = pos;
        _targetStartPosition = transform.localPosition;
    }

    private Vector2 GetMousePosition()
    {
// #if UNITY_ANDROID
        if (Input.touchCount > 0)
            return Input.touches[0].position / Screen.width;
// #endif
        return Input.mousePosition / Screen.width;
    }

    public static void MoveAndRotate(Vector3 position, Vector3 rotation)
    {
        _target = true;
        _targetPosition.x = position.x;
        _targetPosition.z = position.z;
        _targetRotation.y = rotation.y;
        _targetRotation.x = rotation.x;
        _startMoveToTarget = Time.unscaledTime;
    }

    public static bool GetMouseButtonUp(int buttonIndex)
    {
        if (Input.GetMouseButtonUp(buttonIndex))
        {
            return _moveDelta.magnitude < 0.1f;
        }

        return false;
    }
}