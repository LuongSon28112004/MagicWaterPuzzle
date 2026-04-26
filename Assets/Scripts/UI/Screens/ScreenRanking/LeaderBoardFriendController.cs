using UnityEngine;

public class LeaderBoardFriendController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private LeaderBoardManager leaderBoardManager;
    [SerializeField] private GameObject contentFriend;
    [SerializeField] private GameObject contentFriendRequest;
    [Header("Prefab")]
    [SerializeField] private FriendUserInfor friendUserInforPrefab;
    [SerializeField] private FriendUserRequestInfo friendUserRequestInfoPrefab;
}
