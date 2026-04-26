using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    [SerializeField] private Button btnFriend;
    [SerializeField] private Button btnPlayer;

    [Header("Content")]
    [SerializeField] private GameObject contentFriend;
    [SerializeField] private GameObject contentPlayer;


    private void Start()
    {
        btnFriend.onClick.AddListener(OnClickFriend);
        btnPlayer.onClick.AddListener(OnClickPlayer);
    }

    private void OnClickFriend()
    {

    }

    private void OnClickPlayer()
    {

    }
}
