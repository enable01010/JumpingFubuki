using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : Singleton<TimeManager>
{
    #region Fields

    private bool isCount = false;
    public float nowTime{ private set; get; }

    [SerializeField] TextMeshProUGUI textMeshPro;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        nowTime = 0.0f;
        textMeshPro.text = TextChange();
        CountStart();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isCount == true)
        {
            nowTime += Time.fixedDeltaTime;
            textMeshPro.text = TextChange();
        }
    }

    #endregion


    #region CustomMethod

    public void CountStart()
    {
        isCount = true;
    }

    public void CountStop()
    {
        isCount = false;
    }

    public string TextChange()
    {
        return ((int)nowTime / 60).ToString("D2") + ":" + ((int)nowTime % 60).ToString("D2") + "." + (int)(nowTime * 10) % 10;
    }

    public void TextActiveFalse()
    {
        textMeshPro.text = "";
    }

    #endregion
}
