using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Linq;

namespace NodeCanvas.Actions{
	
	//[ScriptName("Mecanim Move To GameObject With BT")]
	[Category("★ Uruk")]
	public class MecanimMoveToObjectWithBT : MecanimMoveTo{
		
		
		public BBParameter<GameObject> target;
        public BBParameter<string> action;

        public bool startInteraction;
        public bool disablePushing;
        public string animationState;
  
        private float radius;
        private InteractiveObjectBT iObject;
        private InteractiveObjectBT.PositionOffset currentTarget;
 
        protected override string info{
			get {
                if (startInteraction)
                {
                    var name = string.IsNullOrEmpty(action.value) ? "Interact" : "Perform " + action;

                    if (target.name != null || target.value != null) {
                        name += " with " + target;
                    }
                    return name;
                } else
                {
                    if (target.value != null)
                    {
                        return "Go To " + target.value.ToString();
                    }
                    else
                    {
                        return "Find " + action;
                    }
                }
                
            }
		}
		
		protected override Vector3 Target {
			get {
                if (target.value == null) {
                    var iobjects = GameObject.FindObjectsOfType<InteractiveObjectBT>();
                    var filtered = iobjects.Where(i => i.actions.Any(o => o.action == action.value)).ToArray();

                    // find closest one
                    if (filtered.Length == 0) {
                        throw new System.Exception("No object provides action: " + action);
                    }

                    // sort them by distance from agent
                    var sorted = filtered.OrderBy((i) => Vector3.Distance(agent.transform.position, i.transform.position));

                    // find the closes available object
                    foreach (var obj in sorted) {
                        var position = obj.FindPosition(true);
                        if (position != null) {
                            this.iObject = obj;
                            this.currentTarget = position;

                            target.value = obj.gameObject;
                            return position.Position;
                        }
                    }
                    throw new System.Exception("There are no free spots!");
                   
                } else {
                    this.iObject = target.value.GetComponent<InteractiveObjectBT>();
                    if (this.iObject == null) {
                        throw new System.Exception("Target object is not interactive (no InteractiveObjectBT component is present)");
                    }
                    this.currentTarget = this.iObject.FindPosition();
                    if (this.currentTarget == null) {
                        throw new System.Exception("Current target has no free spots!");
                    }
                    return this.currentTarget.Position;
                }
			}
		}

		protected override Vector3 LookAt {
			get {
				return currentTarget.LookAt;
			}
		}
		
		protected override void OnExecute(){
			
			if (target.value == null && action.value == null){
				Debug.LogError("You need to set either target object or target action", agent.transform.gameObject);
				EndAction(false);
				return;
			}

			// set look at target so that it can be reused in another parts of the behaviour tree
//			lookAtTarget.value = target.value.GetComponent<InteractiveObjectBT> ().LookAtPosition;

			base.OnExecute ();
		}

        protected override bool Success()
        {
            if (!startInteraction)
            {
                return true;
            }

            if (disablePushing)
            {
                radius = agent.GetComponent<UnityEngine.AI.NavMeshAgent>().radius;
                agent.GetComponent<UnityEngine.AI.NavMeshAgent>().radius = 0;
            }

            // start the coroutine
            StartCoroutine(iObject.InteractWithObject(agent.gameObject, this.currentTarget, action.value, forcePosition, animationState, Finish));

            return false;
        }

        void Finish(bool result)
        {
            if (disablePushing)
            {
                agent.GetComponent<UnityEngine.AI.NavMeshAgent>().radius = this.radius;
            }

            EndAction(true);
        }

        protected override void OnStop()
        {
            base.OnStop();

            // stop the graph on the gameobject
            if (iObject != null)
            {
                iObject.StopGraph();
            }
        }
    }
}
