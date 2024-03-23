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


    public override void GetMouseButtonDownLeft() { Debug.Log("EventListenerTest_1：左クリックの押しはじめ通知"); }
    public override void GetMouseButtonLeft() { Debug.Log("EventListenerTest_1：左クリックの押し込み通知"); }
    public override void GetMouseButtonUpLeft() { Debug.Log("EventListenerTest_1：左クリックの押し込みおわり通知"); }
    public override void GetMouseButtonDownRight() { Debug.Log("EventListenerTest_1：右クリックの押しはじめ通知"); }
    public override void GetMouseButtonRight() { Debug.Log("EventListenerTest_1：右クリックの押し込み通知"); }
    public override void GetMouseButtonUpRight() { Debug.Log("EventListenerTest_1：右クリックの押し込みおわり通知"); }
}
