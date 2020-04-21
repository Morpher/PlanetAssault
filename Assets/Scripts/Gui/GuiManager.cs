using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gui;
using Unity.Entities;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    [SerializeField]
    private Transform healthbarPrefab;

    [SerializeField]
    private Transform selectorPrefab;   
    
    [SerializeField]
    private Transform guiContainer;

    [SerializeField]
    private Button restartButton;
    
    [SerializeField]
    private Button pauseButton;

    [SerializeField]
    private Text popupText;

    [SerializeField] 
    private Text scoreText;
    
    private List<GameObject> elements = new List<GameObject>();
    
    private void OnEnable()
    {
        restartButton.onClick.AddListener(() => { GameManager.Instance.RestartGame(); });
        pauseButton.onClick.AddListener(() => { GameManager.Instance.TogglePause(); });
        HidePopupText();
    }
    
    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();
    }

    public void AddHealthbar(Entity entity) 
    {
        var obj = Instantiate(healthbarPrefab.gameObject, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(guiContainer, false);

        var healthbar = obj.GetComponent<Healthbar>();
        healthbar.Entity = entity;
        elements.Add(obj);
    }
    
    public void AddSelector(Entity entity) 
    {
        var obj = Instantiate(selectorPrefab.gameObject, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(guiContainer, false);

        var selector = obj.GetComponent<Selector>();
        selector.Entity = entity;
        elements.Add(obj);
    }

    public void UpdateButtonText(bool isPaused)
    {
        pauseButton.GetComponentInChildren<Text>().text = isPaused ? "CONTINUE" : "PAUSE";
    }
    
    public void ClearGui()
    {
        foreach (var obj in elements)
        {
            Destroy(obj);
        }
        elements.Clear();
    }

    public async void ShowPopupText(string text, float duration = 2f, float delay = 0)
    {
        await CallShowPopupText(text, duration, delay);
    }
    
    private async Task CallShowPopupText(string text, float duration, float delay)
    {
        await Task.Delay(TimeSpan.FromSeconds(delay));
        if (popupText)
        {
            popupText.gameObject.SetActive(true);
            popupText.text = text;
        }
        await Task.Delay(TimeSpan.FromSeconds(duration));
        HidePopupText();
    }
    
    private void HidePopupText()
    {
        if (popupText)
        {
            popupText.gameObject.SetActive(false);
        }
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = $"SCORE {score.ToString()}";
    }
}
