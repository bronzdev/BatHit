using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UiLevelManagerCanvas : MonoBehaviour
{
    public static Action OnShootBall;
    public static Action OnShootButtonClicked;
    public static Action<int> OnLevelCleared;
    [SerializeField] private Image[] levelsImage;
    [SerializeField] private Button shootButton;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private GameObject shotsRemainingPanel;
    [SerializeField] private TextMeshProUGUI shotsRemainingText;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI noBallCountdownText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject bonusLevelPanel;
    private Vector3 platformSpawnPosition = new Vector3(0, -3, 0);
    private GameObject mainPanel;
    private Platform currentPlatform;
    private bool areBallsEmpty;
    private float counterDown;
    private int semiLevelCounter;
    private int ballsRemaining;
    private bool isSemiLevelCleared;
    private int currentLevelScore;

    private void Awake()
    {
        UiLevelClearedCanvas.OnLevelClearedContinueButtonPressed += OnLevelClearedContinueButtonPressed;
        Ground.OnBlocksDestroyed += OnBlocksDestroyed;
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart += OnGameStart;
        PlayerController.OnGameOver += OnGameOver;
        shootButton.onClick.AddListener(ShootButtonClicked);
        mainPanel = transform.GetChild(0).gameObject;
        mainPanel.SetActive(false);
        if (Player.isPlayerDataLoaded)
        {
            StartNextLevel();
        }
    }

    private void OnDestroy()
    {
        UiLevelClearedCanvas.OnLevelClearedContinueButtonPressed -= OnLevelClearedContinueButtonPressed;
        Ground.OnBlocksDestroyed += OnBlocksDestroyed;
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart -= OnGameStart;
        PlayerController.OnGameOver -= OnGameOver;
        shootButton.onClick.RemoveListener(ShootButtonClicked);
    }

    private void Update()
    {
        if (areBallsEmpty && !isSemiLevelCleared)
        {
            counterDown -= Time.deltaTime;
            if (counterDown >= 0)
            {
                noBallCountdownText.text = counterDown.ToString("F1") + "s";
            }
            else
            {
                noBallCountdownText.text = "";
            }
        }
    }

    private void ToggleBallCountdownPanel()
    {
        countdownPanel.SetActive(areBallsEmpty);
    }

    private void ToggleBonusPanel()
    {
        if (semiLevelCounter == AppData.maxSemiLevel - 1)
        {
            bonusLevelPanel.SetActive(true);
            shotsRemainingPanel.SetActive(false);
        }
        else
        {
            bonusLevelPanel.SetActive(false);
            shotsRemainingPanel.SetActive(true);
        }
    }

    #region Score Stuff
    private void OnBlocksDestroyed(Transform obj)
    {
        SetCurrentLevelScore(1);
    }

    private void SetCurrentLevelScore(int value)
    {
        if (value == 0)
        {
            currentLevelScore = value;
        }
        else
        {
            currentLevelScore += value;
            UiScoreAdder.OnAddScore?.Invoke(value);
        }
        scoreText.text = "Score " + currentLevelScore;
    }

    private void RecordHighScore()
    {
        if (currentLevelScore > Player.GetHighScore())
        {
            Player.save.highScore = currentLevelScore;
        }
    }
    #endregion


    #region Balls/Shots stuff
    private void ShootButtonClicked()
    {
        if (ballsRemaining == 0)
        {
            return;
        }
        SetBallRemaining(ballsRemaining - 1);
    }

    private void SetBallRemaining(int value)
    {
        if (value <= 0)
        {
            value = 0;
            counterDown = AppData.watchAdCountdown;
            areBallsEmpty = true;
            ToggleBallCountdownPanel();
            StartCoroutine(OnBallOver());
        }
        if (!isSemiLevelCleared)
        {
            OnShootBall?.Invoke();
        }
        ballsRemaining = value;
        shotsRemainingText.text = AppData.ballIcon + "x" + ballsRemaining;
        shotsRemainingText.color = ballsRemaining <= 1 ? ColorConstants.RedColor : Color.white;
    }

    private IEnumerator OnBallOver()
    {
        yield return new WaitForSeconds(AppData.ballsOverCooldownTime);
        if (areBallsEmpty)
        {
            mainPanel.SetActive(false);
            PlayerController.OnGameOver?.Invoke();
        }
    }

    private void OnAllBlocksCleared()
    {
        SetCurrentLevelScore(ballsRemaining * AppData.remainingBallsPoints);
        currentPlatform.OnAllBlocksCleared -= OnAllBlocksCleared;
        isSemiLevelCleared = true;
        StartCoroutine(LoadNextSemiLevelAfterDelay());
    }
    #endregion


    private void OnLevelClearedContinueButtonPressed()
    {
        mainPanel.SetActive(true);
        StartNextLevel();
    }

    private void OnPlayerDataLoaded()
    {
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        //mainPanel.SetActive(true);
        semiLevelCounter = 0;
        RecordHighScore();
        SetCurrentLevelScore(0);
        InitNewLevel();
    }

    private void InitNewLevel()
    {
        currentLevelText.text = Player.save.currentLevel.ToString();
        int nextLevel = Player.save.currentLevel + 1;
        nextLevelText.text = nextLevel.ToString();
        for (int i = 0; i < levelsImage.Length; i++)
        {
            levelsImage[i].color = ColorConstants.UiLockedLevel;
        }
        levelsImage[0].color = ColorConstants.UiCurrentLevel;
        StartSemiLevel();
    }

    private void StartSemiLevel()
    {
        ToggleBonusPanel();
        areBallsEmpty = false;
        ToggleBallCountdownPanel();
        noBallCountdownText.text = "";
        string path = AppData.platformLevelPath + "" + semiLevelCounter + "/0";
        currentPlatform = Instantiate(Resources.Load<Platform>(path) as Platform, platformSpawnPosition, Quaternion.identity);
        currentPlatform.transform.DOMoveY(0, 0.5f);
        SetBallRemaining(currentPlatform.shots + 2);
        isSemiLevelCleared = false;
        if (currentPlatform != null)
        {
            currentPlatform.OnAllBlocksCleared += OnAllBlocksCleared;
        }
        else
        {
            print("platform is null");
        }
    }

    private void OnSemiLevelCleared(int semiLevelCounter)
    {
        levelsImage[semiLevelCounter - 1].color = ColorConstants.UiUnlockedLevel;
        levelsImage[semiLevelCounter].color = ColorConstants.UiCurrentLevel;
    }

    private IEnumerator LoadNextSemiLevelAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        semiLevelCounter++;
        if (semiLevelCounter >= AppData.maxSemiLevel)
        {
            Player.save.currentLevel++;
            OnLevelCleared?.Invoke(currentLevelScore);
            mainPanel.SetActive(false);
            Player.SaveGameUserData();
        }
        else
        {
            OnSemiLevelCleared(semiLevelCounter);
            StartSemiLevel();
        }
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