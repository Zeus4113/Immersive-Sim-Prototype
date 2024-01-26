using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MissionDataScriptableObject", order = 2)]
public class MissionDataScriptableObject : ScriptableObject
{
	public float scorePercentage;
	public int alarmsTriggered;
	public int guardsAlerted;
	public char grade;
}
