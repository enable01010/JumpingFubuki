using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MathTTest : MonoBehaviour
{
    float min = 10;
    float max = 30;
    float value = 50;
    [SerializeField] Text _text;
    
    void Start()
    {
        //_text.text = MathT.CastLimit(value,min,max).ToString();
        _text.text = MathT.getRangeToValue(value, min, max).ToString();
    }


}
