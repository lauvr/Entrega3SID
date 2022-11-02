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
    string connectedUserID;
    [SerializeField]
    ButtonLogout _ButtonLogout;
    Button addButton;

    public GameObject templateUserOnline;
    public GameObject templateRequests;
    public GameObject templateFriends;

    public Transform onlineTransform;
    public Transform requestsTransform;
    public Transform friendsTransform;

    Dictionary<string, object> onlineUsersList;
    Dictionary<string, object> requestsList;
    Dictionary<string, object> friendsList;

    List<GameObject> playersList = new List<GameObject>();


    /*//requests
    public GameObject reqList;
    public GameObject templateRequests;
    GameObject h;
    List<GameObject> requestsList = new List<GameObject>();
    public List<string> usersRequestingList = new List<string>();*/


    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
        _ButtonLogout = GameObject.Find("ButtonLogout").GetComponent<ButtonLogout>();
        _GameState.OnDataReady += InitUserOnlineController;
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        FirebaseDatabase.DefaultInstance.GetReference("users-online").ValueChanged += InstantiateUsersOnline;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + _GameState.userID + "/friend-requests").ValueChanged += InstantiateRequests;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + _GameState.userID + "/friends").ValueChanged += InstantiateFriends;
    }

    private void Update()
    {

    }

    void InstantiateUsersOnline(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
        }
        else if (args.Snapshot.Value != null)
        {
            for (int i = 0; i < onlineTransform.childCount; i++)
            {
                Destroy(onlineTransform.GetChild(i).gameObject);
            }

            onlineUsersList = (Dictionary<string, object>)args.Snapshot.Value;
            Vector3 p = new Vector3(0, 0, 0);
            foreach (var item in onlineUsersList)
            {
                if (item.Key != userID)
                {
                    var g = Instantiate(templateUserOnline, onlineTransform.transform);
                    g.transform.GetChild(0).GetComponent<Text>().text = item.Key.ToString();
                    //addButton = g.transform.GetChild(1).GetComponent<Button>();
                    g.name = item.Key;
                    g.transform.localPosition = p;
                    p += new Vector3(0, -25, 0);
                }
            }

        }
    }

    void InstantiateRequests(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
        }
        else if (args.Snapshot.Value != null)
        {
            for (int i = 0; i < requestsTransform.childCount; i++)
            {
                Destroy(requestsTransform.GetChild(i).gameObject);
            }

            requestsList = (Dictionary<string, object>)args.Snapshot.Value;
            
            Vector3 p = new Vector3(0, 0, 0);
            foreach (var item in requestsList)
            {
                var g = Instantiate(templateRequests, requestsTransform.transform);
                g.transform.GetChild(0).GetComponent<Text>().text = item.Value.ToString();
                //addButton = g.transform.GetChild(1).GetComponent<Button>();
                g.name = item.Key;
                g.transform.localPosition = p;
                p += new Vector3(0, -25, 0);
                
            }

        }
        else
        {
            for (int i = 0; i < requestsTransform.childCount; i++)
            {
                Destroy(requestsTransform.GetChild(i).gameObject);
            }
        }
    }

    void InstantiateFriends(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
        }
        else if (args.Snapshot.Value != null)
        {
            for (int i = 0; i < friendsTransform.childCount; i++)
            {
                Destroy(friendsTransform.GetChild(i).gameObject);
            }

            friendsList = (Dictionary<string, object>)args.Snapshot.Value;

            Vector3 p = new Vector3(0, 0, 0);
            foreach (var item in friendsList)
            {
                var g = Instantiate(templateFriends, friendsTransform.transform);
                g.transform.GetChild(0).GetComponent<Text>().text = item.Value.ToString();
                g.name = item.Key;
                g.transform.localPosition = p;
                p += new Vector3(0, -25, 0);

            }

        }
        else
        {
            for (int i = 0; i < friendsTransform.childCount; i++)
            {
                Destroy(friendsTransform.GetChild(i).gameObject);
            }
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
        //onlineUsersList.Add(userConnected["username"]);
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
        //onlineUsersList.Remove(userDisconnected["username"].ToString());
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
