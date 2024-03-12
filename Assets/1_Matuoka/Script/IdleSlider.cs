using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Character : Singleton<Character>
{
    [System.Serializable]
    public class IdleSliderState : A_PlayerState
    {
        //操作感に関する変数
        [SerializeField] float clickAreaRenge = 500.0f;
        Vector3 clickPos;

        //矢印に関する変数
        [SerializeField] bool isVisualizeArrow = true;
        [SerializeField] float arrowsFirstLot = -38;
        [SerializeField] float arrowSizeMin_X = 0.05f;
        [SerializeField] float arrowSizeMax_X = 0.4f;
        [SerializeField] float arrowSizeMin_Y = 0.05f;
        [SerializeField] float arrowSizeMax_Y = 0.15f;

        //予測線に関する変数
        [SerializeField] bool isVisualizeVision = true;
        [SerializeField, Range(0, 15)] int outputCount = 10;
        [SerializeField] int outputPitch = 10;
        GameObject _arrow;
        SpriteRenderer[] _visions;
        float downforce;
        float moveSpeed_Y;
        float moveSpeed_X;
        Transform _hoppingFrontPos;

        //Sliderに関する変数
        [SerializeField] bool isVisualizeSlider = true;
        Slider _slider;
        [SerializeField] float sliderMaxTime = 0.8f;
        [SerializeField] float sliderMaxWaitTime = 0.2f;
        float countTime = 0.0f;
        float sliderValue = 0.0f;
        [SerializeField] AnimationCurve sliderCurveRate;

        //アニメーション対応用の変数
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

        public override void OnEnter()
        {
            this._arrow = character._arrow;
            this._visions = character._visions;
            this._slider = character._slider;
            this.downforce = character.downforce;
            this.moveSpeed_Y = character.moveSpeed_Y;
            this.moveSpeed_X = character.moveSpeed_X;
            this._hoppingFrontPos = character._hoppingFrontPos;

            character.moveDir = Vector3.zero;
            sita = Mathf.PI;
            startPos = character.transform.position;
            startSca = character.transform.localScale;

            SetDefaultSlider();

            if (Input.GetMouseButton(0))
            {
                TouchStart();
            }
        }

        public override void OnUpdate()
        {
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
        }

        /// <summary>
        /// 画面に触れ始めたときの処理
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
        /// 画面に触れ続けているときの処理
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
        /// 画面から指を話したときの処理
        /// </summary>
        private void TouchOut()
        {
            Vector3 dir = CalculatInputDir();
            character.moveDir = dir;
            character.ChangeState(character.move);

            SetDefaultSlider();
        }

        /// <summary>
        /// 矢印を表示する処理
        /// </summary>
        private void SetActiveArrow()
        {
            if (isVisualizeArrow == false) return;

            _arrow.SetActive(true);
            _arrow.transform.eulerAngles = new Vector3(0, 0, arrowsFirstLot + 90);
            _arrow.transform.localScale = new Vector3(arrowSizeMin_X, arrowSizeMin_Y, 1);
        }

        /// <summary>
        /// 予測線を表示する処理
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
        /// Sliderを表示する処理
        /// </summary>
        private void SetActiveSlider()
        {
            if (isVisualizeSlider == false) return;

            _slider.gameObject.SetActive(true);
        }

        /// <summary>
        /// 入力された値を取得、計算し使いやすい値に変換する処理
        /// </summary>
        /// <returns></returns>
        private Vector3 CalculatInputDir()
        {
            //マウスの現在地を取得
            Vector3 mousePosition = Input.mousePosition;

            //マウスの移動距離を算出
            Vector3 distance = mousePosition - clickPos;
            distance *= -1;
            distance = (distance.magnitude < clickAreaRenge) ? distance / clickAreaRenge : distance.normalized;

            return distance;
        }


        /// <summary>
        /// キャラクターのイラストを調整する処理
        /// </summary>
        /// <param name="dir"></param>
        private void CharacterVisualControl(Vector3 dir)
        {
            //アニメーターに値を送信
            character._animator.SetFloat("holizontal", Mathf.Abs(dir.x));
            character._animator.SetFloat("vertical", dir.y);

            //キャラクターの向きを調整
            ReversalDirection(dir.x);
        }

        /// <summary>
        /// 矢印のイラストを調整する処理
        /// </summary>
        private void ArrowVisualControl(Vector3 dir)
        {
            if (isVisualizeArrow == false) return;
            //エラー対策
            if (dir.magnitude < 0.1f) return;

            //角度計算
            float angle = Mathf.Atan2(dir.y, dir.x);
            Vector3 arrowsEulerAngle = new Vector3(0, 0, angle * Mathf.Rad2Deg + arrowsFirstLot);

            //サイズ計算
            float rait = dir.magnitude;
            float size_X = MathT.GetRaitoToValue(rait, arrowSizeMin_X, arrowSizeMax_X, false);
            float size_Y = MathT.CastLimit(size_X, float.NegativeInfinity, arrowSizeMax_Y);
            Vector3 localScale = new Vector3(size_X, size_Y, 1);

            //値の出力
            _arrow.transform.eulerAngles = arrowsEulerAngle;
            _arrow.transform.localScale = localScale;

        }

        /// <summary>
        /// 予測線を調整する処理
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

                //速度調整処理
                Vector3 moveDir = visionDir;
                moveDir.x *= moveSpeed_X;
                moveDir.y *= moveSpeed_Y;
                moveDir *= Time.fixedDeltaTime;

                //あたり判定処理
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
        /// Sliderを調整する処理
        /// </summary>
        private void SliderControl()
        {
            if (isVisualizeArrow == false) return;

            countTime += Time.deltaTime;

            if (countTime > sliderMaxTime + sliderMaxWaitTime) countTime -= sliderMaxTime + sliderMaxWaitTime;

            sliderValue = countTime / sliderMaxTime;

            if (sliderValue > 1.0f) sliderValue = 1.0f;

            _slider.value = sliderValue;
        }


        /// <summary>
        /// Vector3にSliderを反映する処理
        /// </summary>
        /// <returns></returns>
        private Vector3 SliderDir(Vector3 dir)
        {
            if (isVisualizeSlider == false) return dir;

            dir *= sliderCurveRate.Evaluate(sliderValue);

            return dir;
        }

        /// <summary>
        /// キャラクターの向きを調整する処理
        /// </summary>
        /// <param name="direction"></param>
        private void ReversalDirection(float direction)
        {
            if (direction == 0) return;

            Vector3 localScale = character.transform.localScale;
            float xValue = (direction > 0) ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            character.transform.localScale = VectorT.Chenge_X(localScale, xValue);
        }

        /// <summary>
        /// Sliderを初期数値に戻す処理
        /// </summary>
        private void SetDefaultSlider()
        {
            countTime = 0.0f;
            sliderValue = 0.0f;
        }


        /// <summary>
        /// アニメーションを初期位置に戻す処理
        /// </summary>
        private void SetDefaultAnim()
        {
            if (isAnimation == false) return;

            sita = Mathf.PI;
            character.transform.position = startPos;
            character.transform.localScale = startSca;
        }

        /// <summary>
        /// 小ジャンプする処理
        /// </summary>
        private void AnimControl()
        {
            if (isAnimation == false) return;

            sita += Time.deltaTime * setaRate;
            float yValue = Mathf.Sin(sita);
            if (yValue > 0)
            {

                float posYAddValue = yValue * yPosRate;
                character.transform.position = VectorT.Add_Y(startPos, posYAddValue);

                float scaYAddValue = yValue * yScaRate;
                character.transform.localScale = VectorT.Add_Y(startSca, scaYAddValue);

                character._animator.SetFloat("fall", yValue + 1);
            }
            else
            {
                float xValue = Mathf.Cos(sita);
                character._animator.SetFloat("fall", yValue + 1);

                if (xValue < 0)
                {
                    float rait = xValue + 1;
                    float ySca = 1 - (1 - minHopping) * rait;
                    float yPos = downBody * rait;

                    Vector3 sca = new Vector3(1, ySca, 1);
                    _hoppng.localScale = sca;

                    Vector3 pos = startPos;
                    pos.y -= yPos;
                    character.transform.position = pos;
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
                    character.transform.position = pos;
                }
            }
        }

    }

}