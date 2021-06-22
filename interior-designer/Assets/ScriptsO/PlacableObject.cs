using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableObject : MonoBehaviour
{
    private List<Transform> obstructions = new List<Transform>();

    public bool IsPlacable
    {
        get 
        {
            List<Transform> tmp = new List<Transform>();
            foreach (var item in obstructions)
            {
                if (item != null)
                    tmp.Add(item);
            }
            obstructions = tmp;
            return obstructions.Count == 0;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        obstructions.Add(other.transform);
    }

    public void OnTriggerExit(Collider other)
    {
        obstructions.Remove(other.transform);
    }
}
