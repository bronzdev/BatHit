using System;
using UnityEngine;

public class PlatformCollider : MonoBehaviour
{
    public Action<int> OnColliders;
    internal int collidersOnPlatform;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(AppData.tag_block) || collision.gameObject.CompareTag(AppData.tag_bowlingPins))
        {
            CalculateBlocks(+1);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(AppData.tag_block) || collision.gameObject.CompareTag(AppData.tag_bowlingPins))
        {
            CalculateBlocks(-1);
        }
    }

    private void CalculateBlocks(int value)
    {
        collidersOnPlatform += value;
        OnColliders?.Invoke(collidersOnPlatform);
    }
}