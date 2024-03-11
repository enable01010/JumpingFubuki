using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TapToStartText : MonoBehaviour
{
    Text _text;
    float sita = 0;
    [SerializeField] float rate = 2.0f;
    void Start()
    {
        _text = GetComponent<Text>();    
    }

    // Update is called once per frame
    void Update()
    {
        sita += Time.deltaTime * rate;
        float value = (Mathf.Sin(sita) + 1) / 2.0f;

        Color col = _text.color;
        col.a = value;
        _text.color = col;
    }
}
