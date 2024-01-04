using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
	public class CameraActions : MonoBehaviour
	{
		private CameraBehaviour m_behaviour;

		private MeshRenderer m_lightRenderer;
		private AudioSource m_audioSource;
		private Light m_pointLight;

		[SerializeField] private Material m_yellowEmissive;
		[SerializeField] private Material m_redEmissive;
		[SerializeField] private Material m_greenEmissive;

		[SerializeField] private AudioClip m_suspiciousSound;
		[SerializeField] private AudioClip m_alertedSound;

		public void Init(CameraBehaviour cb)
		{
			m_behaviour = cb;

			Transform lightTransform = transform.GetChild(0);

			if (lightTransform == null) return;

			m_lightRenderer = lightTransform.GetComponent<MeshRenderer>();
			m_audioSource = this.GetComponent<AudioSource>();
			m_pointLight = lightTransform.GetComponentInChildren<Light>();
		}

		public void CameraAlerted()
		{
			m_lightRenderer.material = m_redEmissive;
			m_pointLight.color = Color.red;
			m_pointLight.enabled = true;
		}

		public void CameraSuspicious()
		{
			m_lightRenderer.material = m_yellowEmissive;
			m_pointLight.color = Color.yellow;
			m_pointLight.enabled = true;
		}

		public void CameraPassive()
		{
			m_lightRenderer.material = m_greenEmissive;
			m_pointLight.enabled = false;
		}

		public void PlaySound()
		{
			m_audioSource.PlayOneShot(m_suspiciousSound);
		}

		public void TriggerInteractables(GameObject[] interactables)
		{
			for(int i = 0; i < interactables.Length; i++)
			{
				IInteractable interactable = interactables[i].GetComponentInChildren<IInteractable>();
				interactable.Interact();
			}
		}

	}
}
