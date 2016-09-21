using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InteractionTracker : MonoBehaviour {

	[System.NonSerialized]
	public List<GameObject> InteractionList = new List<GameObject>(); // List of all interactive objects currently in range

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void InteractionTrigger() {
		float distance = 20000f;
		GameObject closest = null;
		foreach(GameObject obj in InteractionList) {
			if(obj != null && Vector3.Distance(transform.position, obj.transform.position) < distance) {
				closest = obj;
				distance = Vector3.Distance(transform.position, obj.transform.position);
			}
		}
		if(closest != null) {
			StartCoroutine(closest.GetComponent<Interactive>().InteractWithObject(transform.parent.gameObject));
		}
	}

	void OnTriggerEnter(Collider collider) {
		Interactive iscript = collider.GetComponent<Interactive> ();
		if(iscript != null) {
			InteractionList.Add(collider.gameObject);
		}
	}
	
	void OnTriggerExit(Collider collider) {
		Interactive iscript = collider.GetComponent<Interactive> ();
		if(iscript != null) {
			InteractionList.Remove(collider.gameObject);
		}
	}
}
