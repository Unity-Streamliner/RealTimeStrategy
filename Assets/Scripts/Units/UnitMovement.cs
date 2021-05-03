using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    #region Server

    [ServerCallback]
    private void Update() 
    {
        Targetable target = targeter.GetTargetable();
        if (target != null) 
        {
            //if (Vector3.Distance(agent.transform.position, target.transform.position) > chaseRange) 
            // - this method use square root is quite slow in term of performance
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                // Chase
                agent.SetDestination(target.transform.position);
            } else if (agent.hasPath) // if we compare distance we can reset path all the time when it close to range
            {
                // Stop chase
                agent.ResetPath();
            }
            return;
        }
        if (!agent.hasPath) { return; }
        if (agent.remainingDistance > agent.stoppingDistance) { return; }
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {

        targeter.ClearTarget();
        
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }
        agent.SetDestination(hit.position);
    }

    #endregion

    
}
