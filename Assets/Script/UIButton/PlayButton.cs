using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : BaseButton
{
    private SceneController sceneController;
    private AudioManager audioManager;
    protected override void Start()
    {
        base.Start();
        sceneController = FindAnyObjectByType<SceneController>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    protected override void OnClick()
    {
        Debug.Log("Click play  button");
        audioManager.PlayUIButtonSound();
        sceneController.LoadGameplayScene();
    }
}
