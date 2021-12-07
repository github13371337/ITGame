using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VisualGraph))]
public class GraphGame : MonoBehaviour
{
    [SerializeField] Color normalColor;
    [SerializeField] Color activatedColor;
    [SerializeField] GUIEventButton resetButton;

    private VisualGraph visualGraph;
    private Vector2GraphNode[] graph;
    private List<GUIObject> nodes = new List<GUIObject>();
    private List<GUIObject> activated = new List<GUIObject>();
    private int previousNodeIndex = -1;

    public bool ready { get; private set; }
    public bool won { get; private set; }

    private void Awake()
    {
        visualGraph = GetComponent<VisualGraph>();
        graph = visualGraph.graphCopy;
    }

    private void OnEnable()
    {
        visualGraph.GraphCreated += OnGraphCreation;
        if (resetButton != null) resetButton.Interaction += Reset;
    }
    private void OnDisable()
    {
        visualGraph.GraphCreated -= OnGraphCreation;
        if (resetButton != null) resetButton.Interaction -= Reset;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is GUIInteractiveObject iObj)
            {
                iObj.Interaction -= ObjectInteraction;
            }
        }
    }

    private void OnGraphCreation(List<GUIObject> nodes, List<GUIObject> edges)
    {
        this.nodes = nodes;

        Reset(false, true);

        for (int i = 0; i < this.nodes.Count; i++)
        {
            if (this.nodes[i] is GUIInteractiveObject iObj)
            {
                iObj.Interaction += ObjectInteraction;
            }
        }

        ready = true;
    }

    private void ObjectInteraction(GUIInteractiveObject obj)
    {
        if (!activated.Contains(obj))
        {
            int index = nodes.FindIndex((o) => o == obj);

            if (previousNodeIndex < 0 || graph[previousNodeIndex].connections.Contains(index) || graph[index].connections.Contains(previousNodeIndex))
            {
                obj.SetColorAll(activatedColor);
                activated.Add(obj);

                previousNodeIndex = index;

                if (activated.Count == nodes.Count)
                {
                    won = true;
                    Clear?.Invoke();
                }
            }           
        }
    }

    public void Reset(GUIInteractiveObject obj) => Reset();
    public void Reset(bool resetColor = true, bool resetActivated = true)
    {
        if (ready)
        {
            previousNodeIndex = -1;
            if (resetColor) for (int i = 0; i < activated.Count; i++) activated[i].SetColorAll(normalColor);
            if (resetActivated) activated = new List<GUIObject>(nodes.Count);
        }
    }

    public event Action Clear;
}
