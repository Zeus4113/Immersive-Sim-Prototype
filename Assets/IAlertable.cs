using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAlertable
{
	public delegate void Alert(GameObject alertObject);
	public event Alert alertTriggered;

	public void StartAlerted() { }

	public void StartAlerted(float amount) { }

	public void StartAlerted(float amount, Vector3 position) { }

	public void StopAlerted() { }
}
