using UnityEngine;
using System.Collections;
using NodeCanvas;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace ViAgents.NodeCanvas.Actions{

	//[ScriptName("Interact With Object")]
	[Category("★ ViAgents")]
	public class InteractWithObjectBT : ActionTask<Transform> {

		public BBParameter<GameObject> gameObject;
        public BBParameter<string> action;

		public bool forceTransform;
		public bool disablePushing = true;
		public string animationState;

		private float radius;
		private InteractiveObjectBT iObject;

		protected override void OnExecute ()
		{
			if (gameObject.value == null) {
				Debug.LogError("You did not assign any object!");
				return;
			}

			iObject = gameObject.value.GetComponent<InteractiveObjectBT>();
			if (iObject == null) {
				Debug.LogError("Agent can only interact with objects with behaviour trees: " + gameObject.value.name);
				return;
			}

			if (disablePushing)
			{
				radius = agent.GetComponent<UnityEngine.AI.NavMeshAgent>().radius;
				agent.GetComponent<UnityEngine.AI.NavMeshAgent>().radius = 0;
			}

			// start the coroutine
			StartCoroutine(iObject.InteractWithObject(agent.gameObject, action.value, forceTransform, animationState, Finish));

			base.OnExecute ();
		}

		protected override void OnStop()
		{
			// stop the graph on the gameobject
			iObject.StopGraph();
		}

		void Finish(bool result) {
			if (disablePushing)
			{
				agent.GetComponent<UnityEngine.AI.NavMeshAgent>().radius = this.radius;
			}

			EndAction (true);
		}

		protected override string info{
			get {
				return string.Format("{0} action on '{1}", this.action.value == null ? "Default" : this.action.value, gameObject.value == null ? gameObject.ToString() : gameObject.value.name);
			}
		}
	}
}
