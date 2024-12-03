using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour
{
    // Registration fields
    public InputField registerEmailInput;
    public InputField registerPasswordInput;
    public InputField registerConfirmPasswordInput;
    public InputField registerDisplayNameInput;
    public Text registerMessageText;
    public Button registerButton;

    // Login fields
    public InputField loginEmailInput;
    public InputField loginPasswordInput;
    public Text loginMessageText;
    public Button loginButton;

    public GameObject loadingPanel;

    private FirebaseAuth auth;
    public static FirebaseUser newUser;
    public static DatabaseReference databaseReference;

    void Start()
    {
        // Initialize Firebase and add button listeners
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;

            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            // Use the Auth Emulator if needed
#if USE_AUTH_EMULATOR
                auth.UseEmulator("localhost", 9099);
#endif

            AutoLogin();
        });

        // Add button listeners
        registerButton.onClick.AddListener(RegisterButton);
        loginButton.onClick.AddListener(LoginButton);
    }

    public void RegisterButton()
    {
        string email = registerEmailInput.text;
        string password = registerPasswordInput.text;
        string confirmPassword = registerConfirmPasswordInput.text;
        string displayName = registerDisplayNameInput.text;

        if (IsRegistrationInputValid(email, password, confirmPassword, displayName))
        {
            RegisterNewUser(email, password, displayName);
        }
    }

    public void LoginButton()
    {
        string email = loginEmailInput.text;
        string password = loginPasswordInput.text;

        if (IsLoginInputValid(email, password))
        {
            SignInUser(email, password);
        }
    }

    private void RegisterNewUser(string email, string password, string displayName)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("Register was canceled.");
                registerMessageText.text = "Register was canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Register encountered an error: " + task.Exception);
                registerMessageText.text = "Register encountered an error: " + task.Exception;
                return;
            }

            FirebaseUser newUser = task.Result.User; // Access the User property of AuthResult

            // Update the user's display name
            UserProfile profile = new UserProfile { DisplayName = displayName };
            newUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(profileTask => {
                if (profileTask.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    registerMessageText.text = "UpdateUserProfileAsync was canceled.";
                    return;
                }
                if (profileTask.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + profileTask.Exception);
                    registerMessageText.text = "UpdateUserProfileAsync encountered an error: " + profileTask.Exception;
                    return;
                }

                Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                registerMessageText.text = "User registered successfully";

                // Auto login after successful registration
                SignInUser(email, password);
            });
        });
    }

    private void SignInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignIn was canceled.");
                loginMessageText.text = "SignIn was canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignIn encountered an error: " + task.Exception);
                loginMessageText.text = "SignIn encountered an error: " + task.Exception;
                return;
            }

            newUser = task.Result.User; // Access the User property of AuthResult
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            loginMessageText.text = "User signed in successfully";

            // Save login state
            PlayerPrefs.SetString("UserEmail", email);
            PlayerPrefs.SetString("UserPassword", password);
            PlayerPrefs.Save();

            DatabaseManager.Instance.StartCoroutine(DatabaseManager.Instance.LoadUserData());
            DatabaseManager.Instance.CheckFirebaseDependencies(newUser);

            loadingPanel.SetActive(true);

            StartCoroutine(LoadYourAsyncScene("1"));

        });
    }

    private void AutoLogin()
    {
        string email = PlayerPrefs.GetString("UserEmail", null);
        string password = PlayerPrefs.GetString("UserPassword", null);

        Debug.Log($"email.{email} password {password}");

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            SignInUser(email, password);
        }
    }

    public static IEnumerator LoadYourAsyncScene(string scneneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        //loadingManager.ShowLoadingScreen();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scneneName);

        // AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("FirebaseAuth");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            UnityEngine.Debug.Log(asyncLoad.progress);
            yield return null;
        }
    }

    private bool IsRegistrationInputValid(string email, string password, string confirmPassword, string displayName)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(displayName))
        {
            registerMessageText.text = "Email, password, confirm password, and display name cannot be empty.";
            return false;
        }

        if (password != confirmPassword)
        {
            registerMessageText.text = "Passwords do not match.";
            return false;
        }

        if (!IsEmailValid(email))
        {
            registerMessageText.text = "Invalid email address.";
            return false;
        }

        if (!IsPasswordValid(password))
        {
            registerMessageText.text = "Password must be at least 6 characters long.";
            return false;
        }

        return true;
    }

    private bool IsLoginInputValid(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            loginMessageText.text = "Email and password cannot be empty.";
            return false;
        }

        if (!IsEmailValid(email))
        {
            loginMessageText.text = "Invalid email address.";
            return false;
        }

        return true;
    }

    private bool IsEmailValid(string email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    private bool IsPasswordValid(string password)
    {
        // Firebase requires a minimum of 6 characters for passwords
        return password.Length >= 6;
    }
}
