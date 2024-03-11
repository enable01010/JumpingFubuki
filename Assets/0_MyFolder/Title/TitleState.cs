using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleState : MonoBehaviour
{
    A_TitleState state;
    [SerializeField] StartState start;
    [SerializeField] SelectState select;

    [SerializeField] Animator _anim;

    void Start()
    {
        start.OnInit(this);
        select.OnInit(this);
        ChangeState(start);
    }

    
    void Update()
    {
        state.OnUpdate();
    }

    private void ChangeState(A_TitleState next)
    {
        state?.OnExit();
        state = next;
        state.OnEnter();
    }



    [System.Serializable]
    public class A_TitleState : I_State
    {
        protected TitleState title;
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnExit() { }

        public virtual void OnInit(TitleState title)
        {
            this.title = title;
        }
    }

    [System.Serializable]
    public class StartState : A_TitleState
    {
        public override void OnUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                title.ChangeState(title.select);
            }
        }
    }

    [System.Serializable]
    public class SelectState : A_TitleState
    {
        public override void OnEnter()
        {
            title._anim.SetTrigger("isClick");
        }
    }
}
