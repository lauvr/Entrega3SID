﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using UnityEngine.SceneManagement;
using Firebase.Database;


public class LoadSceneWhenUserAuth : MonoBehaviour
{

    [SerializeField]
    private string _sceneToLoad;

    DatabaseReference mDatabase;
    string userID;

    // Start is called before the first frame update
    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChange;
    }

    private void HandleAuthStateChange(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            SetUserOnline();
            SceneManager.LoadScene(_sceneToLoad);
            Time.timeScale = 1;
        }
    }

    private void SetUserOnline()
    {
        UserData data = new UserData();

        data.online = false;
        string json = JsonUtility.ToJson(data);
        string userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        mDatabase.Child("users").Child(userID).Child("online").SetValueAsync(true);
    }

}

    
