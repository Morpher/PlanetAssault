using System;
using System.Collections;
using Cinemachine;
using Components;
using Unity.Entities;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField]
    private PlanetSpawner planetSpawner;

    [SerializeField] 
    private OrbitsRenderer orbitsRenderer;

    [SerializeField] 
    private GuiManager guiManager;

    [SerializeField]
    [Range(1, 10)]
    private int currentLevel = 1;

    [SerializeField] 
    private float zoomSensitivity = 0.5f;
 
    [SerializeField] 
    private Vector2 zoomRange = new Vector2(3f, 20f);
    
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    private int score;
    
    public GameState State { get; private set; } = GameState.Init;
    
    private World world => World.DefaultGameObjectInjectionWorld;
    
    private void ClearGame()
    {
        var entities = world.EntityManager.GetAllEntities();
        foreach (var entity in entities)
        {
            var isLevelEntity = world.EntityManager.HasComponent<LevelEntityComponent>(entity);
            if (isLevelEntity)
            {
                world.EntityManager.DestroyEntity(entity);                
            }
        }
        orbitsRenderer.ClearOrbits();
        guiManager.ClearGui();
        State = GameState.Init;
    }

    private void Start()
    {
        RestartGame();
        guiManager.ShowPopupText($"LEVEL {currentLevel}", 2f, 2f);
    }
    
    private IEnumerator ChangeStateDelayed(GameState newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        State = newState;
    }
    
    public void RestartGame()
    {
        ClearGame();
        SetZoom(10);
        planetSpawner.SpawnPlayerPlanet();
        planetSpawner.SpawnAiPlanets(currentLevel);
        StartCoroutine(ChangeStateDelayed(GameState.Started, 2f));
    }
    
    public void OnLevelCompleted()
    {
        State = GameState.Init;
        currentLevel = Mathf.Clamp(currentLevel + 1, 1, 10);
        guiManager.ShowPopupText($"YOU WIN!", 2f, 2f);  
        guiManager.ShowPopupText($"LEVEL {currentLevel}", 2f, 6f);
        Invoke(nameof(RestartGame), 4f);
    }

    public void OnLevelFailed()
    {
        State = GameState.Init;
        currentLevel = 1;
        ResetScore();
        guiManager.ShowPopupText($"YOU LOSE", 2f, 2f);        
        guiManager.ShowPopupText($"LEVEL {currentLevel}", 2f, 6f);
        Invoke(nameof(RestartGame), 4f);     
    }
    
    public void IncrementScore()
    {
        score++;
        guiManager.UpdateScoreText(score);
    }

    public void ResetScore()
    {
        score = 0;
        guiManager.UpdateScoreText(score);        
    }
    
    private bool isPaused;
    public void TogglePause()
    {
        isPaused = !isPaused;
        guiManager.UpdateButtonText(isPaused);
        var systems = world.Systems;
        foreach (var system in systems)
        {
            system.Enabled = !isPaused;
        }
    }
    
    private void LateUpdate()
    {
        var zoom = virtualCamera.m_Lens.OrthographicSize - Input.mouseScrollDelta.y * zoomSensitivity;
        SetZoom(zoom);

        //TODO: move somewhere
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void SetZoom(float zoom)
    {
        var lens = virtualCamera.m_Lens;
        lens.OrthographicSize = Mathf.Clamp(zoom, zoomRange.x, zoomRange.y);
        virtualCamera.m_Lens = lens;        
    }
}

public enum GameState
{
    Init,
    Started,
}
