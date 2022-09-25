using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

public class ButtonResetPassword : MonoBehaviour
{
    [SerializeField]
    private Button _resetPasswordButton;

    [SerializeField]
    private InputField _emailInputField;

    private Coroutine _resetPasswordCoroutine;

    public GameObject resetPanel;

    private void Reset()
    {
        _resetPasswordButton = GetComponent<Button>();
        _emailInputField = GameObject.Find("InputEmail").GetComponent<InputField>();
       
    }

    public void ClosePanel()
    {
        resetPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        _resetPasswordButton.onClick.AddListener(HandleLoginButtonClicked);
    }

    private void HandleLoginButtonClicked()
    {
        if (_resetPasswordCoroutine == null)
        {
            _resetPasswordCoroutine = StartCoroutine(ResetPasswordCoroutine(_emailInputField.text));
        }
    }

    private IEnumerator ResetPasswordCoroutine(string email)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var resetTask = auth.SendPasswordResetEmailAsync(email);

        yield return new WaitUntil(() => resetTask.IsCompleted);

        if (resetTask.Exception != null)
        {
            Debug.LogWarning($"Reset Failed with {resetTask.Exception}");
        }
        else
        {
            resetPanel.SetActive(true);
            Debug.Log("Password reset email sent successfully");
        }
    }
}
