using UnityEngine;
using System.Collections;
using NodeCanvas;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace ViAgents.NodeCanvas.Actions 
{
	//[ScriptName("Attach From Inventory With Tag")]
	[Category("★ ViAgents")]
	public class AttachFromInventoryByTag : ActionTask<Transform>
	{
		public BBParameter<string> tag;
		public BBParameter<GameObject> attachedObject;
		
		protected override void OnExecute() 
		{
			var inventory = agent.GetComponent<Inventory> ();
			if (inventory == null) {
				Debug.LogError(agent.name + " has no inventory!");
			}
			var storedObject = inventory.FindOneByTag (tag.value);
			if (storedObject == null) {
				Debug.LogError(agent.name + " has no object with tag: " + tag.value);
			}
			
			// now instantiate a new object and attach to the bone
			var instance =  Object.Instantiate (storedObject.Object) as InventoryObject;
			instance.Attach (agent.transform);
			//storedObject.Object.Attach (agent.transform);

			// remember that we have used this gameobject
			attachedObject.value = instance.gameObject;

			EndAction (true);
		}

		protected override string info {
			get {
				return "Attach with tag '" + tag.value + "'";
			}
		}
	}
}

