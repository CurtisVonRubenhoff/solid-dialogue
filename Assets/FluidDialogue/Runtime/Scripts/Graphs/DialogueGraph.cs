using System.Collections.Generic;
using CleverCrow.Fluid.Dialogues.Nodes;
using UnityEngine;

namespace CleverCrow.Fluid.Dialogues.Graphs {
    public interface IGraphData {
        INodeData Root { get; }
    }

    public class DialogueGraph : ScriptableObject, IGraphData {
        [HideInInspector]
        [SerializeField]
        private List<NodeDataBase> _nodes = new List<NodeDataBase>();

        [HideInInspector]
        public NodeRootData root;

        public INodeData Root => root;
        public IReadOnlyList<NodeDataBase> Nodes => _nodes;

        public void AddNode (NodeDataBase node) {
            node.Setup();
            _nodes.Add(node);
        }
    }
}
