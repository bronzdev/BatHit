using UnityEngine;
using DG.Tweening;
using System;

public class Ground : MonoBehaviour
{
    public static Action OnPinDrop;
    public static Action<Transform> OnBlocksDestroyed;

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case AppData.tag_block:
                OnBlockTouchedGound(collision);
                break;
            case AppData.tag_bowlingPins:
                UiGemsSpawnCanvas.OnSpawnSingleGem3D?.Invoke(collision.transform);
                OnPinDrop?.Invoke();
                OnBlockTouchedGound(collision);
                break;
            default:
                break;
        }
    }

    private void OnBlockTouchedGound(Collision collision)
    {
        Destroy(collision.gameObject.GetComponent<Rigidbody>());
        OnBlocksDestroyed?.Invoke(collision.transform);
    }
}