using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : Singleton<Player>
{
    [System.Serializable]
    public class WallLandingState : A_PlayerState
    {
        int count;
        Vector3 goalPos;
        float downforce;
        float moveSpeed_Y;
        float moveSpeed_X;
        Transform _hoppingFrontPos;

        //���ԂɊւ��鏈��
        [SerializeField] float endTime;
        [SerializeField] float reveseStartTime = 0.2f;
        [SerializeField] float reverseEndTime = 0.5f;
        [SerializeField, ReadOnly] float nowTime;
        Vector3 startSca;
        Vector3 inputDir;

        //���슴�Ɋւ���ϐ�
        [SerializeField] float clickAreaRenge = 500.0f;

        Vector3 clickPos;
        //���Ɋւ���ϐ�
        [SerializeField] bool isVisualizeArrow = true;
        [SerializeField] float arrowsFirstLot = -38;
        [SerializeField] float arrowSizeMin_X = 0.05f;
        [SerializeField] float arrowSizeMax_X = 0.4f;
        [SerializeField] float arrowSizeMin_Y = 0.05f;
        [SerializeField] float arrowSizeMax_Y = 0.15f;

        //�\�����Ɋւ���ϐ�
        [SerializeField] bool isVisualizeVision = true;
        [SerializeField, Range(0, 15)] int outputCount = 10;
        [SerializeField] int outputPitch = 10;
        [SerializeField] GameObject _arrowShadow;
        SpriteRenderer[] _visions;

        [SerializeField] GameObject characterShadow;
        [SerializeField] SpriteRenderer characterShadowRenderer;

        float slowRate;
        float antiSlowRate = 1;

        public override void OnEnter()
        {
            this._visions = player._visions;
            this.downforce = player.downforce;
            this.moveSpeed_Y = player.moveSpeed_Y;
            this.moveSpeed_X = player.moveSpeed_X;
            this._hoppingFrontPos = player._hoppingFrontPos;

            inputDir = Vector3.zero;
            nowTime = 0;
            startSca = player.transform.localScale;
            antiSlowRate = 1;

            float normalTime = count * Time.fixedDeltaTime;
            slowRate = normalTime / endTime;

            goalPos.x += Mathf.Sign(player.moveDir.x) * -1 * 0.5f;

            characterShadow.SetActive(true);
            Vector3 shadowSca = characterShadow.transform.localScale;
            shadowSca.x = Mathf.Abs(shadowSca.x) * Mathf.Sign(player.transform.localScale.x);
            characterShadow.transform.position = goalPos;

            //���͎󂯎�菈��
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                TouchStart();
            }
        }

        public override void OnUpdate()
        {
            //���ԏ���
            nowTime += Time.deltaTime * antiSlowRate;
            if (nowTime > endTime)
            {
                Debug.Log("I" + inputDir.x);
                Debug.Log("M" + player.moveDir.x);
                if (inputDir.magnitude > 0.1f && Mathf.Sign(inputDir.x) != Mathf.Sign(player.moveDir.x))
                {
                    player.ChangeState(player.move);
                }
                else
                {
                    player.moveDir = Vector3.zero;
                    inputDir = Vector3.zero;
                    player.ChangeState(player.fall);
                }
            }

            //��]����
            float rotRait = (nowTime - reveseStartTime) / (reverseEndTime - reveseStartTime);
            rotRait = (rotRait > 1) ? 1 : rotRait;
            rotRait = (rotRait < 0) ? 0 : rotRait;
            Vector3 sca = startSca;
            sca.x = startSca.x - (rotRait * startSca.x * 2);
            player.transform.localScale = sca;

            //���͎󂯎�菈��
            if (Input.GetMouseButtonDown(0))
            {
                TouchStart();
            }
            else if (Input.GetMouseButton(0))
            {
                Touch();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                TouchOut();
            }

            //�e�̏���
            float rait = nowTime / endTime;
            Color col = characterShadowRenderer.color;
            col.a = rait + 0.2f;
            characterShadowRenderer.color = col;
        }

        public override void OnFixedUpdate()
        {
            Vector3 moveDir = player.moveDir;
            moveDir.x *= moveSpeed_X;
            moveDir.y *= moveSpeed_Y;
            moveDir *= Time.fixedDeltaTime * slowRate * antiSlowRate;
            //�ړ�����
            player.transform.position += moveDir;
            player.moveDir.y -= downforce * Time.fixedDeltaTime * slowRate * antiSlowRate;
        }

        public override void OnExit()
        {
            goalPos.x += Mathf.Sign(player.moveDir.x) * -1 * 0.03f;

            player.moveDir = inputDir;
            player.transform.position = goalPos;

            _arrowShadow.SetActive(false);

            for (int i = 0; i < _visions.Length; i++)
            {
                _visions[i].gameObject.SetActive(false);
            }

            characterShadow.SetActive(false);
        }

        public void SetCount(int count)
        {
            this.count = count;
        }

        public void SetGoalPos(Vector3 goalPos)
        {
            this.goalPos = goalPos;
        }

        /// <summary>
        /// ��ʂɐG��n�߂��Ƃ��̏���
        /// </summary>
        private void TouchStart()
        {
            clickPos = Input.mousePosition;

            SetActiveArrow();

            SetActiveVision();
        }

        /// <summary>
        /// ��ʂɐG�ꑱ���Ă���Ƃ��̏���
        /// </summary>
        private void Touch()
        {
            Vector3 inputDir = CalculatInputDir();

            CharacterVisualControl(inputDir);

            ArrowVisualControl(inputDir);

            AdjustVision(inputDir);
        }

        /// <summary>
        /// ��ʂ���w��b�����Ƃ��̏���
        /// </summary>
        private void TouchOut()
        {
            inputDir = CalculatInputDir();
            antiSlowRate = 1 / slowRate;
        }

        /// <summary>
        /// ����\�����鏈��
        /// </summary>
        private void SetActiveArrow()
        {
            if (isVisualizeArrow == false) return;

            _arrowShadow.SetActive(true);
            _arrowShadow.transform.eulerAngles = new Vector3(0, 0, arrowsFirstLot + 90);
            _arrowShadow.transform.localScale = new Vector3(arrowSizeMin_X, arrowSizeMin_Y, 1);
        }

        /// <summary>
        /// �\������\�����鏈��
        /// </summary>
        private void SetActiveVision()
        {
            if (isVisualizeVision == false) return;

            for (int i = 0; i < outputCount; i++)
            {
                _visions[i].gameObject.SetActive(true);
                _visions[i].transform.position = goalPos;

                float rait = 1 - (i / (float)outputCount);
                Color col = _visions[i].color;
                col.a = rait;
                _visions[i].color = col;
            }
        }

        /// <summary>
        /// ���͂��ꂽ�l���擾�A�v�Z���g���₷���l�ɕϊ����鏈��
        /// </summary>
        /// <returns></returns>
        private Vector3 CalculatInputDir()
        {
            //�}�E�X�̌��ݒn���擾
            Vector3 mousePosition = Input.mousePosition;

            //�}�E�X�̈ړ��������Z�o
            Vector3 distance = mousePosition - clickPos;
            distance *= -1;
            distance = (distance.magnitude < clickAreaRenge) ? distance / clickAreaRenge : distance.normalized;

            return distance;
        }


        /// <summary>
        /// �L�����N�^�[�̃C���X�g�𒲐����鏈��
        /// </summary>
        /// <param name="dir"></param>
        private void CharacterVisualControl(Vector3 dir)
        {
            //�A�j���[�^�[�ɒl�𑗐M
            player._animator.SetFloat("holizontal", Mathf.Abs(dir.x));
            player._animator.SetFloat("vertical", dir.y);
        }

        /// <summary>
        /// ���̃C���X�g�𒲐����鏈��
        /// </summary>
        private void ArrowVisualControl(Vector3 dir)
        {
            if (isVisualizeArrow == false) return;
            //�G���[�΍�
            if (dir.magnitude < 0.1f) return;

            //�p�x�v�Z
            float angle = Mathf.Atan2(dir.y, dir.x);
            Vector3 arrowsEulerAngle = new Vector3(0, 0, angle * Mathf.Rad2Deg + arrowsFirstLot);

            //�T�C�Y�v�Z
            float rait = dir.magnitude;
            float size_X = MathT.GetValueToRange(rait, arrowSizeMin_X, arrowSizeMax_X, false);
            float size_Y = MathT.CastLimit(size_X, float.NegativeInfinity, arrowSizeMax_Y);
            Vector3 localScale = new Vector3(size_X, size_Y, 1);

            //�l�̏o��
            _arrowShadow.transform.eulerAngles = arrowsEulerAngle;
            _arrowShadow.transform.localScale = localScale;

        }

        /// <summary>
        /// �\�����𒲐����鏈��
        /// </summary>
        private void AdjustVision(Vector3 visionDir)
        {
            if (isVisualizeVision == false) return;

            Vector3 visionPos = goalPos;
            int checkFrames = outputCount * outputPitch;
            for (int i = 0; i < checkFrames; i++)
            {
                if (i % outputPitch == 0)
                {
                    int count = i / outputPitch;
                    _visions[count].transform.position = visionPos;
                }

                //���x��������
                Vector3 moveDir = visionDir;
                moveDir.x *= moveSpeed_X;
                moveDir.y *= moveSpeed_Y;
                moveDir *= Time.fixedDeltaTime;

                //�����蔻�菈��
                RaycastHit2D hit = Physics2D.Linecast(visionPos, visionPos + moveDir);
                if (hit)
                {
                    int count = i / outputPitch + 1;
                    for (int j = count; j < outputCount; j++)
                    {
                        _visions[j].transform.position = hit.point;
                    }

                    break;
                }

                visionPos += moveDir;
                visionDir.y -= downforce * Time.fixedDeltaTime;
            }
        }
    }

}
