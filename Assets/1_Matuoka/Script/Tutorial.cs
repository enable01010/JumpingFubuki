using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    #region Fields

    private bool isOneTime = false;
    [SerializeField] float distance = 2.0f;
    [SerializeField] GameObject tutorialCanvas;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (isOneTime == false)
        {
            if (Vector2.Distance(Character.instance.transform.position, this.transform.position) < distance)
            {
                Debug.Log("チュートリアルだよ！！");

                tutorialCanvas.SetActive(true);

                isOneTime = true;
            }
        }
    }

    #endregion


    #region CustomMethod

    private void CustomMethod()
    {

    }

    #endregion
}
