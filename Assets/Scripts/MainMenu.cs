using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string firstRoomSceneName = "Room_1";
    [SerializeField] private bool isUpperExit;

    private void Start()
    {
        Scene scene = SceneManager.GetSceneByName(firstRoomSceneName);
        if (scene.IsValid() && scene.isLoaded)
            return;
        var sceneOneLoading = SceneManager.LoadSceneAsync(firstRoomSceneName, LoadSceneMode.Additive);
    }

    public bool IsUpperExit
    {
        get
        {
            return isUpperExit;
        }
        set
        {
            isUpperExit = value;
        }
    }
}
   