using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager Instance { get; private set; }

    private readonly HashSet<string> collectedCoinIDs = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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


