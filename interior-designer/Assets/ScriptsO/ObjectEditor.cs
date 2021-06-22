using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEditor : MonoBehaviour
{
    public static ObjectEditor instance;
    public GameObject EditingPanel;
    public delegate void ChangePanel(bool b);
    public event ChangePanel OnPanelChange;
    public PlacableObject Selected;
    private Vector3 initialPosition;
    private Material initialMat;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else Debug.LogError("There can only be one Object editor.");
    }

    private void Start()
    {
        Selection.instance.OnSelectChange += go =>
        {
            if (go == null && Selected != null)
            {
                Selected.gameObject.transform.position = initialPosition;
                RestoreMaterial();
            }
        };
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Selection.instance.Selected == null &&
            Selected == null && !GameManager.instance.IsPaused && !Selection.instance.SelectionPanel.activeSelf)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 10))
            {
                PlacableObject po = hit.transform.GetComponent<PlacableObject>();
                if (po != null)
                {
                    Selected = po;
                    Show(true);
                }
            }
        }
    }

    public bool TryToTurnOffColorPanel()
    {
        if(EditingPanel.transform.GetChild(1).gameObject.activeSelf)
        {
            EditingPanel.transform.GetChild(1).gameObject.SetActive(false);
            EditingPanel.transform.GetChild(0).gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    public void Delete()
    {
        Destroy(Selected.gameObject);
        Show(false);
    }

    public void Relocate()
    {
        initialPosition = Selected.transform.position;
        initialMat = Selected.GetComponentInChildren<MeshRenderer>().material;
        Placer.instance.Relocating(Selected);
        Show(false);
    }

    public void RestoreMaterial()
    {
        Selected.GetComponentInChildren<MeshRenderer>().material = initialMat;
        Selected.transform.GetChild(0).gameObject.layer = 0;
        Selected = null;
    }

    public void ChangeColor(Color color)
    {
        Selected.GetComponentInChildren<MeshRenderer>().material.color = color;
    }

    public void Show(bool b)
    {
        EditingPanel.SetActive(b);
        OnPanelChange?.Invoke(b);
        if (b)
        {
            EditingPanel.transform.GetChild(0).gameObject.SetActive(true);
            for (int i = 1; i < EditingPanel.transform.childCount; i++)
            {
                EditingPanel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void BackToGame()
    {
        Show(false);
        Selected = null;
    }
}
