using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioButtonn : BaseButton
{
    public Sprite soundOnImg;
    public Sprite soundOffImg;

    protected override void Start()
    {
        base.Start();
    }
    protected override void OnClick()
    {
        if (AudioListener.pause)
        {
            gameObject.GetComponent<Image>().sprite = soundOnImg;

        }
        else
        {
            gameObject.GetComponent<Image>().sprite = soundOffImg;
        }
        AudioListener.pause = !AudioListener.pause;
    }
}
