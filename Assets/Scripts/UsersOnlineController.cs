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


    public GameObject templateUserOnline;
    GameObject g;

    public GameObject templateText;

    public GameObject userList;

    public List<string> onlineUsersList = new List<string>();

    bool isFirstTime;

    List<GameObject> playersList = new List<GameObject>();


    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
        _ButtonLogout = GameObject.Find("ButtonLogout").GetComponent<ButtonLogout>();
        _GameState.OnDataReady += InitUserOnlineController;
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        isFirstTime = true;
        //templateUserOnline = templateUserOnline.transform.gameObject;

        
    }

    private void Update()
    {
        CleanList();
        Vector3 p = new Vector3(0, 0, 0);
        foreach (string item in onlineUsersList)
        {
            g = Instantiate(templateUserOnline, userList.transform);
            g.transform.GetChild(0).GetComponent<Text>().text = item;
            //g.transform.GetChild(1).GetComponent<Button>().GetComponent<Text>().text = "hola";
            g.transform.localPosition = p;
            p += new Vector3(0, -25, 0);
            playersList.Add(g);
        }



        /*templateText.GetComponent<Text>().text = "";

        foreach (string item in onlineUsersList)
        {
            templateText.GetComponent<Text>().text += item + Environment.NewLine;
            Debug.Log("item: "+ item);
        }*/
    }


    public void CleanList()
    {
        foreach (GameObject item in playersList)
        {
            Destroy(item);
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

        onlineUsersList.Add(userConnected["username"].ToString());
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
        
        onlineUsersList.Remove(userDisconnected["username"].ToString());
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
