using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
	[SerializeField] private string m_keyID = string.Empty;

	public void Interact(Player.Interaction interaction)
	{
		interaction.GetController().GetToolset().GetKeychain().AddKey(m_keyID);
		gameObject.transform.position = new Vector3(0,0,0);
	}
}
