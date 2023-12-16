using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectAlert : MonoBehaviour
{

	public void CheckForListeners(float volume)
	{
		SoundSensor[] listeners = CheckColliders(CheckSoundRadius(volume));

		for(int i = 0; i < listeners.Length; i++)
		{
			if (listeners[i].enabled)
			{
				//listeners[i].isAlerted(volume, transform);
			}
		}
	}

	private Collider[] CheckSoundRadius(float radius)
	{
		LayerMask mask = LayerMask.GetMask("Enemy");

		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radius, mask);

		return hitColliders;
	}

	private SoundSensor[] CheckColliders(Collider[] colliders)
	{
		List<SoundSensor> sensors = new List<SoundSensor>();

		for(int i = 0; i < colliders.Length; i++)
		{
			RaycastHit hit;
			Physics.Raycast(transform.position, (colliders[i].transform.position - transform.position).normalized, out hit);

			if (hit.collider == null) return null;

			sensors.Add(hit.collider.gameObject.GetComponent<SoundSensor>());
			
		}

		return sensors.ToArray();
	}
} 
