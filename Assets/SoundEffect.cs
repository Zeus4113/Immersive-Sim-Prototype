using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
	public void OnPlayed(float volume)
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, volume);

		for(int i = 0; i < hitColliders.Length; i++)
		{
			if (hitColliders[i].CompareTag("Sound Detector"))
			{
				float distance = Vector3.Distance(transform.position, hitColliders[i].transform.position);
				RaycastHit hit;
				Physics.Raycast(transform.position, (hitColliders[i].transform.position - transform.position).normalized, out hit);

				if (!hit.collider.CompareTag("Sound Detector"))
				{

					volume = volume / 2;
				}

				hitColliders[i].gameObject.GetComponent<SoundSensor>().isAlerted(volume, distance);
			}
		}
	}
}
