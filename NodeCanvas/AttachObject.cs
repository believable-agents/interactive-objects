using UnityEngine;
using System.Collections;
using NodeCanvas;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace ViAgents.NodeCanvas.Actions
{
    //[ScriptName("Attach From Inventory")]
    [Category("★ ViAgents")]
    public class AttachObject : ActionTask<Transform>
    {
        [RequiredField]
        public BBParameter<GameObject> attachment;

        protected override void OnExecute()
        {
            var inventory = attachment.value.GetComponent<InventoryObject>();
            if (inventory == null)
            {
                inventory = attachment.value.GetComponentInChildren<InventoryObject>();
            }
            if (inventory == null)
            {
                Debug.LogError(attachment.value + " has to have InventoryObject component");
                return;
            }
            inventory.Attach(agent.transform);
            EndAction(true);
        }
    }
}

