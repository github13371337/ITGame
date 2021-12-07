using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MemoryGame))]
public class MemoryGameInterface : MonoBehaviour
{
    [Serializable] private class Row
    {
        public RectTransform frame;
        public GUIObject bottom;
        public GUIInteractiveObject createButton;
        public GUIInteractiveObject deleteButton;
        public RectTransform drop;        
    }

    [SerializeField] Row originalRow;
    [SerializeField] Text blockCounter;
    [SerializeField] Text timer;
    [SerializeField] float indent;
    [SerializeField] GUIObject winScreen;
    [SerializeField] GUIObject loseScreen;

    private int rows;
    private Row[] copyRows;
    private Canvas canvas;
    private MemoryGame game;
    private RectTransform[] activeDrops;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        game = GetComponent<MemoryGame>();

        rows = game.rows;

        blockCounter.text = game.availableBlocks.ToString();

        copyRows = new Row[rows];
        activeDrops = new RectTransform[rows];

        for (int i = 0; i < rows; i++)
        {
            copyRows[i] = new Row();
            copyRows[i].frame = Instantiate(originalRow.frame, canvas.transform);
            copyRows[i].bottom = Instantiate(originalRow.bottom, copyRows[i].frame.transform);
            copyRows[i].createButton = Instantiate(originalRow.createButton, copyRows[i].frame.transform);
            copyRows[i].deleteButton = Instantiate(originalRow.deleteButton, copyRows[i].frame.transform);
            copyRows[i].drop = Instantiate(originalRow.drop, copyRows[i].frame.transform);

            copyRows[i].bottom.transform.position = originalRow.bottom.transform.position;
            copyRows[i].createButton.transform.position = originalRow.createButton.transform.position;
            copyRows[i].deleteButton.transform.position = originalRow.deleteButton.transform.position;
            copyRows[i].drop.position = originalRow.drop.position;

            copyRows[i].frame.position = originalRow.frame.position;
            copyRows[i].frame.anchoredPosition3D += new Vector3((originalRow.frame.sizeDelta.x + indent) * originalRow.frame.localScale.x, 0f) * i;
        }

        Destroy(originalRow.frame.gameObject);
        Destroy(originalRow.bottom.gameObject);
        Destroy(originalRow.createButton.gameObject);
        Destroy(originalRow.deleteButton.gameObject);
        Destroy(originalRow.drop.gameObject);
    }

    private void OnEnable()
    {
        game.NewDrop += OnNewDrop;
        game.DropMoved += OnDromMove;
        game.DropRemoved += OnDropRemove;
        game.BlockActivated += OnBlockActivation;
        game.BlockDeactivated += OnBlockDeactivation;
        game.Win += OnWin;
        game.Lose += OnLose;

        for (int i = 0; i < rows; i++)
        {
            int li = i;
            copyRows[i].createButton.Interaction += (o) => game.ActivateBlock(li);
        }
        for (int i = 0; i < rows; i++)
        {
            int li = i;
            copyRows[i].deleteButton.Interaction += (o) => game.DeactivateBlock(li);
        }
    }
    private void OnDisable()
    {
        game.NewDrop -= OnNewDrop;
        game.DropMoved -= OnDromMove;
        game.DropRemoved -= OnDropRemove;
        game.BlockActivated -= OnBlockActivation;
        game.BlockDeactivated -= OnBlockDeactivation;
        game.Win -= OnWin;
        game.Lose -= OnLose;

        for (int i = 0; i < rows; i++) copyRows[i].createButton.PurgeInteraction();
        for (int i = 0; i < rows; i++) copyRows[i].deleteButton.PurgeInteraction();
    }

    private void Update() => timer.text = game.timeLeft.ToString();
    

    private void OnNewDrop(MemoryGame g, int index)
    {
        activeDrops[index] = Instantiate(copyRows[index].drop, copyRows[index].frame);
        activeDrops[index].gameObject.SetActive(true);
    }
    private void OnDromMove(MemoryGame g, int index, float value)
    {
        activeDrops[index].transform.position = new Vector3
        (
            activeDrops[index].transform.position.x,
            copyRows[index].bottom.transform.position.y + (copyRows[index].drop.transform.position.y - copyRows[index].bottom.transform.position.y) * value * 0.01f,
            activeDrops[index].transform.position.z
        );
    }
    private void OnDropRemove(MemoryGame g, int index)
    {
        GameObject obj = activeDrops[index].gameObject;
        Destroy(obj);
    }

    private void OnBlockActivation(MemoryGame g, int index, int availableBlocks)
    {
        copyRows[index].bottom.SetColorAll(Color.green);
        blockCounter.text = availableBlocks.ToString();
    }
    private void OnBlockDeactivation(MemoryGame g, int index, int availableBlocks)
    {
        copyRows[index].bottom.SetColorAll(Color.red);
        blockCounter.text = availableBlocks.ToString();
    }

    private void OnWin(MemoryGame g) => winScreen.Show();
    private void OnLose(MemoryGame g) => loseScreen.Show();
}
