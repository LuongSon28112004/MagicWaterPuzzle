using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendUserInfor : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Text txtRanking;
    [SerializeField] private Text txtName;
    [SerializeField] private Text txtLevel;
    [SerializeField] private List<Transform> ListRankingIcons;
    [SerializeField] private Button btnSendGilf;
    [SerializeField] private Button btnRemoveFriend;

    private string userId;

    public void SetData(int rank, string name, int level, string id)
    {
        SetRankingIcon(rank);
        txtName.text = name;
        txtLevel.text = level.ToString();
        this.userId = id;
        string myId = PlayerPrefs.GetString("PlayerID", "-1");
        if (!id.Contains(myId))
        {
            btnSendGilf.onClick.RemoveAllListeners();
            btnSendGilf.onClick.AddListener(SendGilfClick);

            if (btnRemoveFriend != null)
            {
                btnRemoveFriend.gameObject.SetActive(true);
                btnRemoveFriend.onClick.RemoveAllListeners();
                btnRemoveFriend.onClick.AddListener(RemoveFriendClick);
            }
        }
        else
        {
            btnSendGilf.onClick.RemoveAllListeners();
            btnSendGilf.gameObject.SetActive(false);

            if (btnRemoveFriend != null)
            {
                btnRemoveFriend.onClick.RemoveAllListeners();
                btnRemoveFriend.gameObject.SetActive(false);
            }
        }
    }

    private void SendGilfClick()
    {
        var UiSendGilf = UIManager.Instance.ShowPopup<PopupSendGilf>(null);
        UiSendGilf.SetIdUser(userId);

    }

    private void RemoveFriendClick()
    {
        var popupConfirm = UIManager.Instance.ShowPopup<PopupConfirm>(null);
        popupConfirm.ShowConfirm("Bạn có chắc chắn muốn xóa hay không?", () =>
        {
            string myId = PlayerPrefs.GetString("PlayerID", "-1");
            if (myId != "-1")
            {
                UserDataFirebaseManager.Instance.RemoveFriend(myId, userId, (success) =>
                {
                    if (success)
                    {
                        UIManager.Instance.NotifyContent("Đã xóa bạn bè!");
                    }
                    else
                    {
                        UIManager.Instance.NotifyContent("Có lỗi xảy ra, vui lòng thử lại!");
                    }
                });
            }
        });
    }

    private void SetRankingIcon(int rank)
    {
        if (rank > 3)
        {
            txtRanking.text = rank.ToString();
            return;
        }
        txtRanking.text = "";
        for (int i = 0; i < ListRankingIcons.Count; i++)
        {
            ListRankingIcons[i].gameObject.SetActive(i == rank - 1);
        }
    }
}
