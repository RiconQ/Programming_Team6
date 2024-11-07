using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Switch : MonoBehaviour
{
    [SerializeField] private GameObject _playButtons;
    [SerializeField] private GameObject _toggleButton;


    private float moveAmount = 13f;

    public void LeftHand_Mode()
    {
        Debug.Log("왼손모드");
        Debug.Log(_playButtons.transform.position);
        Debug.Log(_toggleButton.transform.position);
        _playButtons.transform.position=new Vector2(-13.5f, 0);
        _toggleButton.transform.position = new Vector2(-20, -6);
    }

    public void RightHand_Mode()
    {
        Debug.Log("오른손모드");
        Debug.Log(_playButtons.transform.position);
        Debug.Log(_toggleButton.transform.position);
        _playButtons.transform.position = new Vector2(0, 0);
        _toggleButton.transform.position = new Vector2(10, -4);
    }
}
