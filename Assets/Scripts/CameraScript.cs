using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class CameraScript : MonoBehaviour
{
    public InputActionAsset controls;
    public Camera camera;
    public Hex grabbedHex;
    private void Awake()
    {
    }

    public void HandleClick(InputAction.CallbackContext context)
    {
        if (grabbedHex != null)
        {
            Debug.Log("release hex");
            grabbedHex.grabbed = false;
            grabbedHex = null;
            return;
        }
        Debug.Log(context.action + Mouse.current.position.ReadValue().ToString());
        var p = Mouse.current.position.ReadValue();

        var ray = camera.ScreenPointToRay(new Vector3(p.x, p.y, 0f));
        Debug.Log("Ray ok");
        if (Physics.Raycast(ray, out var hitInfo))
        {
            Debug.Log("raycast ok");
            Hex hex;
            if (hitInfo.collider.gameObject.TryGetComponent(out hex))
            {
                hex.grabbed = true;
                grabbedHex = hex;
            }
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
