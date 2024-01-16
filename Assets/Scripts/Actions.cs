using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{

	public class Actions : MonoBehaviour
	{
		Behaviour m_behaviour;
		Perception m_perception;

		AlertLevel m_currentLevel;

		public void Init(Behaviour b)
		{
			m_behaviour = b;
			m_currentLevel = AlertLevel.passive;
			m_perception = GetComponent<Perception>();
		}

		public void BecomeAlerted()
		{
			if (m_currentLevel == AlertLevel.alerted) return;
			CameraAlerted();

		}

		public void BecomeSuspicous()
		{
			if (m_currentLevel == AlertLevel.suspicious) return;
			CameraSuspicious();
		}

		public void BecomePassive()
		{
			if (m_currentLevel == AlertLevel.passive) return;
			CameraPassive();
		}

		// Class Specific

		[Header("Materials")]
		[SerializeField] private Material m_yellowEmissive;
		[SerializeField] private Material m_redEmissive;
		[SerializeField] private Material m_greenEmissive;
		[Space(2)]

		[Header("Sounds")]
		[SerializeField] private AudioClip m_suspiciousSound;
		[SerializeField] private AudioClip m_alertedSound;
		[Space(2)]

		[Header("References")]
		[SerializeField] private MeshRenderer m_renderer;
		[SerializeField] private AudioSource m_audioSource;
		[SerializeField] private Light m_pointLight;

		void CameraAlerted()
		{
			m_renderer.material = m_redEmissive;
			m_pointLight.color = Color.red;
		}

		void CameraSuspicious()
		{
			m_renderer.material = m_yellowEmissive;
			m_pointLight.color = Color.yellow;
		}

		void CameraPassive()
		{
			m_renderer.material = m_greenEmissive;
			m_pointLight.color = Color.green;
		}

		void PlaySound()
		{
			m_audioSource.PlayOneShot(m_suspiciousSound);
		}

		void TriggerObjects(GameObject[] triggerables)
		{
			for (int i = 0; i < triggerables.Length; i++)
			{
				ITriggerable t = triggerables[i].GetComponentInChildren<ITriggerable>();
				t.Trigger();
			}
		}
	}
}
