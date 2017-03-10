using UnityEngine;
using System.Collections;
using DG.Tweening;
using NodeCanvas.BehaviourTrees;
using NodeCanvas;
using ViAgents;
using NodeCanvas.Framework;
using ViAgents.Unity;
using System.Linq;
using System;

public class InteractiveObjectBT : Interactive
{
    [Serializable]
    public class BTAction
    {
        public string action;
        public BehaviourTree BT;
    }

    public Vector3 positionOffset;
	public Vector3[] positionOffsets; // where will agent stand when it starts interacting with object
	public Vector3 lookAt; // where will agent look 
    public BTAction[] actions;
	private ViAgent agent;
    public bool[] spots;

    private BehaviourTree BT;

	#region implemented abstract members of Interactive

	public Vector3 OffsetPosition { get { return GetPosition (positionOffset); } }
	public Vector3 LookAtPosition { get { return GetPosition (lookAt); } }

    private int GetSpot()
    {
        if (positionOffsets == null || positionOffsets.Length == 0)
        {
            return -1;
        }

        if (spots.Length == 0 && (positionOffsets != null && positionOffsets.Length > 0))
        {
            spots = new bool[positionOffsets.Length];
        }

        for (var i = 0; i < spots.Length; i++)
        {
            if (spots[i] == false)
            {
                spots[i] = true;
                return i;
            }
        }
        throw new UnityException("All spots are taken!");
    }

	public IEnumerator InteractWithObject (GameObject sender, string action, bool forceTransform, string animationState, System.Action<bool> callback)
	{
	    var spotIndex = GetSpot();
	    var spot = spotIndex < 0 ? this.positionOffset : this.positionOffsets[spotIndex];
//		Debug.Log("Starting interaction ...");

        if (this.actions.Length == 0)
        {
            throw new System.Exception("Interactive object provides no actions");
        }

        if (action == null) {
            BT = actions[0].BT;
        } else {
            BT = actions.First(a => a.action == action).BT;
        }
		if (BT == null) {
			Debug.LogError(name + " does not have a behaviour tree assigned");
			yield break;
		}

		// get agent reference for log purposes
		this.agent = sender.GetComponent<ViAgent>();

		// we can force that user will appear at the defined location
		if (forceTransform)
		{
			// wait till avatar is at full stop
			if (!string.IsNullOrEmpty(animationState)) {
				yield return StartCoroutine(MecanimUtility.WaitForState(sender.GetComponent<Animator>(), animationState));
			}

            var p = GetPosition(lookAt);

            // itween to the new position
            sender.transform.DOMove(GetPosition(spot), 0.5f).OnComplete(() =>
		    {
                if (!lookAt.Equals(Vector3.zero))
                {
                    // this.Log("Look at: " + p);
                    sender.transform.DOLookAt(p, 0.5f, AxisConstraint.Y);
                }
            });
			
			this.Log("Position Forced");
            yield return new WaitForSeconds(0.5f);
        }


		// take blackboard from component
		var blackboard = sender.GetComponent<Blackboard> ();

		// copy all values from object blackboard
		blackboard.SetValue ("InteractiveObject", gameObject);
		blackboard.SetValue ("OffsetPosition", OffsetPosition);
		blackboard.SetValue ("LookAtPosition", LookAtPosition);
		//blackboard.SetValue ("Actor", sender);

	    BT.repeat = false;
	    var i = (BehaviourTree) Instantiate(BT);

        // get the behaviour tree owner
        var owner = GetComponent<BehaviourTreeOwner>();
        if (owner != null)
        {
            owner.graph = i;
        }

        // TODO: Investigate the parameter
        i.StartGraph (sender.transform, blackboard, true, (result) => GraphStoppedCallback(callback, result, spotIndex));

		this.Log ("Tree started.");

		yield return null;
	}

	private void GraphStoppedCallback(System.Action<bool> callback, bool result, int idx) {
		this.Log("Tree stopped.");

	    if (idx >= 0)
	    {
	        this.spots[idx] = false;
	    }


	    if (callback != null)
		{
			callback(result);
		}
	}

	public override IEnumerator InteractWithObject (GameObject sender) {
//		sender.transform.position = GetPosition(positionOffset);
//		
//		// set look only if it is not empty
//		if (!lookAt.Equals (Vector3.zero)) {	
//			var p = GetPosition(lookAt);
//			sender.transform.LookAt (
//				new Vector3(
//					p.x,
//					sender.transform.position.y,
//					p.z));
//		}
//		
//		// take blackboard from component
//		var blackboard = GetComponent<Blackboard> ();
//		
//		// add blackboard if it does not exist!
//		if (blackboard == null) {
//			blackboard = gameObject.AddComponent<Blackboard> ();
//		}
//		// copy all values from object blackboard
//		blackboard.SetDataValue ("interactiveObject", gameObject);
//		blackboard.SetDataValue ("actor", sender);
//		
//		BT.StartGraph (sender.transform.FindChild("Avatar"), blackboard, null);
		yield return null;
	}

	public void StopGraph() {
		if (this.BT != null)
		{
			this.BT.Stop();
		}
	}

	private void Log(string message) {
		if (this.agent != null)
		{
			agent.Log(LogLevel.Debug, LogSource.Action,  string.Format("✫ ({0}) {1}", gameObject.name,  message));
		} else
		{
			Debug.Log(message);
		}
	}

	static Vector3 positionSize = new Vector3 (0.3f, 0.05f, 0.3f);
	static Vector3 lookAtSize = new Vector3 (0.2f, 0.05f, 0.2f);

	public void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
        Gizmos.DrawCube(GetPosition(positionOffset), positionSize);

        Gizmos.color = Color.yellow;
        if (positionOffsets != null)
	    {
	        foreach (var offset in positionOffsets)
	        {
	            Gizmos.DrawCube(GetPosition(offset), positionSize);
	        }
	    }

	    Gizmos.color = Color.blue;
		Gizmos.DrawCube(GetPosition(lookAt), lookAtSize);
	}

	Vector3 GetPosition(Vector3 offset) {
		var p = transform.position + offset;
		return  AffineUtility.RotateAroundPoint(p, transform.position, transform.rotation);
	}

	#endregion
}
