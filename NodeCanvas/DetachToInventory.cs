using UnityEngine;
using System.Collections;
using NodeCanvas;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace ViAgents.NodeCanvas.Actions 
{
	//[ScriptName("Detach To Inventory")]
	[Category("★ Uruk")]
	public class DetachToInventory : ActionTask<Transform> 
	{
		public BBParameter<GameObject> attachedObject;

		protected override void OnExecute ()
		{
			Object.Destroy (attachedObject.value);
			EndAction (true);
		}
	}
}

