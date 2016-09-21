using UnityEngine;
using System.Collections;

public static class AffineUtility {

	public static IEnumerator RotateTowards(Transform actor, Vector3 destination, float time = 1f) {
        //		var tim = 0f;
        //		var angle = Vector3.Angle (actor.forward, destination - actor.transform.position);
        //
        //		while (tim < time) {
        //			if (actor == null) yield break;
        //			actor.RotateAround (Vector3.up, angle * Mathf.Deg2Rad * Time.deltaTime * 2);
        //			//actor.transform.rotation = Quaternion.Slerp (actor.transform.rotation, targetRotation, time);
        //			tim += Time.deltaTime * 2;
        //			yield return new WaitForEndOfFrame ();   
        //		}
        //
        //		yield break;

        //		Debug.Log (destination);

        //var hash = iTween.Hash(
        //	"looktarget", destination,
        //	"time", time,
        //	"name", "rotate",
        //	"easetype", iTween.EaseType.linear
        //);
        // iTween.LookTo(actor.gameObject, hash);
        actor.transform.LookAt(destination);

		//var tim = 0f;
		//while (tim < time) {
		//	yield return null;
		//	tim += Time.deltaTime;
		//}
//		Debug.Log ("Finished rotating");
		yield break;

	}
	
	public static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle) {
		return angle * ( point - pivot) + pivot;
	}
}
