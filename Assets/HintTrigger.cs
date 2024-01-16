using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
	[SerializeField] string m_hintText = null;

	private void OnTriggerEnter(Collider other)
	{
		other.GetComponent<Player.Controller>().GetManager().GetUIManager().GetHintPopup().DisplayHint(m_hintText);
	}
}
