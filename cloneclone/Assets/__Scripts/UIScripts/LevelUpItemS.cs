using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelUpItemS : MonoBehaviour {

	private LevelUpS upgradeRef;
	private int _upgradeID = -1;
	public int upgradeID { get { return _upgradeID; } }

	public const int MAX_LEVEL_UP = 49; 

	[Header("UI Properties")]
	public Image upgradeImage;
	public Text upgradeNameText;
	public Text upgradeDescriptionText;
	private int upgradeNum;
	private int upgradeCost;

	private string upgradeName;
	private string upgradeDescription;

	[Header("Special Level Up Properties")]
	public string shuffleName;
	public string shuffleDescription;
	public Sprite shuffleLockedSprite;
	public Sprite shuffleUnlockedSprite;
	public string revertName;
	public string revertDescription;
	public Sprite revertLockedSprite;
	public Sprite revertUnlockedSprite;

	public PlayerLvDisplayS statDisplayRef;
	private PlayerCurrencyDisplayS cDisplay;

	private bool shuffleUpgrade = false;
    private bool revertUpgrade = false;
    public bool IsShuffleUpgrade { get { return shuffleUpgrade; } }
    public bool IsRevertUpgrade { get { return revertUpgrade; }}

	private PlayerStatsS statRef;

	public Color lockedTextColor;

	private int flatAddPerLevel = 100;


	void Start(){
		//lockedTextColor = upgradeNameText.color;
		statRef = GameObject.Find("Player").GetComponent<PlayerStatsS>();
	}


	public void Initialize(LevelUpS l, PlayerCurrencyDisplayS cD, bool isShuffle = false, bool isRevert = false){

		if (!statRef){
			statRef = GameObject.Find("Player").GetComponent<PlayerStatsS>();
		}
		if (!cDisplay){
			cDisplay = cD;
		}

		if (!isShuffle && !isRevert){

		upgradeRef = l;

		_upgradeID = l.upgradeID;

		upgradeNum = upgradeRef.upgradeID;
            upgradeDescription = LocalizationManager.instance.GetLocalizedValue(upgradeRef.upgradeDescription);
            upgradeName = LocalizationManager.instance.GetLocalizedValue(upgradeRef.upgradeName);
		upgradeCost = upgradeRef.upgradeBaseCost+upgradeRef.upgradeCostPerLv*statRef.currentLevel + flatAddPerLevel*(statRef.currentLevel-1);

		



		// add cost per upgrade owned
		float numOwned = 0;
		if (_upgradeID == 0){
			numOwned = statRef.addedHealth;
		}
		if (_upgradeID == 1){
			numOwned = statRef.addedMana;
		}
		if (_upgradeID == 2){
			numOwned = statRef.addedChargeLv;
		}
		if (_upgradeID == 3){
			numOwned = statRef.addedVirtue;
		}
		if (_upgradeID == 4){
			numOwned = statRef.currentChargeRecoverLv-1f;
		}
		if (_upgradeID == 5){
			numOwned = statRef.addedRateLv*1f;
		}
		if (_upgradeID == 6){
			numOwned = statRef.addedStrength;
		}

		if (numOwned > 0){
			float newUpgradeAdd = 0f;
			newUpgradeAdd = Mathf.Pow(upgradeRef.expCostPerUpgradeOwned, numOwned);
			upgradeCost = Mathf.RoundToInt((upgradeCost+newUpgradeAdd)/10f);
			upgradeCost*=10;
		}
		}else if (isShuffle){
			upgradeCost = statRef.currentLevel*10;
			shuffleUpgrade = true;

            upgradeDescription = LocalizationManager.instance.GetLocalizedValue(shuffleDescription);
            upgradeName = LocalizationManager.instance.GetLocalizedValue(shuffleName);
		}else if (isRevert){
			upgradeCost = 0;
			revertUpgrade = true;

            upgradeDescription = LocalizationManager.instance.GetLocalizedValue(revertDescription);
            upgradeName = LocalizationManager.instance.GetLocalizedValue(revertName);
		}

		if ((upgradeCost > PlayerCollectionS.currencyCollected && PlayerInventoryS.I.earnedUpgrades.Count <= MAX_LEVEL_UP) 
			|| (isRevert && statRef.currentLevel <= minRevert())){
			if (isRevert){
				upgradeImage.sprite = revertLockedSprite;
			}else if (isShuffle){
				upgradeImage.sprite = shuffleLockedSprite;
			}else{
			upgradeImage.sprite = upgradeRef.upgradeImgLocked;
			}
		}else{
			if (isRevert){
				upgradeImage.sprite = revertUnlockedSprite;
			}else if (isShuffle){
				upgradeImage.sprite = shuffleUnlockedSprite;
			}else{
				if (PlayerInventoryS.I.earnedUpgrades.Count < MAX_LEVEL_UP){
					upgradeImage.sprite = upgradeRef.upgradeImg;
				}else{
					upgradeImage.sprite = upgradeRef.upgradeImgLocked;
				}
			}
		}

	}

	public void ShowText(){
        upgradeNameText.text = upgradeName + " ( "+ LocalizationManager.instance.GetLocalizedValue("menu_upgrade_cost")+
            ": " + upgradeCost +  " / " + PlayerCollectionS.currencyCollected + " " + 
            LocalizationManager.instance.GetLocalizedValue("game_la")+ " )";
		upgradeDescriptionText.text = upgradeDescription;
        if (revertUpgrade){
            //Debug.Log("Current level: " + statRef.currentLevel + " Current lower revert limit: " + minRevert());
        }
		if ((upgradeCost > PlayerCollectionS.currencyCollected && !revertUpgrade) || (revertUpgrade && statRef.currentLevel<=minRevert())){
			SetTextColors(lockedTextColor);
			upgradeNameText.text = upgradeName 
                + " ( " + LocalizationManager.instance.GetLocalizedValue("menu_upgrade_cost") +
            ": < color=#ff0000ff>" + upgradeCost + "</color> / " + PlayerCollectionS.currencyCollected + " " +
            LocalizationManager.instance.GetLocalizedValue("game_la") + " )";
        }
        else{
			SetTextColors(Color.white);
            upgradeNameText.text = upgradeName + " ( " + LocalizationManager.instance.GetLocalizedValue("menu_upgrade_cost") +
            ": " + upgradeCost + " / " + PlayerCollectionS.currencyCollected + " " +
            LocalizationManager.instance.GetLocalizedValue("game_la") + " )";
        }

		if (!shuffleUpgrade && !revertUpgrade){
			statDisplayRef.HighlightStat(upgradeNum);
        }else if (revertUpgrade){
            statDisplayRef.HighlightStat(PlayerInventoryS.I.GetNextRevertLevelType(), true);
        }
	}

	private int minRevert(){
		return (PlayerInventoryS.I.GetMinRevertLevelAdd()+1);
	}

	private void SetTextColors(Color newCol){
		upgradeNameText.color = upgradeDescriptionText.color = newCol;
	}

	public bool CanBeUpgraded(){
		bool canBuy = false;
		if (PlayerCollectionS.currencyCollected >= upgradeCost){
			canBuy = true;
		}
        if (PlayerInventoryS.I.earnedUpgrades.Count >= MAX_LEVEL_UP && !revertUpgrade && !shuffleUpgrade){
			canBuy = false;
		}
        if (revertUpgrade && statRef.currentLevel <= minRevert()) { canBuy = false; }
		return canBuy;
	}

	public void BuyUpgrade(int index, LevelUpHandlerS lvH){
		cDisplay.AddCurrency(-upgradeCost);
		if (!shuffleUpgrade && !revertUpgrade){
		PlayerInventoryS.I.AddToUpgrades(upgradeNum);
		lvH.nextLevelUps.RemoveAt(index);
		if (upgradeRef.addUpgrades.Length > 0){
			foreach(LevelUpS u in upgradeRef.addUpgrades){
				lvH.AddAvailableUpgrade(u);
			}
			}
		}
        if (revertUpgrade)
        {
            cDisplay.AddCurrency(statRef.currentLevel * 100);
            PlayerInventoryS.I.LevelDown();
            // TODO make sure this doesn't break levelling up again!!
        }

		lvH.NewNextLevelUps(statRef.currentLevel);
	}

	public void TurnOffVisual(){
		upgradeImage.enabled = false;
	}
	public void TurnOnVisual(){
		upgradeImage.enabled = true;
	}
}
