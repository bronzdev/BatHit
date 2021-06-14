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
    public List<Collider> others = new List<Collider>();
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
            else
            {
                Collider col = child.GetComponent<Collider>();
                if (col != null)
                {
                    others.Add(col);
                }
            }
        }
    }
    private void OnDestroy()
    {
        Ground.OnBlocksDestroyed -= OnBlocksDestroyed;
    }

    private void OnBlocksDestroyed(Transform block)
    {
        blocks.Remove(block);
        block.DOScale(Vector3.zero, shrinkSpeed).OnComplete(() => Destroy(block.gameObject));
        Hud.SetHudText?.Invoke("blocks.Count: " + blocks.Count);
        if (blocks.Count <= 0)
        {
            StartCoroutine(DestroyPlatfom());
        }
    }

    private IEnumerator DestroyPlatfom()
    {
        foreach (var item in others)
        {
            Destroy(item);
        }
        yield return new WaitForSeconds(1.5f);
        OnAllBlocksCleared?.Invoke();
        Destroy(gameObject);
    }
}
