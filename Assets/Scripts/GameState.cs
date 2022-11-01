using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public int score;
    public string username;
    public string userID;

    DatabaseReference mDatabase;

    public event Action OnDataReady;

    // Start is called before the first frame update
    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        GetUsserData();
    }

    private void GetUsserData()
    {
        Debug.Log("Get user data " + userID);
        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + userID)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    Dictionary<string, object> userData = (Dictionary<string, object>)snapshot.Value;


                    Debug.Log(@"user connected: "
                        + "username: " + userData["username"]
                        + "\nscore: " + userData["score"]);
                    
                    username = Convert.ToString(userData["username"]);
                    score = Convert.ToInt32(userData["score"]);

                    OnDataReady?.Invoke();
                }
            });
    }

    
}
