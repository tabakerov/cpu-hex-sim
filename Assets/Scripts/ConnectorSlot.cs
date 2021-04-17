using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ConnectorSlot : MonoBehaviour
{
    public ConnectorSlotType type;
    public bool connected = false;
    public List<ConnectorSlot> innerSlots; // this hex's other slots of same type 
    public ConnectorSlot outerSlot; // connected slot from the other hex
    public MeshRenderer meshRenderer;
    public float inactiveIntensity;
    public float activeIntensity;
    public bool stillCollidingWithSameType;
    
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.EnableKeyword("_EMISSIVE_COLOR_MAP");
        meshRenderer.material.SetFloat("_EmissiveIntensity", inactiveIntensity);
        UpdateEmissiveColorFromIntensityAndEmissiveColorLDR(meshRenderer.material);

    }

    void SetConnected(bool _connected)
    {
        connected = _connected;
        if (connected)
        {
            meshRenderer.material.SetFloat("_EmissiveIntensity", activeIntensity);
            UpdateEmissiveColorFromIntensityAndEmissiveColorLDR(meshRenderer.material);
        }
        else
        {
            if (!stillCollidingWithSameType)
            {
                meshRenderer.material.SetFloat("_EmissiveIntensity", inactiveIntensity);
                UpdateEmissiveColorFromIntensityAndEmissiveColorLDR(meshRenderer.material);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ConnectorSlot otherSlot;
        Debug.Log("some collision");
        if (other.TryGetComponent(out otherSlot))
        {
            Debug.Log($"collided with other slot");
            if (otherSlot.type == type)
            {
                Debug.Log($"SAME TYPE!");
                stillCollidingWithSameType = true;
                outerSlot = otherSlot;
                SetConnected(true);
                foreach (var innerSlot in innerSlots)
                {
                    innerSlot.SetConnected(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ConnectorSlot otherSlot;
        if (other.TryGetComponent(out otherSlot))
        {
            Debug.Log($"collided with other slot");
            if (otherSlot.type == type)
            {
                Debug.Log("uncollided");
                stillCollidingWithSameType = false;
                outerSlot = null;
                SetConnected(false);
                foreach (var innerSlot in innerSlots)
                {
                    innerSlot.SetConnected(false);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static void UpdateEmissiveColorFromIntensityAndEmissiveColorLDR(Material material)
    {
        const string kEmissiveColorLDR = "_EmissiveColorLDR";
        const string kEmissiveColor = "_EmissiveColor";
        const string kEmissiveIntensity = "_EmissiveIntensity";
 
        if (material.HasProperty(kEmissiveColorLDR) && material.HasProperty(kEmissiveIntensity) && material.HasProperty(kEmissiveColor))
        {
            // Important: The color picker for kEmissiveColorLDR is LDR and in sRGB color space but Unity don't perform any color space conversion in the color
            // picker BUT only when sending the color data to the shader... So as we are doing our own calculation here in C#, we must do the conversion ourselves.
            Color emissiveColorLDR = material.GetColor(kEmissiveColorLDR);
            Color emissiveColorLDRLinear = new Color(Mathf.GammaToLinearSpace(emissiveColorLDR.r), Mathf.GammaToLinearSpace(emissiveColorLDR.g), Mathf.GammaToLinearSpace(emissiveColorLDR.b));
            material.SetColor(kEmissiveColor, emissiveColorLDRLinear * material.GetFloat(kEmissiveIntensity));
        }
    }
}
