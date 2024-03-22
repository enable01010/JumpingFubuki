using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    #region Fields

    private bool isOneTime = false;

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
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOneTime == false)
        {
            if (collision.gameObject.GetComponent<Character>() != null)
            {
                Debug.Log("チュートリアルだよ！！");

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
