using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Vector3 distance;

    void Start()
    {
        distance = Player.instance.transform.position - transform.position;
    }

    void Update()
    {
        transform.position = Player.instance.transform.position - distance;
    }

}
