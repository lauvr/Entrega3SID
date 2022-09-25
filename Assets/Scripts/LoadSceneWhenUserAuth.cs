using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using UnityEngine.SceneManagement;

public class LoadSceneWhenUserAuth : MonoBehaviour
{

    [SerializeField]
    private string _sceneToLoad;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChange;
    }

    private void HandleAuthStateChange(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            SceneManager.LoadScene(_sceneToLoad);
            Time.timeScale = 1;
        }
    }

}

    
