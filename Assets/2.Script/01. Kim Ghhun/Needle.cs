using System;
using UnityEngine;
using static EventDelegate;

public class Needle : MonoBehaviour
{
    public GameObject fruit;

    [SerializeField]private UIButton _ballButton;

    private void OnEnable()
    {
        Debug.Log("Add Listener");
        _ballButton.onClick.Add(new EventDelegate(() => OnUseNeedle()));    
    }

    // 아이템을 사용하여, 과일을 삭제하는 메서드
    public void OnUseNeedle()
    {
        Debug.Log("Use Needle");
        fruit.SetActive(false);
        UIManager.Instance.ItemEnvironmentBox.SetActive(false);
        NeedleManager.instance.CancleUseNeedle();
    }
}
