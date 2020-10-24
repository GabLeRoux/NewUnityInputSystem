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

using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] bool spawnWhenStart;
    [SerializeField] GameObject coinPrefab;

    protected List<Vector3> spawnLocationList = new List<Vector3>();

	void Awake()
	{
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(false);
            spawnLocationList.Add(child.position);	// save the world position
        }
	}

	void Start()
	{
		if(spawnWhenStart)
		{
            SpawnCoins();
		}
	}

    public int GetCoinCount()
	{
        Coin[] coins = transform.GetComponentsInChildren<Coin>();

        return coins.Length;
    }


	public void ClearCoins()
	{
        Coin[] coins = transform.GetComponentsInChildren<Coin>();
        foreach (Coin coin in coins)
        {
			Destroy(coin.gameObject);
        }
    }

	List<int> GetRandomPosIndex()
	{
        List<int> result = new List<int>();
		for(int i=0; i<10; i++)
		{
            int index = Random.Range(0, spawnLocationList.Count);
			if(result.Contains(index) == false)
			{
                result.Add(index);
			}
		}

		// keep only 4
		if(result.Count <= 4)
		{
            return result;
		}

        return result.GetRange(0, 4);
	}

	protected List<Vector3> GetRandomPosList()
	{
        List<int> posIndexList = GetRandomPosIndex();

        List<Vector3> randomPosList = new List<Vector3>();

		foreach(int index in posIndexList) {
            randomPosList.Add(spawnLocationList[index]);

        }

        return randomPosList;
    }

	public void SpawnCoins()
	{
        List<Vector3> posList = GetRandomPosList();

        foreach (Vector3 pos in posList)
		{
            SpawnCoin(pos);
        }
	}


	void SpawnCoin(Vector3 worldPosition)
	{
        GameObject newCoin = Instantiate(coinPrefab);
		newCoin.name = "Coin";

        Transform tf = newCoin.transform;
        tf.SetParent(transform);
        tf.position = worldPosition;
	}
}
