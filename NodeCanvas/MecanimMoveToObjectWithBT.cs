using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Actions{
	
	//[ScriptName("Mecanim Move To GameObject With BT")]
	[Category("★ Uruk")]
	public class MecanimMoveToObjectWithBT : MecanimMoveTo{
		
		[RequiredField]
		public BBParameter<GameObject> target;

//		public BBVector lookAtTarget = new BBVector();

		[System.NonSerialized]
		Vector3 position;
		
		protected override string info{
			get {return "GoTo " + target.ToString();}
		}
		
		protected override Vector3 Target {
			get {
                var io = target.value.GetComponent<InteractiveObjectBT>();
                if (io == null)
                {
                    Debug.LogError("Target object is not interactive (no InteractiveObjectBT component is present)");
                }
                return io.OffsetPosition;
			}
		}

		protected override Vector3 LookAt {
			get {
				return target.value.GetComponent<InteractiveObjectBT>().LookAtPosition;
			}
		}
		
		protected override void OnExecute(){
			
			if (target.value == null){
				Debug.LogError("Target GameObject location is not set correctly on Move To GameObject Action", agent.transform.gameObject);
				EndAction(false);
				return;
			}

			// set look at target so that it can be reused in another parts of the behaviour tree
//			lookAtTarget.value = target.value.GetComponent<InteractiveObjectBT> ().LookAtPosition;

			base.OnExecute ();
		}
	}
}
