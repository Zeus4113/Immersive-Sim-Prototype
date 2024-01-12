using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour, IInteractable
{
	[SerializeField] private int m_itemValue;

	[Header("Interactable Fields")]
	[SerializeField] private string m_interactText;
	[SerializeField] private Sprite m_interactSprite;

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

	public string GetInteractText()
	{
		return m_interactText;
	}

	public Sprite GetInteractSprite()
	{
		return m_interactSprite;
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
