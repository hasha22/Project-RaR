using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

    [HideInInspector] public InputControls inputControls;

    private PlayerInput playerInput;

    [Header("Actions")]
    public bool hasPressedSpace = false;
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
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChange;

        if (inputControls != null)
        {
            inputControls.Disable();
        }
    }
    private void OnEnable()
    {
        if (inputControls == null)
        {
            inputControls = new InputControls();
            inputControls.Enable();
        }
    }
    public void OnMonitorScreenEnabled(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            hasPressedSpace = true;
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            inputControls.Enable();
        }
        else
        {
            inputControls.Disable();
        }
    }
    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        //Enables controls if loading into Game Scene, disables otherwise
        if (newScene.buildIndex == 1)
        {
            instance.enabled = true;
            if (inputControls != null)
            {
                inputControls.Enable();
            }
        }
        else
        {
            instance.enabled = false;
            if (inputControls != null)
            {
                inputControls.Disable();
            }
        }
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }
    public void DisableInput()
    {
        playerInput.enabled = false;
        hasPressedSpace = false;
    }
    public void EnableInput()
    {
        playerInput.enabled = true;
    }

}
