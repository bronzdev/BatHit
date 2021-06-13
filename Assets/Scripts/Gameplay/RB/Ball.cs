using UnityEngine;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(Destroy), 2);
    }

    private void Destroy()
    {
        transform.DOScale(Vector3.zero, 1).OnComplete(() => Destroy(gameObject));
    }
}