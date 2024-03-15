using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject cameraObject;
    private Camera _camera;

    // [0]...- [1]...’†‰› [2]...+
    private GameObject[] background = new GameObject[3];
    private Vector3 backgroundSize;
    private Vector3 backgroundSize_Y;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        _camera = cameraObject.GetComponent<Camera>();

        background[0] = this.transform.GetChild(0).gameObject;
        Vector3 backgroundStartPos = background[0].transform.position;
        backgroundSize = background[0].GetComponent<SpriteRenderer>().bounds.size;

        background[1] = Instantiate(background[0], backgroundStartPos, Quaternion.identity, this.transform);
        background[2] = Instantiate(background[0], backgroundStartPos, Quaternion.identity, this.transform);

        backgroundSize_Y = new Vector3(0.0f, backgroundSize.y, 0.0f);
        background[0].transform.position -= backgroundSize_Y;
        background[2].transform.position += backgroundSize_Y;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (background[1].transform.position.y + backgroundSize.y / 2 < cameraObject.transform.position.y - _camera.orthographicSize)
        {
            for (int i = 0; i < background.Length; i++)
            {
                background[i].transform.position += backgroundSize_Y;
            }
        }
        else if (background[1].transform.position.y - backgroundSize.y / 2 > cameraObject.transform.position.y + _camera.orthographicSize)
        {
            for (int i = 0; i < background.Length; i++)
            {
                background[i].transform.position -= backgroundSize_Y;
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