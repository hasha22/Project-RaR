using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReportUI : MonoBehaviour
{
    [SerializeField] private GameObject reportPanel;
    [SerializeField] private TextMeshProUGUI reportText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button nextDayButton;

    private void Init() 
    { 
        if (DayManager.Instance != null) DayManager.Instance.OnDayEnd += ShowDailyReport; 
        Debug.Log($"{name}: Subscribing to DayManager events");
    }

    private void Start()
    {
        Init();
        if (reportPanel != null) reportPanel.SetActive(false);
    }

    public void ShowDailyReport()
    {
        ResourceManager.instance.GetDailyChange();
        Debug.Log ("ShowReport OK");

        ReefType currentReef = ReefManager.Instance.activeReefType;

        reportPanel.SetActive(true);
        int dFunds = ResourceManager.instance.deltaFunds;
        // int dPurity = ResourceManager.instance.deltaPurity;
        // int dBiodiversity = ResourceManager.instance.deltaBiodiversity;
        int dPurity = ResourceManager.instance.deltaPurity[currentReef];
        int dBiodiversity = ResourceManager.instance.deltaBiodiversity[currentReef];

        reportText.text = $"Daily Report - Day {DayManager.Instance.currentDay}\n" +
                          $"Funds: {dFunds:+#;-#;0}\n" +
                          $"Purity: {dPurity:+#;-#;0}\n" +
                          $"Biodiversity: {dBiodiversity:+#;-#;0}";

        // Check game over
        if (ResourceManager.instance.isGameOver)
        {
            gameOverText.text = "GAME OVER";
            gameOverText.gameObject.SetActive(true);
            reportText.gameObject.SetActive(false);
            nextDayButton.gameObject.SetActive(false);
        }

        else
        {
            gameOverText.gameObject.SetActive(false);
            nextDayButton.gameObject.SetActive(true);
        }
    }

    public void OnClickNextDay()
    {   
        reportPanel.SetActive(false);
        DayManager.Instance.StartDay();
    }
}