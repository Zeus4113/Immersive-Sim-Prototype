using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBody : MonoBehaviour, IInteractable
{
	[SerializeField] private GameObject m_bodyMesh;
	[SerializeField] private Rigidbody m_rigidbody;
	[SerializeField] private Collider m_collider;

	[SerializeField] private string m_interactTextPickup;
	[SerializeField] private Sprite m_interactSpritePickup;

	[SerializeField] private string m_interactTextDrop;
	[SerializeField] private Sprite m_interactSpriteDrop;

	bool m_isPickedUp = false;

	bool c_isSlumping;
	Coroutine m_slumping;

	void Awake()
	{
		StartCoroutine(Slumping());
	}

	IEnumerator Slumping()
	{
		while (m_bodyMesh.transform.localScale.y > 0.25f)
		{
			m_bodyMesh.transform.localScale = new Vector3(1, Mathf.Lerp(m_bodyMesh.transform.localScale.y, 0.2f, Time.fixedDeltaTime), 1);
			yield return new WaitForFixedUpdate();
		}

		//gameObject.layer = LayerMask.NameToLayer("Interactables");
	}

	public void Interact(Player.Interaction interactor)
	{
		m_isPickedUp = !m_isPickedUp;

		if (m_isPickedUp)
		{
			interactor.OnPickedUp(this.gameObject);

			//transform.parent = interactor.GetPickedUpTransform().gameObject.transform;
			transform.position = new Vector3(0, 0, 0);

			m_rigidbody.useGravity = false;
			m_collider.isTrigger = true;


		}

		else if (!m_isPickedUp)
		{
			interactor.OnDropped();

			//transform.parent = null;
			transform.position = interactor.GetDroppedTransform().position;

			m_rigidbody.useGravity = true;
			m_collider.isTrigger = false;
		}

	}

	public string GetInteractText()
	{
		if (!m_isPickedUp) return m_interactTextPickup;

		return m_interactTextDrop;
	}

	public Sprite GetInteractSprite()
	{
		if (!m_isPickedUp) return m_interactSpritePickup;

		return m_interactSpriteDrop;
	}
}
