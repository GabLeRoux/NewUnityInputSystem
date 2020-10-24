/*
Copyright (c) 2020 Razeware LLC

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom
the Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

Notwithstanding the foregoing, you may not use, copy, modify,
merge, publish, distribute, sublicense, create a derivative work,
and/or sell copies of the Software in any work that is designed,
intended, or marketed for pedagogical or instructional purposes
related to programming, coding, application development, or
information technology. Permission for such use, copying,
modification, merger, publication, distribution, sublicensing,
creation of derivative works, or sale is expressly withheld.

This project and source code may use libraries or frameworks
that are released under various Open-Source licenses. Use of
those libraries and frameworks are governed by their own
individual licenses.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.
*/

﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] float playTime = 50;

    [Header("GameObject Binding")]
    [SerializeField] CoinSpawner spawner;
    [SerializeField] Player player;

    // UI
	[Header("UI Binding")]
    [SerializeField] GameObject gameView;
    [SerializeField] GameObject resultView;
    [SerializeField] Text timeText;
    [SerializeField] Text coinText;
    [SerializeField] Text resultCoinText;

    // Internal Data
    protected int coin;
    protected float time;
    protected bool isPlaying = false;
    protected bool waitForNewCoin = false;

    // Start is called before the first frame update
    void Start()
    {
        SetupPlayer();
        StartGame();
    }

	void SetupPlayer()
	{
        player.onCollectCoin = () =>
        {
			if(isPlaying)
			{
                AddCoin();
			}
        };
	}

    // Update is called once per frame
    void Update()
    {
		if(isPlaying)
		{
            HandlePlayLogic();
        }
    }

	void HandlePlayLogic()
	{
        time -= Time.deltaTime;
		if(time < 0)
		{
            EndGame();
            return;
		}

        CheckAndRespawnCoin();
        UpdateTimeValue();
	}

	void Reset()
	{
        coin = 0;
		time = playTime + 0.3f;
        waitForNewCoin = false;
    }

	public void StartGame()
	{
        Reset();
        ShowGameView();
        spawner.ClearCoins();
        spawner.SpawnCoins();

        isPlaying = true;
	}

    public void EndGame()
    {
        isPlaying = false;
        ShowResultView();
    }

	#region Coin Logic

	void CheckAndRespawnCoin()
	{
		//Debug.Log("Coin Count=" + spawner.GetCoinCount());
		if (spawner.GetCoinCount() == 0 && waitForNewCoin == false)
		{
			waitForNewCoin = true;
            
            StartCoroutine(LateSpawnCoins());
		}
	}

    IEnumerator LateSpawnCoins()
	{
        yield return new WaitForSeconds(1.0f);

		//
        if (isPlaying != false && spawner.GetCoinCount() == 0)
        {
            spawner.SpawnCoins();
            waitForNewCoin = false;	// Reset the flag
        }
    }

	public void AddCoin()
	{
        coin++;
        UpdateCoinValue();
	}

	#endregion

	#region UI Logic

	public void ShowGameView()
	{
        UpdateTimeValue();
        UpdateCoinValue();
        gameView.SetActive(true);
        resultView.SetActive(false);
	}

    public void ShowResultView()
    {
        resultCoinText.text = coin.ToString();

        gameView.SetActive(false);
        resultView.SetActive(true);
    }

    void UpdateTimeValue()
	{
        timeText.text = time.ToString("00");
	}

	void UpdateCoinValue()
	{
        coinText.text = coin.ToString();
    }

	#endregion

}
