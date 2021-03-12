using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Transform barPosition;
    public Animator selfAnimator;

    public int characterMoney;
    public int characterLevel;
    public int increasingValue;

    public bool isOpen;

    public int index;

    public GameObject meshCharacter;

    public Transform barFollowObject;

    private void Start()
    {
        DetermineCharacterMoney();
    }

    public void DetermineCharacterMoney()
    {
        characterMoney += characterLevel * increasingValue;
    }
}

