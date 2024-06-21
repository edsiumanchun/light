// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
public class PageIndexController : MonoBehaviour
{

    public Vector2 cellSize = new Vector2(188f, 200f);
    public GameObject rowTextObject;
    public int cellCount = 10;


    public List<GameObject> gameObjects;
    private int itemsPerPage = 3;
    private int totalPages;
    public int currentPage = 0;

    private GameObject rightButton;
    private GameObject leftButton;
    public GameObject canvasObject;

    public void Start()
    {
        gameObjects = new List<GameObject>();
        rightButton = Utils.CreateButton(DataHolder.commonImages[((int)UICommon.Right)], new Vector2(244, 0), canvasObject.transform, () => {
            NavigateToNextPage();
        });
        leftButton = Utils.CreateButton(DataHolder.commonImages[((int)UICommon.Left)], new Vector2(-244, 0), canvasObject.transform, () => {
            NavigateToPreviousPage();
        });
        UpdateNavigationButtonVisibility();
    }

    private void NavigateToNextPage()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            DisplayPage();
        }
    }

    public void DisplayPage()
    {
        if(gameObjects != null) {
            foreach(GameObject gameObject in gameObjects){
                Destroy(gameObject);
            }
        }
        if(rowTextObject != null) {
            Destroy(rowTextObject);
        }
    }

    private void NavigateToPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            DisplayPage();
        }
    }

    public void UpdateNavigationButtonVisibility()
    {
        int totalItems = gameObjects.Count;
        totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);
        // Hide the navigation buttons if there are less than 4 items
        if (gameObjects.Count <= 3)
        {
            rightButton.SetActive(false);
            leftButton.SetActive(false);
        }
        else
        {
            rightButton.SetActive(true);
            leftButton.SetActive(true);
        }
    }
}