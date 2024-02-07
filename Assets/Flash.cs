using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
	[SerializeField] private float m_fuseTime = 1.5f;
	[SerializeField] private float m_range = 10f;

	private Light m_light;
	private AudioSource m_audioSource;

	private void Awake()
	{
		StartCoroutine(FlashTimer());
		m_light = GetComponent<Light>();
		m_audioSource = GetComponent<AudioSource>();
	}

	IEnumerator FlashTimer()
	{
		yield return new WaitForSeconds(m_fuseTime);

		m_light.enabled = true;
		m_audioSource.Play();

		yield return new WaitForSeconds(0.5f);

		LayerMask mask = LayerMask.GetMask("Enemies");

		Collider[] col = Physics.OverlapSphere(transform.position, 10f, mask);

		LayerMask raymask = LayerMask.GetMask("Environment", "Enemies", "Interactables");

		foreach (Collider c in col)
		{
			Debug.Log(c.gameObject.name);

			RaycastHit hit;
			Physics.Raycast(transform.position, c.transform.position - transform.position, out hit, raymask);
			Debug.DrawRay(transform.position, c.transform.position - transform.position, Color.white, 10f);

			if (hit.collider == c)
			{
				Debug.Log(c.gameObject.name);
				c.gameObject.GetComponentInParent<Enemy.GuardBehaviour>().SetStunned();
			}

		}

		m_light.enabled = false;
		transform.position = new Vector3(0, 0, 0);

		yield return new WaitForSeconds(0.1f);
		Destroy(gameObject);
	}
}
