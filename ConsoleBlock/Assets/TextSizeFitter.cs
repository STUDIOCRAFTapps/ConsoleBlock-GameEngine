using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSizeFitter : MonoBehaviour {

    Text text;
    RectTransform rectTransform;

    void Start () {
        text = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();

        float labelWidth = 0f;
        for(int i = 0; i < text.text.Length; i++) {
            CharacterInfo characterInfo = new CharacterInfo();
            text.font.GetCharacterInfo(text.text[i], out characterInfo, text.fontSize);
            labelWidth += characterInfo.advance;
            Debug.Log(text.text[i] + " / " + characterInfo.uvBottomLeft);
        }
        rectTransform.sizeDelta = new Vector2(labelWidth, rectTransform.sizeDelta.y);

        Debug.Log(labelWidth);
    }
}