using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Vector3 startDistance = new Vector3(0,-1,10);
    Vector3 camStartPos;

    //カメラの移動
    [SerializeField] float moveTime = 5f;
    [SerializeField] float min_Y = -3;
    [SerializeField] float max_Y = 10;
    [SerializeField] float min_X = -3;
    [SerializeField] float max_X = 3;

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
        else if(Input.GetMouseButtonUp(1))
        {
            camSize = minSize;
        }

        if(nowCamSize != camSize)
        {
            int vector = (int)Mathf.Sign(camSize - nowCamSize);
            nowCamSize += moveScale * vector * Time.deltaTime;
            if(Mathf.Abs(camSize - nowCamSize) < 0.1f)
            {
                nowCamSize = camSize;
            }
            Camera.main.orthographicSize = nowCamSize;
        }
    }

    void FixedUpdate()
    {
        Vector3 goalPos = Character.instance.transform.position - startDistance;
        goalPos.x = (goalPos.x < min_X) ? min_X : goalPos.x;
        goalPos.x = (goalPos.x > max_X) ? max_X : goalPos.x;
        goalPos.y = (goalPos.y < min_Y) ? min_Y : goalPos.y;
        goalPos.y = (goalPos.y > max_Y) ? max_Y : goalPos.y;
        transform.position = Vector3.Lerp(transform.position, goalPos, moveTime * Time.fixedDeltaTime);

        Vector3 camMove = camStartPos - transform.position;
        Vector3 backgroundPos = backgroundStartPos - camMove * parse;
        background.position = backgroundPos;
    }
}
