// Assets/_Project/Scripts/UI/MainMenuController.cs
using UnityEngine;
using UnityEngine.UI; // Required for Button

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startGameButton;
    
    [Header("Controller References")]
    [SerializeField] private GameplayUIController gameplayUIController; // Changed to SerializeField

    void Start()
    {
        if (startGameButton == null)
        {
            Debug.LogError("StartGameButton is not assigned in the MainMenuController script.");
        }
        else
        {
            startGameButton.onClick.AddListener(HandleStartGame);
        }

        // Remove the FindAnyObjectByType call
        if (gameplayUIController == null) // Check Inspector assignment
        {
            Debug.LogError("GameplayUIController is not assigned in the MainMenuController Inspector slot.");
        }
        else
        {
            gameplayUIController.HideGameplayUI();
        }

        ShowMenu(); // Call ShowMenu to set initial state correctly (Time.timeScale = 0, etc.)
    }

    private void HandleStartGame()
    {
        Debug.Log("Start Game button clicked!");
        gameObject.SetActive(false); // Hide this StartMenuCanvas

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.StartLevel(); // This now also shows GameplayUI and sets Time.timeScale = 1f
        }
        else
        {
            Debug.LogError("LevelManager.Instance is null. Ensure LevelManager is in the scene and initialized.");
            Time.timeScale = 1f; // Manually unpause if LevelManager fails, for safety
        }
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause game
        
        // When showing main menu, ensure gameplay UI is hidden
        if (gameplayUIController != null)
        {
            gameplayUIController.HideGameplayUI();
        }

        if (InputManager.Instance != null)
        {
            InputManager.Instance.SetGamePlayingState(false); // Game is not playing when menu is up
        }
    }

    private void OnDestroy()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveListener(HandleStartGame);
        }
    }
}