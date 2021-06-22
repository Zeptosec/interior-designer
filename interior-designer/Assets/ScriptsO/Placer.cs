using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public static Placer instance;
    public float GridSpacing = .25f;
    public Transform ObjectsPlace;
    public Material CloneMat;
    private Transform cam;
    private GameObject clone;
    private bool spawnNew = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Debug.LogError("There can only be one placer.");
    }

    void Start()
    {
        cam = Camera.main.transform;
        Selection.instance.OnSelectChange += (go) =>
        {
            if (go == null)
            {
                if(spawnNew)
                    Destroy(clone);
                clone = null;
            }
            else
            {
                clone = Instantiate(go, ObjectsPlace.position, Quaternion.identity, ObjectsPlace);
                clone.GetComponentInChildren<MeshRenderer>().material = CloneMat;
                clone.transform.GetChild(0).gameObject.layer = 2;
                clone.SetActive(false);
                spawnNew = true;
            }
        };

    }

    public void Relocating(PlacableObject po)
    {
        clone = po.gameObject;
        clone.transform.GetChild(0).gameObject.layer = 2;
        clone.GetComponentInChildren<MeshRenderer>().material = CloneMat;
        spawnNew = false;
    }

    void Update()
    {
        if (clone == null)
            return;
        TryShowClone();

        if (Input.GetMouseButtonDown(0))
        {
            TryToPlace();
        }
    }

    public bool TryToPlace()
    {
        PlacableObject placable = clone.GetComponent<PlacableObject>();
        if (placable != null)
        {
            if (placable.IsPlacable && clone.activeSelf)
            {
                if (spawnNew)
                    Instantiate(Selection.instance.Selected, clone.transform.position, clone.transform.rotation, ObjectsPlace);
                else
                {
                    clone = null;
                    ObjectEditor.instance.RestoreMaterial();
                }
                return true;
            }
        }
        else
        {
            Debug.LogError("Placable object must have PlacableObject script.");
        }
        return false;
    }

    public void TryShowClone()
    {
        if(Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 10) && hit.collider.CompareTag("Placable"))
        {
            clone.SetActive(true);
            Vector3 pos;
            if (Input.GetKey(KeyCode.LeftControl))
                pos = GridPosition(hit.point);
            else pos = hit.point;
            clone.transform.position = pos;
            ShowCloneColor();
        }
    }

    private Vector3 GridPosition(Vector3 pos) => new Vector3(
            Mathf.RoundToInt(pos.x / GridSpacing) * GridSpacing,
            pos.y,
            Mathf.RoundToInt(pos.z / GridSpacing) * GridSpacing);

    private void ShowCloneColor()
    {
        PlacableObject placable = clone.GetComponent<PlacableObject>();
        if (placable != null)
        {
            if (placable.IsPlacable)
                placable.GetComponentInChildren<MeshRenderer>().material.color = new Color(0f, 1f, 0f, .5f);
            else placable.GetComponentInChildren<MeshRenderer>().material.color = new Color(1f, 0f, 0f, .5f);
        }
        else
        {
            Debug.LogError("Placable object must have PlacableObject script.");
        }
    }
}
