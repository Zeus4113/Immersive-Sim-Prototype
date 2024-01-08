using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourable
{
	public void ResetBehaviour();

	public void StartThinking();

	public void StopThinking();

	public void Init(EnemyManager em);

}
