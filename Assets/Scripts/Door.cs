using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{

	private Transform m_hingeTransform;
	private bool m_isOpen = false;
	private float m_rotation = 90f;

	private Lock m_lock;

	[SerializeField] private AudioClip m_openingSound, m_closingSound, m_lockedSound;
	[SerializeField] private AudioSource m_source;
	[SerializeField] private bool m_isPlayerInteractable = true;
	[Space(2)]

	[Header("Interactable Fields")]
	[SerializeField] private string m_interactText;
	[SerializeField] private Sprite m_interactIcon;

	void Awake()
	{
		m_hingeTransform = transform.parent;
		m_lock = transform.GetComponent<Lock>();
	}

	public void Interact(Player.Interaction interaction)
	{
		if(m_lock != null && m_lock.GetLocked())
		{
			Debug.LogWarning("Locked");
			m_source.PlayOneShot(m_lockedSound);
		}
		else if(m_lock != null && !m_lock.GetLocked())
		{
			Debug.LogWarning("Unlocked");
			StartDoorMoving();
		}
	}

	public string GetInteractText()
	{
		return m_interactText;
	}

	public Sprite GetInteractSprite()
	{
		return m_interactIcon;
	}

	bool c_isDoorMoving = false;
	Coroutine c_doorMoving;

	void StartDoorMoving()
	{
		if (c_isDoorMoving != false) return;

		c_isDoorMoving = true;

		if (c_doorMoving != null) return;

		c_doorMoving = StartCoroutine(DoorMoving());
	}

	void StopDoorMoving()
	{
		if(c_isDoorMoving == false) return;

		c_isDoorMoving = false;

		if(c_doorMoving == null) return;

		StopCoroutine(c_doorMoving);
		c_doorMoving = null;
	}

	IEnumerator DoorMoving()
	{
		while (c_isDoorMoving)
		{
			if (m_hingeTransform != null)
			{
				if (m_isOpen)
				{
					for (int i = 0; i <= (m_rotation); i++)
					{
						m_hingeTransform.Rotate(0, -1, 0);
						yield return new WaitForFixedUpdate();

						if(i == 75)
						{
							m_source.PlayOneShot(m_closingSound);
						}
					}
					m_interactText = "Open";
					m_isOpen = false;
					StopDoorMoving();
				}

				else if (!m_isOpen)
				{
					m_source.PlayOneShot(m_openingSound);

					for (int i = 0; i <= (m_rotation); i++)
					{
						m_hingeTransform.Rotate(0, 1, 0);
						yield return new WaitForFixedUpdate();
					}

					m_interactText = "Close";
					m_isOpen = true;
					StopDoorMoving();
				}
			}

			break;
		}
	}
}

