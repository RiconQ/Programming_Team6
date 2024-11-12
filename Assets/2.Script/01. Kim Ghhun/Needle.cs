using UnityEngine;

public class Needle : MonoBehaviour
{
    public GameObject fruit;

    [SerializeField]private UIButton _ballButton;

    private void OnEnable()
    {
        Debug.Log("Add Listener");
        _ballButton.onClick.Add(new EventDelegate(() => UseNeedle()));    
    }

    public void UseNeedle()
    {
        Debug.Log("Use Needle");
        fruit.SetActive(false);
    }
}
