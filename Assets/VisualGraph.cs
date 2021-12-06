using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class VisualGraph : MonoBehaviour
{
    [SerializeField] Vector2GraphNode[] nodes;
    [SerializeField] RectTransform nodeObject;
    [SerializeField] RectTransform edgeObject;

    private Canvas canvas;
    private List<(int, int)> connections = new List<(int, int)>();

    public List<RectTransform> nodeObjects { get; private set; } = new List<RectTransform>();
    public List<RectTransform> edgeObjects { get; private set; } = new List<RectTransform>();

    private void Awake() => canvas = GetComponentInParent<Canvas>();   

    [ContextMenu("Create visuals")]
    public void Create()
    {
        (int, int) connection;
        Vector3 point;
        Vector3 point2;
        Vector3 direction;
        RectTransform edge;
        GameObject parent = new GameObject();
        parent.transform.parent = canvas.transform;
        parent.transform.localPosition = Vector3.zero;

        for (int i = 0; i < nodes.Length; i++)
        {
            point = new Vector3(nodes[i].point.x, nodes[i].point.y);

            nodeObjects.Add(Instantiate(nodeObject, point, Quaternion.identity, canvas.transform));
            nodeObjects[nodeObjects.Count - 1].gameObject.SetActive(true);  

            for (int ii = 0; ii < nodes[i].connections.Length; ii++)
            {
                connection = new ValueTuple<int, int>(i, nodes[i].connections[ii]);
                if (connection.Item2 > connection.Item1)
                {
                    int t = connection.Item2;
                    connection.Item2 = connection.Item1;
                    connection.Item1 = t;
                }

                if (!connections.Contains(connection))
                {
                    point2 = new Vector3
                    (
                        nodes[nodes[i].connections[ii]].point.x,
                        nodes[nodes[i].connections[ii]].point.y
                    );

                    connections.Add(connection);

                    edge = Instantiate(edgeObject, point, Quaternion.identity, canvas.transform);
                    edge.gameObject.SetActive(true);
                    edgeObjects.Add(edge);

                    direction = edge.DirectionToTarget(point2, true);
                    edge.eulerAngles = new Vector3
                    (
                        edge.eulerAngles.x,
                        edge.eulerAngles.y,
                        Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
                    );
                    edge.sizeDelta = new Vector2(Vector3.Distance(point, point2), edge.sizeDelta.y);
                }
            }
        }

        for (int i = 0; i < edgeObjects.Count; i++)
        {
            edgeObjects[i].SetParent(parent.transform);
            edgeObjects[i].localScale = new Vector3(1f, 1f, 1f);
        }
        for (int i = 0; i < nodeObjects.Count; i++)
        {
            nodeObjects[i].SetParent(parent.transform);
            nodeObjects[i].localScale = new Vector3(1f, 1f, 1f);
        }

        parent.transform.position = transform.position;
        parent.transform.localScale = transform.localScale;
    }
}
