using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using System;
using Firebase.Database;

public class ButtonRegister : MonoBehaviour
{
    [SerializeField]
    private Button _registrationButton;
    private Coroutine _registrationCoroutine;

    

    public event Action<FirebaseUser> OnUserRegistered;
    public event Action<string> OnUserRegistrationFailed;
    private void Reset()
    {
        _registrationButton = GetComponent<Button>();
    }

    void Start()
    {
        
        _registrationButton.onClick.AddListener(HandleRegistrationButtonClick);
    }


    private void HandleRegistrationButtonClick()
    {
        string email = GameObject.Find("InputEmail").GetComponent<InputField>().text;
        string password = GameObject.Find("InputPassword").GetComponent<InputField>().text;
        _registrationCoroutine = StartCoroutine(RegisterUser(email, password));
    }

   

    private IEnumerator RegisterUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task {registerTask.Exception}");
            OnUserRegistrationFailed.Invoke($"Failed to register task {registerTask.Exception}");
        }
        else
        {
            Debug.Log($"Succesfully registered user {registerTask.Result.Email}");

            UserData data = new UserData();

            data.username = GameObject.Find("InputUsername").GetComponent<InputField>().text;
            data.online = true;
            string json = JsonUtility.ToJson(data);

            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(registerTask.Result.UserId).SetRawJsonValueAsync(json);
            

            OnUserRegistered?.Invoke(registerTask.Result);
        }

        _registrationCoroutine = null;
    }


}
