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
    private List<GameObject> overworldCharacters = new List<GameObject>();

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
        SpawnOverworldMembers();
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
        SpawnOverworldMembers();
    }

    private void SpawnOverworldMembers()
    {
        for(int i = 0; i < overworldCharacters.Count; i++)
        {
            Destroy(overworldCharacters[i]);
        }
        overworldCharacters.Clear();

        List<PartyMember> currentParty = GameObject.FindFirstObjectByType<PartyManager>().GetCurrentParty();

        for(int i = 0; i < currentParty.Count; ++i)
        {
            if(i == 0)
            {
                GameObject player = gameObject;

                GameObject playerVisual = Instantiate(currentParty[i].memberOverworldVisualPrefab, player.transform.position, Quaternion.identity);

                playerVisual.transform.SetParent(player.transform);

                player.GetComponent<PlayerController>().SetOverworldVisuals(playerVisual.GetComponent<Animator>(), playerVisual.GetComponent<SpriteRenderer>());
                playerVisual.GetComponent<SpriteRenderer>();
                playerVisual.GetComponent<MemberFollowAI>().enabled = false;

                overworldCharacters.Add(playerVisual);
            }
            else
            {
                Vector3 positionToSpawn = transform.position;
                positionToSpawn.x -= 1;
                GameObject tempFollower = Instantiate(currentParty[i].memberOverworldVisualPrefab, positionToSpawn, Quaternion.identity);

                tempFollower.GetComponent<MemberFollowAI>().SetFollowDistance(i);
                overworldCharacters.Add(tempFollower);
            }
        }
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
