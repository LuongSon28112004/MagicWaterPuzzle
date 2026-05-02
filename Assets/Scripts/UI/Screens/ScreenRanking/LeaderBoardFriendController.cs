using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardFriendController : MonoBehaviour
{
    [Header("Reference")]
    private LeaderBoardManager leaderBoardManager;
    [SerializeField] private GameObject contentFriend;
    [SerializeField] private GameObject contentFriendRequest;
    [Header("Prefab")]
    [SerializeField] private FriendUserInfor friendUserInforPrefab;
    [SerializeField] private FriendUserRequestInfo friendUserRequestInfoPrefab;
    [Header("Button")]
    [SerializeField] private Button buttonFriend;
    [SerializeField] private Button buttonAddFriend;
    [Header("Layout Panel")]
    [SerializeField] private GameObject panelAddFriend;
    [SerializeField] private GameObject panelFriendList;
    [SerializeField] private GameObject LoadingPanel;
    [Header("Search Friends")]
    [SerializeField] private InputField inputSearchFriend;
    [SerializeField] private GameObject contentSearchFriend;
    [SerializeField] private FriendUserSuggestInfo friendUserSuggestInfoPrefab;
    [SerializeField] private Button buttonSearchFriend;
    [SerializeField] private Text txtMyId;
    private void Start()
    {
        buttonFriend.onClick.AddListener(OnClickFriend);
        buttonAddFriend.onClick.AddListener(OnClickAddFriend);
        buttonSearchFriend.onClick.AddListener(OnClickSearchFriend);
    }

    private void OnClickSearchFriend()
    {
        // if (string.IsNullOrEmpty(inputSearchFriend.text))
        // {
        //     UIManager.Instance.NotifyContent("Vui lòng nhập từ khóa tìm kiếm.");
        //     return;
        // }

        UserDataFirebaseManager.Instance.SearchUsersByIdPrefix(inputSearchFriend.text, users =>
        {
            // Clear old search results
            foreach (Transform child in contentSearchFriend.transform)
            {
                Destroy(child.gameObject);
            }

            if (users == null || users.Count == 0)
            {
                UIManager.Instance.NotifyContent("Không tìm thấy người dùng nào.");
                return;
            }

            // Hiển thị kết quả tìm kiếm
            foreach (var user in users)
            {
                string userId = user.ContainsKey("Id") ? user["Id"].ToString() : "Unknown";
                string userName = user.ContainsKey("Name") ? user["Name"].ToString() : "Unknown";

                GameObject item = Instantiate(friendUserSuggestInfoPrefab.gameObject, contentSearchFriend.transform);
                FriendUserSuggestInfo ui = item.GetComponent<FriendUserSuggestInfo>();
                ui.SetData(userId, userName);
                item.SetActive(true);

            }
        });
    }

    private void OnClickAddFriend()
    {
        panelAddFriend.SetActive(true);
        panelFriendList.SetActive(false);
        string currentUserId = PlayerPrefs.GetString("PlayerID", null);

        if (!string.IsNullOrEmpty(currentUserId))
        {
            txtMyId.text = $"{currentUserId}";

        }
    }

    private void OnClickFriend()
    {
        LoadListFriend();
        // destroy old list
        foreach (Transform child in contentFriend.transform)
        {
            Destroy(child.gameObject);
        }
        panelAddFriend.SetActive(false);
        panelFriendList.SetActive(true);
    }

    public void LoadListFriend()
    {
        string currentUserId = PlayerPrefs.GetString("PlayerID", null);
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("Current user ID not found in PlayerPrefs.");
            return;
        }

        LoadingPanel.SetActive(true);

        UserDataFirebaseManager.Instance.GetFriendsList(currentUserId, friends =>
        {
            if (friends == null)
            {
                Debug.LogError("Failed to load friends list.");
                return;
            }

            // Clear old list
            foreach (Transform child in contentFriend.transform)
            {
                Destroy(child.gameObject);
            }

            //add tôi vào list bạn bè
            UserDataFirebaseManager.Instance.GetUserData(currentUserId, currentUser =>
            {
                if (currentUser != null)
                {
                    friends.Add(currentUser);
                }

                LoadingPanel.SetActive(false);
                //sort theo level giảm dần
                friends.Sort((a, b) =>
                {
                    int levelA = a.ContainsKey("Level") ? Convert.ToInt32(a["Level"]) : 1;
                    int levelB = b.ContainsKey("Level") ? Convert.ToInt32(b["Level"]) : 1;
                    return levelB.CompareTo(levelA);
                });

                // Create new UI for each friend
                int rank = 1;
                foreach (var friend in friends)
                {
                    string friendName = friend.ContainsKey("Name") ? friend["Name"].ToString() : "Unknown";
                    int friendLevel = friend.ContainsKey("Level") ? Convert.ToInt32(friend["Level"]) : 1;

                    GameObject item = Instantiate(friendUserInforPrefab.gameObject, contentFriend.transform);
                    FriendUserInfor ui = item.GetComponent<FriendUserInfor>();
                    ui.SetData(rank, friendName, friendLevel);
                    rank++;
                    item.SetActive(true);
                }
            });

        });
    }

    public void LoadListFriendRequest()
    {
        string currentUserId = PlayerPrefs.GetString("PlayerID", null);
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("Current user ID not found in PlayerPrefs.");
            return;
        }

        UserDataFirebaseManager.Instance.GetMyFriendRequests(currentUserId, friendRequests =>
        {
            if (friendRequests == null)
            {
                Debug.LogError("Failed to load friend requests.");
                return;
            }

            // Clear old list
            foreach (Transform child in contentFriendRequest.transform)
            {
                Destroy(child.gameObject);
            }

            contentFriendRequest.SetActive(friendRequests.Count > 0);

            // Create new UI for each friend request
            foreach (var request in friendRequests)
            {
                string fromUserId = request.ContainsKey("FromUserId") ? request["FromUserId"].ToString() : "Unknown";
                string fromUserName = request.ContainsKey("FromUserName") ? request["FromUserName"].ToString() : "Unknown";

                GameObject item = Instantiate(friendUserRequestInfoPrefab.gameObject, contentFriendRequest.transform);
                FriendUserRequestInfo ui = item.GetComponent<FriendUserRequestInfo>();
                ui.SetData(fromUserId, fromUserName);
                item.SetActive(true);
            }
        });
    }

    public void ClearContent()
    {
        foreach (Transform child in contentFriend.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
