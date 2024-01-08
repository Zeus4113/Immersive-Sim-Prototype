using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootManager : MonoBehaviour
{
	private GameManager m_gameManager;

	public delegate void ScoreChange(int newScore);
	public event ScoreChange ScoreIncrease;

	private List<Loot> m_lootList = new List<Loot>();
	private int m_currentScore = 0;

	public void Init(GameManager gm)
	{
		m_gameManager = gm;
		m_lootList.AddRange(transform.GetComponentsInChildren<Loot>());

		for(int i = 0; i < m_lootList.Count; i++)
		{
			m_lootList[i].Init(this);
		}

		Debug.Log(m_lootList.Count);
	}

	public void AddLoot(Loot newItem)
	{
		m_lootList.Add(newItem);
	}

	public void Remove(Loot newItem)
	{
		m_lootList.Remove(newItem);
		m_currentScore += newItem.GetValue();
		ScoreIncrease?.Invoke(m_currentScore);
	}

	public GameManager GetManager()
	{
		return m_gameManager;
	}

}
