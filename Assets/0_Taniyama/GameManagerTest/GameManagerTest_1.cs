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

    //���[�f�B���O���̃X�e�[�g
    //�X�^�[�g
    //�v���C���[�����ʂɓ�����X�e�[�g
    //�`���[�g���A�����\������Ă��Ԃ̃X�e�[�g
    //�v���C���[�����ʂɓ�����X�e�[�g
    //�S�[����ʂ��\������Ă��Ԃ̃X�e�[�g
    //�|�[�Y��Ԃ̃X�e�[�g

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
