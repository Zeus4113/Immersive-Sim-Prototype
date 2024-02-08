using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
	[SerializeField] string m_hintText = null;
	[SerializeField] bool m_isRetriggerable = false;

	[SerializeField] HintTrigger m_requiredTrigger;

	private int m_triggerCount = 0;

	private void OnTriggerEnter(Collider other)
	{
		if (m_requiredTrigger != null && m_requiredTrigger.GetTriggerCount() > 0)
		{
			if (m_isRetriggerable)
			{
				other.transform.GetComponent<Player.Controller>().GetManager().GetUIManager().GetHintPopup().DisplayHint(m_hintText);
				m_triggerCount++;
			}
			if (!m_isRetriggerable && m_triggerCount == 0)
			{
				other.transform.GetComponent<Player.Controller>().GetManager().GetUIManager().GetHintPopup().DisplayHint(m_hintText);
				m_triggerCount++;
			}
		}
		else if(m_requiredTrigger == null)
		{
			if (m_isRetriggerable)
			{
				other.transform.GetComponent<Player.Controller>().GetManager().GetUIManager().GetHintPopup().DisplayHint(m_hintText);
				m_triggerCount++;
			}
			if (!m_isRetriggerable && m_triggerCount == 0)
			{
				other.transform.GetComponent<Player.Controller>().GetManager().GetUIManager().GetHintPopup().DisplayHint(m_hintText);
				m_triggerCount++;
			}
		}

	}

	public int GetTriggerCount()
	{
		return m_triggerCount;
	}
}
