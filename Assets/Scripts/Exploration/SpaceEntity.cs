using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceEntity : MonoBehaviour
{
    Canvas infoPanel;

    void Awake()
    {
        infoPanel = GetComponentInChildren<Canvas>();
    }

    void Start()
    {
        HideInfo();
    }
    
    void Update()
    {
        
    }

    void ShowInfo()
    {
        infoPanel.gameObject.SetActive(true);
    }

    void HideInfo()
    {
        infoPanel.gameObject.SetActive(false);
    }

    void OnMouseEnter()
    {
        ShowInfo();
    }

    void OnMouseExit()
    {
        HideInfo();
    }
}