using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Firebase.Database;

public class ButtonLogout : MonoBehaviour, IPointerClickHandler{

    DatabaseReference mDatabase;
    string userID;

    private void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetUserOnline();
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("Home");
        Time.timeScale = 1;
    }

    private void SetUserOnline()
    {
        UserData data = new UserData();

        data.online = false;
        string json = JsonUtility.ToJson(data);

        mDatabase.Child("users").Child(userID).Child("online").SetValueAsync(false);
    }
}