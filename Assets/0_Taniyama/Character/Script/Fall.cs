using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Singleton<Character>
{
    [System.Serializable]
    public class Fall : A_PlayerState
    {
        [SerializeField, Range(0, 1)] float slow = 0.5f;
        float downforce;
        float moveSpeed_Y;
        float moveSpeed_X;
        Transform _hoppingFrontPos;

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

            //Ç†ÇΩÇËîªíËèàóù
            RaycastHit2D hit = Physics2D.Linecast(_hoppingFrontPos.position, _hoppingFrontPos.position + moveDir);
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
                    case LinecastVec.other:
                        character.ChangeState(character.idle);
                        Instantiate(character.landingFx, _hoppingFrontPos.position, Quaternion.identity);
                        return;
                }
            }

            Move(moveDir);
        }

        /// <summary>
        /// à⁄ìÆèàóù
        /// </summary>
        private void Move(Vector3 moveDir)
        {
            character.transform.position += moveDir;
            character.moveDir.y -= downforce * Time.fixedDeltaTime * slow;
        }
    }

}
