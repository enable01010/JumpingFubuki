using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShadow : MonoBehaviour
{
    Vector3 distance;
    [SerializeField] Transform shadow;
    void Start()
    {
        distance = shadow.position - transform.position;
    }

    void Update()
    {
        transform.position = shadow.position - distance;
    }
}
