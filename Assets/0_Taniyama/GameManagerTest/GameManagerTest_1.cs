using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTest_1 : A_GameManager<GameManagerTest_1>
{
    [SerializeField] DefaultState defaultState;
    [SerializeField] ChutorialState chutorialState;

    public override void OnInitialize()
    {
        base.OnInitialize();

        defaultState.OnInit(this);
        chutorialState.OnInit(this);
        ChangeState(defaultState);
    }

    //ローディング中のステート
    //スタート
    //プレイヤーが普通に動けるステート
    //チュートリアルが表示されてる状態のステート
    //プレイヤーが普通に動けるステート
    //ゴール画面が表示されてる状態のステート
    //ポーズ状態のステート

    [System.Serializable]
    public class ChutorialState :A_GameManagerState
    {
        [SerializeField] Animator _anim;

        bool isClickDown = false;
        public override void OnEnter()
        {
            base.OnEnter();

            _anim.SetBool("isChutorial", true);
        }

        public override void OnExit()
        {
            base.OnExit();

            _anim.SetBool("isChutorial", false);
        }

        public override void GetMouseButtonDownLeft()
        {
            isClickDown = true;
        }

        public override void GetMouseButtonUpLeft()
        {
            if(isClickDown == true)
            {
                manager.ChangeState(manager.defaultState);
            }
        }

        
    }
}
