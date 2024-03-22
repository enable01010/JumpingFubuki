using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// よく使う色関係のクラス
/// </summary>
public class ColorT
{
    /// <summary>
    /// 色の１値のみを変更する処理
    /// </summary>
    /// <param name="col">元の色</param>
    /// <param name="value">変更後の値(0~1の場合はそのまま）(1～255の場合は比率に変換）</param>
    /// <param name="mode">変更する色（初期値は透明度）</param>
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
    /// 点滅させる処理
    /// </summary>
    /// <param name="col">元の色</param>
    /// <param name="pitch">点滅の周期</param>
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
