using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollItem : MonoBehaviour
{
    public Text countText, maxText;
    private int count;
    public Image itemImage;

    private string key;  // Unique key for storing count in PlayerPrefs

    void Start()
    {
        key = "ScrollItem" + itemImage.sprite.name;
        // Load saved progress on start
        Load();
    }

    public void IncreaseNo()
    {
        PlayPunchScaleAnim();

        count++;
        countText.text = count.ToString();

        Save();
    }

    private void PlayPunchScaleAnim()
    {
        float val = .4f;
        transform.DOPunchScale(new Vector3(val, val, val), .3f, 1);
    }

    private void Save()
    {
        PlayerPrefs.SetInt(key, count);
    }

    private void Load()
    {
        // Load the count from PlayerPrefs if it exists, otherwise use default value of 0
        count = PlayerPrefs.GetInt(key, 0);
        countText.text = count.ToString();
    }
}
