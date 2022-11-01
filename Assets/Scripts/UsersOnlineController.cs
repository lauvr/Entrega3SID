using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Auth;
using Firebase;

public class UsersOnlineController : MonoBehaviour
{
    DatabaseReference mDatabase;
    GameState _GameState;
    string userID;
    [SerializeField]
    ButtonLogout _ButtonLogout;



    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
        _ButtonLogout = GameObject.Find("ButtonLogout").GetComponent<ButtonLogout>();
        _GameState.OnDataReady += InitUserOnlineController;
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        /*FirebaseDatabase.DefaultInstance
            .GetReference("users-online")
            .ValueChanged += HandleValueChanged;*/
    }

    public void InitUserOnlineController()
    {
        FirebaseDatabase.DefaultInstance.LogLevel = LogLevel.Verbose;
        Debug.Log("Init users online controller");
        _ButtonLogout.OnLogout += SetUserOffline;
        var userOnlineRef = FirebaseDatabase.DefaultInstance.GetReference("users-online");
        mDatabase.Child("users-online").ChildAdded += HandleChildAdded;
        mDatabase.Child("users-online").ChildRemoved += HandleChildRemoved;
        SetUserOnline();

        /*FirebaseDatabase.DefaultInstance
            .GetReference("users-online")
            .ValueChanged += HandleValueChanged;*/

        //GameObject.Find("LogoutButton").GetComponent<ButtonLogout>().OnLogout 
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userConnected = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userConnected["username"] + " is online");
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userDisconnected = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userDisconnected["username"] + " is offline");
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.Log(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> usersList = (Dictionary<string, object>)args.Snapshot.Value;
        if (usersList != null)
        {
            foreach (var userDoc in usersList)
            {
                Dictionary<string, object> userOnline = (Dictionary<string, object>)userDoc.Value;
                Debug.Log("ONLINE:" + userOnline["username"]);
            }
        }
    }

    private void SetUserOnline()
    {
        mDatabase.Child("users-online").Child(userID).Child("username").SetValueAsync(_GameState.username);
    }

    private void SetUserOffline()
    {
        mDatabase.Child("users-online").Child(userID).SetValueAsync(null);
    }

    void OnApplicationQuit()
    {
        SetUserOffline();
    }

    

}
