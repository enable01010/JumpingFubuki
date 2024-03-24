using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : Singleton<Player>
{
    [System.Serializable]
    public class IdleState : A_PlayerState
    {
        //���슴�Ɋւ���ϐ�
        [SerializeField] float clickAreaRenge = 500.0f;
        Vector3 clickPos;

        //���Ɋւ���ϐ�
        [SerializeField] bool isVisualizeArrow = true;
        [SerializeField] float arrowsFirstLot = -47;
        [SerializeField] float arrowSizeMin_X = 0.05f;
        [SerializeField] float arrowSizeMax_X = 0.2f;
        [SerializeField] float arrowSizeMin_Y = 0.05f;
        [SerializeField] float arrowSizeMax_Y = 0.15f;

        //�\�����Ɋւ���ϐ�
        [SerializeField] bool isVisualizeVision = true;
        [SerializeField, Range(0, 15)] int outputCount = 7;
        [SerializeField] int outputPitch = 3;
        GameObject _arrow;
        SpriteRenderer[] _visions;
        float downforce;
        float moveSpeed_Y;
        float moveSpeed_X;
        Transform _hoppingFrontPos;

        //Slider�Ɋւ���ϐ�
        [SerializeField] bool isVisualizeSlider = true;
        [SerializeField] bool isSliderSpeedFixedTime = true;
        [SerializeField] bool isNormalized = false;
        [SerializeField, Range(0.5f, 2.0f)] float isNormalizedRate = 1.0f;
        Slider _slider;
        [SerializeField] float sliderMaxTime = 0.8f;
        [SerializeField] float sliderMaxWaitTime = 0.4f;
        float countTime = 0.0f;
        float sliderValue = 0.0f;
        [SerializeField] AnimationCurve sliderCurveRate;

        //�A�j���[�V�����Ή��p�̕ϐ�
        [SerializeField] bool isAnimation = true;
        [SerializeField] float setaRate = 3.0f;
        [SerializeField] float yPosRate = 1.0f;
        [SerializeField] float yScaRate = 1.0f;
        [SerializeField] float stopTime = 0.1f;
        [SerializeField] Transform _hoppng;
        [SerializeField] float minHopping = 0.5f;
        [SerializeField] float downBody = 0.5f;
        float sita = 0;
        Vector3 startPos;
        Vector3 startSca;

        //�g���C���̏���
        float trailStartTime;
        [SerializeField, Range(0, 1)] float trailLessPitch = 0.9f;
        [SerializeField] TrailRenderer _treail;

        public override void OnEnter()
        {
            this._arrow = player._arrow;
            this._visions = player._visions;
            this._slider = player._slider;
            this.downforce = player.downforce;
            this.moveSpeed_Y = player.moveSpeed_Y;
            this.moveSpeed_X = player.moveSpeed_X;
            this._hoppingFrontPos = player._hoppingFrontPos;

            player.moveDir = Vector3.zero;
            sita = Mathf.PI;
            startPos = player.transform.position;
            startSca = player.transform.localScale;

            SetDefaultSlider();

            if (Input.GetMouseButton(0))
            {
                TouchStart();
            }

            //�g���C���֌W�̏���
            trailStartTime = _treail.time;
            Debug.Log(trailStartTime);
        }

        public override void OnUpdate()
        {
            SetTrailParameter();

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
            else
            {
                AnimControl();
            }

        }

        public override void OnExit()
        {
            _arrow.SetActive(false);

            for (int i = 0; i < _visions.Length; i++)
            {
                _visions[i].gameObject.SetActive(false);
            }

            _slider.gameObject.SetActive(false);
            SetDefaultSlider();

            //�g���C���֌W�̏���
            _treail.time = trailStartTime;
        }

        /// <summary>
        /// ��ʂɐG��n�߂��Ƃ��̏���
        /// </summary>
        private void TouchStart()
        {
            clickPos = Input.mousePosition;

            SetActiveArrow();

            SetActiveVision();

            SetActiveSlider();

            SetDefaultAnim();
        }

        /// <summary>
        /// ��ʂɐG�ꑱ���Ă���Ƃ��̏���
        /// </summary>
        private void Touch()
        {
            Vector3 inputDir = CalculatInputDir();

            SliderControl();

            inputDir = SliderDir(inputDir);

            CharacterVisualControl(inputDir);

            ArrowVisualControl(inputDir);

            AdjustVision(inputDir);
        }

        /// <summary>
        /// ��ʂ���w��b�����Ƃ��̏���
        /// </summary>
        private void TouchOut()
        {
            Vector3 dir = CalculatInputDir();

            SliderControl();
            dir = SliderDir(dir);

            player.moveDir = dir;
            player.ChangeState(player.move);

            SetDefaultSlider();
        }

        /// <summary>
        /// ����\�����鏈��
        /// </summary>
        private void SetActiveArrow()
        {
            if (isVisualizeArrow == false) return;

            _arrow.SetActive(true);
            _arrow.transform.eulerAngles = new Vector3(0, 0, arrowsFirstLot + 90);
            _arrow.transform.localScale = new Vector3(arrowSizeMin_X, arrowSizeMin_Y, 1);
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
                _visions[i].transform.position = _hoppingFrontPos.position;

                float rait = 1 - (i / (float)outputCount);
                Color col = _visions[i].color;
                col.a = rait;
                _visions[i].color = col;
            }
        }

        /// <summary>
        /// Slider��\�����鏈��
        /// </summary>
        private void SetActiveSlider()
        {
            if (isVisualizeSlider == false) return;

            _slider.gameObject.SetActive(true);
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

            //�L�����N�^�[�̌����𒲐�
            ReversalDirection(dir.x);
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
            _arrow.transform.eulerAngles = arrowsEulerAngle;
            _arrow.transform.localScale = localScale;

        }

        /// <summary>
        /// �\�����𒲐����鏈��
        /// </summary>
        private void AdjustVision(Vector3 visionDir)
        {
            if (isVisualizeVision == false) return;

            Vector3 visionPos = _hoppingFrontPos.position;
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

        /// <summary>
        /// Slider�𒲐����鏈��
        /// </summary>
        private void SliderControl()
        {
            if (isVisualizeSlider == false) return;

            countTime += Time.deltaTime;

            if (countTime > sliderMaxTime + sliderMaxWaitTime) countTime -= sliderMaxTime + sliderMaxWaitTime;

            sliderValue = countTime / sliderMaxTime;

            //if (sliderValue > 1.0f) sliderValue = 1.0f;
            sliderValue = (sliderValue > 1.0f) ? 1.0f : sliderValue;

            _slider.value = sliderValue;

            SliderSpeedControl();
        }


        /// <summary>
        /// Slider��speed�𒲐����鏈��
        /// </summary>
        private void SliderSpeedControl()
        {
            if (isSliderSpeedFixedTime == false) return;

            float numerator = sliderCurveRate.Evaluate(sliderValue) - sliderCurveRate.Evaluate(0.0f);
            float denominator = sliderCurveRate.Evaluate(1.0f) - sliderCurveRate.Evaluate(0.0f);

            _slider.value = numerator / denominator;
        }


        /// <summary>
        /// Vector3��Slider�𔽉f���鏈��
        /// </summary>
        /// <returns></returns>
        private Vector3 SliderDir(Vector3 dir)
        {
            if (isVisualizeSlider == false) return dir;

            if (isNormalized == true)
            {
                dir = dir.normalized * isNormalizedRate;
            }

            dir *= sliderCurveRate.Evaluate(sliderValue);

            return dir;
        }

        /// <summary>
        /// �L�����N�^�[�̌����𒲐����鏈��
        /// </summary>
        /// <param name="direction"></param>
        private void ReversalDirection(float direction)
        {
            if (direction == 0) return;

            Vector3 localScale = player.transform.localScale;
            float xValue = (direction > 0) ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            player.transform.localScale = VectorT.Chenge_X(localScale, xValue);
        }

        /// <summary>
        /// Slider���������l�ɖ߂�����
        /// </summary>
        private void SetDefaultSlider()
        {
            countTime = 0.0f;
            sliderValue = 0.0f;
        }


        /// <summary>
        /// �A�j���[�V�����������ʒu�ɖ߂�����
        /// </summary>
        private void SetDefaultAnim()
        {
            if (isAnimation == false) return;

            sita = Mathf.PI;
            player.transform.position = startPos;
            player.transform.localScale = startSca;
            Vector3 sca = new Vector3(1, 1, 1);
            _hoppng.localScale = Vector3.one;
        }

        /// <summary>
        /// ���W�����v���鏈��
        /// </summary>
        private void AnimControl()
        {
            if (isAnimation == false) return;

            sita += Time.deltaTime * setaRate;
            float yValue = Mathf.Sin(sita);
            if (yValue > 0)
            {

                float posYAddValue = yValue * yPosRate;
                player.transform.position = VectorT.Add_Y(startPos, posYAddValue);

                float scaYAddValue = yValue * yScaRate;
                player.transform.localScale = VectorT.Add_Y(startSca, scaYAddValue);

                player._animator.SetFloat("fall", yValue + 1);
            }
            else
            {
                float xValue = Mathf.Cos(sita);
                player._animator.SetFloat("fall", yValue + 1);

                if (xValue < 0)
                {
                    float rait = xValue + 1;
                    float ySca = 1 - (1 - minHopping) * rait;
                    float yPos = downBody * rait;

                    Vector3 sca = new Vector3(1, ySca, 1);
                    _hoppng.localScale = sca;

                    Vector3 pos = startPos;
                    pos.y -= yPos;
                    player.transform.position = pos;
                }
                else
                {
                    float rait = xValue;
                    float ySca = minHopping + (1 - minHopping) * rait;
                    float yPos = downBody * (1 - rait);

                    Vector3 sca = new Vector3(1, ySca, 1);
                    _hoppng.localScale = sca;

                    Vector3 pos = startPos;
                    pos.y -= yPos;
                    player.transform.position = pos;
                }
            }
        }

        /// <summary>
        /// �g���C���̕\���𒲐����鏈��
        /// </summary>
        private void SetTrailParameter()
        {
            if (_treail.time < 0.001f) return;

            _treail.time *= trailLessPitch;
        }

    }

}
