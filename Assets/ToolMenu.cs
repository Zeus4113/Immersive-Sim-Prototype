using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolMenu : MonoBehaviour
{
	[SerializeField] private Image 
		m_lockpickImage, 
		m_keychainImage, 
		m_flashlightImage, 
		m_dartgunImage, 
		m_whackerImage, 
		m_flashbangImage;

	[SerializeField] private GameObject m_wheel;

	[SerializeField] private TMPro.TextMeshProUGUI m_toolText;

	[SerializeField] private Color m_highlightColor;
	[SerializeField] private Color m_baseColor;

	public void OpenMenu(Player.Toolset.Tools tool)
	{
		m_wheel.SetActive(true);

		for(int i = 0; i > m_wheel.transform.childCount; i++)
		{
			m_wheel.transform.GetChild(i).gameObject.SetActive(true);
		}
	}

	public void CloseMenu(Player.Toolset.Tools tool)
	{
		m_wheel.SetActive(false);

		for (int i = 0; i > m_wheel.transform.childCount; i++)
		{
			m_wheel.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public void HighlightImage(Player.Toolset.Tools tool)
	{
		m_lockpickImage.color = m_baseColor;
		m_keychainImage.color = m_baseColor;
		m_flashlightImage.color = m_baseColor;
		m_dartgunImage.color = m_baseColor;
		m_whackerImage.color = m_baseColor;
		m_flashbangImage.color = m_baseColor;

		switch (tool)
		{
			case Player.Toolset.Tools.lockpick:
				m_toolText.text = "Lockpick";
				m_lockpickImage.color = m_highlightColor;

				break;

			case Player.Toolset.Tools.keychain:
				m_toolText.text = "Keychain";
				m_keychainImage.color = m_highlightColor;
				break;

			case Player.Toolset.Tools.flashlight:
				m_toolText.text = "Flashlight";
				m_flashlightImage.color = m_highlightColor;
				break;

			case Player.Toolset.Tools.whacker:
				m_toolText.text = "Whacker";
				m_whackerImage.color = m_highlightColor;
				break;

			case Player.Toolset.Tools.dartgun:
				m_toolText.text = "Dartgun";
				m_dartgunImage.color = m_highlightColor;
				break;

			case Player.Toolset.Tools.flashbang:
				m_toolText.text = "Flashbang";
				m_flashbangImage.color = m_highlightColor;
				break;
		}
	}
}
