using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleManager : MonoBehaviour
{
    public static NeedleManager instance = null;

    [SerializeField] private List<UIButton> buttons = new List<UIButton>();
    [SerializeField] private int needleLevel = 5;
    public int needleItemCount = 10;
    [SerializeField] private Color disenableColor = Color.black;
    [SerializeField] private GameManager gameManager;

    public bool isUseNeedle = false;

    public bool test;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        UIManager.Instance.OnUpdateUIItemCount();
    }


    private void Update()
    {
        //  if(test)
        //  {
        //      ReadyUseNeedle();
        //  }
        //  else
        //  {
        //      CancleUseNeedle();
        //  }
    }

    public void AddButton(UIButton button)
    {
        buttons.Add(button);
        button.enabled = false;
    }

    public void ReadyUseNeedle(Action callback)
    {
        foreach (var button in buttons)
        {
            if (button.transform.parent.GetComponent<Ball>().level <= needleLevel ||
               button.transform.parent.GetComponent<Ball>() == gameManager.lastBall)
            {
                button.transform.parent.GetComponent<UISprite>().color = disenableColor;
                button.enabled = false;
            }
            else
            {
                button.enabled = true;
                button.transform.parent.GetComponent<UISprite>().color = Color.white;

            }
        }
    }

    public void CancleUseNeedle()
    {
        foreach (var button in buttons)
        {
            button.enabled = false;
            button.transform.parent.GetComponent<UISprite>().color = Color.white;

            // action.Invoke();
        }
    }
}
