using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;


namespace DecisionMaking.BehaviorTree
{
    //public class BehaviorTreeNPC : BehaviorTree
    //{
    //    [Header("References")]
    //    [SerializeField] private NavMeshAgent agent;
    //    [SerializeField] private NPC npc;
    //    [SerializeField] private RivalsWarehouse warehouse;
    //    [SerializeField] private Farm[] farms;
    //    [SerializeField] private float wanderRadius;

    //    protected override Node SetupTree()
    //    {
    //        return new Selector(new List<Node>
    //        {
    //            new Sequence(new List<Node>
    //            {
    //                new StunnedNode(npc),
    //                new WanderNode(npc, agent, wanderRadius),
    //            }),
    //            new Sequence(new List<Node>
    //            {
    //                new RunToFarmNode(npc, agent, farms),
    //                new CollectResourcesNode(npc),
    //                new RunToWarehouseNode(npc, agent, warehouse),
    //                new GiveAwayResourcesNode(npc),
    //            }),
    //        });
    //    }
    //}
}
