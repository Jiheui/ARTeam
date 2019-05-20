using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour {

	public Color[] colours = {Color.black};

	public float deathDelay = 10f;
	public float spawnDelay = 15f;
	private float counter = 15f;

	private GameObject parentObject;

	void Start () {
		parentObject = GameObject.Find("Vehicles");
	}

	void Update () {
		counter += Time.deltaTime;
		if (counter >= spawnDelay)
		{
			int carID = Random.Range(0, 5);
			GameObject car = (GameObject)Instantiate(Resources.Load("Car" + carID));
			Destroy (car, deathDelay);
			car.transform.SetParent(parentObject.transform);
			int colourIndex = Random.Range(0, colours.Length);
			foreach (Material mat in car.GetComponent<Renderer> ().materials) {
				if (mat.name == "CarBody (Instance)")
				{
					mat.color = colours[colourIndex];
				}
			}
			counter = 0f;
		}
	}
}
