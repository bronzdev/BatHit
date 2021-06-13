using UnityEngine;
using DG.Tweening;
using System;

public class Ground : MonoBehaviour
{
    public static Action<Transform> OnBlocksDestroyed;
    private const float shrinkSpeed = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case AppData.tag_block:
                //collision.transform.DOScale(Vector3.zero, shrinkSpeed).OnComplete(() => Destroy(collision.gameObject));
                OnBlocksDestroyed?.Invoke(collision.transform);
                break;
            case AppData.tag_bowlingPins:
                UiGemsSpawnCanvas.OnSpawnSingleGem3D?.Invoke(collision.transform);
                //collision.transform.DOScale(Vector3.zero, shrinkSpeed).OnComplete(() => Destroy(collision.gameObject));
                OnBlocksDestroyed?.Invoke(collision.transform);
                break;
            default:
                break;
        }
    }
}