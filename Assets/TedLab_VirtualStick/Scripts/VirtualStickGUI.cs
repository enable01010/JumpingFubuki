using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class VirtualStickGUI : Singleton<VirtualStickGUI>, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public bool IsOperating{ get{ return m_operating; } }
    public Vector2 InputValue{ get{ return m_InputValue; } }

    [SerializeField] RectTransform refRectTransform = null;
    [SerializeField] Image joyStickBack = null;
    [SerializeField] Image joyStick = null;
    [SerializeField] float radius = 100f;
    [SerializeField] bool isVisualize = false;

    Vector2 m_InputValue = Vector2.zero;
    Vector2 m_StartPosition = Vector2.zero;
    bool m_enable = true;
    bool m_operating = false;

    public override void OnInitialize()
    {
        joyStickBack.gameObject.SetActive(false);
        joyStick.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData data)
    {
        if(!m_enable){
            return;
        }
        m_operating = true;
        Vector2 pos = GetLocalPosition(data.position);
        joyStickBack.rectTransform.localPosition = pos;
        joyStick.rectTransform.localPosition = pos;
        m_StartPosition = pos;

        if (isVisualize)
        {
            joyStickBack.gameObject.SetActive(true);
            joyStick.gameObject.SetActive(true);
        }

        m_InputValue = Vector2.zero;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if(!m_enable){
            return;
        }
        m_operating = false;
        joyStickBack.gameObject.SetActive(false);
        joyStick.gameObject.SetActive(false);

        m_InputValue = Vector2.zero;
    }

    public void OnDrag(PointerEventData data)
    {
        if(!m_enable){
            return;
        }

        Vector2 pos = GetLocalPosition(data.position);
        Vector2 vec = pos - m_StartPosition;
        float rad = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        float dist = vec.magnitude;

        if (dist <= radius)
        {
            joyStick.rectTransform.localPosition = pos;
        }
        else
        {
            joyStick.rectTransform.localPosition = new Vector2(m_StartPosition.x + radius * Mathf.Cos(rad * Mathf.Deg2Rad), m_StartPosition.y + radius * Mathf.Sin(rad * Mathf.Deg2Rad));
        }

        // 入力値(0~1)
        float param = Mathf.Min(dist, radius) / radius;
        vec.Normalize();
        m_InputValue = vec * param;
    }

    Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(refRectTransform, screenPosition, null, out result);
        return result;
    }

    // 有効設定
    public void SetStickEnable(bool enable)
    {
        m_enable = enable;

        if(!m_enable)
        {
            joyStickBack.gameObject.SetActive(false);
            joyStick.gameObject.SetActive(false);
            m_InputValue = Vector2.zero;
        }
    }

}

