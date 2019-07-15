using System;
using System.Collections.Generic;
using CleverCrow.Fluid.Dialogues.Actions;
using CleverCrow.Fluid.Dialogues.Conditions;
using UnityEngine;

namespace CleverCrow.Fluid.Dialogues.Nodes {
    public interface INodeData : IGetRuntime<INode> {
    }

    public abstract class NodeDataBase : ScriptableObject, INodeData {
        [HideInInspector]
        [SerializeField]
        private string _uniqueId;

        public List<NodeDataBase> children;
        public List<ConditionDataBase> conditions;
        public List<ActionDataBase> enterActions;
        public List<ActionDataBase> exitActions;
        public Vector2 position;

        public string UniqueId => _uniqueId;

        public abstract INode GetRuntime (IDialogueController dialogue);

        public void Setup () {
            _uniqueId = Guid.NewGuid().ToString();
        }
    }
}
