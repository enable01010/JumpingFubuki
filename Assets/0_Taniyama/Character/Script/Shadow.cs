using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shadow : MonoBehaviour
{
    [SerializeField] Transform _hoppingFrontPos;
    [SerializeField] Vector3 rayStartPosAdjustValue;
    [SerializeField] float shadowRnage;
    [SerializeField,Range(0,1)] float maxAlpha;
    [SerializeField, Range(0, 1)] float minAlpha;
    [SerializeField] Vector3 maxSize;
    [SerializeField] Vector3 minSize;

    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {

        RaycastHit2D hit = CheckDistanceToStage();
        if (hit == true)
        {
            SetParamater(hit);
        }
        else
        {
            HiddenSprite();
        }
    }

    /// <summary>
    /// �����̏����e�̕\���͈͓��ɂ��邩�m�F���鏈��
    /// </summary>
    /// <returns></returns>
    private RaycastHit2D CheckDistanceToStage()
    {
        RaycastHit2D answer;

        Vector3 rayStartPos = _hoppingFrontPos.position + rayStartPosAdjustValue;
        answer = Physics2D.Raycast(rayStartPos, Vector2.down, shadowRnage);

        return answer;
    }

    /// <summary>
    /// Ray�̓��������������Ƀp�����[�^�𒲐����鏈��
    /// </summary>
    /// <param name="hit"></param>
    private void SetParamater(RaycastHit2D hit)
    {
        //�\���̗L����
        sr.enabled = true;

        //�l�̌v�Z
        float raito = MathT.getRangeToValue(hit.distance, 0, shadowRnage);
        raito = MathT.OneMinus(raito);
        Vector3 size = new Vector3();
        size.x = MathT.GetValueToRange(raito, minSize.x, maxSize.x);
        size.y = MathT.GetValueToRange(raito, minSize.y, maxSize.y);
        float alpha = MathT.GetValueToRange(raito, minAlpha, maxAlpha);

        //�l�̑}��
        transform.position = hit.point;
        transform.localScale = size;
        sr.color = ColorT.ChengeOneDir(sr.color, alpha);
    }

    /// <summary>
    /// �e�̕\������������
    /// </summary>
    private void HiddenSprite()
    {
        sr.enabled = false;
    }
}
