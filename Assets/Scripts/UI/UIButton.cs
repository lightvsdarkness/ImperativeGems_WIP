using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour {
    public Image ButtonImage;

    public List<Sprite> ButtonSprites = new List<Sprite>();
    public int Index = 0;

    void Start() {
        if (ButtonImage == null)
            ButtonImage = GetComponent<Image>();

        ButtonImage.sprite = ButtonSprites[Index];
    }

    public void UIButtonClick() {
        Index++;
        if (Index >= ButtonSprites.Count)
            Index = 0;

        //Debug.Log("Index: " + Index, this);
        ButtonImage.sprite = ButtonSprites[Index];
    }
}
