using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject joinPopup;
    [SerializeField] private TextMeshProUGUI joinPopupText;

    private bool inFrontOfPartyMember;
    private GameObject joinableMember;
    private PlayerControl playerControl;

    private const string PARTY_JOINED_MESSAGE = " Joined The Party!";
    private const string NPC_JOINABLE_TAG = "NPCJoinable";

    private void Awake()
    {
        playerControl = new PlayerControl();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerControl.Player.Interact.performed += _ => Interact();
    }

    private void OnEnable()
    {
        playerControl.Enable();
    }

    private void OnDisable()
    {
        playerControl.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Interact()
    {
        if(inFrontOfPartyMember && joinableMember) {
            MemberJoined(joinableMember.GetComponent<JoinableCharacterScript>().memberToJoin);
            inFrontOfPartyMember = false;
            joinableMember = null;
        }
    }

    private void MemberJoined(PartyMemberInfo partyMember)
    {
        GameObject.FindFirstObjectByType<PartyManager>().AddMemberToPartyByName(partyMember.memberName);
        joinableMember.GetComponent<JoinableCharacterScript>().CheckIfJoined();
        joinPopup.SetActive(true);
        joinPopupText.text = partyMember.memberName + PARTY_JOINED_MESSAGE;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            inFrontOfPartyMember = true;
            joinableMember = other.gameObject;
            joinableMember.GetComponent<JoinableCharacterScript>().ShowInteractPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            inFrontOfPartyMember = false;
            joinableMember.GetComponent<JoinableCharacterScript>().ShowInteractPrompt(false);
            joinableMember = null;
        }
    }
}
