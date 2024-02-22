using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour, IInteractable
{
	[SerializeField] private int m_itemValue;
	[SerializeField] private bool m_isRequiredItem;

	[Header("Interactable Fields")]
	[SerializeField] private string m_interactText;
	[SerializeField] private Sprite m_interactSprite;

	private LootManager m_manager;

	public void Init(LootManager lm)
	{
		m_manager = lm;
	}

	public bool GetRequiredItem()
	{
		return m_isRequiredItem;
	}

	public void Interact(Player.Interaction interaction)
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
		this.GetComponent<AudioSource>().Play();
		this.transform.position = m_manager.transform.position;
		m_manager.RemoveLoot(this);

		yield return new WaitForSeconds(1.5f);

		Destroy(this.gameObject);
	}

	public int GetValue()
	{
		return m_itemValue;
	}

	void OnDestroy()
	{
		m_manager.RemoveLoot(this);
	}

}
