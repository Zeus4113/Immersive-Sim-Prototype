using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootManager : MonoBehaviour
{
	private GameManager m_gameManager;

	public delegate void ScoreChange(int newScore);
	public event ScoreChange ScoreIncrease;

	private List<Loot> m_lootList = new List<Loot>();

	private List<Door> m_doorList = new List<Door>();

	private List<Key> m_keyList = new List<Key>();
 
	private int m_currentScore = 0;
	private int m_totalScore = 0;
	private int m_requiredItemTotal = 0;
	private int m_currentRequiredItems = 0;

	public void Init(GameManager gm)
	{
		m_currentScore = 0;
		m_totalScore = 0;
		m_currentRequiredItems = 0;
		m_requiredItemTotal = 0;

		m_gameManager = gm;
		m_lootList.AddRange(transform.GetComponentsInChildren<Loot>());
		m_keyList.AddRange(transform.GetComponentsInChildren<Key>());
		m_doorList.AddRange(transform.GetComponentsInChildren<Door>());

		for(int i = 0; i < m_lootList.Count; i++)
		{
			m_lootList[i].Init(this);

			if(m_lootList[i].GetRequiredItem()) m_requiredItemTotal++;

			m_totalScore += m_lootList[i].GetValue();
		}

		for(int i = 0; i < m_keyList.Count; i++)
		{
			m_keyList[i].Init(this);

			if (m_keyList[i].IsRequired()) m_requiredItemTotal++;
		}

		Debug.Log("Total Score: " + m_totalScore);
	}

	public void RemoveLoot(Loot newItem)
	{
		m_currentScore += newItem.GetValue();

		if (newItem.GetRequiredItem()) m_currentRequiredItems++;
		ScoreIncrease?.Invoke(m_currentScore);

		m_lootList.Remove(newItem);
	}

	public void RemoveKey(Key newKey)
	{
		if (newKey.IsRequired()) m_currentRequiredItems++;
		m_keyList.Remove(newKey);
	}

	public float GetScore()
	{
		float score = (((float) m_currentScore) / ((float) m_totalScore)) * 100;

		Debug.Log("Current Score: " + m_currentScore);
		Debug.Log("Total Score: " + m_totalScore);
		Debug.Log("Percentage: " + score);

		return score;
	}

	public bool CheckRequiredItems()
	{
		if (m_currentRequiredItems == m_requiredItemTotal) return true;
		return false;
	}

	public GameManager GetManager()
	{
		return m_gameManager;
	}

}
