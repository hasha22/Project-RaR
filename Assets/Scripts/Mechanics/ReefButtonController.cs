using UnityEngine;
using UnityEngine.UI; 

// Assigned to each Reef Button
public class ReefButtonController : MonoBehaviour
{
    [SerializeField] private ReefType targetReef; 
    [SerializeField]private Button button;

    private void Awake()
    {
        button.onClick.RemoveAllListeners(); // 이전에 등록된 리스너 제거
        button.onClick.AddListener(OnReefButtonClicked);
    }

    public void OnReefButtonClicked()
    {
        ReefManager.Instance.SwitchReef(targetReef);
    }
    
    public void SetTargetReef(ReefType newReef)
    {
        targetReef = newReef;
    }
}