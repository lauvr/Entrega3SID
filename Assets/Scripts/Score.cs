using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Linq;

public class Score : MonoBehaviour {

    DatabaseReference mDatabase;
    string UserID;

    public int playerScore;
    public int highestScore;
    public int scoreInt;


    public Text scoreDisplay;
    public Dictionary<string, object> userObject;

    public Text leadersText;

    public GameObject leaderPanel;


    private void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        UserID = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        

    }

    private void Update()
    {
        scoreDisplay.text = playerScore.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerScore++;
        Destroy(other.gameObject);
    }

    public void WriteNewScore(int score)
    {

        SetUserHighscore();
       
        /*data.score = highestScore;
        data.username = username;
        string json = JsonUtility.ToJson(data);

        mDatabase.Child("users").Child(UserID).SetRawJsonValueAsync(json);*/
    }

   
    public void SetUserHighscore()
    {
        
        FirebaseDatabase.DefaultInstance
           .GetReference("users/" + UserID)
           .GetValueAsync().ContinueWithOnMainThread(task => {
               //Debug.Log("entrando al metodo" + task);
               if (task.IsFaulted)
               {
                   Debug.Log(task.Exception);
                   //Debug.Log("faulted");
               }
               else if (task.IsCompleted)
               {
                   try
                   {
                       //Debug.Log("completed");
                       DataSnapshot snapshot = task.Result;
                       Debug.Log("snapshot value: " + snapshot.Value);

                       var data = (Dictionary<string, object>)snapshot.Value;
                       //Debug.Log("data" + data["score"]);

                  

                       highestScore = Convert.ToInt32(data["score"]);
                       var username = Convert.ToString(data["username"]);
                       //Debug.Log("Highscore: " + highestScore);
                      // Debug.Log("Playerscore: " + playerScore);
                      // Debug.Log("Username: " + username);

                       UserData update = new UserData();
                       if (playerScore >= highestScore)
                       {
                           update.score = playerScore;
                           update.username = username;
                           string json = JsonUtility.ToJson(update);

                           mDatabase.Child("users").Child(UserID).SetRawJsonValueAsync(json);
                           highestScore = playerScore;
                       }
                       }
                   catch (Exception e)
                   {
                       Debug.Log(e);
                   }
                   
               }
           });
    }

    public void GetUserScore()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + UserID)
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log(snapshot.Value);

                    var data = (Dictionary<string, object>)snapshot.Value;
                    highestScore = (int)data["score"];
                    
                    Debug.Log("Puntaje: " + data["score"]);

/*                    foreach (var userDoc in (Dictionary<string,object>) snapshot.Value)
                    {
                        Debug.Log(userDoc.Key);
                        Debug.Log(userDoc.Value);
                    }*/
                   
                }
            });
    }


    public void GetLeaders()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").LimitToLast(5).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                //Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //Debug.Log(snapshot);
                leaderPanel.SetActive(true);

                foreach (var userDoc in (Dictionary<string, object>)snapshot.Value)
                {
                        
                    userObject = (Dictionary<string, object>)userDoc.Value;
                    //var orderedScoresList = userObject.Values.OrderByDescending(x => ((Dictionary<string, object>)x)["score"]);
                    Debug.Log(userObject["username"] + ":" + userObject["score"]);
                        
                    leadersText.text += (userObject["username"] + ":" + userObject["score"] + Environment.NewLine);
                }
                
            }
        });
    }

    public void ClosePanel()
    {
        leaderPanel.SetActive(false);
        leadersText.text = "";
    }

    public void ScoretoInt()
    {
        scoreInt = Convert.ToInt32(userObject["score"]);
        //Debug.Log("Score to Int: " + scoreInt);


    }

   


}

public class UserData
{
   public int score;
   public string username;
}

