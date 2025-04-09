using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTree{
    public class Location{
        byte OriginalX, OriginalY, DestinationX, DestinationY;
    }
    public class Node{
        public Location val {get; }
        public Node trueBranch {get ; set; }
        public Node falseBranch {get ; set; }


        public Node(Location val) { 
            this.val = val;
            this.trueBranch = null;
            this.falseBranch = null;
        }

        ~Node(){
            Debug.Log($"Destructor Called");
        }
    }
    public delegate bool ProcessMatch(Location loc);
    Node root = null;

    bool match(Node node, Location loc){
        return true;
    }
    bool noMatch(Node node, Location loc){
        return true;
    }

}
