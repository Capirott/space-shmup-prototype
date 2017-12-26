using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    [Header("Set in Inspector")]
    public float rotationPerSecond = 0.1f;
    [Header("Set Dynamically")]
    public int levelShow = 0;
    Material mat;

	void Start () {
        mat = GetComponent<Renderer>().material;	
	}
	
	// Update is called once per frame
	void Update () {
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        if (levelShow != currLevel)
        {
            levelShow = currLevel;
            mat.mainTextureOffset = new Vector2(0.2f * levelShow, 0);
        }
        float rZ = -(rotationPerSecond * Time.time * 3600) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
