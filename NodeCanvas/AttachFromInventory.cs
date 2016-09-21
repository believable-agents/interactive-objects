using UnityEngine;
using System.Collections;
using NodeCanvas;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace ViAgents.NodeCanvas.Actions 
{
	//[ScriptName("Attach From Inventory")]
	[Category("★ Uruk")]
	public class AttachFromInventory : ActionTask<Transform> 
	{
		public BBParameter<string> name;

		protected override void OnExecute() 
		{
			var inventory = agent.GetComponent<Inventory> ();
			var storedObject = inventory.FindByName (name.value);
			if (storedObject == null) {
				Debug.LogError(agent.name + " has no object with name: " + name.value);
			}

			// now instantiate a new object and attach to the bone
			//var instance = Instantiate (storedObject.Object) as GameObject;
			//var newObject = instance.GetComponent<InventoryObject> ();
			//newObject.Attach (agent);
			storedObject.Object.Attach (agent.transform);
		}
	}
}

