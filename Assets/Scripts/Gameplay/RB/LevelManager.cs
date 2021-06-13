using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public static Action OnNoBallsLeft;
    public static Action OnShootBall;
    public static Action<int> OnUpdateBalls;
    public static Action<int> OnSemiLevelCleared;
    public static Action OnLevelCleared;
    private int semiLevelCounter = 0;
    private Vector3 platformSpawnPosition = new Vector3(0, -3, 0);
    private int shotsRemaining;

    private void Awake()
    {
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiLevelManagerCanvas.OnShootButtonClicked += OnShootButtonClicked;
    }

    private void OnDestroy()
    {
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiLevelManagerCanvas.OnShootButtonClicked -= OnShootButtonClicked;
    }

    private void OnShootButtonClicked()
    {
        shotsRemaining--;
        OnUpdateBalls?.Invoke(shotsRemaining);
        if (shotsRemaining <= 0)
        {
            OnNoBallsLeft?.Invoke();
            StartCoroutine(OnBallOver());
        }
        else
        {
            OnShootBall?.Invoke();
        }
    }

    private IEnumerator OnBallOver()
    {
        yield return new WaitForSeconds(AppData.watchAdCountdown);

        PlayerController.OnGameOver?.Invoke();
    }

    private void OnPlayerDataLoaded()
    {
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        print("StartNextLevel");
        OnLevelCleared?.Invoke();
        semiLevelCounter = 0;
        StartSemiLevel();
    }

    private void StartSemiLevel()
    {
        print("Level " + Player.save.currentLevel + " SM: " + semiLevelCounter);
        string path = AppData.platformLevelPath + "" + semiLevelCounter + "/0";
        Platform platform = Instantiate(Resources.Load<Platform>(path) as Platform, platformSpawnPosition, Quaternion.identity);
        platform.transform.DOMoveY(0, 0.5f);
        shotsRemaining = platform.shots;
        OnUpdateBalls?.Invoke(shotsRemaining);
        if (platform != null)
        {
            platform.OnAllBlocksCleared += OnAllBlocksCleared;
        }
        else
        {
            print("platform is null");
        }
    }

    private void OnAllBlocksCleared()
    {
        StartCoroutine(LoadNextSemiLevelAfterDelay());
    }

    private IEnumerator LoadNextSemiLevelAfterDelay()
    {
        yield return new WaitForSeconds(1);
        //Show someting after semi level cleared
        semiLevelCounter++;
        if (semiLevelCounter >= AppData.maxSemiLevel)
        {
            Player.save.currentLevel++;
            StartNextLevel();
            Player.SaveGameUserData();
        }
        else
        {
            OnSemiLevelCleared?.Invoke(semiLevelCounter);
            StartSemiLevel();
        }
    }
}