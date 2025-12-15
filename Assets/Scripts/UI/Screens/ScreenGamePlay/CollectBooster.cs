using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TypeCollectBooster
{
    NONE,
    FREEZE,
    BOMB,
    HAMMER
}

public class CollectBooster : MonoBehaviour
{
    [Header("Collect Booster Config")]
    [SerializeField] TypeCollectBooster typeCollectBooster;
    [SerializeField] Button ClaimButton;
    [SerializeField] GameObject Pannel;
    [Header("Image")]
    [SerializeField] Image Freeze;
    [SerializeField] Image Bomb;
    [SerializeField] Image Hammer;
    [Header("Image SRC")]
    [SerializeField] Sprite FreezeImage;
    [SerializeField] Sprite BombImage;
    [SerializeField] Sprite HammerImage;
    [Header("Spanwer")]
    [SerializeField] Transform ParentSpawner;
    [SerializeField] Transform TargetIcon;
    [SerializeField] Transform TargetFreeze;
    [SerializeField] Transform TargetBomb;
    [SerializeField] Transform TargetHammer;
    [Header("Title text")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] public Action<bool> DoneAction;

    public TypeCollectBooster TypeCollectBooster { get => typeCollectBooster; set => typeCollectBooster = value; }

    private void AddEventListener()
    {
        ClaimButton.onClick.AddListener(ClaimClick);
    }

    private void ClaimClick()
    {
        StartCoroutine(ShowAnimClaim());
    }

    private IEnumerator ShowAnimClaim()
    {

        Sprite Choice = ChoiceSprite();
        Transform TargetChoice = ChoiceTarget();
        if (TargetChoice == null) yield break;
        GameObject booster = new GameObject("Booster");
        booster.transform.SetParent(ParentSpawner, false);
        booster.transform.position = TargetIcon.position;
        Image img = booster.AddComponent<Image>();
        img.sprite = Choice;
        img.SetNativeSize();

        Sequence seq = DOTween.Sequence();
        seq.Append(booster.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.6f));
        seq.Append(booster.transform.DORotate(new Vector3(0, 0, 45f), 0.15f, RotateMode.FastBeyond360));
        seq.Append(booster.transform.DORotate(new Vector3(0, 0, -45f), 0.15f, RotateMode.FastBeyond360));
        seq.Append(booster.transform.DORotate(Vector2.zero, 0.15f, RotateMode.FastBeyond360));


        Pannel.transform.DOScale(Vector3.zero, 0.4f);
        yield return seq.WaitForCompletion();
        seq.Kill();
        booster.transform.DOScale(Vector3.one, 0.4f);
        booster.transform.DORotate(new Vector3(0, 0, 0f), 0.4f, RotateMode.FastBeyond360);
        booster.transform.DOMove(TargetChoice.position, 0.4f).OnComplete(() =>
        {
            Destroy(booster);
            DoneAction.Invoke(true);
            AudioManager.Instance.PlayOneShot("CollectBooster", 1f);
            transform.gameObject.SetActive(false);
        });
    }

    private Transform ChoiceTarget()
    {
        switch (typeCollectBooster)
        {

            case TypeCollectBooster.BOMB:
                return TargetBomb;
            case TypeCollectBooster.FREEZE:
                return TargetFreeze;
            case TypeCollectBooster.HAMMER:
                return TargetHammer;
            default:
                return null;

        }
    }

    private Sprite ChoiceSprite()
    {
        switch (typeCollectBooster)
        {

            case TypeCollectBooster.BOMB:
                return BombImage;
            case TypeCollectBooster.FREEZE:
                return FreezeImage;
            case TypeCollectBooster.HAMMER:
                return HammerImage;
            default:
                return BombImage;

        }
    }

    public void setTypeCollect(TypeCollectBooster typeCollectBooster)
    {
        this.typeCollectBooster = typeCollectBooster;
        ChangeIcon();
        ChangeTitle();
        AddEventListener();
    }

    private void ChangeTitle()
    {
        switch (typeCollectBooster)
        {

            case TypeCollectBooster.BOMB:
                titleText.text = Contacts.Instance.GetTutBooster(TypeCollectBooster.BOMB);
                break;
            case TypeCollectBooster.FREEZE:
                titleText.text = Contacts.Instance.GetTutBooster(TypeCollectBooster.FREEZE);
                break;
            case TypeCollectBooster.HAMMER:
                titleText.text = Contacts.Instance.GetTutBooster(TypeCollectBooster.HAMMER);
                break;

        }
    }

    private void ChangeIcon()
    {
        switch (typeCollectBooster)
        {

            case TypeCollectBooster.BOMB:
                Bomb.gameObject.SetActive(true);
                break;
            case TypeCollectBooster.FREEZE:
                Freeze.gameObject.SetActive(true);
                break;
            case TypeCollectBooster.HAMMER:
                Hammer.gameObject.SetActive(true);
                break;

        }
    }
}
