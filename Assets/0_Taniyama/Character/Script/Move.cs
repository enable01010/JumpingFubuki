using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Singleton<Character>
{
    [System.Serializable]
    public class MoveState : A_PlayerState
    {
        float downforce;
        float moveSpeed_Y;
        float moveSpeed_X;
        Transform _hoppingFrontPos;

        [SerializeField] float headLine = 4.0f;
        [SerializeField] float headWidth = 0.5f;
        [SerializeField] float headHeight = 2.5f;
        [SerializeField] int wallCheckFrame = 10;

        public override void OnEnter()
        {
            this.downforce = character.downforce;
            this.moveSpeed_Y = character.moveSpeed_Y;
            this.moveSpeed_X = character.moveSpeed_X;
            this._hoppingFrontPos = character._hoppingFrontPos;
        }

        public override void OnFixedUpdate()
        {
            //ë¨ìxí≤êÆèàóù
            Vector3 moveDir = character.moveDir;
            moveDir.x *= moveSpeed_X;
            moveDir.y *= moveSpeed_Y;
            moveDir *= Time.fixedDeltaTime;

            //â∫Ç†ÇΩÇËîªíËèàóù
            RaycastHit2D hit = Physics2D.Raycast(_hoppingFrontPos.position, moveDir, moveDir.magnitude);
            if (hit)
            {
                Vector3 hitPos = hit.point;
                Vector3 dir = hitPos - _hoppingFrontPos.position;
                Vector3 goalPos = character.transform.position + dir;
                goalPos.y = hit.point.y + 0.2f;
                character.transform.position = goalPos;

                LinecastVec vec = character.CheckLinecastVec(hit.point);
                switch (vec)
                {
                    case LinecastVec.horizontal:
                        character.ChangeState(character.idle);
                        Instantiate(character.landingFx, _hoppingFrontPos.position, Quaternion.identity);
                        return;

                    case LinecastVec.vertical:
                        character.moveDir = Vector3.zero;
                        character.ChangeState(character.fall);
                        return;
                }
            }

            //è„Ç†ÇΩÇËîªíËèàóù
            Vector3 headLeftUpPos = character.transform.position;
            headLeftUpPos.y += headLine;
            Vector3 headFrontUpPos = character.transform.position;
            headFrontUpPos.y += headLine;
            headFrontUpPos.x += headWidth * Mathf.Sign(character.moveDir.x);
            RaycastHit2D hitTop = Physics2D.Raycast(headLeftUpPos, Vector3.right * Mathf.Sign(character.moveDir.x), headWidth);
            RaycastHit2D hitFront = Physics2D.Raycast(headFrontUpPos, Vector3.down, headHeight);
            if (hitTop || hitFront)
            {
                character.ChangeState(character.fall);
                character.moveDir = Vector3.zero;
                return;
            }

            //è´óàÇÃï«îªíËèàóù
            Vector3 nextPos = _hoppingFrontPos.position;
            Vector3 nextDir = character.moveDir;
            bool isEnd = false;
            for (int i = 0; i < wallCheckFrame && isEnd == false; i++)
            {
                Vector3 nowDir = nextDir;
                nowDir.x *= moveSpeed_X;
                nowDir.y *= moveSpeed_Y;
                nowDir *= Time.fixedDeltaTime;
                RaycastHit2D wallHit = Physics2D.Raycast(nextPos, nowDir, nowDir.magnitude);
                if (wallHit)
                {

                    LinecastVec vec = character.CheckLinecastVec(wallHit.point);
                    switch (vec)
                    {
                        case LinecastVec.horizontal:
                            isEnd = true;
                            break;
                        case LinecastVec.vertical:
                            character.wallLanding.SetCount(i);
                            character.wallLanding.SetGoalPos(wallHit.point);
                            character.ChangeState(character.wallLanding);
                            return;
                    }
                }

                nextPos += nowDir;
                nextDir.y -= downforce * Time.fixedDeltaTime;
            }

            //à⁄ìÆèàóù
            character.transform.position += moveDir;
            character.moveDir.y -= downforce * Time.fixedDeltaTime;

        }
    }

}
