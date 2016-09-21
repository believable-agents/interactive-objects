using UnityEngine;
using System.Collections;

public abstract class Interactive : MonoBehaviour {

	public abstract IEnumerator InteractWithObject (GameObject sender);
}
