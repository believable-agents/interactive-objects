using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



public class ReferenceManager : MonoBehaviour {
	[SerializeField]
	List<Reference> references;

	public List<Reference> References {
		get {
			return references;
		}
		set {
			references = value;
		}
	}

	public GameObject GetObject(int id) {
		// TODO: Optimize
		try {
			return references.Find (w => w.Id == id).Object;
		} catch (System.Exception ex) {
			Debug.LogWarning ("No reference for '" + id + "': " + ex.Message);
		}
		return null;
	}

	public int AddObject(GameObject obj) {
		if (references == null) {
			references = new List<Reference> ();
		}

		// check if this object already exists
		if (references.Exists (w => w.Object == obj)) {
			return references.Find (w => w.Object == obj).Id;
		}

		var id = 1;
		if (references.Count > 0) {
			id = references.Max (w => w.Id) + 1;
		}
		references.Add (new Reference (id, obj));
		return id;
	}
}

[System.Serializable]
public class Reference {
	public int Id;
	public GameObject Object;

	public Reference() { } 

	public Reference(int id, GameObject obj) {
		Id = id;
		Object = obj;
	}
}
