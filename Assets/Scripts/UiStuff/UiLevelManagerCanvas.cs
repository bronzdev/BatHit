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
    [SerializeField] private Image[] levelsImage;
    [SerializeField] private Button shootButton;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private TextMeshProUGUI shotsRemainingText;
    [SerializeField] private TextMeshProUGUI noBallCountdownText;
    private GameObject mainPanel;
    private bool areBallsEmpty;
    private float counter;
    private int semiLevelCounter = 0;
    private Vector3 platformSpawnPosition = new Vector3(0, -3, 0);
    private int shotsRemaining;
    private bool isSemiLevelCleared;
    private Platform currentPlatform;

    private void Awake()
    {
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

    private void OnPlayerDataLoaded()
    {
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        print("StartNextLevel");
        semiLevelCounter = 0;
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
        BallsRemaining = currentPlatform.shots + 4;
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

    private void ShootButtonClicked()
    {
        if (BallsRemaining == 0)
        {
            return;
        }
        BallsRemaining--;
    }

    private int BallsRemaining
    {
        get
        {
            return shotsRemaining;
        }
        set
        {
            if (value <= 0)
            {
                value = 0;
                counter = AppData.watchAdCountdown;
                areBallsEmpty = true;
                StartCoroutine(OnBallOver());
            }
            else if (!isSemiLevelCleared)
            {
                OnShootBall?.Invoke();
            }
            shotsRemaining = value;
            shotsRemainingText.text = AppData.ballIcon + "x" + BallsRemaining;
            shotsRemainingText.color = BallsRemaining <= 1 ? ColorConstants.RedColor : Color.white;
        }
    }

    private IEnumerator OnBallOver()
    {
        print(areBallsEmpty);
        yield return new WaitForSeconds(AppData.ballsOverCooldownTime);
        print(areBallsEmpty);
        if (areBallsEmpty)
        {
            PlayerController.OnGameOver?.Invoke();
        }
    }

    private void OnAllBlocksCleared()
    {
        currentPlatform.OnAllBlocksCleared -= OnAllBlocksCleared;
        isSemiLevelCleared = true;
        StartCoroutine(LoadNextSemiLevelAfterDelay());
    }

    private IEnumerator LoadNextSemiLevelAfterDelay()
    {
        yield return new WaitForSeconds(1);
        semiLevelCounter++;
        if (semiLevelCounter >= AppData.maxSemiLevel)
        {
            Player.save.currentLevel++;
            StartNextLevel();
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