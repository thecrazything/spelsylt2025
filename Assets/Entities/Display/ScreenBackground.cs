using System;
using UnityEngine;

public class ScreenBackground : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void SetIsOn(bool v)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.SetFloat("_GridEnabled", v ? 1f : 0f);
        }
    }

}
