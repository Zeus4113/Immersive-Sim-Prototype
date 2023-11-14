using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureDetector : MonoBehaviour
{
	[SerializeField] private float brightnessOffset;
	private Transform raycastOrigin;
	private float brightness;

	void Start()
	{
		brightness = 0f;
		raycastOrigin = transform;
		StartCoroutine(DetectFloorCollider());
	}

	public float GetVisibility()
	{
		return brightness;
	}

	private IEnumerator DetectFloorCollider()
	{
		while (true)
		{
			RaycastHit hit;
			LayerMask layerMask = LayerMask.GetMask("Environment");
			Physics.Raycast(raycastOrigin.position, Vector3.down, out hit, 10f, layerMask);
			Debug.DrawLine(raycastOrigin.position, raycastOrigin.position + new Vector3(0, -1, 0), Color.red, 0.1f);

			if (hit.collider == null) yield return null;

			//Debug.Log(hit.collider.gameObject);

			Renderer floorRendererRef = hit.transform.GetComponent<Renderer>();
			MeshCollider meshColliderRef = hit.collider as MeshCollider;

			if (floorRendererRef == null || floorRendererRef.lightmapIndex > 253) yield return null;

			LightmapData lightMapInfo = LightmapSettings.lightmaps[floorRendererRef.lightmapIndex];
			Texture2D texture = lightMapInfo.lightmapDir;
			Vector2 pixelCoord = hit.lightmapCoord;

			Color color = texture.GetPixelBilinear(pixelCoord.x, pixelCoord.y);			

			float red, green, blue;

			red = color.r * 2.2f;
			green = color.g * 2.2f;
			blue = color.b * 2.2f;

			color = new Color((0.2126f * red), (0.7152f * green), (0.0722f * blue));

			brightness = (color.r + color.g + color.b) - brightnessOffset;

			//brightnessPercentage = Mathf.Clamp(brightness, 0, 1);

			//brightnessPercentage = Mathf.Round(brightnessPercentage);

			yield return new WaitForSeconds(0.1f);
		}
	}
}
