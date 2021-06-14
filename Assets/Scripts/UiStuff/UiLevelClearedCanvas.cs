using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UiLevelClearedCanvas : MonoBehaviour
{
    public static Action OnLevelClearedContinueButtonPressed;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button continueButton;
    private GameObject mainPanel;

    private void Awake()
    {
        UiLevelManagerCanvas.OnLevelCleared += OnLevelCleared;
        mainPanel = transform.GetChild(0).gameObject;
        mainPanel.SetActive(false);
        continueButton.onClick.AddListener(OnContinueButtonPressed);
    }

    private void OnDestroy()
    {
        UiLevelManagerCanvas.OnLevelCleared -= OnLevelCleared;
        continueButton.onClick.RemoveListener(OnContinueButtonPressed);
    }

    private void OnContinueButtonPressed()
    {
        OnLevelClearedContinueButtonPressed?.Invoke();
        mainPanel.SetActive(false);
    }

    private void OnLevelCleared(int score)
    {
        if (score > Player.GetHighScore())
        {
            scoreText.text = "New High Score " + score;
        }
        else
        {
            scoreText.text = "Score " + score;
        }
        mainPanel.SetActive(true);
    }
}