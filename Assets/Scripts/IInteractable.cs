using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
	public void Interact()
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
