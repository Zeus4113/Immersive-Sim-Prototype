using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class Flashlight : MonoBehaviour
	{
		[SerializeField] float m_visibilityIncrease;

		private Light m_light;
		private VisibilityCalculator m_visibilityCalculator;
		private Toolset m_toolset;
		private bool m_isFlashlightEnabled = false;

		public void Init(Toolset tools)
		{
			m_light = GetComponent<Light>();
			m_toolset = tools;
			m_visibilityCalculator = m_toolset.GetController().GetVisibilityCalculator();
			m_isFlashlightEnabled = false;
		}

		public void ToggleFlashlight()
		{
			if (m_light.enabled == true)
			{
				m_light.enabled = false;
				m_visibilityCalculator.FlashlightEnabled(false);
			}
			else if (m_light.enabled == false)
			{
				m_light.enabled = true;
				m_visibilityCalculator.FlashlightEnabled(true);
			}

		}

	}
}
