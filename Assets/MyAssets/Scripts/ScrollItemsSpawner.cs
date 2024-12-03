using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ScrollItemsSpawner : MonoBehaviour
{
    public List<Sprite> sprites;
    public List<int> duplicatesCount;

    public List<GameObject> tempItemsList = new List<GameObject>();
    public List<GameObject> uniqueGameObjects;

    void Start()
    {
        //Transform searchItems = GameObject.Find("SearchItems").transform;

        //for (int i = 0; i < sprites.Count; i++)
        //{
        //    //    Sprite loadedSprite = FindSpriteByName(searchItems.GetChild(i).name);
        //    //    sprites.Add(loadedSprite);
        //    GameObject obj = Instantiate(scrollItemPrefab, content);
        //    obj.GetComponentInChildren<Image>().sprite = sprites[i];
        //}
    }

    //private Sprite FindSpriteByName(string name)
    //{
    //    string[] guids = AssetDatabase.FindAssets(name, new[] { "Assets/Sprite" });

    //    foreach (string guid in guids)
    //    {
    //        string path = AssetDatabase.GUIDToAssetPath(guid);
    //        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
    //        if (sprite != null && sprite.name == name)
    //        {
    //            return sprite;
    //        }
    //    }
    //    return null;
    //}
}
