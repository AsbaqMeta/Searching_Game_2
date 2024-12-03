using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public List<GameObject> zones;
    private RadialSlider radialSlider;
    private CameraDrag cameraDrag;
    private GameManager gameManager;
    public List<int> zoneItemsCount;

    private void Start()
    {
        radialSlider = FindObjectOfType<RadialSlider>();
        cameraDrag = FindObjectOfType<CameraDrag>();
        gameManager = FindObjectOfType<GameManager>();

        CheckZoneUnlock(true);
    }

    public void CheckZoneUnlock(bool atGameStart)
    {
        if (radialSlider.currentFill >= zoneItemsCount[0])
        {
            if (zones[0])
            {
                Destroy(zones[0]);

                if (!atGameStart)
                    gameManager.ShowNewAreaPopup();

                cameraDrag.IncreaseYMaxValue();
            }
        }
        if (radialSlider.currentFill >= zoneItemsCount[1])
        {
            if (zones[1])
            {
                Destroy(zones[1]);

                if (!atGameStart)
                    gameManager.ShowNewAreaPopup();

                cameraDrag.SetXMaxValue(15);
            }
        }
        if (radialSlider.currentFill >= zoneItemsCount[2])
        {
            if (zones[2])
            {
                Destroy(zones[2]);

                if (!atGameStart)
                    gameManager.ShowNewAreaPopup();
            }

        }
        if (radialSlider.currentFill >= zoneItemsCount[3])
        {
            if (zones[3])
            {
                Destroy(zones[3]);

                if (!atGameStart)
                    gameManager.ShowNewAreaPopup();

                cameraDrag.SetXMaxValue(43);
            }
        }
        if (radialSlider.currentFill >= zoneItemsCount[4])
        {
            if (zones[4])
            {
                Destroy(zones[4]);

                if (!atGameStart)
                    gameManager.ShowNewAreaPopup();
            }
        }
    }
}