using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// よく使うVector関係の処理用クラス
/// </summary>
public class VectorT 
{
    /// <summary>
    /// TransoformのXの値のみを変更する処理
    /// </summary>
    /// <param name="vector">元のベクトル</param>
    /// <param name="value">変更後の値</param>
    /// <returns></returns>
    static public Vector3 Chenge_X(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.x = value;

        return answer;
    }

    /// <summary>
    /// TransoformのYの値のみを変更する処理
    /// </summary>
    /// <param name="vector">元のベクトル</param>
    /// <param name="value">変更後の値</param>
    /// <returns></returns>
    static public Vector3 Chenge_Y(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.y = value;

        return answer;
    }

    /// <summary>
    /// TransoformのZの値のみを変更する処理
    /// </summary>
    /// <param name="vector">元のベクトル</param>
    /// <param name="value">変更後の値</param>
    /// <returns></returns>
    static public Vector3 Chenge_Z(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.z = value;

        return answer;
    }

    /// <summary>
    /// TransoformのXの値のみを加算する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static public Vector3 Add_X(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.x += value;

        return answer;
    }

    /// <summary>
    /// TransoformのYの値のみを加算する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static public Vector3 Add_Y(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.y += value;

        return answer;
    }

    /// <summary>
    /// TransoformのZの値のみを加算する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static public Vector3 Add_Z(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.z += value;

        return answer;
    }


    /// <summary>
    /// 2次元ベクトルを角度（弧度法）に変更する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public float ChengeVectorToRadian_2D(Vector2 vector)
    {
        float answer = 0;

        answer = Mathf.Atan2(vector.y, vector.x);

        return answer;
    }

    /// <summary>
    ///  2次元ベクトルを角度（弧度法）に変更する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public float ChengeVectorToRadian_2D(Vector3 vector)
    {
        float answer = 0;

        answer = Mathf.Atan2(vector.y, vector.x);

        return answer;
    }

    /// <summary>
    /// 2次元ベクトルを角度（弧度法）に変更する処理
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    static public float ChengeVectorToRadian_2D(float x, float y)
    {
        float answer = 0;

        answer = Mathf.Atan2(y, x);

        return answer;
    }

    /// <summary>
    /// 2次元ベクトルを角度（度数法）に変更する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public float ChengeVectorToDegree_2D(Vector2 vector)
    {
        float answer = 0;

        float raian = ChengeVectorToRadian_2D(vector);
        answer = raian * Mathf.Rad2Deg;

        return answer;
    }

    /// <summary>
    ///  2次元ベクトルを角度（度数法）に変更する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public float ChengeVectorToDegree_2D(Vector3 vector)
    {
        float answer = 0;

        float raian = ChengeVectorToRadian_2D(vector);
        answer = raian * Mathf.Rad2Deg;

        return answer;
    }

    /// <summary>
    ///  2次元ベクトルを角度（度数法）に変更する処理
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    static public float ChengeVectorToDegree_2D(float x, float y)
    {
        float answer = 0;

        float raian = ChengeVectorToRadian_2D(x,y);
        answer = raian * Mathf.Rad2Deg;

        return answer;
    }

    /// <summary>
    /// 2次元ベクトルを角度 (Vector3)に変更する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public Vector3 ChengeVectorToEulerAngle_2D(Vector2 vector)
    {
        Vector3 answer = Vector3.zero;

        float degree = ChengeVectorToDegree_2D(vector);
        answer = Chenge_Z(answer, degree);

        return answer;
    }


    /// <summary>
    /// 2次元ベクトルを角度 (Vector3)に変更する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public Vector3 ChengeVectorToEulerAngle_2D(Vector3 vector)
    {
        Vector3 answer = Vector3.zero;

        float degree = ChengeVectorToDegree_2D(vector);
        answer = Chenge_Z(answer, degree);

        return answer;
    }

    /// <summary>
    /// 2次元ベクトルを角度 (Vector3)に変更する処理
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public Vector3 ChengeVectorToEulerAngle_2D(float x, float y)
    {
        Vector3 answer = Vector3.zero;

        float degree = ChengeVectorToDegree_2D(x,y);
        answer = Chenge_Z(answer, degree);

        return answer;
    }
}
