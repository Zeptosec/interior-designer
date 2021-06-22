using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relocator : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Selection.instance.Selected == null)
        {
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 10))
            {
                PlacableObject po = null;
                if (hit.transform.parent != null)
                    po = hit.transform.parent.GetComponent<PlacableObject>();
                if(po != null)
                {

                }
            }
        } 
    }
}
