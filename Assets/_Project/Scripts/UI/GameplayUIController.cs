// Assets/_Project/Scripts/UI/GameplayUIController.cs
using UnityEngine;
using UnityEngine.UI; // Required for Button

public class GameplayUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject levelCompletePanel;

    [Header("Buttons")]
    [SerializeField] private Button returnToMenuButtonLevelComplete;

    [Header("Controller References")]
    [SerializeField] private MainMenuController mainMenuController; // Changed to SerializeField

    void Awake()
    {
        if (levelCompletePanel == null)
            Debug.LogError("LevelCompletePanel is not assigned in GameplayUIController.");
        else
            levelCompletePanel.SetActive(false); // Ensure it's hidden at start

        if (returnToMenuButtonLevelComplete == null)
            Debug.LogError("ReturnToMenuButton_LevelComplete is not assigned.");
        else
            returnToMenuButtonLevelComplete.onClick.AddListener(HandleReturnToMenu);

        if (mainMenuController == null) // Check Inspector assignment
        {
            Debug.LogError("MainMenuController is not assigned in the GameplayUIController Inspector slot.");
        }
    }

    public void ShowLevelCompletePanel()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            Time.timeScale = 0f; // Pause game when level complete message is up
        }
    }

    public void HideLevelCompletePanel()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
            // Time.timeScale will be set to 1f by MainMenuController or if we add a "Next Level"
        }
    }

    private void HandleReturnToMenu()
    {
        Debug.Log("Return to Menu button clicked from Level Complete panel.");
        HideLevelCompletePanel(); // Hide this panel
        gameObject.SetActive(false); // Hide the entire GameplayUICanvas

        if (LevelManager.Instance != null)
        {
            // LevelManager.Instance.ReturnToMainMenu() will use its own reference
            // to the mainMenuController, so this script doesn't strictly need to call it.
            // However, the LevelManager's call is what ultimately shows the menu.
            LevelManager.Instance.ReturnToMainMenu(); // LevelManager handles cleaning up level objects
        }
        else
        {
            Debug.LogError("LevelManager.Instance is null. Cannot properly return to menu.");
            // Fallback if LevelManager is missing, try to show menu directly
            if (mainMenuController != null)
            {
                mainMenuController.ShowMenu();
            }
        }
    }

    public void ShowGameplayUI()
    {
        gameObject.SetActive(true);
        HideLevelCompletePanel(); // Ensure sub-panels are in their default state
    }

    public void HideGameplayUI()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (returnToMenuButtonLevelComplete != null)
        {
            returnToMenuButtonLevelComplete.onClick.RemoveListener(HandleReturnToMenu);
        }
    }
}