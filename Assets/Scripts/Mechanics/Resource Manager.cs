using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance { get; private set; }

    [Header("Player Resources")]
    public int playerFunds;
    public int waterPurity;
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
    public void OnDialogueChoice(bool answer)
    {
        // requires data about the choice
        //impacts resources based on choice
        if (answer)
        {

        }
        else
        {

        }
    }
    public void OnDayPassed()
    {
        //requires data about the day, how many resources to award the player
    }
    public void AddFunds()
    {

    }
    public void AddPurity()
    {

    }
    public void AddBiodiversity()
    {

    }
    public void SubtractFunds()
    {

    }
    public void SubtractWaterPurity()
    {

    }
    public void SubtractBiodiversity()
    {

    }
}
