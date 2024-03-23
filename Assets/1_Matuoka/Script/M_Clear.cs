using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Clear : MonoBehaviour
{
    #region Fields

    private bool isOneTime = false;
    [SerializeField] float distance = 2.0f;

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
                Debug.Log("クリアだよ！！");

                SceneAnimation.instance.LoadScene(0);

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