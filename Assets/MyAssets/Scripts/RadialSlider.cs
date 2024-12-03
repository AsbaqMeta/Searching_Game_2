using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialSlider : MonoBehaviour
{
    public float currentFill = 0; // Current fill amount
    public float maxFill; // Maximum fill value
    public Image fillImage;
    private string currentFillKey = "CurrentFill"; // Key to store the fill value in PlayerPrefs
    public Text currentFillText, maxFillText;

    private void Awake()
    {
        LoadProgress();
        // Update the fill amount of the image based on loaded value
        UpdateFillImage();
    }
    
    // Function to increase the fill amount by a specified value
    public void IncreaseFillAmount(int increment)
    {
        // Increment the current fill value
        currentFill += increment;

        // Clamp the fill value between 0 and maxFill
        currentFill = Mathf.Clamp(currentFill, 0f, maxFill);

        // Update the fill amount of the image
        UpdateFillImage();

        if (currentFill == maxFill)
            FindObjectOfType<GameManager>().Win();

        // Save the current fill value
        SaveProgress();

        // Increment the Score of the Game in Firebase Database
        DataManager.totalExp += (int)increment;
        DatabaseManager.Instance.SaveandUpdateUserData();
    }

    // Function to update the fill amount of the image
    private void UpdateFillImage()
    {
        fillImage.fillAmount = currentFill / maxFill;
        SetText();
    }

    private void SetText()
    {
        currentFillText.text = currentFill.ToString();
        maxFillText.text = maxFill.ToString();
    }

    // Function to load the saved fill amount
    private void LoadProgress()
    {
        currentFill = PlayerPrefs.GetFloat(currentFillKey, 0);
    }

    // Function to save the current fill amount
    private void SaveProgress()
    {
        PlayerPrefs.SetFloat(currentFillKey, currentFill);
    }
}