using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LightSettingsScriptableObject", order = 2)]
public class LightSettingsScriptableObject : ScriptableObject
{
	public float distanceModiferer = 0f;
	public float intensityModiferer = 0f;

}
