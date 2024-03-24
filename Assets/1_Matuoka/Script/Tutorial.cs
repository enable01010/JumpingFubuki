using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    #region Fields

    private bool isOneTime = false;
    [SerializeField] private float distance = 2.0f;
    [SerializeField] private GameObject tutorialCanvasPrefab;
    private GameObject tutorialCanvas;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        //tutorialCanvas = Instantiate(tutorialCanvasPrefab);
        tutorialCanvas = tutorialCanvasPrefab;
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
                //Debug.Log("チュートリアルだよ！！");

                tutorialCanvas.SetActive(true);

                isOneTime = true;

                TimeManager.instance.CountStop();
            }
        }
    }

    #endregion


    #region CustomMethod

    public void CanvasActiveFalse ()
    {
        tutorialCanvas.SetActive(false);

        TimeManager.instance.CountStart();
    }

    #endregion
}
