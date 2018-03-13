using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar {

    [Serializable]
    public class NodePath {

        public List<Node> allPathNodes;
        public Vector3[] vectorWaypoint;


        public NodePath(List<Node> allPathNodes) {
            this.allPathNodes = allPathNodes;
        }

    }

}
