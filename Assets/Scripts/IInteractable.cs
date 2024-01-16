using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
	public void Interact()
	{

	}

	public void Interact(Player.Interaction interaction)
	{

	}

	public string GetInteractText()
	{
		return "";
	}

	public Sprite GetInteractSprite()
	{
		return null;
	}

}
