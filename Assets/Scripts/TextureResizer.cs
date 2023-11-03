using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureResizer : MonoBehaviour
{

	[SerializeField] private float scaleFactor = 5.0f;

    // Start is called before the first frame update
    void Awake()
    {

        GetComponent<Renderer>().material.mainTextureScale = new Vector2 (transform.localScale.z / scaleFactor, transform.localScale.y / scaleFactor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
