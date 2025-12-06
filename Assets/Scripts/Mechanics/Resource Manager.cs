using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance { get; private set; }

    [Header("Player Resources")]
    public int funds;
    public int purity;
    public int biodiversity;
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
    public void OnDayPassed()
    {
        //requires data about the day, how many resources to award the player
    }
    public void AddFunds(int newFunds)
    {
        funds += newFunds;
        UIManager.instance.UpdateBiodiversityUI(funds);
    }
    public void AddPurity(int newPurity)
    {
        purity += newPurity;
        UIManager.instance.UpdatePurityUI(purity);
    }
    public void AddBiodiversity(int newBiodiversity)
    {
        biodiversity += newBiodiversity;
        UIManager.instance.UpdateBiodiversityUI(biodiversity);
    }
    public void SubtractFunds(int newFunds)
    {
        funds -= newFunds;
        UIManager.instance.UpdateFundsUI(funds);
    }
    public void SubtractPurity(int newPurity)
    {
        purity -= newPurity;
        UIManager.instance.UpdatePurityUI(purity);
    }
    public void SubtractBiodiversity(int newBiodiversity)
    {
        biodiversity -= newBiodiversity;
        UIManager.instance.UpdateBiodiversityUI(biodiversity);
    }
}
