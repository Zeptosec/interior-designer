using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public GameObject TheObject;

    public void OnClick()
    {
        Selection.instance.Select(this);
    }
}
