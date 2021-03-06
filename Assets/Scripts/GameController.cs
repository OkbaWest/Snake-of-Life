﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	private GameObject apple;

	public int score;
	public int highscore;

	public delegate void ScoreDelegate( int score );
	public event ScoreDelegate newScoreEvent;
	public event ScoreDelegate newHighscoreEvent;

	public event Action pauseGameEvent;
	public event Action unpauseGameEvent;

	enum GameState
	{
		playing,
		paused,
		gameOver
	}
	GameState _gameState;

	// Start is called before the first frame update
	void Awake()
	{
		score = 0;

		highscore = PlayerPrefs.GetInt( "Highscore" );

		// Add listeners
		FindObjectOfType<PlayerController>().deathEvent += OnPlayerDeath;
		FindObjectOfType<AppleSpawner>().appleCollectEvent += OnAppleCollectEvent;

		_gameState = GameState.playing;

		Time.timeScale = 1;
	}

	private void Start()
	{
		if( highscore > 0 )
			newHighscoreEvent?.Invoke( highscore );
	}

	// Update is called once per frame
	void Update()
	{
		switch( _gameState )
		{
			case GameState.playing:
				PlayingUpdate();
				break;
			case GameState.paused:
				PausedUpdate();
				break;
			case GameState.gameOver:
				GameOverUpdate();
				break;
		}
	}

	void PlayingUpdate()
	{
		// Pause game if player presses ESC on a keyboard or START on a controller
		if( Input.GetKeyDown( KeyCode.Escape ) || Input.GetKeyDown( "joystick button 7" ) )
		{
			_gameState = GameState.paused;
			Time.timeScale = 0;
			pauseGameEvent();
		}
	}

	void PausedUpdate()
	{
		// Unpause game if player presses ESC on a keyboard or START on a controller
		if( Input.GetKeyDown( KeyCode.Escape ) || Input.GetKeyDown( "joystick button 7" ) )
		{
			_gameState = GameState.playing;
			Time.timeScale = 1;
			unpauseGameEvent();
		}
	}

	void GameOverUpdate()
	{
		
	}

	void OnAppleCollectEvent()
	{
		score++;
		newScoreEvent?.Invoke( score );
	}

	void OnPlayerDeath()
	{
		Time.timeScale = 0;

		if( score > highscore )
		{
			highscore = score;
			newHighscoreEvent?.Invoke( highscore );
			PlayerPrefs.SetInt( "Highscore", highscore );
		}
		PlayerPrefs.Save();

		_gameState = GameState.gameOver;
	}
}
