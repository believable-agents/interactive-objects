using UnityEngine;
using System.Collections;
using NodeCanvas;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace ViAgents.NodeCanvas.Actions 
{
	//[ScriptName("Attach From Inventory")]
	[Category("★ ViAgents")]
	public class AttachFromInventory : ActionTask<Transform> 
	{
		public BBParameter<string> objectName;

		protected override void OnExecute() 
		{
			var inventory = agent.GetComponent<Inventory> ();
			var storedObject = inventory.FindByName (objectName.value);
			if (storedObject == null) {
				Debug.LogError(agent.name + " has no object with name: " + objectName.value);
			}

			// now instantiate a new object and attach to the bone
			//var instance = Instantiate (storedObject.Object) as GameObject;
			//var newObject = instance.GetComponent<InventoryObject> ();
			//newObject.Attach (agent);
			storedObject.Object.Attach (agent.transform);
		}
	}
}

