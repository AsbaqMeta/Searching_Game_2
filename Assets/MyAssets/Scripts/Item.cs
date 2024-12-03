using DG.Tweening;
using RDG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    ScrollToImage scrollToImage;
    private float jumpHeight = 7;   // Height of the jump
    private float duration = .5f;  // Duration of the movement

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float elapsedTime = 0f;
    private bool go;
    private Transform targetImage;

    private string key;  // Unique key for storing count in PlayerPrefs

    private void Awake()
    {
        key = "Item" + gameObject.name + transform.GetSiblingIndex();

        if (PlayerPrefs.HasKey(key))
            Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        AudioManager.Instance.Play("Click");

        Debug.Log("Down " + gameObject.name);

        SaveProgress();

        scrollToImage = FindObjectOfType<ScrollToImage>();

        scrollToImage.itemClickStopCollider.SetActive(true);

        scrollToImage.ScrollToTargetImage(gameObject.name);
        targetImage = scrollToImage.targetImage.transform;

        Invoke("Move", .03f);
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetString(key, "");
    }

    private void Move()
    {
        StartJump(targetImage.position);
        transform.DOScale(Vector3.one, .2f);
    }

    void Update()
    {
        if (!go)
            return;

        if (elapsedTime < duration)
        {
            targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(scrollToImage.targetImage.transform.position.x, scrollToImage.targetImage.transform.position.y, Camera.main.nearClipPlane));

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float height = Mathf.Sin(Mathf.PI * t) * jumpHeight;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;
        }
        else
        {
            go = false;
            scrollToImage.targetImage.GetComponentInParent<ScrollItem>().IncreaseNo();
            FindObjectOfType<RadialSlider>().IncreaseFillAmount(1);
            FindObjectOfType<ZoneManager>().CheckZoneUnlock(false);
            scrollToImage.itemClickStopCollider.SetActive(false);
            AudioManager.Instance.Play("Go");
            Vibration.Vibrate(50);

            Destroy(gameObject);
        }
    }

    // Call this method to start the jump
    public void StartJump(Vector3 screenPosition)
    {
        targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.nearClipPlane));
        targetPosition.z = 0;  // Ensure the target position is on the same plane as the sprite
        startPosition = transform.position;
        elapsedTime = 0f;
        go = true;

    }
}