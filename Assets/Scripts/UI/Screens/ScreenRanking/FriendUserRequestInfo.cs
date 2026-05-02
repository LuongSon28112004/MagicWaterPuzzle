using System;
using UnityEngine;
using UnityEngine.UI;

public class FriendUserRequestInfo : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Text txtName;

    private string userId;
    [SerializeField] private Button buttonAccept;
    [SerializeField] private Button buttonDecline;
    public void SetData(string id, string name)
    {
        txtName.text = name;
        userId = id;
        buttonAccept.onClick.RemoveAllListeners();
        buttonDecline.onClick.RemoveAllListeners();
        buttonAccept.onClick.AddListener(OnClickAccept);
        buttonDecline.onClick.AddListener(OnClickDecline);
    }

    private void OnClickAccept()
    {
        string myId = PlayerPrefs.GetString("PlayerID", null);
        // Handle accept friend request logic here
        UserDataFirebaseManager.Instance.AcceptFriendRequest(myId, userId, success =>
        {
            if (success)
            {
                UIManager.Instance.NotifyContent("Yêu cầu kết bạn đã được chấp nhận.");
            }
            else
            {
                UIManager.Instance.NotifyContent("Đã xảy ra lỗi khi chấp nhận yêu cầu kết bạn.");
            }
        });
    }


    private void OnClickDecline()
    {
        string myId = PlayerPrefs.GetString("PlayerID", null);
        // Handle decline friend request logic here
        UserDataFirebaseManager.Instance.DeclineFriendRequest(myId, userId, success =>
        {
            if (success)
            {
                UIManager.Instance.NotifyContent("Yêu cầu kết bạn đã được từ chối.");
            }
            else
            {
                UIManager.Instance.NotifyContent("Đã xảy ra lỗi khi từ chối yêu cầu kết bạn.");
            }
        });
    }
}
