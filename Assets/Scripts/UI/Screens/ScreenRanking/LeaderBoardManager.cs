using System;
using master;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardManager : master.Singleton<LeaderBoardManager>
{
    [SerializeField] private Button btnFriend;
    [SerializeField] private Button btnPlayer;

    [Header("Content")]
    [SerializeField] private GameObject contentFriend;
    [SerializeField] private GameObject contentPlayer;

    public static Action onUpdateFriendList;
    public static Action onUpdatePlayerList;


    private void Start()
    {
        btnFriend.onClick.AddListener(OnClickFriend);
        btnPlayer.onClick.AddListener(OnClickPlayer);
    }

    void OnEnable()
    {
        // Unsubscribe first to avoid duplicate subscriptions if OnDisable was somehow skipped
        onUpdateFriendList -= OnClickFriend;
        onUpdatePlayerList -= OnClickPlayer;
        
        onUpdateFriendList += OnClickFriend;
        onUpdatePlayerList += OnClickPlayer;
        
        OnClickFriend();
    }

    void OnDisable()
    {
        onUpdateFriendList -= OnClickFriend;
        onUpdatePlayerList -= OnClickPlayer;
        
        // Clear content to ensure a fresh state next time
        LeaderBoardPlayerController playerController = contentPlayer.GetComponent<LeaderBoardPlayerController>();
        if (playerController != null) playerController.ClearContent();
        
        // Optionally clear friend content too if needed
    }

    private void OnClickFriend()
    {
        LeaderBoardPlayerController playerController = contentPlayer.GetComponent<LeaderBoardPlayerController>();
        if (playerController != null) playerController.ClearContent();

        LeaderBoardFriendController friendController = contentFriend.GetComponent<LeaderBoardFriendController>();
        if (friendController != null)
        {
            friendController.RefreshAndReset();
        }
        contentFriend.SetActive(true);
        contentPlayer.SetActive(false);
    }

    private void OnClickPlayer()
    {
        LeaderBoardFriendController friendController = contentFriend.GetComponent<LeaderBoardFriendController>();
        if (friendController != null) friendController.ClearContent();

        LeaderBoardPlayerController playerController = contentPlayer.GetComponent<LeaderBoardPlayerController>();
        if (playerController != null)
        {
            playerController.LoadListPlayer();
        }
        contentFriend.SetActive(false);
        contentPlayer.SetActive(true);
    }

    public void ClearContent()
    {
        LeaderBoardPlayerController playerController = contentPlayer.GetComponent<LeaderBoardPlayerController>();
        if (playerController != null) playerController.ClearContent();

        LeaderBoardFriendController friendController = contentFriend.GetComponent<LeaderBoardFriendController>();
        if (friendController != null) friendController.ClearContent();
    }
}
