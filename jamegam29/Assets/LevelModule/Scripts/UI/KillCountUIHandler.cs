using System;
using System.Collections;
using System.Collections.Generic;
using GameEvents;
using TMPro;
using UnityEngine;

public class KillCountUIHandler : MonoBehaviour, IGameEventListener
{
    [SerializeField] private TextMeshProUGUI killAmountText;
    [SerializeField] private GameEvent killConfirmedEvent;

    private int currentKillAmount;
    // Start is called before the first frame update
    void Awake()
    {
        killConfirmedEvent.RegisterListener(this);
    }

    private void OnDestroy()
    {
        killConfirmedEvent.UnregisterListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetKillAmountText()
    {
        killAmountText.text = "0";
        currentKillAmount = 0;
    }

    public void OnEventRaised()
    {
        if(currentKillAmount < 9999)
        {
            currentKillAmount++;
        }
        killAmountText.gameObject.GetComponent<Animator>().SetBool("raised", true);
        killAmountText.text = currentKillAmount.ToString();
        StartCoroutine(sorryBwadeButThisIsTheOnlyWayIKnowHowToDoThings());
    }

    IEnumerator sorryBwadeButThisIsTheOnlyWayIKnowHowToDoThings()
    {
        yield return new WaitForSeconds(0.07f);
        killAmountText.gameObject.GetComponent<Animator>().SetBool("raised", false);
    }
}
