using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
	private Transform[] registeredLights = new Transform[0];
	private RaycastCheck[] checks = new RaycastCheck[0];
	private List<RaycastCheck> checks_ = new List<RaycastCheck>();

	public void RegisterNewRaycast(Transform lightObject)
	{
		if (lightObject == null) return;

		registeredLights[registeredLights.Length] = lightObject;
		RaycastCheck newCheck = Instantiate(new GameObject().AddComponent<RaycastCheck>(), transform);
		checks_.Add(newCheck);
		//newCheck.TriggerCoroutine(lightObject, true);
	}

	public void RemoveRegisteredRaycast(Transform lightObject)
	{
		if (lightObject == null) return;

		for(int i = 0; i < checks_.Count; i++)
		{
			if (checks_[i] == lightObject.GetComponent<RaycastCheck>())
			{

			}
		}

		for (int i = 0; i < registeredLights.Length; i++)
		{
			if (registeredLights[i] == lightObject)
			{
				//checks[checks.Length].TriggerCoroutine(lightObject, false);
				registeredLights[i] = null;
				Destroy(checks[i]);
			}
		}
	}
}
