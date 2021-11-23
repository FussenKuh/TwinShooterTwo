using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ConfigureNavGrid : MonoBehaviour
{
    static GridGraph graph = null;

    [SerializeField]
    int _debugGridWidth = 10;
    [SerializeField]
    int _debugGridHeight = 10;
    [SerializeField]
    float _debugNodeSize = 0.5f;
    [SerializeField]
    Vector3 _debugLowerLeft = Vector3.zero;
    [SerializeField]
    bool _debugConfigureGraph = false;

    static public void SetDimensions(int gridWidth, int gridHeight, float nodeSize)
    {
        /*if (graph == null) */{ graph = AstarPath.active.data.gridGraph; }



        Debug.LogFormat("Configuring Nav Grid: {0} / {1}", (int)(gridWidth / nodeSize), (int)(gridHeight / nodeSize));

        graph.SetDimensions((int)(gridWidth / nodeSize), (int)(gridHeight / nodeSize), nodeSize);
        // Recalculate the graph
        AstarPath.active.Scan();
    }


    static public void SetCenter(Vector3 worldPosition)
    {
        /*if (graph == null)*/ { graph = AstarPath.active.data.gridGraph; }

        graph.center = worldPosition;
        // Recalculate the graph
        AstarPath.active.Scan();
    }

    static public void SetLowerLeft(Vector3 worldPosition)
    {
        if (graph == null) { graph = AstarPath.active.data.gridGraph; }

        graph.center = worldPosition + new Vector3((graph.Width*graph.nodeSize) / 2, (graph.Depth*graph.nodeSize) / 2, 0);
        // Recalculate the graph
        AstarPath.active.Scan();
    }

    static public bool Walkable(Vector3 worldPosition)
    {
        bool retVal = false;
        /*if (graph == null)*/ { graph = AstarPath.active.data.gridGraph; }
        var potentialNode = graph.GetNearest(worldPosition, NNConstraint.None);

        if (potentialNode.node != null)
        {
            //Debug.LogFormat("node[{0},{1}] at position {2} walkable = {3}. Selected position = {4}", 
            //    potentialNode.node.position.x, potentialNode.node.position.y, (Vector3)potentialNode.node.position, potentialNode.node.Walkable, worldPosition);

            if (potentialNode.node.Walkable)
            {
                retVal = true;
            }
        }

        return retVal;
    }

    static public Vector3 NearestWalkablePosition(Vector3 worldPosition)
    {
        Vector3 retVal = Vector3.zero;
        /*if (graph == null)*/ { graph = AstarPath.active.data.gridGraph; }
        var potentialNode = graph.GetNearest(worldPosition, NNConstraint.Default);

        if (potentialNode.node != null)
        {
            //Debug.LogFormat("node[{0},{1}] at position {2} walkable = {3}. Selected position = {4}", 
            //    potentialNode.node.position.x, potentialNode.node.position.y, (Vector3)potentialNode.node.position, potentialNode.node.Walkable, worldPosition);

            if (potentialNode.node.Walkable)
            {
                retVal = (Vector3)potentialNode.node.position;
            }
        }

        return retVal;
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugConfigureGraph)
        {
            _debugConfigureGraph = false;
            ConfigureNavGrid.SetDimensions(_debugGridWidth, _debugGridHeight, _debugNodeSize);
            ConfigureNavGrid.SetLowerLeft(_debugLowerLeft);
        }
    }
}
