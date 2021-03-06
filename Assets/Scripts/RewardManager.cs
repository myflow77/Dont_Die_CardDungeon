﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour {
	public static RewardManager instance;

	public Transform waitPosition;

	public Transform[] newCardPositions = new Transform[3];
	public GameObject[] newCardBorders = new GameObject[3];
	public GameObject[] newCards = new GameObject[3];

	// 선택된 새 카드와 직업이 같은 카드 리스트
	public Transform[] currentCardPositions = new Transform[3];
	public GameObject[] currentCardBorders = new GameObject[3];

	// 현재 덱의 카드 리스트
	public GameObject[] currentCards = new GameObject[9];
	public GameObject[] currentSelectedCards = new GameObject[3];


	public int selectedNewCardIndex = -1;
	public int selectedCurrentCardIndex = -1;

	private string randomKnightString;
	private string randomWizardString;
	private string randomPriestString;

	void Awake()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.SetResolution(1080, 1920, true);

		instance = this;

        selectedNewCardIndex = -1;
		selectedCurrentCardIndex = -1;

        // 랜덤 카드 3개 생성
        int randomKnightIndex = Random.Range(1, GlobalDataManager.instance.allCardList.knightCardList.Count);
        randomKnightString = GlobalDataManager.instance.allCardList.knightCardList[randomKnightIndex];

        int randomWizardIndex = Random.Range(1, GlobalDataManager.instance.allCardList.wizardCardList.Count);
        randomWizardString = GlobalDataManager.instance.allCardList.wizardCardList[randomWizardIndex];

        int randomPriestIndex = Random.Range(1, GlobalDataManager.instance.allCardList.priestCardList.Count);
        randomPriestString = GlobalDataManager.instance.allCardList.priestCardList[randomPriestIndex];

		// 프리팹에서 카드 데이터를 가져온다
		int stageNumber = GlobalDataManager.instance.saveData.stageNumber;

		// 리소스를 불러온 후 인스턴스화 필요
		newCards[0] = Instantiate(Resources.Load("Prefabs/Card/" + randomKnightString) as GameObject, new Vector3(0, 0, 0), Quaternion.identity);
        newCards[0].transform.position = newCardPositions[0].position;
		SetCardStatusAsLevel(newCards[0], stageNumber);
		newCardBorders[0].transform.position = newCardPositions[0].position;
        newCardBorders[0].SetActive(false);

        newCards[1] = Instantiate(Resources.Load("Prefabs/Card/" + randomWizardString) as GameObject, new Vector3(0, 0, 0), Quaternion.identity);
        newCards[1].transform.position = newCardPositions[1].position;
		SetCardStatusAsLevel(newCards[1], stageNumber);
		newCardBorders[1].transform.position = newCardPositions[1].position;
        newCardBorders[1].SetActive(false);

        newCards[2] = Instantiate(Resources.Load("Prefabs/Card/" + randomPriestString) as GameObject, new Vector3(0, 0, 0), Quaternion.identity);
        newCards[2].transform.position = newCardPositions[2].position;
		SetCardStatusAsLevel(newCards[2], stageNumber);
		newCardBorders[2].transform.position = newCardPositions[2].position;
        newCardBorders[2].SetActive(false);
		
		// 현재 카드 목록
        for (int i = 0; i < 9; i++)
        {
            SaveData.CardData cardData = GlobalDataManager.instance.saveData.currentCardList[i];

			// 리소스를 불러온 후 인스턴스화 필요
            currentCards[i] = Instantiate(Resources.Load("Prefabs/Card/" + cardData.cardName) as GameObject, new Vector3(0, 0, 10), Quaternion.identity);
            if (i % 3 == 0)
                currentCards[i].GetComponent<CardBase>().seal = CardBase.Seal.J;
            if (i % 3 == 1)
                currentCards[i].GetComponent<CardBase>().seal = CardBase.Seal.Q;
            if (i % 3 == 2)
                currentCards[i].GetComponent<CardBase>().seal = CardBase.Seal.K;

			
			SetCardStatusAsLevel(currentCards[i], cardData.level);
		}

		// 선택된 카드 목록을 NULL로 초기화
        for (int i = 0; i < 3; i++)
            currentSelectedCards[i] = null;

		// 카드 자리 테두리 위치를 설정
        currentCardBorders[0].transform.position = currentCardPositions[0].position;
        currentCardBorders[0].SetActive(false);
        currentCardBorders[1].transform.position = currentCardPositions[1].position;
        currentCardBorders[1].SetActive(false);
        currentCardBorders[2].transform.position = currentCardPositions[2].position;
        currentCardBorders[2].SetActive(false);
    }

	// Use this for initialization
	void Start ()
    {
		// 보상 씬 시작
		StartCoroutine(StartRewardScene());
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

	void SetCardStatusAsLevel(GameObject cardObject, int level)
	{
		CardBase cardBase = cardObject.GetComponent<CardBase>();
		double ratio = (1 + (level - 1) * 0.1);
		cardBase.level = level;
		cardBase.baseAttackPoint = (int)(cardBase.baseAttackPoint * ratio);
		cardBase.baseHealPoint = (int)(cardBase.baseHealPoint * ratio);
		cardBase.baseHp = (int)(cardBase.baseHp * ratio);
		cardBase.attackPoint = cardBase.baseAttackPoint;
		cardBase.healPoint = cardBase.baseHealPoint;
		cardBase.hp = cardBase.baseHp;
	}

	public IEnumerator StartRewardScene()
	{
		// 뽑은 카드를 한 장씩 뒤집을 때 사용
		yield return new WaitForSeconds(0.1f);
	}

	// 진행 버튼이 눌리면 실행
	public void ClickProgressButton()
	{
		if (selectedCurrentCardIndex != -1 && selectedNewCardIndex != -1)
		{
			EndRewardScene();
            GlobalDataManager.instance.saveData.stageNumber++;
            GlobalDataManager.instance.ChangeSceneToBattle();
		}
	}

	// 선택된 카드를 현재 카드 목록에 추가
	private void EndRewardScene()
	{
		if (selectedNewCardIndex == 0)
		{
            GlobalDataManager.instance.saveData.currentCardList[selectedCurrentCardIndex]
                = new SaveData.CardData(randomKnightString, GlobalDataManager.instance.saveData.stageNumber);
        }
		else if (selectedNewCardIndex == 1)
		{
            GlobalDataManager.instance.saveData.currentCardList[3 + selectedCurrentCardIndex]
                = new SaveData.CardData(randomWizardString, GlobalDataManager.instance.saveData.stageNumber);
        }
		else if (selectedNewCardIndex == 2)
		{
            GlobalDataManager.instance.saveData.currentCardList[6 + selectedCurrentCardIndex]
                = new SaveData.CardData(randomPriestString, GlobalDataManager.instance.saveData.stageNumber);
        }
		else
		{
			Debug.Log("Selected Card is none");
		}

	}

	public void ClickNewCard(int index)
	{
		if (index == 0)
		{
			SetKnightCard();
		}
		else if (index == 1)
		{
			SetWizardCard();
		}
		else if (index == 2)
		{
			SetPriestCard();
		}
		for (int i = 0; i < 3; i++)
		{
			if (i == index)
				newCardBorders[i].SetActive(true);
			else
				newCardBorders[i].SetActive(false);
		}
		selectedNewCardIndex = index;
	}

	public void SetKnightCard()
	{
		int count = 0;
		for (int i = 0; i < 9; i++)
		{
			if (0 <= i && i < 3)
			{
				currentCards[i].transform.position = currentCardPositions[count].position;
				currentSelectedCards[count++] = currentCards[i];
			}
			else
				currentCards[i].transform.position = waitPosition.position;
		}
	}

	public void SetWizardCard()
	{
		int count = 0;
		for (int i = 0; i < 9; i++)
		{
			if (3 <= i && i < 6)
			{
				currentCards[i].transform.position = currentCardPositions[count].position;
				currentSelectedCards[count++] = currentCards[i];
			}
			else
				currentCards[i].transform.position = waitPosition.position;
		}
	}

	public void SetPriestCard()
	{
		int count = 0;
		for (int i = 0; i < 9; i++)
		{
			if (6 <= i && i < 9)
			{
				currentCards[i].transform.position = currentCardPositions[count].position;
				currentSelectedCards[count++] = currentCards[i];
			}
			else
				currentCards[i].transform.position = waitPosition.position;
		}
	}

	public void ClickCurrentCard(int index)
	{
		for (int i = 0; i < 3; i++)
		{
			if (i == index)
				currentCardBorders[i].SetActive(true);
			else
				currentCardBorders[i].SetActive(false);
		}
		selectedCurrentCardIndex = index;
	}
}
