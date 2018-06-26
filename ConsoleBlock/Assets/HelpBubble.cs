using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpBubble : MonoBehaviour {

    public RectTransform ValidRect;
    public BubbleSolve bubbleSolve;
    public InputControl.InputType[] keycode;
    public InputControl.InputType[] keycode2;
    public InputControl.InputType[] keycode3;

    public HelpBubble[] EnabledBubble;

    bool InProgress = false;

    IEnumerator CompleteAnimation () {
        if(InProgress) {
            yield break;
        } else {
            InProgress = true;
        }
        ValidRect.gameObject.SetActive(true);
        foreach(HelpBubble hb in EnabledBubble) {
            hb.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

    public void Valid () {
        StartCoroutine(CompleteAnimation());
    }

    void OnEnable () {
        if(InProgress && ValidRect.gameObject.activeSelf) {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
        if(bubbleSolve == BubbleSolve.Keycodes && InputControl.GetAnyInput(keycode)) {
            StartCoroutine(CompleteAnimation());
        }
        if(bubbleSolve == BubbleSolve.KeycodesCombo && InputControl.GetAnyInput(keycode) && InputControl.GetAnyInput(keycode2)) {
            StartCoroutine(CompleteAnimation());
        }
        if(bubbleSolve == BubbleSolve.KeycodesTrio && InputControl.GetAnyInput(keycode) && InputControl.GetAnyInput(keycode2) && InputControl.GetAnyInput(keycode3)) {
            StartCoroutine(CompleteAnimation());
        }
    }

    public enum BubbleSolve {
        ScriptCalls,
        Keycodes,
        KeycodesCombo,
        KeycodesTrio
    }
}
