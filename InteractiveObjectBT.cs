using UnityEngine;
using System.Collections;
using DG.Tweening;
using NodeCanvas.BehaviourTrees;
using NodeCanvas;
using ViAgents;
using NodeCanvas.Framework;
using System.Linq;
using System;
using Ei.Agents.Core;

public class InteractiveObjectBT : Interactive
{
    [Serializable]
    public class PositionOffset
    {
        public Vector3 Position;
        public Vector3 LookAt;
        public int Index;
        public static PositionOffset zero = new PositionOffset { Position = Vector3.zero, LookAt = Vector3.zero };
    }

    [Serializable]
    public class BTAction
    {
        public string action;
        public string owner;
        public BehaviourTree BT;
    }

    public PositionOffset[] positionOffsets = new PositionOffset[0]; // where will agent stand when it starts interacting with object
    public BTAction[] actions;
    private Agent agent;

    public bool[] spots;

    private BehaviourTree BT;

    #region implemented abstract members of Interactive

    private int GetSpot(bool occupy = true) {
        if (positionOffsets == null || positionOffsets.Length == 0) {
            return -1;
        }

        if (spots.Length != positionOffsets.Length) {
            spots = new bool[positionOffsets.Length];
        }

        for (var i = 0; i < spots.Length; i++) {
            if (spots[i] == false) {
                if (occupy) {
                    spots[i] = true;
                }
                return i;
            }
        }
        return -2;
    }

    public PositionOffset FindPosition(bool reserve = false) {
        PositionOffset position = PositionOffset.zero;
        var spot = GetSpot(reserve);
        if (spot == -2) {
            return null;
        }
        if (spot != -1) {
            position = positionOffsets[spot];
        }
        return new PositionOffset {
            Position = GetPosition(position.Position),
            LookAt = GetPosition(position.LookAt),
            Index = spot
        };
    }

    public IEnumerator InteractWithObject(GameObject sender, PositionOffset position, string action, bool forceTransform, string animationState, System.Action<bool> callback) {
        // var position = FindPosition();
        if (position == null) {
            position = FindPosition();
        }

        if (this.actions.Length == 0) {
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
        this.agent = sender.GetComponent<Agent>();

        // we can force that user will appear at the defined location
        if (forceTransform) {
            // wait till avatar is at full stop
            if (!string.IsNullOrEmpty(animationState)) {
                yield return StartCoroutine(MecanimUtility.WaitForState(sender.GetComponent<Animator>(), animationState));
            }


            // itween to the new position
            sender.transform.DOMove(position.Position, 0.5f).OnComplete(() => {
                sender.transform.DOLookAt(position.LookAt, 0.5f, AxisConstraint.Y);
            });

            this.Log("Position Forced");
            yield return new WaitForSeconds(0.5f);
        }


        // take blackboard from component
        var blackboard = sender.GetComponent<Blackboard>();
        if (blackboard == null) {
            blackboard = sender.AddComponent<Blackboard>();
        }


        // copy all values from object blackboard
        blackboard.SetValue("CurrentAction", action);
        blackboard.SetValue("InteractiveObject", gameObject);
        blackboard.SetValue("OffsetPosition", position.Position);
        blackboard.SetValue("LookAtPosition", position.LookAt);
        //blackboard.SetValue ("Actor", sender);

        BT.repeat = false;
        var i = (BehaviourTree)Instantiate(BT);

        // get the behaviour tree owner
        var owner = GetComponent<BehaviourTreeOwner>();
        if (owner != null) {
            owner.graph = i;
        }

        // TODO: Investigate the parameter
        i.StartGraph(sender.transform, blackboard, true, (result) => GraphStoppedCallback(callback, result, position.Index));

        this.Log("Tree started.");

        yield return null;
    }

    private void GraphStoppedCallback(System.Action<bool> callback, bool result, int idx) {
        this.Log("Tree stopped.");

        if (idx >= 0) {
            this.spots[idx] = false;
        }


        if (callback != null) {
            callback(result);
        }
    }

    public override IEnumerator InteractWithObject(GameObject sender) {
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
        if (this.BT != null) {
            this.BT.Stop();
        }
    }

    private void Log(string message) {
        if (this.agent != null) {
            Debug.Log(string.Format("✫ ({0}) {1}", gameObject.name, message));
        } else {
            Debug.Log(message);
        }
    }

    static Vector3 positionSize = new Vector3(0.3f, 0.05f, 0.3f);
    static Vector3 lookAtSize = new Vector3(0.2f, 0.05f, 0.2f);

    public void OnDrawGizmosSelected() {
        if (positionOffsets != null) {
            foreach (var offset in positionOffsets) {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(GetPosition(offset.Position), positionSize);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(GetPosition(offset.LookAt), positionSize);
            }
        }

    }

    Vector3 GetPosition(Vector3 offset) {
        var p = transform.position + offset;
        return AffineUtility.RotateAroundPoint(p, transform.position, transform.rotation);
    }

    #endregion
}
