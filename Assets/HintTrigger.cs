using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
	[SerializeField] string m_hintText = null;
	[SerializeField] bool m_isRetriggerable = false;

	private int m_triggerCount = 0;

	private void OnTriggerEnter(Collider other)
	{
		if (m_isRetriggerable)
		{
			other.transform.GetComponent<Player.Controller>().GetManager().GetUIManager().GetHintPopup().DisplayHint(m_hintText);
		}
		if(!m_isRetriggerable && m_triggerCount == 0)
		{
			other.transform.GetComponent<Player.Controller>().GetManager().GetUIManager().GetHintPopup().DisplayHint(m_hintText);
			m_triggerCount++;
		}

	}
}
