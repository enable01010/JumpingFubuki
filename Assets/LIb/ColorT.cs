using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �悭�g���F�֌W�̃N���X
/// </summary>
public class ColorT
{
    /// <summary>
    /// �F�̂P�l�݂̂�ύX���鏈��
    /// </summary>
    /// <param name="col">���̐F</param>
    /// <param name="value">�ύX��̒l(0~1�̏ꍇ�͂��̂܂܁j(1�`255�̏ꍇ�͔䗦�ɕϊ��j</param>
    /// <param name="mode">�ύX����F�i�����l�͓����x�j</param>
    /// <returns></returns>
    static public Color ChengeOneDir(Color color,float value,ColorT_Mode mode = ColorT_Mode.a)
    {
        Color answer = new Color();
        
        value = MathT.CastLimit(value, 0, float.PositiveInfinity);
        value = (value <= 1.0f) ? value : MathT.getRangeToValue(value, 0, 255);
        answer = color;
        switch (mode)
        {
            case ColorT_Mode.r:
                answer.r = value;
                break;
            case ColorT_Mode.g:
                answer.g = value;
                break;
            case ColorT_Mode.b:
                answer.b = value;
                break;
            case ColorT_Mode.a:
                answer.a = value;
                break;
        }

        return answer;
    }

    /// <summary>
    /// �_�ł����鏈��
    /// </summary>
    /// <param name="col">���̐F</param>
    /// <param name="pitch">�_�ł̎���</param>
    /// <returns></returns>
    static public Color FlashColor(Color col,float pitch)
    {
        Color answer = new Color();

        float alpha = MathT.GetMetoronomeZeroToOne(pitch);
        answer = ChengeOneDir(col, alpha);

        return answer;
    }
}

public enum ColorT_Mode
{
    r,
    g,
    b,
    a
}
