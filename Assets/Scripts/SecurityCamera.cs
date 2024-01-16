using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class SecurityCamera : MonoBehaviour, IAlertable
{

	[Header("Floats")]
	[SerializeField] private float m_updateTime = 0.1f;
	[SerializeField] private float m_cameraRotation = 45f;
	[SerializeField] private float m_cameraStopTime = 0.5f;
	[SerializeField] private float m_initialRotation = 0f;
	[SerializeField] private float m_defaultAlertTime = 5f;
	[Space(2)]

	[Header("Data Sets")]
	[SerializeField] private PerceptionDataScriptableObject m_perceptionData;
	[Space(2)]

	[Header("References")]
	[SerializeField] private Perception m_perception;
	[SerializeField] private MeshRenderer m_renderer;
	[SerializeField] private Light m_light;
	[SerializeField] private AudioSource m_audioSource;
	[Space(2)]

	[Header("Materials")]
	[SerializeField] private Material m_greenEmissive;
	[SerializeField] private Material m_redEmissive;
	[Space(2)]

	[Header("Triggerables")]
	[SerializeField] private GameObject[] m_triggerables;
	[SerializeField] private Alarm m_alarm;

	void TriggerObjects()
	{
		if(m_triggerables.Length > 0)
		{
			for(int i = 0; i < m_triggerables.Length; i++)
			{
				m_triggerables[i].GetComponent<ITriggerable>().Trigger();
			}
		}
	}

	private void Awake()
	{
		//m_initialRotation = transform.rotation.y;

		Debug.Log(gameObject.name + " Rotation " + m_initialRotation);
		Debug.Log(gameObject.name + "Position " + transform.position);

		m_perception.Init(m_perceptionData, m_updateTime);
		m_perception.perceptionAlerted += StartAlerted;
		m_perception.StartLooking();

		if (m_cameraRotation > 0) StartRotating();

		m_light.color = Color.green;
		m_renderer.material = m_greenEmissive;
		m_audioSource.Play();
	}

	bool c_isAlerted = false;
	Coroutine c_alerted;
	float m_alertTime;

	public void StartAlerted(float amount, Vector3 position)
	{
		m_alertTime = m_defaultAlertTime;

		if (m_alarm != null) m_alarm.StartAlerted(m_defaultAlertTime);

		if (c_isAlerted) return;
		c_isAlerted = true;

		if (c_alerted != null) return;
		c_alerted = StartCoroutine(Alerted(position));

		m_light.color = Color.red;
		m_renderer.material = m_redEmissive;

		StopRotating();
		TriggerObjects();
	}

	public void StopAlerted()
	{
		if (!c_isAlerted) return;
		c_isAlerted = false;

		if (c_alerted == null) return;
		StopCoroutine(c_alerted);
		c_alerted = null;

		m_light.color = Color.green;
		m_renderer.material = m_greenEmissive;
		StartRotating();
	}

	IEnumerator Alerted(Vector3 AlertPosition)
	{
		while (c_isAlerted && m_alertTime > 0)
		{

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(AlertPosition - transform.position), Time.fixedDeltaTime);
			m_alertTime -= Time.fixedDeltaTime;
			if (m_alertTime < 0) m_alertTime = 0;

			yield return new WaitForFixedUpdate();
		}

		StopAlerted();
	}

	bool c_isRotating = false;
	Coroutine c_rotating;

	void StartRotating()
	{
		if(c_isRotating) return;
		c_isRotating = true;

		if (c_rotating != null) return;
		c_rotating = StartCoroutine(Rotating());
	}

	void StopRotating()
	{
		if (!c_isRotating) return;
		c_isRotating = false;

		if (c_rotating == null) return;
		StopCoroutine(c_rotating);
		c_rotating = null;
	}

	IEnumerator Rotating()
	{
		Debug.Log(m_initialRotation);

		Quaternion leftTarget = Quaternion.Euler(transform.rotation.x, m_initialRotation + m_cameraRotation, transform.rotation.z);
		Quaternion rightTarget = Quaternion.Euler(transform.rotation.x, m_initialRotation - m_cameraRotation, transform.rotation.z);

		Quaternion targetRot = leftTarget;

		while (c_isRotating)
		{

			if(transform.rotation == leftTarget)
			{
				targetRot = rightTarget;
				yield return new WaitForSeconds(m_cameraStopTime);
			}

			if (transform.rotation == rightTarget)
			{
				targetRot = leftTarget;
				yield return new WaitForSeconds(m_cameraStopTime);
			}

			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime);

			yield return new WaitForFixedUpdate();
		}
	}
}
