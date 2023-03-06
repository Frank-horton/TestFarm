using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private GameObject _sourceObject;
    [SerializeField] private Joystick _joystick; 

    private IControllabe _controllabe;
    private void Start()
    {
        _controllabe = _sourceObject.GetComponent<IControllabe>(); 
    }
    public void LateUpdate()
    {
        MoveController();
    }
    public void MoveController()
    {
        Vector3 axes = Vector3.zero;
        axes.x = _joystick.Horizontal;
        axes.z = _joystick.Vertical;
        _controllabe.Move(axes);
    }
}
