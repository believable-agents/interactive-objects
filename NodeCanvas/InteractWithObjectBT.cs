using UnityEngine;
using System.Collections;
using NodeCanvas;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace ViAgents.NodeCanvas.Actions{

	//[ScriptName("Interact With Object")]
	[Category("★ Uruk")]
	public class InteractWithObjectBT : ActionTask<Transform> {

		public BBParameter<GameObject> gameObject;

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
				radius = agent.GetComponent<NavMeshAgent>().radius;
				agent.GetComponent<NavMeshAgent>().radius = 0;
			}

			// start the coroutine
			StartCoroutine(iObject.InteractWithObject(agent.gameObject, forceTransform, animationState, Finish));

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
				agent.GetComponent<NavMeshAgent>().radius = this.radius;
			}

			EndAction (true);
		}

		protected override string info{
			get {
				return "Interact with: " + (gameObject.value == null ? gameObject.ToString() : gameObject.value.name);
			}
		}
	}
}
