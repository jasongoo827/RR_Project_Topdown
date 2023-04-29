using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button_Sound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Action internalOnPointerEnterFunc = null, internalOnPointerClickFunc = null;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"OnPointerClick {this.name}");
        if (internalOnPointerClickFunc != null) internalOnPointerClickFunc();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"OnPointerEnter {this.name}");
        if (internalOnPointerEnterFunc != null) internalOnPointerEnterFunc();
    }
    private void Awake()
    {
        internalOnPointerClickFunc += () => { AudioManager.Instance.Play("UI_Button_Mouse_Click", 1); };
        internalOnPointerEnterFunc += () => { AudioManager.Instance.Play("UI_Button_Mouse_On", 1); };
    }
}
