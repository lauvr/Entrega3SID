using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Firebase.Database;
using System;

public class ButtonLogout : MonoBehaviour, IPointerClickHandler{

    DatabaseReference mDatabase;
    string userID;

    public event Action OnLogout;

    private void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //SetUserOnline();

        FirebaseAuth.DefaultInstance.SignOut();

        OnLogout?.Invoke();

        SceneManager.LoadScene("Home");
        Time.timeScale = 1;
    }

  
}