using System.Collections;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;

public class Requests : MonoBehaviour
{
    DatabaseReference mDatabase;
    GameState _GameState;

    private void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
    }
    public void SendFriendRequest()
    {
        StartCoroutine(IESendFriendRequest(gameObject.name));
    }

    public void DeleteFriendRequest()
    {
        StartCoroutine(IEDeleteFriendRequest(gameObject.name));
    }

    public void AddFriend()
    {
        StartCoroutine(IEAddFriend(gameObject.name));
    }

    IEnumerator IESendFriendRequest(string userId)
    {
        var databaseTask = mDatabase.Child("users").Child(userId).Child("friend-requests")
            .Child(_GameState.userID).SetValueAsync(_GameState.username);

        yield return new WaitUntil(() => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
    }
    IEnumerator IEDeleteFriendRequest(string userId)
    {
        var databaseTask = mDatabase.Child("users").Child(_GameState.userID)
            .Child("friend-requests").Child(userId).RemoveValueAsync();

        yield return new WaitUntil(() => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            Debug.Log("Friend request from " + _GameState.username + " deleted");
        }
    }
    IEnumerator IEAddFriend(string userId)
    {
        var databaseTask_1 = mDatabase.Child("users").Child(_GameState.userID)
            .Child("friends").Child(userId).SetValueAsync(_GameState.username);

        yield return new WaitUntil(() => databaseTask_1.IsCompleted);

        if (databaseTask_1.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask_1.Exception}");
        }
        else
        {
            var databaseTask_2 = mDatabase.Child("users").Child(userId).Child("friends")
                .Child(_GameState.userID).SetValueAsync(_GameState.username);
            yield return new WaitUntil(() => databaseTask_2.IsCompleted);

            if (databaseTask_2.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {databaseTask_2.Exception}");
            }
            else
            {
                StartCoroutine(IEDeleteFriendRequest(gameObject.name));
            }
        }
    }
}
