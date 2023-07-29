using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused
}

public interface Interactable
{
    void Interact();
}

public class UberManager : MonoSingleton<UberManager>
{
    public override void Init()
    {
        Application.runInBackground = false;
        game_timer = 0;

        StopTimer();

        SetState(GameState.Playing);
    }

    static int interaction_mask = -1;
    public static int GetInteractionMask()
    {
        if(interaction_mask == -1)
            interaction_mask = LayerMask.GetMask("Interaction");

        return interaction_mask;
    }

    static int player_collision_mask = -1;
    public static int GetPlayerCollisionMask()
    {
        if(player_collision_mask == -1)
            player_collision_mask = LayerMask.GetMask("Ground", "Default", "Interaction");

        return player_collision_mask;
    }

    public static GameState GetState()
    {
        return Instance.state;
    }

    public GameState state;

    public float timeScale = 1;
    [Range(0.1f, 3)]
    public float timeScale_target = 1;
    public float timeChangeSpeed = 0.15f;

    void SetState(GameState _state)
    {
        switch(_state)
        {
            case(GameState.Paused):
            {
                Time.timeScale = 0;
                MenuManager.Instance.SetPauseVisibile(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            }
            case(GameState.Playing):
            {
                Time.timeScale = timeScale;
                MenuManager.Instance.SetPauseVisibile(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            }
        }
        state = _state;
    }

    void Update()
    {
        timeScale = Mathf.MoveTowards(timeScale, timeScale_target, Time.deltaTime * timeChangeSpeed);
        switch(state)
        {
            case(GameState.Paused):
            {
                break;
            }
            case(GameState.Playing):
            {
                Time.timeScale = timeScale;
                break;
            }
        }

        if(Input.GetKeyDown(KeyCode.T) && state == GameState.Playing)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(state == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
            else if(state == GameState.Paused)
            {
                ResumeGame();
            }
        }

        if(IsGameTimerRunning)
            game_timer += Time.deltaTime;
    }

    public static float game_timer;


    public static float GetProgressionCoefficient()
    {
        return 0.1f * (float)UberManager.GetProgressMinutes() + 0.09f * (float)(UberManager.GetProgressMinutes() / 3) + 0.045f * (float)(UberManager.GetProgressMinutes() / 5);
    }

    public void Reload_scene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    

    public static void StartTimer()
    {
        IsGameTimerRunning = true;
    }

    public static void StopTimer()
    {
        IsGameTimerRunning = false;
    }

    public static void ResetTimer()
    {
        game_timer = 0;
    }

    public static int GetProgressMinutes()
    {
        return (int)game_timer / 60;
    }

    static bool IsGameTimerRunning = false;


    public void ResumeGame()
    {
        SetState(GameState.Playing);
    }
}
