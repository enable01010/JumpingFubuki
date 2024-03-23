using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public partial class Character : Singleton<Character>
{
    Animator _animator;

    //ステートに関係する処理
    public A_PlayerState state { private set; get; }
    [SerializeField] IdleSliderState idle;// 変更
    [SerializeField] MoveState move;
    [SerializeField] Fall fall;
    [SerializeField] WallLandingMotion wallLanding;

    //ステート間で保持したいデータ
    [SerializeField] float downforce = 2.0f;
    [SerializeField] float moveSpeed_Y = 20.0f;
    [SerializeField] float moveSpeed_X = 16.0f;
    [SerializeField] Transform _hoppingFrontPos;
    [SerializeField,ReadOnly]Vector3 moveDir;
    [SerializeField] GameObject _arrow;
    [SerializeField] SpriteRenderer[] _visions;
    [SerializeField] Slider _slider;// 追加
    [SerializeField] GameObject landingFx;
    [SerializeField] GameObject headingFx;

    //ステート関係無いデータ
    [SerializeField,Range(0,1)] float freePartRait = 0.5f;
    [SerializeField] float freePartRaitMoveSpeed = 0.01f;

    public override void OnInitialize()
    {
        base.OnInitialize();

        _animator = GetComponent<Animator>();

        idle.OnInit(this);
        move.OnInit(this);
        fall.OnInit(this);
        wallLanding.OnInit(this);

        ChangeState(idle);
    }

    // Update is called once per frame
    void Update()
    {
        state.OnUpdate();
    }

    private void FixedUpdate()
    {
        state.OnFixedUpdate();

        ControlFreePart();
    }

    private void ControlFreePart()
    {
        float bestRait = 0.5f;
        bestRait = (moveDir.y > 0.01f) ? 1 : bestRait;
        bestRait = (moveDir.y < -0.01f) ? 0 : bestRait;
        
        if(Mathf.Abs(bestRait - freePartRait) < freePartRaitMoveSpeed)
        {
            freePartRait = bestRait;
        }
        else if(bestRait > freePartRait)
        {
            freePartRait += freePartRaitMoveSpeed;
        }
        else if(bestRait < freePartRait)
        {
            freePartRait -= freePartRaitMoveSpeed;
        }
        _animator.SetFloat("fall", freePartRait);
    }

    /// <summary>
    /// ステートを変更する処理
    /// </summary>
    /// <param name="next"></param>
    private void ChangeState(A_PlayerState next)
    {
        state?.OnExit();
        state = next;
        state.OnEnter();
    }

    [System.Serializable]
    public class A_PlayerState : I_State
    {
        protected Character character;
        public void OnInit(Character character)
        {
            this.character = character;
        }

        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnExit() { }

    }

    
    private LinecastVec CheckLinecastVec(Vector3 point)
    {
        LinecastVec answer = LinecastVec.other;

        //変数の定義
        const float range = 0.2f;
        bool isLeftVertical = false;
        bool isRightVertical = false;
        bool isUpHolizontal = false;
        bool isDownHolizontal = false;

        //頂点の計算
        Vector2 pos_LeftUp = point;
        pos_LeftUp.x -= range;
        pos_LeftUp.y += range;
        Vector2 pos_RightUp = point;
        pos_RightUp.x += range;
        pos_RightUp.y += range;
        Vector2 pos_LeftDown = point;
        pos_LeftDown.x -= range;
        pos_LeftDown.y -= range;
        Vector2 pos_RightDown = point;
        pos_RightDown.x += range;
        pos_RightDown.y -= range;

        //判定のチェック
        if (Physics2D.Linecast(pos_LeftUp, pos_LeftDown))
        {
            isLeftVertical = true;
        }
        if (Physics2D.Linecast(pos_RightUp, pos_RightDown))
        {
            isRightVertical = true;
        }
        if (Physics2D.Linecast(pos_LeftUp, pos_RightUp))
        {
            isUpHolizontal = true;
        }
        if (Physics2D.Linecast(pos_LeftDown, pos_RightDown))
        {
            isDownHolizontal = true;
        }

        //横の場合
        if ((isLeftVertical == true || isRightVertical == true) && 
            (isUpHolizontal == false || isDownHolizontal && false))
        {
            answer = LinecastVec.horizontal;
        }

        //縦の場合
        if ((isUpHolizontal == true || isDownHolizontal == true) &&
            (isLeftVertical == false || isRightVertical == false))
        {
            answer = LinecastVec.vertical;
        }

        return answer;
    }

    public enum LinecastVec
    {
        vertical,
        horizontal,
        other,
    }
}
