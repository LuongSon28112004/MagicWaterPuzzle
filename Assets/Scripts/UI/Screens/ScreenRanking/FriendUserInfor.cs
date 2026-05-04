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

    public void SetData(int rank, string name, int level)
    {
        SetRankingIcon(rank);
        txtName.text = name;
        txtLevel.text = level.ToString();
        btnSendGilf.onClick.RemoveAllListeners();
        btnSendGilf.onClick.AddListener(SendGilfClick);
    }

    private void SendGilfClick()
    {
        UIManager.Instance.ShowPopup<PopupSendGilf>(null);
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
