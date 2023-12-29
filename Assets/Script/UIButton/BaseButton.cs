using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class BaseButton : MonoBehaviour
{
    [Header("Base Button")]
    [SerializeField] protected Button button;

    protected virtual void Start()
    {
        button = GetComponent<Button>();
        this.AddOnClickEvent();
    }
    protected void AddOnClickEvent()
    {
        this.button.onClick.AddListener(OnClick);
    }
    protected abstract void OnClick();
}
