using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PerceptionDataScriptableObject", order = 1)]
public class PerceptionDataScriptableObject : ScriptableObject
{
	public bool canLook = true;
	public bool canListen = true;

	public float lookRange = 10f;
	public float listenRange = 10f;

	public float lookThreshold = 30f;
	public float listenThreshold = 0.35f;

	public float volumeFalloff = 2f;
	public float fovAngle = 45f;
	public bool distanceFalloff = false;

	public bool hasPeripherals = false;
	public float peripheralDistance = 10f;
	public float peripheralThreshold = 10f;
}
