using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class WinEvent : MonoBehaviour
{
    [SerializeField] GraphGame game;

    private void Awake()
    {
        game.Clear += () => GetComponent<Text>().text = "NICE";
    }
}
