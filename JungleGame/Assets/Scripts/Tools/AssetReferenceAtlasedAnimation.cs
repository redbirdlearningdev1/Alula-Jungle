using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetReferenceAtlasedAnimation : MonoBehaviour
{
    public List<AssetReferenceAtlasedSprite> animationFrames = new List<AssetReferenceAtlasedSprite>();
    List<AsyncOperationHandle<Sprite>> animationHandles = new List<AsyncOperationHandle<Sprite>>();

    public void LoadAnimation()
    {
        StartCoroutine(LoadAnimationRoutine());
    }

    IEnumerator LoadAnimationRoutine()
    {
        animationHandles.Clear();

        foreach (AssetReferenceAtlasedSprite frame in animationFrames)
        {
            AsyncOperationHandle<Sprite> handle = frame.LoadAssetAsync<Sprite>();
            animationHandles.Add(handle);

            yield return handle;
        }
    }

    public void UnloadAnimation()
    {
        if (animationHandles.Count == animationFrames.Count)
        {
            foreach (AsyncOperationHandle<Sprite> handle in animationHandles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
        }
    }
}
