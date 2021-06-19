using UnityEngine;
using DG.Tweening;
using System;

public class Ground : MonoBehaviour
{
    public static Action<Transform> OnBlocksDestroyed;

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case AppData.tag_block:
                OnBlocksTouchedGound(collision);
                break;
            case AppData.tag_bowlingPins:
                UiGemsSpawnCanvas.OnSpawnSingleGem3D?.Invoke(collision.transform);
                OnBlocksTouchedGound(collision);
                break;
            default:
                break;
        }
    }

    private void OnBlocksTouchedGound(Collision collision)
    {
        Destroy(collision.gameObject.GetComponent<Rigidbody>());
        OnBlocksDestroyed?.Invoke(collision.transform);
    }
}