using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class Platform : MonoBehaviour
{
    public Action OnAllBlocksCleared;
    [SerializeField] internal int shots = 4;
    public int collidersOnPlatform = 0;
    public List<Transform> blocks = new List<Transform>();
    public List<PlatformCollider> platformColliders = new List<PlatformCollider>();
    private const float shrinkSpeed = 1f;

    private void Awake()
    {
        Ground.OnBlocksDestroyed += OnBlocksDestroyed;
        foreach (Transform child in transform)
        {
            if (child.CompareTag(AppData.tag_block) || child.CompareTag(AppData.tag_bowlingPins))
            {
                blocks.Add(child);
            }
            PlatformCollider platformCollider = child.GetComponent<PlatformCollider>();
            if (platformCollider != null)
            {
                platformColliders.Add(platformCollider);
                platformCollider.OnColliders += OnColliders;
            }
        }
    }

    private void OnDestroy()
    {
        Ground.OnBlocksDestroyed -= OnBlocksDestroyed;
        for (int i = 0; i < platformColliders.Count; i++)
        {
            platformColliders[i].OnColliders -= OnColliders;
        }
    }

    private void OnBlocksDestroyed(Transform block)
    {
        blocks.Remove(block);
        block.DOScale(Vector3.zero, shrinkSpeed).OnComplete(() => Destroy(block.gameObject));
    }

    private void OnColliders(int colliders)
    {
        Hud.SetHudText?.Invoke("colliders.Count: " + colliders);
        collidersOnPlatform = colliders;
        if (collidersOnPlatform <= 0)
        {
            StartCoroutine(DestroyPlatfom());
        }
    }

    private IEnumerator DestroyPlatfom()
    {
        yield return new WaitForSeconds(1.5f);
        if (collidersOnPlatform > 0)
        {
            StartCoroutine(DestroyPlatfom());
            yield return null;
        }
        if (blocks.Count > 0)
        {
            StartCoroutine(DestroyPlatfom());
            yield return null;
        }
        yield return new WaitUntil(() => blocks.Count == 0);
        OnAllBlocksCleared?.Invoke();
        Destroy(gameObject);
    }
}
