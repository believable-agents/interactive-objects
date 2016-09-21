using UnityEngine;
using System.Collections;

public class MecanimUtility  {

	public static IEnumerator MoveForward(Transform actor, float distance) {
		var anim = actor.GetComponent<Animator> ();
		var pos = actor.position;
		anim.SetFloat ("Speed", 0.25f);
		while (Vector3.Distance (actor.position, pos) < distance) {
//			Debug.Log ("Moving");
			yield return null;
		}
//		Debug.Log ("Finished movibg");
		anim.SetFloat ("Speed", 0f);
	}

	public static IEnumerator WaitForState(Animator anim, string stopState) {
		var stopStateHash= Animator.StringToHash(stopState);

		// now wait till we reach this state
		while (anim.GetCurrentAnimatorStateInfo(0).nameHash != stopStateHash) {
			yield return new WaitForEndOfFrame();
		}
	}

	public static IEnumerator WaitForAnimation(Animator anim, string stopState, float ratio) {
		// if we want to wait for animation, we do so
		if (!string.IsNullOrEmpty (stopState)) {
			var stopStateHash= Animator.StringToHash(stopState);

			// now wait till we reach this state
			while (anim.GetCurrentAnimatorStateInfo(0).nameHash != stopStateHash) {
//				Debug.Log ("waiting for state ...");
				yield return new WaitForEndOfFrame();
			}
		}

		// we may want to wait for a specific ratio in the clip
		if (ratio > 0) {
			var frac = ratio / 100f;
			while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime + float.Epsilon + Time.deltaTime < frac) {
//				Debug.Log ("Waiting for progress: " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime + float.Epsilon + Time.deltaTime);
				yield return new WaitForEndOfFrame ();
			}
		}
	}

	public static void SetParameter(Animator anim, string name, string value) {
		// parse parameter
		bool bParam;
		if (bool.TryParse (value, out bParam)) {
			anim.SetBool (name, bParam);
		} else {
			int iParam;
			if (int.TryParse (value, out iParam)) {
				anim.SetInteger (name, iParam);
			} else {
				float fParam;
				if (float.TryParse (value, out fParam)) {
					anim.SetFloat (name, fParam);
				} else {
					throw new System.NotImplementedException ("Value '" + value + "' not implemented: " + value);
				}
			}
		}
	}
}
