using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class Hex : MonoBehaviour
{
    public List<GameObject> slotPrefabs;
    public List<Transform> slotPlaces;
    public List<ConnectorSlot> slots;
    private Vector2Control _previousMousePosition;
    public bool grabbed;
    public float moveSensitivity;
    public InputAction click;

    
    // Start is called before the first frame update
    void Start()
    {
        slots = new List<ConnectorSlot>(6);
        SpawnSlots();
        ConnectInnerSlots();
    }

    void SpawnSlots()
    {
        foreach (var slotPlace in slotPlaces)
        {
            var gameObject = Instantiate(slotPrefabs[Random.Range(0, slotPrefabs.Count)], slotPlace);
            slots.Add(gameObject.GetComponent<ConnectorSlot>());
        }
    }

   

    void ConnectInnerSlots()
    {
        foreach (var slot in slots)
        {
            foreach (var otherSlot in slots)
            {
                if (otherSlot == slot) continue;
                if (slot.type == otherSlot.type)
                {
                    slot.innerSlots.Add(otherSlot);
                }
            }    
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (grabbed)
        {
            var delta = moveSensitivity * Mouse.current.delta.ReadValue();
            transform.Translate(delta.x, 0f, delta.y);
        }
        
        
        
    }
}
