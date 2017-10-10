﻿using UnityEngine;

[System.Serializable]
public class CardBase : MonoBehaviour {
	public enum Seal
    {
		J,
		Q,
		K
	};
	public enum Job
    {
		Knight,
		Wizard,
		Priest
	};
	public enum Status
    {
        inField,
		inHand,
		inDeck,
		inTomb
	};
    public int index = 0; // 카드가 속한 곳에서 몇번째에 있는지 저장하는 변수

	public Seal seal;
	public Job job;
    public Status status = Status.inDeck;

    public string cardName;
	public string description;

    public int baseAttackPoint;
    public int baseHealPoint;
    public int baseHealthPoint;

    public int attackPoint;
	public int healPoint;
    public int healthPoint;
    public bool GenerateRandomeData = false;
    public bool canPlay = false;

	public TextMesh attackText;
	public TextMesh healText;

	public Vector3 newPos;

	float distance_to_screen;
	bool Selected = false;

    public delegate void CustomAction();

    // Use this for initialization
    void Start () {
        //distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z - 1;
        distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
    }

    private void Update()
    {
        attackText.text = attackPoint.ToString();
        healText.text = healPoint.ToString();
    }

    void FixedUpdate()
	{
        attackText.text = attackPoint.ToString();
        healText.text = healPoint.ToString();

        if (Battle.instance.gameState == Battle.GameState.Default)
        {
            // 카드가 선택된 상태가 아니라면 newPos로 이동하려는 성질을 가진다.
            if (!Selected)
            {
                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 3);
                if (status == Status.inTomb || status == Status.inDeck)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, 0.0f), Time.deltaTime * 3);
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 3);
                }

            }
        }
        else if (Battle.instance.gameState == Battle.GameState.CardAttacking)
        {
            if (gameObject == Battle.instance.attackingCard)
            {
                float distance = Vector3.Distance(transform.position, Battle.instance.monsterPos.position);
                if (distance < 2)
                {
                    Debug.Log("CardBase : distance < 2");
                    //Battle.instance.gameState = Battle.GameState.CardAttackEnd;
                    AttackMonster(Battle.instance.monster, null);
                    Battle.instance.gameState = Battle.GameState.CardAttacked;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, Battle.instance.monsterPos.position, Time.deltaTime * 3);
                }
            }
        }

    }

	void OnMouseDown()
	{
		if (status == Status.inHand && status == Status.inField)
		{
			Selected = true;
		}
	}
	void OnMouseUp()
	{
        Debug.Log("onMouseUp()");
		Selected = false;
        // 다른 카드와 겹쳐있으면 그 카드와 원래의 위치를 변경
        Battle.instance.CheckPlace(gameObject);

    }
	void OnMouseOver()
	{
		//Debug.Log("On Mouse Over Event");
	}
	void OnMouseEnter()
	{
		//Debug.Log("On Mouse Enter Event");
		//newPos += new Vector3(0,0.5f,0);
	}
	void OnMouseExit()
	{
		//Debug.Log("On Mouse Exit Event");
		//newPos -= new Vector3(0,0.5f, 0);
	}
	void OnMouseDrag()
	{
        Debug.Log("OnMouseDrag()");
        if (status == Status.inField || status == Status.inHand)
        {
            Vector3 tempVector3 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
            tempVector3.z = transform.position.z;
            transform.position = tempVector3;
        }
    }

    public void AttackMonster(GameObject target, CustomAction action) // 몬스터를 공격!!
    {
        Debug.Log("Attack");
        target.GetComponent<MonsterBase>().healthPoint -= attackPoint;

        //action();
        Battle.instance.AddHistory(this, target.GetComponent<MonsterBase>());
    }

    public void Destroy(CardBase card)
	{
		if (card)
		{
			if (card.gameObject != null)
			{
                //Battle.instance.fieldCards.Remove(card.gameObject);

				//BoardBehaviourScript.instance.PlaySound(BoardBehaviourScript.instance.cardDestroy);
				Destroy(card.gameObject);

			}

		}
        else
		{
			//card = null;
		}
	}

	/*public void AddToHero(CardBehaviourScript magic, HeroBehaviourScript target, CustomAction action) // 카드에서 어떤 옵션을 다른 카드나 몬스터에 적용하는 함수 example
    {
		if (magic.canPlay)
		{
			target._Attack += magic.AddedAttack;
			if (target.health + magic.AddedHealth <= 30)
				target.health += magic.AddedHealth;
			else
				target.health = 30;
			action();
			Battle.instance.AddHistory(magic, target);
		}
	}

	public void AddToEnemies(CardBehaviourScript magic, List<CardBehaviourScript> targets, bool addhistory, CustomAction action) // 카드에서 어떤 옵션을 다른 카드나 몬스터에 적용하는 함수 example
    {
		if (magic.canPlay)
		{
			foreach (var target in targets)
			{
				AddToMonster(magic, target, addhistory, delegate { });
			}
			action();
		}
	}*/

	public object Clone()
	{
		CardBase temp = new CardBase();

        temp.seal = this.seal;
        temp.job = this.job;
        temp.status = this.status;
        temp.index = this.index;
		temp.cardName = this.cardName;
		temp.description = this.description;
		temp.attackPoint = this.attackPoint;
		temp.healPoint = this.healPoint;
        temp.healthPoint = this.healthPoint;
        temp.GenerateRandomeData = this.GenerateRandomeData;
		temp.canPlay = this.canPlay;
		temp.newPos = this.newPos;
		temp.distance_to_screen = this.distance_to_screen;
		temp.Selected = this.Selected;

		return temp;
	}
}