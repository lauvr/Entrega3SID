using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using UnityEngine.UI;

public class UsersOnlineController : MonoBehaviour
{
    DatabaseReference mDatabase;
    GameState _GameState;
    string userID;
    [SerializeField]
    ButtonLogout _ButtonLogout;
    

   
    public GameObject templateText;
    public List<string> onlineUsersList = new List<string>();
    



    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
        _ButtonLogout = GameObject.Find("ButtonLogout").GetComponent<ButtonLogout>();
        _GameState.OnDataReady += InitUserOnlineController;
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        
    }

    private void Update()
    {
        templateText.GetComponent<Text>().text = "";

        foreach (string item in onlineUsersList)
        {
            templateText.GetComponent<Text>().text += item + Environment.NewLine;
        }
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
        //onlineText.text = userConnected["username"].ToString();

        
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

        onlineUsersList.Add(_GameState.username);
        
    }

    private void SetUserOffline()
    {
        onlineUsersList.Remove(_GameState.username);
        
        mDatabase.Child("users-online").Child(userID).SetValueAsync(null);
    }

    void OnApplicationQuit()
    {
        SetUserOffline();
    }

  
}
