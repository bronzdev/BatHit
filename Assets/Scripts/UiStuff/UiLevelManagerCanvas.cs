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
    [SerializeField] private TextMeshProUGUI shotsRemainingText;
    [SerializeField] private TextMeshProUGUI noBallCountdownText;
    [SerializeField] private TextMeshProUGUI scoreText;
    private GameObject mainPanel;
    private bool areBallsEmpty;
    private float counter;
    private int semiLevelCounter = 0;
    private Vector3 platformSpawnPosition = new Vector3(0, -3, 0);
    private int ballsRemaining;
    private bool isSemiLevelCleared;
    private Platform currentPlatform;
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
            counter -= Time.deltaTime;
            if (counter >= 0)
            {
                noBallCountdownText.text = counter.ToString("F1") + "s";
            }
            else
            {
                noBallCountdownText.text = "";
            }
        }
        else
        {
            noBallCountdownText.text = "";
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
            counter = AppData.watchAdCountdown;
            areBallsEmpty = true;
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
        print(areBallsEmpty);
        yield return new WaitForSeconds(AppData.ballsOverCooldownTime);
        print(areBallsEmpty);
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
        areBallsEmpty = false;
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