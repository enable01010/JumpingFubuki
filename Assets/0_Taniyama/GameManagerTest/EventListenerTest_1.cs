using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListenerTest_1: A_GameManagerEventListenerMono<GameManagerTest_1>
{
    protected override void Start()
    {
        base.Start();
    }


    public override void GetMouseButtonDownLeft() { Debug.Log("EventListenerTest_1�F���N���b�N�̉����͂��ߒʒm"); }
    public override void GetMouseButtonLeft() { Debug.Log("EventListenerTest_1�F���N���b�N�̉������ݒʒm"); }
    public override void GetMouseButtonUpLeft() { Debug.Log("EventListenerTest_1�F���N���b�N�̉������݂����ʒm"); }
    public override void GetMouseButtonDownRight() { Debug.Log("EventListenerTest_1�F�E�N���b�N�̉����͂��ߒʒm"); }
    public override void GetMouseButtonRight() { Debug.Log("EventListenerTest_1�F�E�N���b�N�̉������ݒʒm"); }
    public override void GetMouseButtonUpRight() { Debug.Log("EventListenerTest_1�F�E�N���b�N�̉������݂����ʒm"); }
}
