using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using System;
using Firebase.Database;
using Firebase.Extensions;

public class UsernameLabel : MonoBehaviour
{
    [SerializeField]
    private Text _label;

    private void Reset()
    {
        _label = GetComponent <Text>();   
    }

    // Start is called before the first frame update
    void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthChange;
    }

    private void HandleAuthChange(object sender, EventArgs e)
    {
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

        if (currentUser != null)
        {
            SetLabelUsername(currentUser.UserId);
        }

    }

    private void SetLabelUsername(string UserId)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + UserId + "/username")
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                    _label.text = "Null";
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log(snapshot.Value);

                    _label.text = (string)snapshot.Value;

                }
            });
    }

   
}
