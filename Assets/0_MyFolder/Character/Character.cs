using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Character : Singleton<Character>
{
    Animator _animator;

    //ステートに関係する処理
    A_PlayerState state;
    [SerializeField] IdleState idle;
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

    [System.Serializable]
    public class IdleState : A_PlayerState
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
        [SerializeField, Range(0,15)] int outputCount = 10;
        [SerializeField] int outputPitch = 10;
        GameObject _arrow;
        SpriteRenderer[] _visions;
        float downforce;
        float moveSpeed_Y;
        float moveSpeed_X;
        Transform _hoppingFrontPos;

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
            this.downforce = character.downforce;
            this.moveSpeed_Y = character.moveSpeed_Y;
            this.moveSpeed_X = character.moveSpeed_X;
            this._hoppingFrontPos = character._hoppingFrontPos;

            character.moveDir = Vector3.zero;
            sita = Mathf.PI;
            startPos = character.transform.position;
            startSca = character.transform.localScale;

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
        }

        /// <summary>
        /// 画面に触れ始めたときの処理
        /// </summary>
        private void TouchStart()
        {
            clickPos = Input.mousePosition;

            SetActiveArrow();

            SetActiveVision();

            SetDefaultAnim();
        }

        /// <summary>
        /// 画面に触れ続けているときの処理
        /// </summary>
        private void Touch()
        {
            Vector3 inputDir = CalculatInputDir();

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

            for(int i = 0; i < outputCount; i++)
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
            Vector3 arrowsEulerAngle = new Vector3(0, 0, angle*Mathf.Rad2Deg + arrowsFirstLot);

            //サイズ計算
            float rait = dir.magnitude;
            float size_X = MathT.GetRaitoToValue(rait, arrowSizeMin_X, arrowSizeMax_X,false);
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
                    for(int j = count; j < outputCount; j++)
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

                character._animator.SetFloat("fall", yValue+1);
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
            this.downforce          = character.downforce;
            this.moveSpeed_Y        = character.moveSpeed_Y;
            this.moveSpeed_X        = character.moveSpeed_X;
            this._hoppingFrontPos = character._hoppingFrontPos;
        }

        public override void OnFixedUpdate()
        {
            //速度調整処理
            Vector3 moveDir = character.moveDir;
            moveDir.x *= moveSpeed_X;
            moveDir.y *= moveSpeed_Y;
            moveDir *= Time.fixedDeltaTime;

            //下あたり判定処理
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
                        return;

                    case LinecastVec.vertical:
                        character.moveDir = Vector3.zero;
                        character.ChangeState(character.fall);
                        return;
                }
            }

            //上あたり判定処理
            Vector3 headLeftUpPos = character.transform.position;
            headLeftUpPos.y += headLine;
            Vector3 headFrontUpPos = character.transform.position;
            headFrontUpPos.y += headLine;
            headFrontUpPos.x += headWidth * Mathf.Sign(character.moveDir.x);
            RaycastHit2D hitTop = Physics2D.Raycast(headLeftUpPos, Vector3.right* Mathf.Sign(character.moveDir.x), headWidth);
            RaycastHit2D hitFront = Physics2D.Raycast(headFrontUpPos, Vector3.down, headHeight);
            if (hitTop || hitFront)
            {
                character.ChangeState(character.fall);
                character.moveDir = Vector3.zero;
                return;
            }

            //将来の壁判定処理
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

            //移動処理
            character.transform.position += moveDir;
            character.moveDir.y -= downforce * Time.fixedDeltaTime;
            
        }
    }

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
            //速度調整処理
            Vector3 moveDir = character.moveDir;
            moveDir.x *= moveSpeed_X;
            moveDir.y *= moveSpeed_Y;
            moveDir *= Time.fixedDeltaTime;

            //あたり判定処理
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
                        return;
                    case LinecastVec.other:
                        character.ChangeState(character.idle);
                        return;
                }
            }

            Move(moveDir);
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        private void Move(Vector3 moveDir)
        {
            character.transform.position += moveDir;
            character.moveDir.y -= downforce * Time.fixedDeltaTime * slow;
        }
    }

    [System.Serializable]
    public class WallLandingMotion:A_PlayerState
    {
        int count;
        Vector3 goalPos;
        float downforce;
        float moveSpeed_Y;
        float moveSpeed_X;
        Transform _hoppingFrontPos;
        
        //時間に関する処理
        [SerializeField] float endTime;
        [SerializeField] float reveseStartTime = 0.2f;
        [SerializeField] float reverseEndTime = 0.5f;
        [SerializeField, ReadOnly] float nowTime;
        Vector3 startSca;
        Vector3 inputDir;

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
        [SerializeField] GameObject _arrowShadow;
        SpriteRenderer[] _visions;

        [SerializeField] GameObject characterShadow;
        [SerializeField] SpriteRenderer characterShadowRenderer;

        float slowRate;
        float antiSlowRate = 1;

        public override void OnEnter()
        {
            this._visions = character._visions;
            this.downforce = character.downforce;
            this.moveSpeed_Y = character.moveSpeed_Y;
            this.moveSpeed_X = character.moveSpeed_X;
            this._hoppingFrontPos = character._hoppingFrontPos;

            inputDir = Vector3.zero;
            nowTime = 0;
            startSca = character.transform.localScale;
            antiSlowRate = 1;

            float normalTime = count * Time.fixedDeltaTime;
            slowRate = normalTime / endTime;

            goalPos.x += Mathf.Sign(character.moveDir.x) * -1 * 0.5f;

            characterShadow.SetActive(true);
            Vector3 shadowSca = characterShadow.transform.localScale;
            shadowSca.x = Mathf.Abs(shadowSca.x) * Mathf.Sign(character.transform.localScale.x);
            characterShadow.transform.position = goalPos;
        }

        public override void OnUpdate()
        {
            //時間処理
            nowTime += Time.deltaTime * antiSlowRate;
            if(nowTime > endTime)
            {
                Debug.Log("I"+ inputDir.x);
                Debug.Log("M"+ character.moveDir.x);
                if(inputDir.magnitude > 0.1f && Mathf.Sign(inputDir.x) != Mathf.Sign(character.moveDir.x))
                {
                    character.ChangeState(character.move);
                }
                else
                {
                    character.moveDir = Vector3.zero;
                    inputDir = Vector3.zero;
                    character.ChangeState(character.fall);
                }
            }

            //回転処理
            float rotRait = (nowTime - reveseStartTime) / (reverseEndTime - reveseStartTime);
            rotRait = (rotRait > 1) ? 1 : rotRait;
            rotRait = (rotRait < 0) ? 0 : rotRait;
            Vector3 sca = startSca;
            sca.x = startSca.x - (rotRait * startSca.x * 2);
            character.transform.localScale = sca;

            //入力受け取り処理
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

            //影の処理
            float rait = nowTime / endTime;
            Color col = characterShadowRenderer.color;
            col.a = rait + 0.2f;
            characterShadowRenderer.color = col;
        }

        public override void OnFixedUpdate()
        {
            Vector3 moveDir = character.moveDir;
            moveDir.x *= moveSpeed_X;
            moveDir.y *= moveSpeed_Y;
            moveDir *= Time.fixedDeltaTime * slowRate * antiSlowRate;
            //移動処理
            character.transform.position += moveDir;
            character.moveDir.y -= downforce * Time.fixedDeltaTime * slowRate * antiSlowRate;
        }

        public override void OnExit()
        {
            goalPos.x += Mathf.Sign(character.moveDir.x) * -1 * 0.03f;

            character.moveDir = inputDir;
            character.transform.position = goalPos;

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
        /// 画面に触れ始めたときの処理
        /// </summary>
        private void TouchStart()
        {
            clickPos = Input.mousePosition;

            SetActiveArrow();

            SetActiveVision();
        }

        /// <summary>
        /// 画面に触れ続けているときの処理
        /// </summary>
        private void Touch()
        {
            Vector3 inputDir = CalculatInputDir();

            CharacterVisualControl(inputDir);

            ArrowVisualControl(inputDir);

            AdjustVision(inputDir);
        }

        /// <summary>
        /// 画面から指を話したときの処理
        /// </summary>
        private void TouchOut()
        {
            inputDir = CalculatInputDir();
            antiSlowRate = 1 / slowRate;
        }

        /// <summary>
        /// 矢印を表示する処理
        /// </summary>
        private void SetActiveArrow()
        {
            if (isVisualizeArrow == false) return;

            _arrowShadow.SetActive(true);
            _arrowShadow.transform.eulerAngles = new Vector3(0, 0, arrowsFirstLot + 90);
            _arrowShadow.transform.localScale = new Vector3(arrowSizeMin_X, arrowSizeMin_Y, 1);
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
                _visions[i].transform.position = goalPos;

                float rait = 1 - (i / (float)outputCount);
                Color col = _visions[i].color;
                col.a = rait;
                _visions[i].color = col;
            }
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
            _arrowShadow.transform.eulerAngles = arrowsEulerAngle;
            _arrowShadow.transform.localScale = localScale;

        }

        /// <summary>
        /// 予測線を調整する処理
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
