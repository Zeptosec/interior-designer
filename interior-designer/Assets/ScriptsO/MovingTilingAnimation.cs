using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTilingAnimation : MonoBehaviour
{
    public float Speed;

    private Material mat;
    private Vector2 offs;
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        offs = new Vector2(Random.Range(0, 1), Random.Range(0, 1));
    }

    void Update()
    {
        offs.x += Time.deltaTime * Speed;
        mat.mainTextureOffset = offs;
    }
}
