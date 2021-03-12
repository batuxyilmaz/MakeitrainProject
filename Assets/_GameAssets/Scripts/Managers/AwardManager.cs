using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AwardManager : MonoBehaviour
{
    public static AwardManager instance;

    public int currentCharacterIndex;
    [SerializeField] private GameObject container;
    [SerializeField] private int[] characterIndex;
    public Button inviteButton;
    public GameObject finishTick;
    public bool clickedButton;
    public Animator danceCharacter;
    public GameObject target;
    public TextMeshProUGUI characterCostText;
    public int characterrIndex;
    public TextMeshProUGUI characterLevelText;
    public GameObject lockImage;
    public TextMeshProUGUI upgradeMoneyText;
    public bool upgraded;


    [SerializeField] private GameObject[] displayCharacters;
    [SerializeField] private GameObject[] buttons;

    [SerializeField] private TextMeshProUGUI[] text_characterUpgradeCosts;
    [SerializeField] private GameObject[] moneyImage;
    [SerializeField] private GameObject[] upgradeFinish;

    [SerializeField] private GameObject platformUpgradeFinishTick;
    public Button platformUpgradeButton;
    public TextMeshProUGUI platformText;
    [SerializeField] private GameObject platformUpgradeMoneyImage;

    public int[] costs_characterUpgrade;
    public int[] speedValuesForMarket;
    public int[] extraDanceValuesForMarket;
    [SerializeField] private TextMeshProUGUI[] text_speeds;
    [SerializeField] private TextMeshProUGUI[] text_extraDances;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (GameManager.instance.platformCount == 2)
        {
            platformUpgradeButton.interactable = false;
            platformUpgradeButton.gameObject.SetActive(false);
            platformText.gameObject.SetActive(false);
            platformUpgradeMoneyImage.SetActive(false);

            platformUpgradeFinishTick.SetActive(true);
        }

        costs_characterUpgrade = new int[10];

        for (int i = 0; i < 10; i++)
        {
            if (GameManager.instance.characterLevels[i] < 9)
            {
                costs_characterUpgrade[i] = (GameManager.instance.characterLevels[i] + 1) * 100;
                text_characterUpgradeCosts[i].text = costs_characterUpgrade[i].ToString();

                text_speeds[i].text = "%" + speedValuesForMarket[GameManager.instance.characterLevels[i]].ToString();
                text_extraDances[i].text = "+" + extraDanceValuesForMarket[GameManager.instance.characterLevels[i]].ToString();
            }
            else
            {
                UIManager.instance.buttons[i].interactable = false;

                UIManager.instance.buttons[characterrIndex].gameObject.SetActive(false);
                moneyImage[characterrIndex].SetActive(false);

                upgradeFinish[characterrIndex].SetActive(true);
            }
        }
    }

    public void characterSelection()
    {
        GameObject characters = Instantiate(displayCharacters[0], target.transform.position, Quaternion.identity);
        characters.transform.localScale = characters.transform.localScale * 0.1f;
        characters.transform.eulerAngles = new Vector3(0, 180f, 0);
    }

    public void DanceCharacter()
    {
        //    danceCharacter.SetTrigger("Dance");
        container.transform.GetChild(currentCharacterIndex).GetChild(0).GetComponent<Animator>().SetTrigger("Dance");
        clickedButton = true;
        container.transform.GetChild(currentCharacterIndex).GetChild(0).GetComponent<RotateCharacter>().Rotatechar();
        GameManager.instance.openCharacterIndex.Add(currentCharacterIndex+1);

        if (clickedButton)
        {
            GameManager.instance.openCharacterIndex.Add(currentCharacterIndex+1);
            PlayerPref.SetInts("OpenCharacterIndex", GameManager.instance.openCharacterIndex);

            GameManager.instance.userTotalMoney -= GameManager.instance.characterInviteCosts[currentCharacterIndex+1];
            GameManager.instance.SetMoneyPanel(GameManager.instance.userTotalMoney);
            PlayerPrefs.SetInt("UserTotalMoney", GameManager.instance.userTotalMoney);

            inviteButton.gameObject.SetActive(false);
            finishTick.SetActive(true);
        }
    }

    public void UpgradeCharacter()
    {
        upgraded = true;

        characterrIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name);

        GameManager.instance.characterLevels[characterrIndex]++;
        PlayerPref.SetInts("CharacterLevels", GameManager.instance.characterLevels);
        GameManager.instance.prefabs_character[characterrIndex].GetComponent<Character>().characterLevel = GameManager.instance.characterLevels[characterrIndex];

        GameManager.instance.userTotalMoney -= costs_characterUpgrade[characterrIndex];
        GameManager.instance.SetMoneyPanel(GameManager.instance.userTotalMoney);
        PlayerPrefs.SetInt("UserTotalMoney", GameManager.instance.userTotalMoney);

        costs_characterUpgrade[characterrIndex] = (GameManager.instance.characterLevels[characterrIndex] + 1) * 100;
        text_characterUpgradeCosts[characterrIndex].text = costs_characterUpgrade[characterrIndex].ToString();

        text_speeds[characterrIndex].text = "%" + speedValuesForMarket[GameManager.instance.characterLevels[characterrIndex]].ToString();
        text_extraDances[characterrIndex].text = "+" + extraDanceValuesForMarket[GameManager.instance.characterLevels[characterrIndex]].ToString();

        


        UIManager.instance.CharacterUpgradeMarketControl();


        if (GameManager.instance.characterLevels[characterrIndex] == 9)
        {
            UIManager.instance.buttons[characterrIndex].interactable = false;
            UIManager.instance.buttons[characterrIndex].gameObject.SetActive(false);
            moneyImage[characterrIndex].SetActive(false);

            upgradeFinish[characterrIndex].SetActive(true);
        }
    }

    public void UpgradePlatform()
    {
        upgraded = true;

        GameManager.instance.platformCount++;
        PlayerPrefs.SetInt("PlatformCount", GameManager.instance.platformCount);

        if (GameManager.instance.platformCount == 1)
        {
            platformText.text = "x3";
            upgradeMoneyText.text = GameManager.instance.platformUpgradeCost[GameManager.instance.platformCount].ToString();

            GameManager.instance.userTotalMoney -= GameManager.instance.platformUpgradeCost[GameManager.instance.platformCount - 1];
            GameManager.instance.SetMoneyPanel(GameManager.instance.userTotalMoney);
            PlayerPrefs.SetInt("UserTotalMoney", GameManager.instance.userTotalMoney);

            if (PlayerPrefs.GetInt("UserTotalMoney") >= GameManager.instance.platformUpgradeCost[GameManager.instance.platformCount])
            {
                platformUpgradeButton.interactable = true;
            }
            else
            {
                platformUpgradeButton.interactable = false;
            }
        }

        if (GameManager.instance.platformCount == 2)
        {
            platformUpgradeButton.interactable = false;
            platformUpgradeButton.gameObject.SetActive(false);
            platformText.gameObject.SetActive(false);
            platformUpgradeMoneyImage.SetActive(false);

            platformUpgradeFinishTick.SetActive(true);

        //    upgradeMoneyText.text = "";

            GameManager.instance.userTotalMoney -= GameManager.instance.platformUpgradeCost[GameManager.instance.platformCount - 1];
            GameManager.instance.SetMoneyPanel(GameManager.instance.userTotalMoney);
            PlayerPrefs.SetInt("UserTotalMoney", GameManager.instance.userTotalMoney);
        }
    }

    public void characterShopControl()
    {
        characterCostText.text = GameManager.instance.characterInviteCosts[currentCharacterIndex + 1].ToString();
        characterLevelText.text = "LEVEL" + " " + GameManager.instance.characterUnlockStages[currentCharacterIndex + 1].ToString();

        if (GameManager.instance.openCharacterIndex.Contains(currentCharacterIndex + 1))
        {
            inviteButton.gameObject.SetActive(false);
            characterLevelText.gameObject.SetActive(false);
            lockImage.SetActive(false);

            finishTick.SetActive(true);
        }
        else
        {
            inviteButton.gameObject.SetActive(true);
            characterLevelText.gameObject.SetActive(true);

            finishTick.SetActive(false);
        }

        if (PlayerPrefs.GetInt("UserTotalMoney") >= GameManager.instance.characterInviteCosts[currentCharacterIndex + 1] && PlayerPrefs.GetInt("Stage") + 1 >= GameManager.instance.characterUnlockStages[currentCharacterIndex + 1])
        {
            inviteButton.interactable = true;
        }
        else
        {
            inviteButton.interactable = false;
        }

        if (PlayerPrefs.GetInt("Stage") + 1 >= GameManager.instance.characterUnlockStages[currentCharacterIndex + 1])
        {
            lockImage.SetActive(false);
        }
        else
        {
            lockImage.SetActive(true);
        }

    }
}
