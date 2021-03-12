using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region VARIABLES

    [Header("GENERAL")]
    public static GameManager instance;
    public Camera mainCamera;
    public bool canTouch;
    public bool isFinished;
    public Level[] levels;
    [SerializeField] private Color[] backgroundColors;
    [SerializeField] private MeshRenderer mesh_Wall;
    public TextMeshProUGUI totalMoneyText;
    public TextMeshProUGUI currentStageText;
    public TextMeshProUGUI nextStageText;
    public int[] characterInviteCosts;
    public int[] characterUnlockStages;
    public int[] platformUpgradeCost;

    [Header("TUTORIAL")]
    public int theLevel;
    public bool isSwipeTutorialOpen;
    public bool isWrongMoneyTutorialOpen;
    private bool isWrongMoneyTutorialFinished;
    private bool canMoveToForward;
    private bool canMoveToBack;

    [Header("MONEY")]
    public GameObject money;
    [SerializeField] private Transform moneySpawnPoint;
    private Vector3 moneySpawnPointPosition;
    [SerializeField] private Vector3 scale;
    public GameObject prefab_trueMoney;
    [SerializeField] private GameObject prefab_wrongMoney;
    [SerializeField] private int[] leftAnimationIndex;
    [SerializeField] private int[] forwardAnimationIndex;
    [SerializeField] private int[] rightAnimationIndex;
    private List<int> list_leftAnimationIndex = new List<int>();
    private List<int> list_forwardAnimationIndex = new List<int>();
    private List<int> list_rightAnimationIndex = new List<int>();
    [SerializeField] private GameObject combopartical;
    [SerializeField] private AudioSource moneySound;

    [Header("ANIMATION CONTROLS")]
    private int currentDance;
    private float timer;
    private int randomDance;
    private bool isStopped;
    private float animationSpeed;

    [Header("GENERATE LEVEL")]
    [SerializeField] private GameObject prefab_level;
    private GameObject level;

    //DEFAULT VALUES

    [Header("LEVEL COMPLETED")]
    public bool levelCompleted;
    [SerializeField] private GameObject completedConfetti;

    [Header("LEVEL FAILED")]
    public bool levelFailed;
    [SerializeField] private GameObject backgorundHandsUp;
    private Animator[] animators_handsup;

    [Header("THROW MONEY")]
    [SerializeField] private Animator animator_hand;
    [SerializeField] private Animator animator_money;
    [SerializeField] private Animator animator_dance;
    private Vector3 firstMousePosition;
    private Vector3 lastMousePosition;
    private float swipeSpeed;
    private bool firstTouch;

    //   private GameObject nextMoney;
    private List<Money> moneys = new List<Money>();
    private bool trueMoney;

    private int wrongMoveCount;
    private int wrongMoveLimit;

    [Header("SET BAR POSITION")]
    [SerializeField] private GameObject scrollBar;
    private bool canChangeBarPosition;


    [Header("FILL BAR")]
    private float barSize;
    private int moneyCount;
    [SerializeField] private Image barProgress;
    [SerializeField] private TextMeshProUGUI characterBarText;
    private int characterBarProgressMoneyCount;
    [SerializeField] private TextMeshProUGUI text_overMoney;


    private int currentCharacterEarnedMoney;
    private int difference;
    private float moneyEffectTimer;
    private bool isMoneyEffectActive;
    private List<GameObject> moneyEffects = new List<GameObject>();

    [Header("SPAWN PLATFORM")]
    [SerializeField] private GameObject[] prefabs_platform;
    public int platformCount;
    private GameObject thePlatform;
    private Platform[] thePlatforms;

    [Header("SPAWN CHARACTERS")]
    public GameObject[] prefabs_character;
    private List<GameObject> list_prefabs_character = new List<GameObject>();
    public List<int> openCharacterIndex = new List<int>();
    private List<Character> characters = new List<Character>();
    public List<int> characterLevels = new List<int>();
    private Character currentCharacter;
    private Character previousCharacter;

    [Header("MONEY VALUES")]
    public int[] danceMoney;
    private int dancingMoneyValue;
    private int currentCharacterTotalMoney;
    public int userTotalMoney;
    private int backMoney;

    [Header("STAGE")]
    private int currentStage;
    [SerializeField] private GameObject stageBar;
    private float stageBarFillAmount;
    [SerializeField] private float stageBarSize;
    [SerializeField] private Image stageBarProgress;
    [SerializeField] private int[] stageValues;
    private float nextStageFillAmount;
    private bool stageCompleted;
    private int currentStageValue;

    [Header("CHANGE PLATFORM POSITION")]
    private Platform currentPlatform;
    private Platform previousPlatform;
    [SerializeField] private Transform platformRotationObject;

    [SerializeField] private GameObject[] moneyAnimationsImages;
    private int counterMoneyEffectMoney;



    //SPAWN MONEY
    private int trueMoneyCount;
    [SerializeField] private int[] moneyValues;

    //TEMP, WILL BE DELETED
    private Vector3[] animationPositions;

    private Vector3 offset;
    //  public int danceAnimation;

    [SerializeField] private GameObject[] prefabs_combo;
    private int moneyCounterForCombo;
    [SerializeField] private Transform[] comboSpawnPoints;


    #endregion

    private void Awake()
    {
        instance = this;

        theLevel = PlayerPrefs.GetInt("TheLevel");

        openCharacterIndex = PlayerPref.GetInts("OpenCharacterIndex");
        if (openCharacterIndex.Count == 0)
        {
            openCharacterIndex.Add(0);
            PlayerPref.SetInts("OpenCharacterIndex", openCharacterIndex);
        }

        characterLevels = PlayerPref.GetInts("CharacterLevels");

        if (characterLevels.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                characterLevels.Add(0);
            }
            PlayerPref.SetInts("CharacterLevels", characterLevels);
        }

        for (int i = 0; i < prefabs_character.Length; i++)
        {
            prefabs_character[i].GetComponent<Character>().characterLevel = characterLevels[i];
        }

        userTotalMoney = PlayerPrefs.GetInt("UserTotalMoney");
        currentStage = PlayerPrefs.GetInt("Stage");
        currentStageText.text = (currentStage + 1).ToString();
        nextStageText.text = (currentStage + 2).ToString();

        stageBarFillAmount = PlayerPrefs.GetFloat("StageBarFillAmount");
        stageBarProgress.fillAmount = stageBarFillAmount;
        if (currentStage >= stageValues.Length)
        {
            currentStageValue = (1000 + (5 * (currentStage - stageValues.Length + 1))) * 2;
        }
        else
        {
            currentStageValue = stageValues[currentStage];
        }
        stageBarSize = 1f / currentStageValue;

        moneySpawnPointPosition = moneySpawnPoint.position;

        canChangeBarPosition = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        combopartical.SetActive(false);
        scale = money.transform.localScale;
        animationSpeed = currentCharacter.selfAnimator.speed;
        SetMoneyPanel(userTotalMoney);
        #region Animation Positions

        animationPositions = new Vector3[7];
        animationPositions[0] = new Vector3(-2.9f, 0f, 15.2f);
        animationPositions[1] = new Vector3(-0.18f, 0f, 11.84f);
        animationPositions[2] = new Vector3(4.83f, 0f, 11.85f);
        animationPositions[3] = new Vector3(3.0f, 0f, 15.2f);
        animationPositions[4] = new Vector3(-4.88f, 0f, 11.95f);
        animationPositions[5] = new Vector3(0f, 0f, 6.93f);
        animationPositions[6] = new Vector3(-0.18f, 0f, 11.84f);

        #endregion

        FillMoneyAnimationIndexList(leftAnimationIndex, list_leftAnimationIndex);
        FillMoneyAnimationIndexList(forwardAnimationIndex, list_forwardAnimationIndex);
        FillMoneyAnimationIndexList(rightAnimationIndex, list_rightAnimationIndex);

        animators_handsup = backgorundHandsUp.GetComponentsInChildren<Animator>();

        wrongMoveLimit = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (canTouch)
        {
            ThrowMoney();
        }

        if (currentCharacter)
        {
            BarPositionFixed();
        }

        if (isMoneyEffectActive)
        {
            moneyEffectTimer += Time.deltaTime;
        }

        IsDancerIdleOrDance();
        SwipeSpeedComboControl();
    }

    private void ThrowMoney()
    {
        timer += Time.deltaTime;


        if (Input.GetMouseButtonDown(0))
        {
            firstMousePosition = Input.mousePosition;

        }

        if (Input.GetMouseButtonUp(0))
        {
            lastMousePosition = Input.mousePosition;


            if (lastMousePosition.y > firstMousePosition.y + 20f && canMoveToForward)
            {
                PlayHandAnimation();
                backMoney = 0;
                ChooseMoneyAnimation(moneys[0].selfObject);
                moneySound.Play();
                if (moneys[0].isTrueMoney)//trueMoney)
                {
                    PlayDanceAnimation();



                    FillBar(moneys[0].moneyValue);
                    MMVibrationManager.Haptic(HapticTypes.HeavyImpact, false);

                    IncreaseDancerAnimationSpeed();
                    IncreaseMoney(moneys[0].moneyValue);
                    UIControlTextsAndBars(moneys[0].moneyValue);

                    moneyCounterForCombo++;
                }
                else
                {
                    wrongMoveCount++;

                    WarningPlatformLight(false);

                    currentCharacter.selfAnimator.speed = 1f;
                    MMVibrationManager.Haptic(HapticTypes.Failure, false);

                    if (wrongMoveCount == wrongMoveLimit)
                    {
                        LevelFailed();
                    }
                    else
                    {
                        mainCamera.GetComponent<ShakeObject>().ShakeIt();
                    }
                }
                SpawnMoney();
                ReduceMoneyScale();

                if (IsBarFilled())
                {
                    difference = currentCharacterEarnedMoney - moneyCount;
                    if (difference > 0)
                    {
                        text_overMoney.text = "+" + difference.ToString();
                        text_overMoney.transform.parent.gameObject.SetActive(true);
                        StartCoroutine(MoneyEffectOnCharacterCompleted());
                    }
                    CharacterCompleted();
                }

                timer = 0f;

                if (isSwipeTutorialOpen)
                {
                    UIManager.instance.ClosePanels();
                    isSwipeTutorialOpen = false;
                }
                moneys.Remove(moneys[0]);
            }
            else if (lastMousePosition.y < firstMousePosition.y - 20f
                && LevelManager.instance.currentLevel != 0 && canMoveToBack) //for tutorial
            {

                //para aşağı akacak ve oyun devam edecek
                if (!moneys[0].isTrueMoney)//trueMoney)
                {
                    WarningPlatformLight(false);
                    if (isWrongMoneyTutorialOpen)
                    {
                        UIManager.instance.ClosePanels();
                        isWrongMoneyTutorialOpen = false;
                        isWrongMoneyTutorialFinished = true;
                        canMoveToForward = true;
                    }
                }

                animator_hand.SetTrigger("HandWrongMove");
                animator_hand.SetTrigger("WrongMove");
                moneys[0].selfAnim.SetTrigger("WrongMoney");
                backMoney++;
                DanceToIdle();
                SpawnMoney();
                ReduceMoneyScale();

                timer = 0f;

                moneys.Remove(moneys[0]);
            }
        }
        else
        {
            DecreaseDancerAnimationSpeed();
        }
    }

    private void IsDancerIdleOrDance()
    {
        if (timer >= .8f)
        {
            currentCharacter.selfAnimator.SetInteger("RandomDance", 50);
            currentCharacter.selfAnimator.speed = 1.0f;
            timer = .8f;
            isStopped = true;
        }
    }

    private void DanceToIdle()
    {
        if (backMoney == 7)
        {
            currentCharacter.selfAnimator.SetInteger("RandomDance", 50);
            currentCharacter.selfAnimator.speed = 1.0f;
            backMoney = 0;
        }
    }

    private void PlayHandAnimation()
    {
        animator_hand.SetBool("HandBack", false);
        animator_hand.SetTrigger("ThrowMoney");
    }

    private void SwipeSpeedComboControl()
    {
        animationSpeed = currentCharacter.selfAnimator.speed;
        if (animationSpeed >= 1.3f)
        {
            combopartical.SetActive(true);
            if (moneyCounterForCombo >= 3)
            {
                Combo();
                moneyCounterForCombo = 0;
            }
        }
        else
        {
            combopartical.SetActive(false);
            moneyCounterForCombo = 0;
        }
    }

    private void IncreaseMoney(int increaseValue)
    {
        if (animationSpeed > 1.5f)
        {
            userTotalMoney += 2 + increaseValue;
        }
        else if (animationSpeed == 2f)
        {
            userTotalMoney += 3 + increaseValue;
        }
        else
        {
            userTotalMoney += increaseValue;
        }

        currentCharacterEarnedMoney += increaseValue;
    }

    private void ChooseMoneyAnimation(GameObject money)
    {
        if (lastMousePosition.x > firstMousePosition.x + 20f)
        {
            PlayMoneyAnimation("Right", money);
        }
        else if (lastMousePosition.x < firstMousePosition.x - 20f)
        {
            PlayMoneyAnimation("Left", money);
        }
        else
        {
            PlayMoneyAnimation("Forward", money);
        }
    }

    private void SpawnMoney()
    {
        GameObject nextMoney;
        if (trueMoneyCount == 0 && LevelManager.instance.currentLevel != 0)
        {
            trueMoneyCount = Random.Range(7, 15);
            nextMoney = Instantiate(prefab_wrongMoney, moneySpawnPoint.position, moneySpawnPoint.rotation);
            trueMoney = false;

            if (!isWrongMoneyTutorialFinished && !isWrongMoneyTutorialOpen && LevelManager.instance.currentLevel == 1)
            {
                UIManager.instance.ShowWrongMoneyTutorial();
                isWrongMoneyTutorialOpen = true;
                canMoveToBack = true;
                canMoveToForward = false;
            }
        }
        else
        {
            nextMoney = Instantiate(prefab_trueMoney, moneySpawnPoint.position, moneySpawnPoint.rotation);
            trueMoney = true;
            trueMoneyCount--;
        }

        if (trueMoneyCount == 1 && LevelManager.instance.currentLevel != 0)
        {
            WarningPlatformLight(true);
        }

        nextMoney.transform.SetParent(level.transform);
        moneys.Add(nextMoney.GetComponent<Money>());

        int moneyValueIndex = ChooseMoneyValueIndex();
        moneys[moneys.Count - 1].text_topMoneyValue.text = moneyValues[moneyValueIndex].ToString();
        moneys[moneys.Count - 1].text_bottomMoneyValue.text = moneyValues[moneyValueIndex].ToString();

        moneys[moneys.Count - 1].moneyValue = moneyValues[moneyValueIndex];
    }

    private int ChooseMoneyValueIndex()
    {
        int moneyValueIndex;
        if (moneyCount < 100)
        {
            moneyValueIndex = Random.Range(0, 2);
        }
        else if (moneyCount < 300)
        {
            int randomValue = Random.Range(0, 5);
            if (randomValue == 4)
            {
                moneyValueIndex = 2;
            }
            else
            {
                moneyValueIndex = Random.Range(0, 2);
            }
        }
        else if (moneyCount < 500)
        {
            int randomValue = Random.Range(0, 5);
            if (randomValue == 4)
            {
                moneyValueIndex = 3;
            }
            else
            {
                moneyValueIndex = Random.Range(0, 3);
            }
        }
        else
        {
            int randomValue = Random.Range(0, 5);
            if (randomValue == 4)
            {
                moneyValueIndex = 4;
            }
            else
            {
                moneyValueIndex = Random.Range(0, 4);
            }
        }

        return moneyValueIndex;
    }

    private void UIControlTextsAndBars(int moneyValue)
    {
        stageBarProgress.fillAmount += stageBarSize * moneyValue;
        if (stageBarProgress.fillAmount >= 1)
        {
            if (currentStage + 1 >= stageValues.Length)
            {
                currentStageValue = (1000 + (5 * (currentStage + 1 - stageValues.Length + 1))) * 2;
            }
            else
            {
                currentStageValue = stageValues[currentStage + 1];
            }
            stageBarSize = 1f / currentStageValue;
            nextStageFillAmount += stageBarSize;
        }
        SetMoneyPanel(userTotalMoney);
    }

    private void IncreaseDancerAnimationSpeed()
    {
        currentCharacter.selfAnimator.speed += .3f;
        if (currentCharacter.selfAnimator.speed > 2.0f)
        {
            currentCharacter.selfAnimator.speed = 2.0f;
        }
        animationSpeed = currentCharacter.selfAnimator.speed;
    }

    private void DecreaseDancerAnimationSpeed()
    {
        currentCharacter.selfAnimator.speed -= .01f;
        if (currentCharacter.selfAnimator.speed < 1.0f)
        {
            currentCharacter.selfAnimator.speed = 1.0f;
        }
        animationSpeed = currentCharacter.selfAnimator.speed;
    }

    private void SetBarPosition(Vector3 barPosition)
    {
        //  scrollBar.transform.position = mainCamera.WorldToScreenPoint(barPosition);
        scrollBar.transform.position = barPosition;
    }

    private void BarPositionFixed()
    {
        scrollBar.transform.position = currentCharacter.barFollowObject.position + offset;
    }

    private void PlayMoneyAnimation(string direction, GameObject moneyObject)
    {

        if (direction == "Left")
        {
            ChoosingAnimation(leftAnimationIndex, list_leftAnimationIndex, moneyObject);
        }
        else if (direction == "Forward")
        {
            ChoosingAnimation(forwardAnimationIndex, list_forwardAnimationIndex, moneyObject);
        }
        else
        {
            ChoosingAnimation(rightAnimationIndex, list_rightAnimationIndex, moneyObject);
        }

        StartCoroutine(ForEndOfMoneyAnimation(moneyObject));
    }

    private void ChoosingAnimation(int[] arrayIndex, List<int> listIndex, GameObject money)
    {
        if (listIndex.Count == 0)
        {
            FillMoneyAnimationIndexList(arrayIndex, listIndex);
        }

        int animationParameterIndex = Random.Range(0, listIndex.Count);
        money.GetComponent<Animator>().SetInteger("RandomThrow", listIndex[animationParameterIndex]);
        listIndex.Remove(listIndex[animationParameterIndex]);
    }

    private void PlayDanceAnimation()
    {
        currentCharacter.selfAnimator.SetInteger("RandomDance", currentDance);
    }

    private void CharacterCompleted()
    {
        canTouch = false;
        currentCharacter.selfAnimator.SetTrigger("HappyIdle");
        currentCharacter.selfAnimator.speed = 1.0f;
        Invoke("ChangePlatform", 1f);
        //   ChangePlatform();
    }

    private void ReduceMoneyScale()
    {
        if (scale.y > 6f)
        {
            scale.y = scale.y - 1f;
            money.transform.localScale = scale;
            moneySpawnPoint.position -= new Vector3(0, .009f, 0);
        }
    }

    private void FillBar(int fillValue)
    {
        barProgress.fillAmount += barSize * fillValue;

        characterBarProgressMoneyCount += fillValue;
        characterBarText.text = characterBarProgressMoneyCount.ToString() + " / " + moneyCount.ToString();
    }

    private bool IsBarFilled()
    {
        if (barProgress.fillAmount >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpawnPlatform()
    {
        platformCount = PlayerPrefs.GetInt("PlatformCount");
        thePlatform = (GameObject)Instantiate(prefabs_platform[platformCount]);
        thePlatform.transform.SetParent(level.transform);
        thePlatforms = thePlatform.GetComponentsInChildren<Platform>();

        currentPlatform = thePlatforms[0];
    }

    private void SpawnCharacters()
    {
        characters.Clear();

        for (int i = 0; i <= platformCount; i++)
        {
            thePlatforms[i].index = i;
            if (list_prefabs_character.Count == 0)
            {
                FillTheCharacterList();
            }
            int randomCharacterIndex = Random.Range(0, list_prefabs_character.Count);
            GameObject character = (GameObject)Instantiate(list_prefabs_character[randomCharacterIndex], thePlatforms[i].characterSpawnPoint.position, Quaternion.identity);
            characters.Add(character.GetComponent<Character>());
            character.transform.SetParent(thePlatforms[i].transform);

            int randomPose = Random.Range(1, 9);
            characters[characters.Count - 1].selfAnimator.SetInteger("RandomPose", randomPose);

            list_prefabs_character.Remove(list_prefabs_character[randomCharacterIndex]);
        }

        currentCharacter = characters[0];
        currentCharacter.meshCharacter.layer = 0;
        currentCharacter.selfAnimator.SetTrigger("PoseToIdle");
        SetBarPosition(currentCharacter.barPosition.position);
        currentDance = Random.Range(1, 12);
        // currentDance = danceAnimation;
        dancingMoneyValue = danceMoney[currentDance];
        currentCharacterTotalMoney = (dancingMoneyValue + currentCharacter.characterMoney + (currentStage + 1) * 15) * 2;

        if (LevelManager.instance.currentLevel == 0)
        {
            currentCharacterTotalMoney = 50;
        }

        SetMoneyCount(currentCharacterTotalMoney);

        offset = scrollBar.transform.position - currentCharacter.barFollowObject.position;
        canChangeBarPosition = true;
    }

    private void FillTheCharacterList()
    {
        int arrayLength = prefabs_character.Length;
        for (int i = 0; i < arrayLength; i++)
        {
            if (openCharacterIndex.Contains(i))
            {
                list_prefabs_character.Add(prefabs_character[i]);
            }
        }
    }

    private void ChangePlatform()
    {

        text_overMoney.transform.parent.gameObject.SetActive(false);

        moneyEffectTimer = 0;
        isMoneyEffectActive = false;

        if (currentPlatform.index + 1 == thePlatforms.Length)
        {
            //level completed
            StartCoroutine(LevelCompleted());


            PlayerPrefs.SetInt("UserTotalMoney", userTotalMoney);
            PlayerPrefs.SetFloat("StageBarFillAmount", stageBarProgress.fillAmount);
            if (stageBarProgress.fillAmount >= 1)
            {
                StartCoroutine(StageCompleted());
            }

            combopartical.SetActive(false);
            WarningPlatformLight(false);
            return;
        }




        previousPlatform = currentPlatform;
        previousCharacter = currentCharacter;

        canChangeBarPosition = false;

        currentPlatform = thePlatforms[previousPlatform.index + 1];
        currentCharacter = characters[previousPlatform.index + 1];

        SetBarPosition(currentCharacter.barPosition.position);

        canChangeBarPosition = true;

        currentDance = Random.Range(1, 12);
        //  currentDance = danceAnimation;
        dancingMoneyValue = danceMoney[currentDance];

        currentCharacterTotalMoney = (dancingMoneyValue + currentCharacter.characterMoney + (currentStage + 1) * 15) * 2;
        SetMoneyCount(currentCharacterTotalMoney);


        currentCharacter.selfAnimator.SetTrigger("PoseToIdle");

        canTouch = false;
        barProgress.fillAmount = 0;
        wrongMoveCount = 0;
        currentCharacterEarnedMoney = 0;


        currentCharacter.meshCharacter.layer = 0;

        ChangePlatformPosition();
    }

    /*  private IEnumerator OverMoneyEffect()
      {
          yield return new WaitForSecondsRealtime(1f);
          text_overMoney.transform.parent.gameObject.SetActive(false);
      }*/

    private IEnumerator MoneyEffectOnCharacterCompleted()
    {
        yield return new WaitForSecondsRealtime(.1f);
        
        int randomIndexValue = Random.Range(0, moneyAnimationsImages.Length);
           GameObject animationMoneyObject = (GameObject)Instantiate(moneyAnimationsImages[randomIndexValue]);
        //  GameObject animationMoneyObject = Canvas.Instantiate(moneyAnimationsImages[randomIndexValue]);
        animationMoneyObject.transform.SetParent(moneyAnimationsImages[randomIndexValue].transform.parent);
        animationMoneyObject.GetComponent<RectTransform>().position = moneyAnimationsImages[randomIndexValue].GetComponent<RectTransform>().position;
        animationMoneyObject.transform.localScale = moneyAnimationsImages[randomIndexValue].transform.localScale;
        animationMoneyObject.SetActive(true);

        moneyEffects.Add(animationMoneyObject);

        counterMoneyEffectMoney++;

        if (counterMoneyEffectMoney >= difference || moneyEffectTimer > 2f)
        {
            moneyEffectTimer = 0;
            isMoneyEffectActive = false;
            counterMoneyEffectMoney = 0;
            
        }
        else
        {
            StartCoroutine(MoneyEffectOnCharacterCompleted());
        }
    }

    private void ChangePlatformPosition()
    {
        if (currentPlatform.transform.position.x < previousPlatform.transform.position.x)
        {
            currentPlatform.transform.DOLocalMove(animationPositions[4], .25f).OnComplete(() =>
            {
                currentPlatform.transform.DOLocalMove(animationPositions[5], .25f);
            });

            previousPlatform.transform.DOLocalMove(animationPositions[1], .25f).OnComplete(() =>
            {
                previousPlatform.transform.DOLocalMove(animationPositions[0], .25f).OnComplete(() =>
                {
                    canTouch = true;
                });
            });
        }
        else
        {
            currentPlatform.transform.DOLocalMove(animationPositions[2], .25f).OnComplete(() =>
            {
                currentPlatform.transform.DOLocalMove(animationPositions[5], .25f);
            });

            previousPlatform.transform.DOLocalMove(animationPositions[6], .25f).OnComplete(() =>
            {
                previousPlatform.transform.DOLocalMove(animationPositions[3], .25f).OnComplete(() =>
                {
                    canTouch = true;
                });
            });
        }
    }

    #region Level Completed & Failed Methods

    IEnumerator LevelCompleted()
    {

        isFinished = true;
        levelCompleted = true;

        combopartical.SetActive(false);

        completedConfetti.SetActive(true);
        LevelManager.instance.NextLevel();

        yield return new WaitForSecondsRealtime(1f);
        if (stageCompleted)
        {
            UIManager.instance.panel_levelCompleted.SetActive(false);
        }
        else
        {
            UIManager.instance.ShowLevelCompletedPanel();
        }
        UIManager.instance.panel_market.SetActive(true);
    }

    private void LevelFailed()
    {
        isFinished = true;
        levelFailed = true;
        canTouch = false;
        combopartical.SetActive(false);
        foreach (Animator anim in animators_handsup)
        {
            anim.enabled = false;
        }

        foreach (Character character in characters)
        {
            character.selfAnimator.SetTrigger("Failed");
        }

        UIManager.instance.ShowLevelFailedPanel();
        UIManager.instance.panel_market.SetActive(true);
    }

    private IEnumerator StageCompleted()
    {
        combopartical.SetActive(false);
        stageCompleted = true;
        currentStage++;
        PlayerPrefs.SetInt("Stage", currentStage);
        PlayerPrefs.SetFloat("StageBarFillAmount", nextStageFillAmount);

        yield return new WaitForSeconds(1f);
        UIManager.instance.ShowStageCompletedPanel(currentStage + 1);
    }

    #endregion

    public void DefaultValues()
    {
        #region Load Level

        level = (GameObject)Instantiate(prefab_level);

        moneySpawnPoint.position = moneySpawnPointPosition;

        GameObject nextMoney;
        nextMoney = Instantiate(prefab_trueMoney, moneySpawnPoint.position, moneySpawnPoint.rotation);
        nextMoney.transform.SetParent(level.transform);
        trueMoney = true;

        moneys.Clear();
        moneys.Add(nextMoney.GetComponent<Money>());

        int moneyValueIndex = ChooseMoneyValueIndex();
        moneys[moneys.Count - 1].text_topMoneyValue.text = moneyValues[moneyValueIndex].ToString();
        moneys[moneys.Count - 1].text_bottomMoneyValue.text = moneyValues[moneyValueIndex].ToString();

        moneys[moneys.Count - 1].moneyValue = moneyValues[moneyValueIndex];

        moneySpawnPoint.position -= new Vector3(0, .009f, 0);
        SpawnPlatform();
        SpawnCharacters();



        #endregion

        if (LevelManager.instance.currentLevel == 0 || LevelManager.instance.currentLevel == 1)
        {
            canMoveToBack = false;
            canMoveToForward = true;
        }
        else
        {
            canMoveToForward = true;
            canMoveToBack = true;
        }

        int random = Random.Range(0, backgroundColors.Length);
        mesh_Wall.material.color = backgroundColors[random];

        money.transform.localScale = new Vector3(20f, 20f, 20f);
        scale = money.transform.localScale;

        stageCompleted = false;
        trueMoneyCount = Random.Range(7, 15);
        wrongMoveCount = 0;
        currentCharacterEarnedMoney = 0;

        moneyEffectTimer = 0;
        isMoneyEffectActive = false;

        foreach(GameObject go in moneyEffects)
        {
            Destroy(go);
        }

        moneyEffects.Clear();

        text_overMoney.transform.parent.gameObject.SetActive(false);

        SetMoneyPanel(PlayerPrefs.GetInt("UserTotalMoney"));
        userTotalMoney = PlayerPrefs.GetInt("UserTotalMoney");

        stageBarFillAmount = PlayerPrefs.GetFloat("StageBarFillAmount");
        stageBarProgress.fillAmount = stageBarFillAmount;




        if (isFinished)
        {
            StartCoroutine(CanTouchToTrue());

            foreach (Animator anim in animators_handsup)
            {
                anim.enabled = true;
            }
        }

        barProgress.fillAmount = 0;

        completedConfetti.SetActive(false);
        isFinished = false;
        levelCompleted = false;
        levelFailed = false;


        firstTouch = true;
    }

    private IEnumerator ForEndOfMoneyAnimation(GameObject money)
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Vector3 randomForce;
        if (Random.Range(0, 1) == 0)
        {
            randomForce = new Vector3(Random.Range(-70, -40), Random.Range(40, 70), Random.Range(-70, -40));
        }
        else
        {
            randomForce = new Vector3(Random.Range(40, 70), Random.Range(-70, -40), Random.Range(40, 70));
        }
        if (money)
        {
            money.GetComponent<Animator>().enabled = false;
            money.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
            money.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(randomForce);
        }

    }

    private void FillMoneyAnimationIndexList(int[] arrayIndex, List<int> listIndex)
    {
        listIndex.Clear();

        foreach (int index in arrayIndex)
        {
            listIndex.Add(index);
        }
    }

    public void ResetStageBar()
    {
        stageBarProgress.fillAmount = 0;
        currentStageText.text = (currentStage + 1).ToString();
        nextStageText.text = (currentStage + 2).ToString();
        if (nextStageFillAmount > 0)
        {
            DOTween.To(x => stageBarProgress.fillAmount = x, 0, nextStageFillAmount, 1f);
        }
    }

    private void SetMoneyCount(int count)
    {
        moneyCount = count;
        barSize = 1f / moneyCount;

        characterBarText.text = "0 / " + moneyCount.ToString();
        characterBarProgressMoneyCount = 0;
    }


    private IEnumerator CanTouchToTrue()
    {
        yield return new WaitForSeconds(.5f);
        canTouch = true;
    }

    public void SetMoneyPanel(int money)
    {
        if (money >= 1000)
        {
            float value = money / 1000;
            totalMoneyText.text = Mathf.FloorToInt(value).ToString() + "K";
        }
        else
        {
            totalMoneyText.text = money.ToString();
        }
    }

    private void WarningPlatformLight(bool isOpen)
    {
        foreach (Platform platform in thePlatforms)
        {
            platform.redLight.SetActive(isOpen);
        }
    }

    public void Combo()
    {
        int pointNumber = Random.Range(0, comboSpawnPoints.Length);
        GameObject comboss = Instantiate(prefabs_combo[Random.Range(0, prefabs_combo.Length)], comboSpawnPoints[pointNumber].position, Quaternion.identity);
        print(comboss.transform.position);
        comboss.GetComponent<Animation>().Play("Fading");
        Destroy(comboss, 3f);
    }
}

