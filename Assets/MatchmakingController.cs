﻿using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchmakingController : MonoBehaviour
{
    [SerializeField]
    Button matchmakingB, cancelMatchmakingB;

    [SerializeField]
    Text matchFoundText;

    DatabaseReference mDatabase;
    string UserId;

    GameState _GameState;

    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
        _GameState.OnDataReady += InitUsersOnMatchmakingController;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        cancelMatchmakingB.gameObject.SetActive(false);
        matchFoundText.gameObject.SetActive(false);
    }

    public void InitMatchmaking()
    {
        matchmakingB.gameObject.SetActive(false);
        cancelMatchmakingB.gameObject.SetActive(true);

        SetUserMatchmaking();
    }

    public void CancelMatchmaking()
    {
        cancelMatchmakingB.gameObject.SetActive(false);
        matchmakingB.gameObject.SetActive(true);

        SetUserOffMatchmaking();
    }

    public void InitUsersOnMatchmakingController()
    {
        var userOnMatchmakingRef = FirebaseDatabase.DefaultInstance.GetReference("matchmaking-queue");

        mDatabase.Child("matchmaking-queue").ChildAdded += HandleChildAdded;
        mDatabase.Child("matchmaking-queue").ChildRemoved += HandleChildRemoved;
        mDatabase.Child("matchmaking-queue").ValueChanged += HandleValueChanged;
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userConnectedToQueue = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userConnectedToQueue["username"] + " is on a queue");
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userDisconnectedFromQueue = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userDisconnectedFromQueue["username"] + " is off the queue");
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.Log(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> usersList = (Dictionary<string, object>)args.Snapshot.Value;

        if (usersList != null && usersList.Count >= 2)
        {
            SetGameMatch();
            matchFoundText.gameObject.SetActive(true);
            matchFoundText.text = "match found with: ";

            foreach (var userDoc in usersList)
            {
                Dictionary<string, object> userOnQueue = (Dictionary<string, object>)userDoc.Value;
                matchFoundText.text += " " + userOnQueue["username"].ToString();
            }
        }
    }

    private void SetUserMatchmaking()
    {
        mDatabase.Child("matchmaking-queue").Child(UserId).Child("username").SetValueAsync(_GameState.username);
    }

    private void SetUserOffMatchmaking()
    {
        mDatabase.Child("matchmaking-queue").Child(UserId).SetValueAsync(null);
    }

    private void SetGameMatch()
    {
        mDatabase.Child("match").Child(UserId).Child("username").SetValueAsync(_GameState.username);
    }

    private void UnsetGameMatch()
    {
        mDatabase.Child("match").Child(UserId).SetValueAsync(null);
    }

    void OnApplicationQuit()
    {
        SetUserOffMatchmaking();
        UnsetGameMatch();
    }
}