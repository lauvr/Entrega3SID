using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Extensions;

public class LoadSceneWhenUserAuth : MonoBehaviour
{

    [SerializeField]
    private string _sceneToLoad;

    DatabaseReference mDatabase;
    string userID;
    string username;

    // Start is called before the first frame update
    void Start()
    {
        //mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChange;
        
    }

    private void HandleAuthStateChange(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            //SetUserOnline();
            SceneManager.LoadScene("Lobby");
            Time.timeScale = 1;
        }
    }
 
/*
    private void SetUserOnline()
    {
        OnlineUsersData data = new OnlineUsersData();

        Debug.Log("Set User Online");
        string userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        

        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser != null)
        {
            FirebaseDatabase.DefaultInstance
             .GetReference("users/" + userID + "/username")
             .GetValueAsync().ContinueWithOnMainThread(task => {
                 if (task.IsFaulted)
                 {
                     Debug.Log(task.Exception);
                     username = "Null";
                 }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot snapshot = task.Result;
                     Debug.Log("imprimiendo username " + snapshot.Value);

                     username = (string)snapshot.Value;

                 }
             });
        }

        data.username = username;

        Debug.Log("username = " + username);
        Debug.Log("Data.username = " + data.username);
        string json = JsonUtility.ToJson(data);

        mDatabase.Child("users-online").Child(userID).SetRawJsonValueAsync(json);
    }*/

    
}

    
