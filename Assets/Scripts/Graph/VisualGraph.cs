using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class VisualGraph : MonoBehaviour
{
    [SerializeField] Vector2GraphNode[] nodes;
    [SerializeField] GUIObject nodeObject;
    [SerializeField] GUIObject edgeObject;
    [SerializeField] bool createOnStart;

    private Canvas canvas;
    private List<(int, int)> connections = new List<(int, int)>();
    private GameObject parent;

    public List<GUIObject> nodeObjects { get; private set; } = new List<GUIObject>();
    public List<GUIObject> edgeObjects { get; private set; } = new List<GUIObject>();

    public Vector2GraphNode[] graphCopy => nodes.Copy();

    private void Awake() => canvas = GetComponentInParent<Canvas>();

    private void Start() { if (createOnStart) Create(); }

    [ContextMenu("Create visuals")]
    public void Create()
    {
        for (int i = 0; i < nodeObjects.Count; i++) Destroy(nodeObjects[i]);
        for (int i = 0; i < edgeObjects.Count; i++) Destroy(edgeObjects[i]);
        nodeObjects = new List<GUIObject>(nodes.Length);
        edgeObjects.Clear();
        connections.Clear();
        if (parent != null) Destroy(parent);

        (int, int) connection;
        Vector3 point;
        Vector3 point2;
        Vector3 direction;
        GUIObject edge;
        parent = new GameObject();
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

                    direction = edge.rect.DirectionToTarget(point2, true);
                    edge.rect.eulerAngles = new Vector3
                    (
                        edge.rect.eulerAngles.x,
                        edge.rect.eulerAngles.y,
                        Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
                    );
                    edge.rect.sizeDelta = new Vector2(Vector3.Distance(point, point2), edge.rect.sizeDelta.y);
                }
            }
        }

        for (int i = 0; i < edgeObjects.Count; i++)
        {
            edgeObjects[i].rect.SetParent(parent.transform);
            edgeObjects[i].rect.localScale = new Vector3(1f, 1f, 1f);
        }
        for (int i = 0; i < nodeObjects.Count; i++)
        {
            nodeObjects[i].rect.SetParent(parent.transform);
            nodeObjects[i].rect.localScale = new Vector3(1f, 1f, 1f);
        }

        parent.transform.position = transform.position;
        parent.transform.localScale = transform.localScale;

        GraphCreated?.Invoke(nodeObjects, edgeObjects);
    }

    public event Action<List<GUIObject>, List<GUIObject>> GraphCreated;
}
