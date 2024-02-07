using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractIcon : MonoBehaviour
{
	[SerializeField] private Sprite m_crosshair;
	[SerializeField] private Sprite m_interactIcon;

	private Image m_progressWheel;
	private Image m_icon;
	private TMPro.TextMeshProUGUI m_interactText;


	private void Awake()
	{
		m_icon = transform.Find("Crosshair").GetComponent<Image>();
		m_interactText = transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
		m_progressWheel = transform.Find("Progress Wheel").GetComponent<Image>();
	}

	public void ToggleInteract(bool isEnabled)
	{
		//Debug.Log(isEnabled);
		if (isEnabled) m_icon.sprite = m_interactIcon;
		else if(!isEnabled) m_icon.sprite = m_crosshair;

		Debug.Log("Image Set: " + m_icon.sprite);
	}

	public void SetInteractText(string newText)
	{
		if (m_interactText == null) return;

		if(m_interactText.text == newText) return;

		m_interactText.text = newText;
	}

	public void SetIcon(GameObject interactable)
	{
		if(interactable == null)
		{
			m_interactText.text = "";
			m_icon.sprite = m_crosshair;
		}
		else if(interactable != null)
		{
			IInteractable i = interactable.GetComponent<IInteractable>();

			if(i != null)
			{
				m_interactText.text = i.GetInteractText();
				m_icon.sprite = i.GetInteractSprite();
			}
		}	
	}

	public void SetProgressWheel(float progress)
	{
		m_progressWheel.fillAmount = progress;
	}
}
