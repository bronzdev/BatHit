using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiLevelManagerCanvas : MonoBehaviour
{
    public static Action OnShootButtonClicked;
    [SerializeField] private Image[] levelsImage;
    [SerializeField] private Button shootButton;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private TextMeshProUGUI shotsRemainingText;
    [SerializeField] private TextMeshProUGUI noBallCountdownText;
    private GameObject mainPanel;
    private bool areNoBallsLeft;
    private float counter;

    private void Awake()
    {
        shootButton.onClick.AddListener(ShootButtonClicked);
        LevelManager.OnSemiLevelCleared += OnSemiLevelCleared;
        LevelManager.OnLevelCleared += OnLevelCleared;
        LevelManager.OnUpdateBalls += OnUpdateBalls;
        LevelManager.OnNoBallsLeft += OnNoBallsLeft;
        UiStartCanvas.OnGameStart += OnGameStart;
        PlayerController.OnGameOver += OnGameOver;
        mainPanel = transform.GetChild(0).gameObject;
        mainPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        shootButton.onClick.RemoveListener(ShootButtonClicked);
        LevelManager.OnSemiLevelCleared -= OnSemiLevelCleared;
        LevelManager.OnLevelCleared -= OnLevelCleared;
        LevelManager.OnUpdateBalls -= OnUpdateBalls;
        LevelManager.OnNoBallsLeft += OnNoBallsLeft;
        UiStartCanvas.OnGameStart -= OnGameStart;
        PlayerController.OnGameOver -= OnGameOver;
    }

    private void OnNoBallsLeft()
    {
        counter = AppData.watchAdCountdown;
        areNoBallsLeft = true;
    }

    private void Update()
    {
        if (areNoBallsLeft)
        {
            counter -= Time.deltaTime;
            if (counter <= 0)
            {
                areNoBallsLeft = false;
                noBallCountdownText.text = "";
            }
            else
            {
                noBallCountdownText.text = counter.ToString("F1") + "s";
            }
        }
    }

    private void ShootButtonClicked()
    {
        OnShootButtonClicked?.Invoke();
    }

    private void OnUpdateBalls(int shots)
    {
        shotsRemainingText.text = "x" + shots;
        if (shots <= 2)
        {
            shotsRemainingText.color = ColorConstants.RedColor;
        }
        else
        {
            shotsRemainingText.color = Color.white;
        }
    }

    private void InitLevelItems()
    {
        currentLevelText.text = Player.save.currentLevel.ToString();
        currentLevelText.color = ColorConstants.UiUnlockedLevel;
        int nextLevel = Player.save.currentLevel + 1;
        nextLevelText.text = nextLevel.ToString();
        for (int i = 0; i < levelsImage.Length; i++)
        {
            levelsImage[i].color = ColorConstants.UiLockedLevel;
        }
        levelsImage[0].color = ColorConstants.UiCurrentLevel;
    }

    private void OnLevelCleared()
    {
        InitLevelItems();
    }

    private void OnSemiLevelCleared(int semiLevelCounter)
    {
        levelsImage[semiLevelCounter - 1].color = ColorConstants.UiUnlockedLevel;
        levelsImage[semiLevelCounter].color = ColorConstants.UiCurrentLevel;
    }

    private void OnGameOver()
    {
        mainPanel.SetActive(false);
    }

    private void OnGameStart()
    {
        mainPanel.SetActive(true);
    }
}