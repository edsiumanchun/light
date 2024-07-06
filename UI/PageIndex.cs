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

    public List<GameObject> gameObjects;
    public int itemsPerPage = 3;
    public int totalPages;
    public int totalItems;
    public int currentPage = 0;

    private GameObject rightButton;
    private GameObject leftButton;
    public GameObject canvasObject;
    public string folderName;
    public void Start()
    {
        totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);
        gameObjects = new List<GameObject>();
        rightButton = Utils.CreateButton(DataHolder.commonImages[((int)UICommon.Right)], new Vector2(355, 0), canvasObject.transform, () => {
            NavigateToNextPage();
        });
        leftButton = Utils.CreateButton(DataHolder.commonImages[((int)UICommon.Left)], new Vector2(-355, 0), canvasObject.transform, () => {
            NavigateToPreviousPage();
        });
    }


    private void NavigateToNextPage()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            DisplayPage();
        }
    }

    public virtual void DisplayPage()
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
        // Debug.Log("???" + totalPages);
        // Hide the navigation buttons if there are less than 4 items
        if (totalPages <= 1)
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

    public Sprite GetImage(string name) {
        List<string> paths = new List<string>((new[] { name }));
        List<Sprite> sprites = Utils.LoadSprites(folderName, paths);
        return sprites[0];
    } 

    public int GetItemsLeft() {
        if(totalItems / itemsPerPage < totalPages) {
            return itemsPerPage;
        } else {
            int itemsLeft = totalItems % itemsPerPage;
            Debug.Log("??" + itemsLeft);
            return itemsLeft == 0 ? 3 : itemsLeft;
        }
    }
}