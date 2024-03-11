using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Vector3 distance;

    void Start()
    {
        distance = Character.instance.transform.position - transform.position;
    }

    void Update()
    {
        transform.position = Character.instance.transform.position - distance;
    }

}
