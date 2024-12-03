using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Data 
{
   
    public int totalExp;
    
    public Data(DatabaseManager databaseManager)
    {
        totalExp = DataManager.totalExp;
    }
}
