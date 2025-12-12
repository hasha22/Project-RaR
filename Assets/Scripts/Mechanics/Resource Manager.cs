using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance { get; private set; }

    [Header("Player Resources")]
    //these need to be assigned at the beginning of a day (most likely), not through inspector
    public int funds;
    public int purity;
    public int biodiversity;
    private int maxFunds = 10000;
    private int maxPurity = 100;
    private int maxBiodiversity = 100;

    private int fundsAtStart;
    private int purityAtStart;
    private int biodiversityAtStart;

    public bool isGameOver { get; private set; }

    public int deltaFunds { get; private set; }
    public int deltaPurity { get; private set; }
    public int deltaBiodiversity { get; private set; }

    [Header("Resource UI Scripts")]
    public ResourceBarUI fundsUI;
    public ResourceBarUI purityUI;
    public ResourceBarUI biodiversityUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void Init()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart += AssignResources;
            DayManager.Instance.OnDayEnd += GetDailyChange;
            Debug.Log($"{name}: Subscribing to DayManager events");
        }
    }

    private void AssignResources()
    {
        if (DayManager.Instance.currentDay == 1)
        {
            funds = 2000;
            purity = 25;
            biodiversity = 40;
        }

        fundsUI.SetMaxValue(maxFunds);
        purityUI.SetMaxValue(maxPurity);
        biodiversityUI.SetMaxValue(maxBiodiversity);

        fundsUI.SetValue(funds);
        purityUI.SetValue(purity);
        biodiversityUI.SetValue(biodiversity);

        UIManager.instance.UpdateFundsUI(funds);
        UIManager.instance.UpdatePurityUI(purity);
        UIManager.instance.UpdateBiodiversityUI(biodiversity);

        // 증감 계산을 위해 시작 값 기록
        fundsAtStart = funds;
        purityAtStart = purity;
        biodiversityAtStart = biodiversity;
    }

    public void GetDailyChange()
    {
        deltaFunds = funds - fundsAtStart;
        deltaPurity = purity - purityAtStart;
        deltaBiodiversity = biodiversity - biodiversityAtStart;

        Debug.Log($"delta funds: {deltaFunds}\ndelta purity: {deltaPurity}\ndelta biodiversity: {deltaBiodiversity}");
    }

    private void CheckGameOver()
    {
        if (funds <= 0 || purity <= 0 || biodiversity <= 0)
        {
            Debug.Log("GAME OVER");

            isGameOver = true;
            DayManager.Instance.AdvanceDay();
        }
    }
    /////////////////////////////////////////////////////////////////////////////

    private void Start()
    {
        Init();

        /*
        fundsUI.SetMaxValue(maxFunds);
        purityUI.SetMaxValue(maxPurity);
        biodiversityUI.SetMaxValue(maxBiodiversity);

        fundsUI.SetValue(funds);
        purityUI.SetValue(purity);
        biodiversityUI.SetValue(biodiversity);

        UIManager.instance.UpdateFundsUI(funds);
        UIManager.instance.UpdatePurityUI(purity);
        UIManager.instance.UpdateBiodiversityUI(biodiversity);
        */
    }

    public void OnDayPassed()
    {
        //requires data about the day, how many resources to award the player
    }
    public void AddFunds(int newFunds)
    {
        funds += newFunds;
        fundsUI.SetValue(funds);
        UIManager.instance.UpdateFundsUI(funds);
    }
    public void AddPurity(int newPurity)
    {
        purity += newPurity;
        purityUI.SetValue(purity);
        UIManager.instance.UpdatePurityUI(purity);
    }
    public void AddBiodiversity(int newBiodiversity)
    {
        biodiversity += newBiodiversity;
        biodiversityUI.SetValue(biodiversity);
        UIManager.instance.UpdateBiodiversityUI(biodiversity);
    }
    public void SubtractFunds(int newFunds)
    {
        funds -= newFunds;
        fundsUI.SetValue(funds);
        UIManager.instance.UpdateFundsUI(funds);

        CheckGameOver();
    }
    public void SubtractPurity(int newPurity)
    {
        purity -= newPurity;
        purityUI.SetValue(purity);
        UIManager.instance.UpdatePurityUI(purity);

        CheckGameOver();
    }
    public void SubtractBiodiversity(int newBiodiversity)
    {
        biodiversity -= newBiodiversity;
        biodiversityUI.SetValue(biodiversity);
        UIManager.instance.UpdateBiodiversityUI(biodiversity);

        CheckGameOver();
    }
}
