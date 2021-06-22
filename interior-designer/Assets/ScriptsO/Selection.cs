using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public static Selection instance;
    public GameObject Selected;
    public GameObject SelectionPanel;
    public delegate void Change(bool state);
    public event Change OnPanelToggle;
    public delegate void SelectChange(GameObject go);
    public event SelectChange OnSelectChange;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Debug.LogError("There can only be one instance of Selection");
    }

    public void Show(bool state)
    {
        SelectionPanel.SetActive(state);
        OnPanelToggle?.Invoke(state);
    }

    public void Toggle()
    {
        Show(!SelectionPanel.activeSelf);
    }

    public void Select(Selectable selected)
    {
        if (selected == null)
        {
            OnSelectChange?.Invoke(null);
            Selected = null;
        }
        else
        {
            Selected = selected.TheObject;
            OnSelectChange?.Invoke(selected.TheObject);
        }
        Show(false);
    }
    
    public void SelectRelocate(PlacableObject selected)
    {

    }
}
