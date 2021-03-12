using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("PANELS")]
    public GameObject panel_start;
    public GameObject panel_levelCompleted;
    public GameObject panel_stageCompleted;
    public GameObject panel_levelFailed;

    public GameObject panel_Tutorial;
    public GameObject panel_SwipeTutorial;
    public GameObject panel_WrongMoneyTutorial;

    public GameObject panel_market;
    public Animator panel_characterUpgradeMarket;
    public Animator panel_characterMarket;
    public Animator panel_platformUpgradeMarket;



    private bool isCharacterUpgradeOpen;
    private bool isCharacterUnlockOpen;
    private bool isPlatformUpgradeOpen;
    public Button shopButton;
    public Button characterButton;
    public Button moneyButton;
    public Button[] buttons;
    public GameObject[] icons;
    public GameObject scrollSnap;
    public ScrollSnapRect otherScript;
    [SerializeField] private Vector3 vector;

    [SerializeField] private TextMeshProUGUI text_stageCompletedCup;


    [Header("TEXTS")]
    public Text text_level;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        vector = scrollSnap.GetComponent<RectTransform>().anchoredPosition;
        //vector = scrollSnap.transform.localPosition;
        ClosePanels();
        panel_start.SetActive(true);
        //     Time.timeScale = 0f;
        panel_market.SetActive(true);
        panel_characterUpgradeMarket.gameObject.SetActive(true);
        panel_characterMarket.gameObject.SetActive(true);
        panel_platformUpgradeMarket.gameObject.SetActive(true);

        for (int i = 0; i < 10; i++)
        {
            buttons[i].interactable = false;
            icons[i].GetComponent<Image>().color = Color.gray;
        }
    }

    public void StartGame()
    {
        ClosePanels();
        Time.timeScale = 1f;
        // GameManager.instance.canTouch = true;
        StartCoroutine(CanTouchToTrue());
        GameManager.instance.SetMoneyPanel(GameManager.instance.userTotalMoney);

        if (LevelManager.instance.currentLevel == 0)
        {
            ShowSwipeTutorial();
            GameManager.instance.isSwipeTutorialOpen = true;
        }
    }

    public void ShowLevelFailedPanel()
    {
        ClosePanels();
        panel_levelFailed.SetActive(true);
    }

    public void ShowLevelCompletedPanel()
    {
        ClosePanels();
        panel_levelCompleted.SetActive(true);
    }

    public void HomeButton()
    {
        ClosePanels();
        panel_start.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClosePanels()
    {
        panel_start.SetActive(false);
        panel_levelCompleted.SetActive(false);
        panel_stageCompleted.SetActive(false);
        panel_levelFailed.SetActive(false);
        panel_market.SetActive(false);
        panel_characterUpgradeMarket.gameObject.SetActive(false);
        panel_characterMarket.gameObject.SetActive(false);
        panel_platformUpgradeMarket.gameObject.SetActive(false);
        panel_Tutorial.SetActive(false);
        panel_SwipeTutorial.SetActive(false);
        panel_WrongMoneyTutorial.SetActive(false);
    }

    public void LevelAdding()
    {
        //  text_level.text = "LEVEL" + " " + (LevelManager.instance.currentLevel + 1);
    }

    private IEnumerator CanTouchToTrue()
    {
        yield return new WaitForSeconds(.5f);
        GameManager.instance.canTouch = true;
    }

    public void ShowAwardPanel()
    {
        AwardManager.instance.characterShopControl();
        isCharacterUnlockOpen = true;
        shopButton.interactable = true;
        moneyButton.interactable = true;

        if (isCharacterUpgradeOpen || isPlatformUpgradeOpen)
        {
            panel_characterUpgradeMarket.gameObject.SetActive(false);
            panel_platformUpgradeMarket.gameObject.SetActive(false);
            isCharacterUpgradeOpen = false;
            isPlatformUpgradeOpen = false;
            //   panelanim3.Play("PanelAnimation6");
            //   panelanim2.Play("PanelAnimation4");

        }


        panel_characterMarket.gameObject.SetActive(true);
        
        characterButton.interactable = false;

        panel_characterMarket.SetTrigger("Open");
    }

    public void ShowShopPanel()
    {
        characterButton.interactable = true;
        moneyButton.interactable = true;

        isCharacterUpgradeOpen = true;

        if (isCharacterUnlockOpen || isPlatformUpgradeOpen)
        {
            panel_characterMarket.gameObject.SetActive(false);
            panel_platformUpgradeMarket.gameObject.SetActive(false);

            isCharacterUnlockOpen = false;
            isPlatformUpgradeOpen = false;
        }

        panel_characterUpgradeMarket.gameObject.SetActive(true);

        shopButton.interactable = false;

        panel_characterUpgradeMarket.SetTrigger("Open");


        CharacterUpgradeMarketControl();

    }

    public void CharacterUpgradeMarketControl()
    {
        for (int i = 0; i < 10; i++)
        {
            if (GameManager.instance.openCharacterIndex.Contains(i) && PlayerPrefs.GetInt("UserTotalMoney") >= AwardManager.instance.costs_characterUpgrade[i])
            {
                buttons[i].interactable = true;
            }
            else
            {
                buttons[i].interactable = false;
            }

            if (GameManager.instance.openCharacterIndex.Contains(i))
            {
                icons[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                icons[i].GetComponent<Image>().color = Color.gray;
            }
        }
    }

    public void CloseShopPanel()
    {
        panel_characterUpgradeMarket.SetTrigger("Close");
        shopButton.interactable = true;
        if (AwardManager.instance.upgraded && !GameManager.instance.isFinished)
        {
            LevelManager.instance.RestartLevel();
        }

        scrollSnap.GetComponent<RectTransform>().anchoredPosition = vector;
        otherScript.Invoke("DelaySetPage", 0.25f);
    }

    public void CloseAwardPanel()
    {
        panel_characterMarket.SetTrigger("Close");
        characterButton.interactable = true;
        if (AwardManager.instance.upgraded && !GameManager.instance.isFinished)
        {
            LevelManager.instance.RestartLevel();
        }
        scrollSnap.GetComponent<RectTransform>().anchoredPosition = vector;
        otherScript.Invoke("DelaySetPage", 0.25f);

    }

    public void CloseMoneyPanel()
    {
        panel_platformUpgradeMarket.SetTrigger("Close");
        moneyButton.interactable = true;
        if (AwardManager.instance.upgraded && !GameManager.instance.isFinished)
        {
            LevelManager.instance.RestartLevel();
        }
    }

    public void ShowMoneyPanel()
    {
        isPlatformUpgradeOpen = true;
        shopButton.interactable = true;
        characterButton.interactable = true;

        if (isCharacterUpgradeOpen || isCharacterUnlockOpen)
        {
            panel_characterUpgradeMarket.gameObject.SetActive(false);
            panel_characterMarket.gameObject.SetActive(false);

            isCharacterUpgradeOpen = false;
            isCharacterUnlockOpen = false;

        }

        panel_platformUpgradeMarket.gameObject.SetActive(true);

        moneyButton.interactable = false;

        panel_platformUpgradeMarket.SetTrigger("Open");

        if (GameManager.instance.platformCount == 2)
        {
            AwardManager.instance.platformUpgradeButton.interactable = false;
            return;
        }

        if (PlayerPrefs.GetInt("UserTotalMoney") < GameManager.instance.platformUpgradeCost[GameManager.instance.platformCount])
        {
            AwardManager.instance.platformUpgradeButton.interactable = false;
        }
        else
        {
            AwardManager.instance.platformUpgradeButton.interactable = true;
        }

       

    }

    public void ShowStageCompletedPanel(int stage)
    {
        panel_levelCompleted.SetActive(false);
        panel_levelFailed.SetActive(false);
        panel_characterUpgradeMarket.gameObject.SetActive(false);
        panel_characterMarket.gameObject.SetActive(false);
        panel_platformUpgradeMarket.gameObject.SetActive(false);
        panel_Tutorial.SetActive(false);
        panel_SwipeTutorial.SetActive(false);
        panel_WrongMoneyTutorial.SetActive(false);

        text_stageCompletedCup.text = stage.ToString();

        panel_stageCompleted.SetActive(true);
    }

    public void ShowSwipeTutorial()
    {
        ClosePanels();

        panel_SwipeTutorial.SetActive(true);
        panel_Tutorial.SetActive(true);
    }

    public void ShowWrongMoneyTutorial()
    {
        ClosePanels();

        panel_WrongMoneyTutorial.SetActive(true);
        panel_Tutorial.SetActive(true);
    }
}
