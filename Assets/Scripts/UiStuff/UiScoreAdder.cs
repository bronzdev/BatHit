using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UiScoreAdder : MonoBehaviour
{
    public static Action<int> OnAddScore;
    [SerializeField] private TextMeshProUGUI scoreAdderTextPrefab;
    private RectTransform rect;
    private const float animSpeed = 0.5f;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        OnAddScore += AddScore;
    }

    private void OnDestroy()
    {
        OnAddScore -= AddScore;
    }

    private void AddScore(int points)
    {
        if (points<=0)
        {
            return;
        }
        TextMeshProUGUI point = Instantiate(scoreAdderTextPrefab, new Vector3(UnityEngine.Random.Range(rect.rect.xMin, rect.rect.xMax),
                    UnityEngine.Random.Range(rect.rect.yMin, rect.rect.yMax), 0) + rect.transform.position, Quaternion.identity, this.transform);
        if (points >= 50)
        {
            point.color = ColorConstants.RedColor;
        }
        point.GetComponent<RectTransform>().DOAnchorPosY(50, animSpeed).OnComplete(() => Destroy(point.gameObject));
        point.text = "+" + points;
    }
}