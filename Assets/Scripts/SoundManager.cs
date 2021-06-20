using System;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource batHit;
    [SerializeField] private AudioSource blockDrop;
    [SerializeField] private AudioSource gotGem;
    [SerializeField] private AudioSource gameOver;
    [SerializeField] private AudioSource levelClear;
    [SerializeField] private AudioSource bossLevelClear;
    [SerializeField] private bool isSound;
    [SerializeField] private bool isVibrate;

    protected override void Awake()
    {
        base.Awake();
        Ground.OnBlocksDestroyed += OnBlocksDestroyed;
        Ground.OnPinDrop += OnPinDrop;
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiLevelManagerCanvas.OnAllBallOver += OnAllBallOver;
        UiLevelManagerCanvas.OnShootBall += OnShootBall;
        UiLevelManagerCanvas.OnLevelCleared += OnLevelCleared;
        UiLevelManagerCanvas.OnBossLevelCleared += OnBossLevelCleared;
        UiLevelClearedCanvas.OnLevelClearedContinueButtonPressed += OnLevelClearedContinueButtonPressed;
        UiSettingsCanvas.IsSoundEnabled += IsSoundEnabledToggle;
        UiSettingsCanvas.IsVibrateEnabled += IsVibrateEnabledToggle;
    }

    private void OnDestroy()
    {
        Ground.OnBlocksDestroyed -= OnBlocksDestroyed;
        Ground.OnPinDrop -= OnPinDrop;
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiLevelManagerCanvas.OnAllBallOver -= OnAllBallOver;
        UiLevelManagerCanvas.OnShootBall -= OnShootBall;
        UiLevelManagerCanvas.OnLevelCleared -= OnLevelCleared;
        UiLevelManagerCanvas.OnBossLevelCleared -= OnBossLevelCleared;
        UiLevelClearedCanvas.OnLevelClearedContinueButtonPressed -= OnLevelClearedContinueButtonPressed;
        UiSettingsCanvas.IsSoundEnabled -= IsSoundEnabledToggle;
        UiSettingsCanvas.IsVibrateEnabled -= IsVibrateEnabledToggle;
    }

    private void OnLevelClearedContinueButtonPressed()
    {
        levelClear.Stop();
    }

    private void OnShootBall()
    {
        if (isSound)
        {
            batHit.Play();
        }
    }

    private void OnBlocksDestroyed(Transform obj)
    {
        if (isSound)
        {
            blockDrop.Play();
        }
    }

    private void OnAllBallOver()
    {
        if (isSound)
        {
            gameOver.Play();
        }
        if (isVibrate)
        {
            Handheld.Vibrate();
        }
    }

    private void OnPinDrop()
    {
        if (isSound)
        {
            gotGem.Play();
        }
    }

    private void OnLevelCleared(int a)
    {
        if (isSound)
        {
            levelClear.Play();
        }
    }

    private void OnBossLevelCleared()
    {
        if (isSound)
        {
            bossLevelClear.Play();
        }
    }

    #region Settings stuff
    private void IsSoundEnabledToggle(bool isSound)
    {
        this.isSound = isSound;
    }

    private void IsVibrateEnabledToggle(bool isVibrate)
    {
        this.isVibrate = isVibrate;
    }

    private void OnPlayerDataLoaded()
    {
        isSound = Player.save.isSoundEnabled;
        isVibrate = Player.save.isVibrateEnabled;
    }
    #endregion
}