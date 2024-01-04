using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour, IInteractable
{
	[SerializeField] private int m_itemValue;

	private LootManager m_manager;

	public void Init(LootManager lm)
	{
		m_manager = lm;
	}

	public void Interact()
	{
		if (m_manager == null) return;

		StartCoroutine(RemoveObject());
	}

	IEnumerator RemoveObject()
	{
		this.transform.position = m_manager.transform.position;

		yield return new WaitForSeconds(0.5f);

		Destroy(this.gameObject);
	}

	public int GetValue()
	{
		return m_itemValue;
	}

	void OnDestroy()
	{
		m_manager.Remove(this);
	}

}
