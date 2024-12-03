using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollToImage : MonoBehaviour
{
    public ScrollRect scrollRect; // Reference to the ScrollRect component
    public RectTransform content; // Reference to the Content RectTransform
    //public Sprite sprite;
    private List<Image> images;
    [HideInInspector] public Image targetImage;
    public GameObject itemClickStopCollider;

    private void Start()
    {
        scrollRect = FindObjectOfType<ScrollRect>();
        content = GameObject.Find("Content").GetComponent<RectTransform>();
        images = new List<Image>();
        images.AddRange(content.GetComponentsInChildren<Image>());
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        ScrollToTargetImage(sprite.name);
    //    }
    //}

    public void ScrollToTargetImage(string targetSpriteName)
    {
        targetImage = FindImageInContent(targetSpriteName);
        if (targetImage == null)
        {
            Debug.LogError(targetSpriteName + " The target image was not found in the content.");
            return;
        }

        content.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(content.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(targetImage.rectTransform.position);

    }

    public Image FindImageInContent(string targetSpriteName)
    {
        foreach (Image image in images)
        {
            if (image != null && image.sprite.name == targetSpriteName)
            {
                return image;
            }
        }
        return null;
    }
}