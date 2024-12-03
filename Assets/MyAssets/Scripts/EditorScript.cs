#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Net.Http.Headers;

public class EditorScript : EditorWindow
{
    public Transform content;
    public GameObject scrollItemPrefab;

    [MenuItem("Tools/Run Editor Script")]
    public static void ShowWindow()
    {
        GetWindow<EditorScript>("Prefab Instantiator");
    }

    private void OnGUI()
    {
        content = GameObject.Find("Content").transform;
        scrollItemPrefab = EditorGUILayout.ObjectField("scrollItemPrefab", scrollItemPrefab, typeof(GameObject), true) as GameObject;

        //if (GUILayout.Button("Change Selected GameObject Name"))
        //{
        //    GameObject[] selectedGameObjects = Selection.gameObjects;

        //    if (selectedGameObjects.Length > 0)
        //    {
        //        foreach (GameObject selectedGameObject in selectedGameObjects)
        //        {
        //            //selectedGameObject.name += " full";
        //            selectedGameObject.name = selectedGameObject.name.Substring(0, selectedGameObject.name.Length - 4);
        //            EditorUtility.SetDirty(selectedGameObject);
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Please select one or more game objects in the hierarchy.");
        //    }
        //}

        if (GUILayout.Button("FixItemNamesDigit"))
        {
            foreach (Transform child in GameObject.Find("SearchItems").transform.GetComponentsInChildren<Transform>())
            {
                if (IsNumberedName(child.gameObject.name))
                {
                    child.gameObject.name = child.gameObject.name.Substring(0, child.gameObject.name.Length - 4);
                    EditorUtility.SetDirty(child.gameObject);
                }
            }
        }

        if (GUILayout.Button("FillList"))
        {
            FillList();
        }

        if (GUILayout.Button("SpawnScrollItems"))
        {
            SpawnScrollItems();
        }

        if (GUILayout.Button("FixItemsColliders"))
        {
            FixItemsColliders();
        }

        if (GUILayout.Button("FixEnviromentColliders"))
        {
            FixEnvironmentColliders();
        }

        if (GUILayout.Button("SetMaxFillCountRadial"))
        {
            SetMaxFillCountRadial();
        }

        //if (GUILayout.Button("FillZones"))
        //{
        //    FillZones();
        //}
        
        if (GUILayout.Button("AddItemsScriptToItems"))
        {
            AddItemsScriptToItems();
        } 
        
        if (GUILayout.Button("FixSearchItemsChilds"))
        {
            FixSearchItemsChilds();
        }

        //if (GUILayout.Button("SETUP"))
        //{
        //    FillList();
        //    SpawnScrollItems();
        //    FixItemsColliders();
        //    SetMaxFillCountRadial();
        //    FillZones();
        //    AddItemsScriptToItems();
        //}
    }

    bool IsNumberedName(string name)
    {
        Debug.Log("Name " + " " + name + " " +name[name.Length - 1]);
        return name[name.Length-1] == ')';
    }

    private void AddItemsScriptToItems()
    {
        Transform searchItems = GameObject.Find("SearchItems").transform;

        foreach (Transform item in searchItems)
        {
            if (item.gameObject.GetComponent<Item>())
                return;

            item.gameObject.AddComponent<Item>();
            EditorUtility.SetDirty(item);
        }
    }

    private void FillZones()
    {
        if(!GameObject.Find("ZonesGrid").GetComponent<ZoneManager>())
            GameObject.Find("ZonesGrid").AddComponent<ZoneManager>();

        ZoneManager zoneManager = FindObjectOfType<ZoneManager>();
        zoneManager.zones = new List<GameObject>(new GameObject[0]);

        for (int i = 0; i < zoneManager.transform.childCount; i++)
        {
            zoneManager.zones.Add(zoneManager.transform.GetChild(i).gameObject);
        }

        EditorUtility.SetDirty(zoneManager);
    }

    private void SetMaxFillCountRadial()
    {
        Transform searchItems = GameObject.Find("SearchItems").transform;

        FindObjectOfType<RadialSlider>().maxFill = searchItems.childCount;
        FindObjectOfType<RadialSlider>().maxFillText.text = searchItems.childCount.ToString();

        EditorUtility.SetDirty(FindObjectOfType<RadialSlider>());
    }

    private void FillList()
    {
        ScrollItemsSpawner scrollItemsSpawner = FindObjectOfType<ScrollItemsSpawner>();
        Transform searchItems = GameObject.Find("SearchItems").transform;
        scrollItemsSpawner.sprites = new List<Sprite>();

        for (int i = 0; i < searchItems.childCount; i++)
        {
            Sprite loadedSprite = FindSpriteByName(searchItems.GetChild(i).name);
            if (!scrollItemsSpawner.sprites.Contains(loadedSprite) && loadedSprite != null)
                scrollItemsSpawner.sprites.Add(loadedSprite);
        }

        EditorUtility.SetDirty(scrollItemsSpawner);
    }

    private void FixSearchItemsChilds()
    {
        Transform searchItems = GameObject.Find("SearchItems").transform;

        Debug.Log(searchItems.childCount + " Child Count");

        List<Transform> transforms = new List<Transform>();

        foreach (Transform child in searchItems)
        {
            for (int i = 0; i < child.childCount; i++)
            {

                transforms.Add(child.GetChild(i));
            }
        }

        foreach (Transform child in transforms)
        {
            child.parent = searchItems;
        }

        EditorUtility.SetDirty(searchItems);
    }

    private Sprite FindSpriteByName(string name)
    {
        string[] guids = AssetDatabase.FindAssets(name, new[] { "Assets/MyAssets/Sprites/Sprite" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null && sprite.name == name)
            {
                return sprite;
            }
        }

        Debug.Log(name);
        return null;
    }


    private void SpawnScrollItems()
    {
        ScrollItemsSpawner scrollItemsSpawner = FindObjectOfType<ScrollItemsSpawner>();

        Transform searchItems = GameObject.Find("SearchItems").transform;
        Debug.Log(searchItems.childCount);


        foreach (Transform item in searchItems)
        {
            scrollItemsSpawner.tempItemsList.Add(item.gameObject);
        }

        scrollItemsSpawner.uniqueGameObjects = RemoveDuplicates(scrollItemsSpawner.tempItemsList);

        scrollItemsSpawner.tempItemsList = scrollItemsSpawner.uniqueGameObjects;

        scrollItemsSpawner.duplicatesCount = new List<int>(new int[scrollItemsSpawner.tempItemsList.Count]);

        for (int i = 0; i < scrollItemsSpawner.tempItemsList.Count; i++)
        {
            for (int j = 0; j < searchItems.childCount; j++)
            {
                if (scrollItemsSpawner.tempItemsList[i].name == searchItems.GetChild(j).name)
                {
                    scrollItemsSpawner.duplicatesCount[i]++;
                }
            }
        }

        EditorUtility.SetDirty(scrollItemsSpawner);

        foreach (ScrollItem scrollItem in FindObjectsOfType<ScrollItem>())
        {
            DestroyImmediate(scrollItem.gameObject);
        }

        for (int i = 0; i < scrollItemsSpawner.sprites.Count; i++)
        {
            GameObject obj = PrefabUtility.InstantiatePrefab(scrollItemPrefab, content) as GameObject;
            obj.GetComponent<ScrollItem>().itemImage.sprite = scrollItemsSpawner.sprites[i];
            obj.GetComponent<ScrollItem>().maxText.text = scrollItemsSpawner.duplicatesCount[i].ToString();
        }

        EditorUtility.SetDirty(content);
    }

    List<GameObject> RemoveDuplicates(List<GameObject> gameObjects)
    {
        Dictionary<string, GameObject> uniqueGameObjectDict = new Dictionary<string, GameObject>();

        foreach (GameObject obj in gameObjects)
        {
            if (!uniqueGameObjectDict.ContainsKey(obj.name))
            {
                uniqueGameObjectDict[obj.name] = obj;
            }
        }

        return new List<GameObject>(uniqueGameObjectDict.Values);
    }

    private void FixItemsColliders()
    {
        Transform searchItems = GameObject.Find("SearchItems").transform;

        foreach (Transform child in searchItems)
        {
            if (!child.gameObject.GetComponent<BoxCollider2D>())
            {
                Debug.Log("child Name " + child.gameObject.name);
                BoxCollider2D boxCollider2D = child.GetComponentInChildren<BoxCollider2D>();
                child.gameObject.AddComponent<BoxCollider2D>();


                BoxCollider2D boxColliderNew = child.gameObject.GetComponent<BoxCollider2D>();
                boxColliderNew.offset = boxCollider2D.offset;
                boxColliderNew.size = boxCollider2D.size;

                DestroyImmediate(child.GetChild(0).GetComponent<BoxCollider2D>());

                EditorUtility.SetDirty(child);
            }
        }
    }

    private void FixEnvironmentColliders()
    {
        Transform environment = GameObject.Find("Environment").transform;

        foreach (BoxCollider2D child in environment.GetComponentsInChildren<BoxCollider2D>())
        {
                Debug.Log("Name " + child.gameObject.name);
                DestroyImmediate(child.gameObject);
        }

        EditorUtility.SetDirty(environment);
    }
}
#endif