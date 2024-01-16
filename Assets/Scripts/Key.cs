using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
	[SerializeField] private string m_keyID = string.Empty;
	[SerializeField] private string m_interactText;
	[SerializeField] private Sprite m_interactSprite;

	public void Interact(Player.Interaction interaction)
	{
		interaction.GetController().GetToolset().GetKeychain().AddKey(m_keyID);
		gameObject.transform.position = new Vector3(0,0,0);
	}

	public string GetInteractText()
	{
		return m_interactText;
	}

	public Sprite GetInteractSprite()
	{
		return m_interactSprite;
	}
}
