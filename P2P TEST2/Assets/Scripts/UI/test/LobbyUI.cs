using Epic.OnlineServices;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class LobbyUI : MonoBehaviour
{
    public GameObject LobbiesUIParent;

    [Header("Lobbies UI - Search")]
    public GameObject UILobbyEntryPrefab;
    public GameObject SearchContentParent;

    public UIConsoleInputField SearchByBucketIdBox;
    public UIConsoleInputField SearchByLobbyIdBox;

    [Header("Controller")]
    public GameObject UIFirstSelected;

    // UI Cache
    private int lastMemberCount = 0;
    private ProductUserId currentLobbyOwnerCache;
    private bool lastCurrentLobbyIsValid = false;

    private List<UIMemberEntry> UIMemberEntries = new List<UIMemberEntry>();

    private EOSLobbyManager LobbyManager;
    private EOSFriendsManager FriendsManager;

    private bool UIDirty = false;

    public void Awake()
    {
        HideMenu();
    }

    private void Start()
    {
        LobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
        FriendsManager = EOSManager.Instance.GetOrCreateManager<EOSFriendsManager>();

        LobbyManager.AddNotifyMemberUpdateReceived(OnMemberUpdate);
        // Clear any search results that's in by default
        ClearSearchResults();
    }

    private void OnDestroy()
    {
        LobbyManager?.RemoveNotifyMemberUpdate(OnMemberUpdate);

        EOSManager.Instance.RemoveManager<EOSLobbyManager>();
        EOSManager.Instance.RemoveManager<EOSFriendsManager>();
        EOSManager.Instance.RemoveManager<EOSEACLobbyManager>();
        EOSManager.Instance.RemoveManager<EOSAntiCheatClientManager>();
    }

    private void OnMemberUpdate(string LobbyId, ProductUserId MemberId)
    {
        Lobby currentLobby = LobbyManager.GetCurrentLobby();
        if (currentLobby.Id != LobbyId)
        {
            return;
        }

        UIMemberEntry uiEntry = UIMemberEntries.Find((UIMemberEntry entry) => { return entry.ProductUserId == MemberId; });
        if (uiEntry != null)
        {
            LobbyMember updatedMember = currentLobby.Members.Find((LobbyMember member) => { return member.ProductId == MemberId; });
            if (updatedMember != null)
            {
                uiEntry.UpdateMemberData(updatedMember);
                uiEntry.UpdateUI();
            }
        }
    }



    public void HideMenu()
    {
        LobbyManager?.OnLoggedOut();

        LobbiesUIParent.gameObject.SetActive(false);
    }

    private void ClearSearchResults()
    {
        // Destroy current UI member list
        foreach (Transform child in SearchContentParent.transform)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
