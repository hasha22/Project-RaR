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
}
