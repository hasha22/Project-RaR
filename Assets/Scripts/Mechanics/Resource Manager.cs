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
    private void Start()
    {
        fundsUI.SetMaxValue(maxFunds);
        purityUI.SetMaxValue(maxPurity);
        biodiversityUI.SetMaxValue(maxBiodiversity);

        fundsUI.SetValue(funds);
        purityUI.SetValue(purity);
        biodiversityUI.SetValue(biodiversity);

        UIManager.instance.UpdateFundsUI(funds);
        UIManager.instance.UpdatePurityUI(purity);
        UIManager.instance.UpdateBiodiversityUI(biodiversity);
    }
    public void OnDayPassed()
    {
        //requires data about the day, how many resources to award the player
    }
    public void AddFunds(int newFunds)
    {
        funds += newFunds;
        fundsUI.SetValue(funds);
        UIManager.instance.UpdateBiodiversityUI(funds);
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
    }
    public void SubtractPurity(int newPurity)
    {
        purity -= newPurity;
        purityUI.SetValue(purity);
        UIManager.instance.UpdatePurityUI(purity);
    }
    public void SubtractBiodiversity(int newBiodiversity)
    {
        biodiversity -= newBiodiversity;
        biodiversityUI.SetValue(biodiversity);
        UIManager.instance.UpdateBiodiversityUI(biodiversity);
    }
}
