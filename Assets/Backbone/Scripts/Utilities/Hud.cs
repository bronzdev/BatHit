using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public static Action<string> AddHudText;
    public static Action<string> SetHudText;
    public static Action<bool> OnShowDebugText;
    public static Action<bool> OnShowFpsText;
    [SerializeField] private RectTransform safeFrame;
    [SerializeField] private bool showDebug;
    [SerializeField] private bool printDebug;
    [SerializeField] private bool showFps;
    private Text debugText;
    private Text fpsText;
    private bool isFpsShowing = false;
    private const float fpsCalculateFrequency = 0.5f;

    private void Awake()
    {
        OnShowDebugText += ShowDebugText;
        OnShowFpsText += ShowFpsText;
        AddHudText += AddDebugText;
        SetHudText += SetDebugText;
        debugText = transform.GetChild(0).GetComponent<Text>();
        fpsText = transform.GetChild(1).GetComponent<Text>();
    }

    private void Start()
    {
        Rect safeRect = Screen.safeArea;
        safeFrame.rect.Set(safeRect.x, safeRect.y, safeRect.width, safeRect.height);
        if (showFps)
        {
            StartCoroutine(FPS());
            isFpsShowing = true;
        }
    }

    private void OnDestroy()
    {
        AddHudText -= AddDebugText;
        OnShowDebugText -= ShowDebugText;
        OnShowFpsText -= ShowFpsText;
        SetHudText -= SetDebugText;
    }



    private void ShowDebugText(bool isShowing)
    {
        showDebug = isShowing;
        if (!showDebug)
        {
            debugText.text = "";
        }
    }

    private void ShowFpsText(bool isShowing)
    {
        showFps = isShowing;
        if (showFps)
        {
            if (!isFpsShowing)
            {
                StartCoroutine(FPS());
            }
        }
        else
        {
            StopCoroutine(FPS());
            fpsText.text = "";
        }
    }

    private void SetDebugText(string text)
    {
        if (showDebug)
        {
            debugText.text = text;
        }
        if (printDebug)
        {
            Debug.Log(text);
        }
    }

    private void AddDebugText(string text)
    {
        if (showDebug)
        {
            debugText.text += "\n" + text;
        }
        if (printDebug)
        {
            Debug.Log(text);
        }
    }

    private IEnumerator FPS()
    {
        for (; ; )
        {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(fpsCalculateFrequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;
            fpsText.text = Mathf.RoundToInt(frameCount / timeSpan).ToString() + " ";
        }
    }
}