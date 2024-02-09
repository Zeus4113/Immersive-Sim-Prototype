using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
	[SerializeField] private string m_keyID = string.Empty;
	[SerializeField] private string m_interactText;
	[SerializeField] private Sprite m_interactSprite;
	[SerializeField] private bool m_isRequired = false;

	private LootManager m_manager;
	private AudioSource m_audioSource;

	public void Init(LootManager lm)
	{
		m_manager = lm;
		m_audioSource = GetComponent<AudioSource>();
	}

	public void Interact(Player.Interaction interaction)
	{
		m_audioSource.Play();

		interaction.GetController().GetToolset().GetKeychain().AddKey(m_keyID);
		gameObject.transform.position = new Vector3(0,0,0);

		if(m_manager != null)
		{
			m_manager.RemoveKey(this);
		}
	}

	public string GetInteractText()
	{
		return m_interactText;
	}

	public Sprite GetInteractSprite()
	{
		return m_interactSprite;
	}

	public bool IsRequired()
	{
		return m_isRequired;
	}
}
