using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_CameraMove : MonoBehaviour
{
    [SerializeField] Vector3 startDistance = new Vector3(0, -1, 10);
    Vector3 camStartPos;

    //カメラの移動
    [SerializeField] float moveTime = 5f;
    [SerializeField] float min_Y = -3;
    [SerializeField] float max_Y = 150;
    [SerializeField] float min_X = -12;
    [SerializeField] float max_X = 15;
    [SerializeField] float goalDistance = 1.0f;

    //バックグラウンド
    Vector3 backgroundStartPos;
    [SerializeField] Transform background;
    [SerializeField] float parse = 0.9f;
    
    //背景サイズ
    [SerializeField] float minSize = 6;
    [SerializeField] float maxSize = 10;
    [SerializeField] float moveScale = 4f;
    float camSize;
    float nowCamSize;


    void Start()
    {
        camStartPos = transform.position;
        backgroundStartPos = background.position;
        camSize = minSize;
        nowCamSize = minSize;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            camSize = maxSize;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            camSize = minSize;
        }

        if (nowCamSize != camSize)
        {
            int vector = (int)Mathf.Sign(camSize - nowCamSize);
            nowCamSize += moveScale * vector * Time.deltaTime;
            if (Mathf.Abs(camSize - nowCamSize) < 0.1f)
            {
                nowCamSize = camSize;
            }
            Camera.main.orthographicSize = nowCamSize;
        }
    }

    void FixedUpdate()
    {
        Vector3 goalPos = Character.instance.transform.position - startDistance;
        goalPos.x = Mathf.Clamp(goalPos.x, min_X, max_X);
        goalPos.y = Mathf.Clamp(goalPos.y, min_Y, max_Y);

        if(goalDistance < Vector3.Magnitude(transform.position - goalPos))
        {
            transform.position = Vector3.Lerp(transform.position, goalPos, moveTime * Time.fixedDeltaTime);

            Vector3 camMove = camStartPos - transform.position;
            Vector3 backgroundPos = backgroundStartPos - camMove * parse;
            background.position = backgroundPos;
        }

        //if (Character.instance.state.GetType() == typeof(Character.IdleSliderState))
        //{

        //}
    }
}
