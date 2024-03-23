using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingFxDestroy : MonoBehaviour
{
    #region Fields

    

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        //Destroy(gameObject, 1.0f);
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    #endregion


    #region CustomMethod

    private void FxDestroy()
    {
        Destroy(this.gameObject);
    }

    #endregion
}
