using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Gameplay UI")]
    [SerializeField] private Text instructionsText;
    [SerializeField] private Slider progressBar;

    [Header("Timer UI")]
    [SerializeField] private Text timerCountDisplay;
    [SerializeField] private Image timerDisplay;

    private bool _hasGameStarted;
    private bool _hasGameEnded;
    private bool _hasWonGame;
    private bool _isFirstRun;
    private int _keystrokes;
    private int _keystrokesToReach;

    private void Start()
    {
        // First Run calculates how much the player can type in 10 seconds and sets that as the cap for the subsequent runs
        StartCoroutine(FirstRun());
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!_hasGameStarted) { _hasGameStarted = true; }

            if (!_isFirstRun) { _keystrokes++; }
            else { _keystrokesToReach++; }
        }

        if (_keystrokes > _keystrokesToReach)
        {
            _hasGameEnded = true;
            _hasWonGame = true;
        }
    }

    private IEnumerator FirstRun()
    {
        float timer = 10f;
        _isFirstRun = true;
        _keystrokesToReach = 0;
        progressBar.value = 0;
        progressBar.maxValue = 300;
        while (!_hasGameEnded)
        {
            if (!_hasGameStarted)
            {
                instructionsText.text = "You have 10 seconds before they find you, start hacking.";
                timerCountDisplay.text = string.Empty;
                timerDisplay.fillAmount = timer;
            }
            else
            {
                progressBar.value = _keystrokesToReach;
                instructionsText.text = string.Empty;
                timerCountDisplay.text = $"{(int)timer}";
                timerDisplay.fillAmount = timer / 10f;
                if (timer > 0) { timer -= Time.deltaTime; }
                else
                {
                    timerCountDisplay.text = string.Empty;
                    _hasGameEnded = true;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        _isFirstRun = false;
        ResetGame();
    }

    private IEnumerator GameLoop()
    {
        float timer = 10f;
        progressBar.maxValue = _keystrokesToReach;
        while (!_hasGameEnded)
        {
            if (!_hasGameStarted)
            {
                instructionsText.text = "They found you and increased their defenses. Try to get in before they catch you";
                timerCountDisplay.text = string.Empty;
                timerDisplay.fillAmount = timer;
            }
            else
            {
                progressBar.value = _keystrokes;
                instructionsText.text = string.Empty;
                timerCountDisplay.text = $"{(int)timer}";
                timerDisplay.fillAmount = timer / 10f;
                if (timer > 0) { timer -= Time.deltaTime; }
                else
                {
                    timerCountDisplay.text = string.Empty;
                    _hasGameEnded = true;
                }
            }
            yield return new WaitForEndOfFrame();
        }

        instructionsText.text = _hasWonGame ? "Congratulations, you've hacked the mainframe! Press space to play again." : "Better luck next time. Press space to play again.";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        _isFirstRun = true;
        ResetGame();
    }

    private void ResetGame()
    {
        _hasGameStarted = false;
        _hasGameEnded = false;
        _hasWonGame = false;
        _keystrokes = 0;
        if (_isFirstRun) { StartCoroutine(FirstRun()); }
        else { StartCoroutine(GameLoop()); }
    }
}
