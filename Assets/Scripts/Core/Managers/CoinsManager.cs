using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : Singleton<CoinsManager>
{
    private readonly HashSet<string> collectedCoinIDs = new();

    protected override void Awake()
    {
        base.Awake();
        if (I != this) return;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkCollected(string coinID)
    {
        collectedCoinIDs.Add(coinID);
    }

    public bool IsCollected(string coinID)
    {
        return collectedCoinIDs.Contains(coinID);
    }
}


