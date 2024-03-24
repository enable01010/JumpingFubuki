using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class M_Clear : MonoBehaviour
{
    #region Fields

    private bool isOneTime = false;
    [SerializeField] private float distance = 2.0f;
    [SerializeField] private GameObject clearCanvasPrefab;
    private GameObject clearCanvas;
    [SerializeField] TextMeshProUGUI textMeshPro;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        //clearCanvas = Instantiate(clearCanvasPrefab);
        clearCanvas = clearCanvasPrefab;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (isOneTime == false)
        {
            if (Vector2.Distance(Player.instance.transform.position, this.transform.position) < distance)
            {
                //Debug.Log("クリアだよ！！");

                TimeManager.instance.CountStop();

                TimeManager.instance.TextActiveFalse();

                textMeshPro.text += "\n" + TimeManager.instance.TextChange();

                clearCanvas.SetActive(true);

                isOneTime = true;
            }
        }
    }

    #endregion


    #region CustomMethod

    public void TitleLoad()
    {
        SceneAnimation.instance.LoadScene(0);
    }

    #endregion
}