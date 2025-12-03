using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance { get; private set; }

    [HideInInspector] public InputControls inputControls;
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
