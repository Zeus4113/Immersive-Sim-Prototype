using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerable
{
	public void Trigger() { }

	public void Trigger(GameObject triggerObject) { }

	public void Trigger(float amount) { }
}
