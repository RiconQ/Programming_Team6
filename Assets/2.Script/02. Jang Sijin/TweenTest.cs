using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenTest : MonoBehaviour
{
    [SerializeField] GameObject _start;
    [SerializeField] GameObject _end;
    [SerializeField] Transform _control;

    [SerializeField] UIButton _startButton;

    // Start is called before the first frame update
    void Start()
    {
        _startButton.onClick.Add(new EventDelegate(() =>
        {
            Extension.MoveCurve(_start, _end, _control, 1f);
        }));        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
