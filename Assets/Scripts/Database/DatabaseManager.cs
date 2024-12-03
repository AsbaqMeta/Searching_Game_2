using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using System;

public class DatabaseManager : GenericSingleton<DatabaseManager>
{

    #region FirebaseVariables

    public FirebaseDatabase database;
    //FirebaseAuth auth;  
    FirebaseUser user;

    #endregion

    public Action onFirstTimeDataSave;
    public Action OnLoadToLobby;
    public static bool FirstTimeforDynamicLink;
   
    private void Start()
    {
        //auth = FirebaseAuth.DefaultInstance;
        Debug.Log("Initialized Successfully");
    }


    public void CheckFirebaseDependencies(FirebaseUser user)
    {

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                {

                    InitializeFirebase(user);

                }
                
            }

            if (task.Exception != null)
            {
                Debug.LogError($"Failed to initialize firebase with {task.Exception}");
                return;
            }
            Debug.Log("Initialized Successfully");

        });
    }


    private void InitializeFirebase(FirebaseUser user)
    {
        this.user = user;
    }

  


    public IEnumerator LoadUserData()
    {
        Debug.Log("LoadUserData");

        var dbTask = FirebaseManager.databaseReference.Child("users").Child(FirebaseManager.newUser.UserId).GetValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);
        if (dbTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else if (dbTask.Result.Value == null)
        {
            DataManager.totalExp = 50;
            CreateUserSaveProfile();
            SaveandUpdateUserData();
           
            Debug.Log("LoadUserDataCoroutine: Null");
        }
        else
        {
            // Data has been retrived
            DataSnapshot snapshot = dbTask.Result;
            DataManager.totalExp = int.Parse(snapshot.Child("totalExp").Value.ToString());
           
        }
        Debug.Log("DataManager.totalExp " + DataManager.totalExp);
       
     
        if (onFirstTimeDataSave != null)
        {
            onFirstTimeDataSave.Invoke();
            Debug.Log("triggered");
        }

        if (OnLoadToLobby != null)
        {
            OnLoadToLobby.Invoke();
            Debug.Log("triggered");
        }
        
        SaveandUpdateUserData();
        
    }


    public void CreateUserSaveProfile()
    {
        StartCoroutine(UpdateUsernameAuth(FirebaseManager.newUser.DisplayName));
        StartCoroutine(UpdateUsernameDatabase(FirebaseManager.newUser.DisplayName));

    }
    private IEnumerator UpdateUsernameAuth(string _username)
    {
        // Create the user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        // Call the firebase auth update user profile function passing the profile with the username]
        //user = auth.CurrentUser;
        var profileTask = user.UpdateUserProfileAsync(profile);

        //Wait until the task complete
        yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

        if (profileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {profileTask.Exception}");
        }
        else
        {
            // Auth username is now updated
        }
    }

    public void SaveandUpdateUserData()
    {
        StartCoroutine(UpdateSaveData());
    }

    public IEnumerator UpdateSaveData()
    {
        string json = SaveData.SaveNow(this);
        //Debug.Log(json);
      
        var dbTask = FirebaseManager.databaseReference.Child("users").Child(FirebaseManager.newUser.UserId).SetRawJsonValueAsync(json);
       // Debug.Log(dbTask);
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);
        if (dbTask.Exception != null)
        {
            Debug.Log(message: $"Failed to register task with{ dbTask.Exception} 11");
        }
        else
        {
           // Debug.Log(message: $"Failed to register task with{ dbTask.Exception} 22");
        }


    }

    private IEnumerator UpdateUsernameDatabase(string _userName)
    {
        var dbTask = FirebaseManager.databaseReference.Child("users").Child(FirebaseManager.newUser.UserId).Child("username").SetValueAsync(_userName);

        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);
        if (dbTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            // Database username is now updated
        }
    }

}