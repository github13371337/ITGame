using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGame : MonoBehaviour
{
    [SerializeField][EditorReadOnly] bool started;
    [SerializeField][EditorReadOnly] float TimeLeft;
    [SerializeField] int Rows = 3;
    [SerializeField] int AvailableBlocks = 2;
    [SerializeField] float winTimeout = 30f;
    [SerializeField] Vector2 rowWait = new Vector2(0f, 5f);
    [SerializeField] Vector2 speedRange = new Vector2(5f, 20f);
    [SerializeField] bool startOnStartup;
    [SerializeField][EditorReadOnly] float[] rowValues;
    [SerializeField] bool[] blocks;

    public int availableBlocks => AvailableBlocks;
    public int rows => Rows;
    public float timeLeft => TimeLeft;

    public const float rowStartValue = 100f;

    private void Start() { if (startOnStartup) StartGame(); } 

    private void Update()
    {
        if (started)
        {
            if (TimeLeft >= 0) TimeLeft -= Time.deltaTime;
            else
            {
                started = false;
                Stop();
                Win?.Invoke(this);
            }
        }
    }

    [ContextMenu("Start")]
    public void StartGame()
    {
        rowValues = new float[Rows];
        blocks = new bool[Rows];
        for (int i = 0; i < Rows; i++) StartCoroutine(Run(i));       
        TimeLeft = winTimeout;
        started = true;
        Started?.Invoke(this);
    }
    public void ActivateBlock(int index)
    {
        if (!blocks[index])
        {
            if (AvailableBlocks > 0)
            {
                blocks[index] = true;
                AvailableBlocks--;
                BlockActivated?.Invoke(this, index, AvailableBlocks);
            }
        }
    }
    public void DeactivateBlock(int index)
    {
        if (blocks[index])
        {            
            blocks[index] = false;
            AvailableBlocks++;
            BlockDeactivated?.Invoke(this, index, AvailableBlocks);           
        }
    }
    public void Stop()
    {
        StopAllCoroutines();
        started = false;
        TimeLeft = 0f;
        Stopped?.Invoke(this);
    }

    private IEnumerator Run(int index)
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(rowWait.x, rowWait.y));

            rowValues[index] = rowStartValue;
            float moveSpeed = UnityEngine.Random.Range(speedRange.x, speedRange.y);

            NewDrop?.Invoke(this, index);

            while (true)
            {
                rowValues[index] -= moveSpeed * Time.deltaTime;
                DropMoved?.Invoke(this, index, rowValues[index]);

                if (rowValues[index] < 0f)
                {
                    DropRemoved(this, index);

                    if (blocks[index] == false)
                    {
                        Lose?.Invoke(this);
                        Stop();
                    }
                    else break;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }

    public event Action<MemoryGame, int, int> BlockActivated;
    public event Action<MemoryGame, int, int> BlockDeactivated;
    public event Action<MemoryGame, int> NewDrop;
    public event Action<MemoryGame, int, float> DropMoved;
    public event Action<MemoryGame, int> DropRemoved;
    public event Action<MemoryGame> Started;
    public event Action<MemoryGame> Win;
    public event Action<MemoryGame> Lose;
    public event Action<MemoryGame> Stopped;
}
