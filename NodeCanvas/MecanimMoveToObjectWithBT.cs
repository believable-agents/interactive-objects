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
        public bool forceTransform;
        public bool disablePushing;
        public string animationState;
  
        private float radius;
        private InteractiveObjectBT iObject;

        protected override string info{
			get {
                if (startInteraction)
                {
                    if (target.value != null)
                    {
                        return "Interact with " + target.value.ToString();
                    } else
                    {
                        return "Perform " + action;
                    }
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
                if (target.value == null)
                {
                    var iobjects = GameObject.FindObjectsOfType<InteractiveObjectBT>();
                    var filtered = iobjects.Where(i => i.actions.Any(o => o.action == action.value)).ToArray();

                    // find closest one
                    if (filtered.Length == 0)
                    {
                        throw new System.Exception("No object provides action: " + action);
                    }
                    var distance = float.MaxValue;
                    var closest = filtered[0];

                    foreach (var iobject in filtered)
                    {
                        var current = Vector3.Distance(agent.transform.position, iobject.transform.position);
                        if (current < distance)
                        {
                            distance = current;
                            closest = iobject;
                        }
                    }
                    target.value = closest.gameObject;
                }
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

            iObject = target.value.GetComponent<InteractiveObjectBT>();
            if (iObject == null)
            {
                Debug.LogError("Agent can only interact with objects with behaviour trees: " + target.value.name);
                return true;
            }

            if (disablePushing)
            {
                radius = agent.GetComponent<UnityEngine.AI.NavMeshAgent>().radius;
                agent.GetComponent<UnityEngine.AI.NavMeshAgent>().radius = 0;
            }

            // start the coroutine
            StartCoroutine(iObject.InteractWithObject(agent.gameObject, action.value, forceTransform, animationState, Finish));

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
