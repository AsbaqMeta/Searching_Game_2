using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveData
{

    public static string SaveNow(DatabaseManager databaseManager)
    {
        Data data = new Data(databaseManager);
        string json = JsonUtility.ToJson(data);
        return json;
    }


}
