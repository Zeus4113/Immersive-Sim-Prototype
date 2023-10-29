using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathTools : object
{
	public static float ClampAngle(float angle, float min, float max)
	{

		angle = NormalizeAngle(angle);
		if (angle > 180)
		{
			angle -= 360;
		}
		else if (angle < -180)
		{
			angle += 360;
		}

		min = NormalizeAngle(min);
		if (min > 180)
		{
			min -= 360;
		}
		else if (min < -180)
		{
			min += 360;
		}

		max = NormalizeAngle(max);
		if (max > 180)
		{
			max -= 360;
		}
		else if (max < -180)
		{
			max += 360;
		}

		return Mathf.Clamp(angle, min, max);
	}

	public static float NormalizeAngle(float angle)
	{
		while (angle > 360)
			angle -= 360;
		while (angle < 0)
			angle += 360;
		return angle;
	}

}
