using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerInventoryS : MonoBehaviour
{

    public LoadoutMasterScriptableObject masterLoadoutList;

    private bool initialized = false;

    private List<int> _collectedItems;
    private List<int> _collectedKeyItems;
    private List<int> _collectedItemCount;
    public List<int> collectedItems { get { return _collectedItems; } }
    public List<int> collectedKeyItems { get { return _collectedKeyItems; } }
    public List<int> collectedItemCount { get { return _collectedItemCount; } }

    private List<float> _revertDarknessNums = new List<float>(1) { 0 };
    public List<float> revertDarknessNums { get { return _revertDarknessNums; } }

    private List<int> _revertedCollectedItems;
    private List<int> _revertedCollectedKeyItems;
    private List<int> _revertedCollectedItemCount;

    private List<int> _earnedUpgrades;
    public List<int> earnedUpgrades { get { return _earnedUpgrades; } }
    public List<int> _earnedVirtues;
    public List<int> earnedVirtues { get { return _earnedVirtues; } }
    public List<int> _earnedTech;
    public List<int> earnedTech { get { return _earnedTech; } }

    private List<int> _clearedWalls;
    public List<int> clearedWalls { get { return _clearedWalls; } }
    private List<int> _revertedClearedWalls;
    private List<int> _openedDoors;
    public List<int> openedDoors { get { return _openedDoors; } }
    private List<int> _revertedOpenedDoors;

    private List<int> _revertedProgressNums;

    public List<PlayerWeaponS> unlockedWeapons;
    public List<BuddyS> unlockedBuddies;
    private static List<PlayerWeaponS> equippedWeapons;
    private static List<PlayerWeaponS> subWeapons;

    private List<int> healNums;
    private List<int> laPickupNums;
    private List<int> chargeNums;
    private List<int> vpNums;

    private List<int> checkpointsReachedScenes;
    private List<int> checkpointsReachedSpawns;
    private List<int> scenesIveBeenTo;

    private List<int> _revertedCheckpointsReachedScenes;
    private List<int> _revertedCheckpointsReachedSpawns;
    private List<int> _revertedScenesIveBeenTo;

    private static List<GameObject> equippedBuddies;

    private int _tvNum = 999;
    public int tvNum { get { return _tvNum; } }

    private InventoryManagerS _iManager;
    public InventoryManagerS iManager { get { return _iManager; } }
    private PlayerDestructionS _dManager;
    public PlayerDestructionS dManager { get { return _dManager; } }

    public static PlayerInventoryS I;

    public static InventorySave inventoryData;

    [Header("Demo Properties")]
    private bool unlockForDemo = false;
    private bool eraseOnNewGame = false;
    public List<PlayerWeaponS> weaponsToAddForDemo;
    public List<GameObject> buddiesToAddForDemo;

    private List<int> skippableScenes = new List<int>();
    public List<int> SkippableScenes { get { return skippableScenes; } }

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
    public static bool DO_NOT_SAVE = false;
#endif

    void Awake()
    {

        if (I == null)
        {
            Initialize();
        }
        else
        {
            if (I != this)
            {
                Destroy(gameObject);
            }
        }

    }
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            DO_NOT_SAVE = !DO_NOT_SAVE;
            Debug.Log("DO_NOT_SAVE turned to " + DO_NOT_SAVE);
        }
    }
#endif

    public void AddCheckpoint(int sceneIndex, int spawnN)
    {
        if (!checkpointsReachedScenes.Contains(sceneIndex))
        {
            checkpointsReachedScenes.Add(sceneIndex);
            checkpointsReachedSpawns.Add(spawnN);
        }
    }

    public void AddSceneIveBeenTo(int sceneIndex)
    {
        if (!scenesIveBeenTo.Contains(sceneIndex))
        {
            scenesIveBeenTo.Add(sceneIndex);
        }
    }

    public int ReturnCheckpointIndex(int scene)
    {
        int checkpointNum = 0;
        if (checkpointsReachedScenes.Contains(scene))
        {
            checkpointNum = checkpointsReachedScenes.IndexOf(scene);
        }
        return checkpointNum;
    }
    public int ReturnCheckpointAtIndex(int index)
    {
        int checkpointNum = -1;
        if (checkpointsReachedScenes.Count > index)
        {
            checkpointNum = checkpointsReachedScenes[index];
        }
        return checkpointNum;
    }
    public int ReturnCheckpointSpawnAtIndex(int index)
    {
        int checkpointNum = -1;
        if (checkpointsReachedScenes.Count > index)
        {
            checkpointNum = checkpointsReachedSpawns[index];
        }
        return checkpointNum;
    }
    public int ReturnCheckpointSpawnAtScene(int sceneIndex)
    {
        int checkpointNum = -1;
        if (ReturnCheckpointIndex(sceneIndex) > -1)
        {
            checkpointNum = ReturnCheckpointSpawnAtIndex(ReturnCheckpointIndex(sceneIndex));
        }
        return checkpointNum;
    }
    public int CheckpointsReached()
    {
        return checkpointsReachedScenes.Count;
    }

    public bool HasReachedScene(int checkpointSceneIndex)
    {
        bool hasReached = false;
        if (checkpointsReachedScenes != null)
        {
            for (int i = 0; i < checkpointsReachedScenes.Count; i++)
            {
                if (checkpointsReachedScenes[i] == checkpointSceneIndex)
                {
                    hasReached = true;
                }
            }
        }
        return hasReached;
    }

    public bool HasBeenToScene(int checkScene)
    {
        bool hasReached = false;
        if (scenesIveBeenTo != null)
        {
            for (int i = 0; i < scenesIveBeenTo.Count; i++)
            {
                if (scenesIveBeenTo[i] == checkScene)
                {
                    hasReached = true;
                }
            }
        }
        return hasReached;
    }

    public void AddKeyItem(int i)
    {
        // for key items that should NOT be in inventory (memos)
        if (!_collectedKeyItems.Contains(i))
        {
            _collectedKeyItems.Add(i);
        }
    }

    public void AddToInventory(int i, bool isKey = false, bool dontAddMultiple = false)
    {
        if (!_collectedItems.Contains(i))
        {
            if (i != 1)
            {
                _collectedItems.Add(i);
                _collectedItemCount.Add(1);
                _iManager.AddNextAvailable(i);
            }
            else
            {
                _collectedItems.Insert(1, i);
                _collectedItemCount.Insert(1, i);
                _iManager.AddAt(1, i);
            }
            if (isKey)
            {
                _collectedKeyItems.Add(i);
            }
        }
        else
        {
            if (!dontAddMultiple)
            {
                _collectedItemCount[_collectedItems.IndexOf(i)]++;
                _iManager.RefreshUI();
            }
        }
    }

    public void AddHeal(int i)
    {
        //if (!healNums.Contains(i)){
        healNums.Add(i);
        //}
    }
    public void AddCharge(int i)
    {
        if (!chargeNums.Contains(i))
        {
            chargeNums.Add(i);
        }
    }
    public void AddLaPickup(int i, int sinAmt)
    {
        if (!laPickupNums.Contains(i))
        {
            laPickupNums.Add(i);
        }

        GameObject.Find("Player").GetComponent<PlayerController>().myStats.uiReference.cDisplay.AddCurrency(sinAmt);
    }
    public void AddVP(int i)
    {
        if (!vpNums.Contains(i))
        {
            vpNums.Add(i);
        }
    }

    public int GetVPUpgradeCount()
    {
        if (vpNums != null)
        {
            return vpNums.Count;
        }
        else
        {
            return 0;
        }
    }

    public int GetItemCount(int i)
    {
        int count = 0;
        if (_collectedItems != null)
        {

            if (_collectedItems.Contains(i))
            {
                count = _collectedItemCount[_collectedItems.IndexOf(i)];
            }
        }
        return count;
    }

    public int GetUpgradeCount(int i)
    {
        int count = 0;
        if (_earnedUpgrades != null)
        {
            if (_earnedUpgrades.Contains(i))
            {
                for (int j = 0; j < _earnedUpgrades.Count; j++)
                {
                    if (_earnedUpgrades[j] == i)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    public void RemoveFromInventory(int i, bool rechargeable = false)
    {
        if (_collectedItems.Contains(i))
        {
            _collectedItemCount[_collectedItems.IndexOf(i)]--;
            if (_collectedItemCount[_collectedItems.IndexOf(i)] <= 0 && !rechargeable)
            {
                _collectedItemCount.RemoveAt(_collectedItems.IndexOf(i));
                _collectedItems.Remove(i);
            }
        }
    }

    public void AddSkippableScene(int skippableSceneIndex)
    {
        if (skippableSceneIndex > -1 && !PlayerInventoryS.I.skippableScenes.Contains(skippableSceneIndex))
        {
            skippableScenes.Add(skippableSceneIndex);
        }
    }

    public bool CheckForItem(int i)
    {
        return (_collectedItems.Contains(i));
    }

    public bool CheckForWeaponNum(int weaponNum)
    {
        bool hasWeapon = false;
        for (int i = 0; i < unlockedWeapons.Count; i++)
        {
            if (weaponNum == unlockedWeapons[i].weaponNum)
            {
                hasWeapon = true;
            }
        }
        return hasWeapon;
    }

    public void AddToUpgrades(int i)
    {
        _earnedUpgrades.Add(i);

        // account for upgrades that add virtues

        // Add "Perceptive" virtue
        if (i == 7)
        {
            AddEarnedVirtue(5);
        }
        // Add "Adaptive" virtue
        if (i == 8)
        {
            AddEarnedVirtue(4);
        }
        // Add "Driven" virtue
        if (i == 9)
        {
            AddEarnedVirtue(6);
        }
        // Add "Agile" virtue
        if (i == 10)
        {
            AddEarnedVirtue(9);
        }
        // Add "Loved" virtue
        if (i == 11)
        {
            AddEarnedVirtue(14);
        }
        // Add "Empowered" virtue
        if (i == 12)
        {
            AddEarnedVirtue(2);
        }
    }

    public int GetMinRevertLevelAdd()
    {
        int numAdd = 0;
        for (int i = 0; i < _earnedUpgrades.Count; i++)
        {
            if (i > 6)
            {
                numAdd++;
            }
        }
        return numAdd;
    }

    public void AddEarnedVirtue(int i)
    {
        if (!_earnedVirtues.Contains(i))
        {
            _earnedVirtues.Add(i);
        }
        if (i == 15 && !PlayerController.equippedVirtues.Contains(i))
        {
            PlayerController.equippedVirtues.Add(i);
        }
    }

    public void AddEarnedTech(int i, bool autoEquip = true)
    {
        if (!_earnedTech.Contains(i))
        {
            _earnedTech.Add(i);
            if (autoEquip)
            {
                PlayerController.equippedTech.Add(i);
            }
        }
    }

    public void AddClearedWall(int i)
    {
        _clearedWalls.Add(i);
    }

    void Initialize()
    {
        I = this;
        DontDestroyOnLoad(gameObject);

        _iManager = GetComponent<InventoryManagerS>();
        _dManager = GetComponent<PlayerDestructionS>();

        unlockedBuddies = new List<BuddyS>();

        if (inventoryData != null)
        {

            LoadInventoryData();

        }
        else
        {

            PlayerController pRef = GameObject.Find("Player").GetComponent<PlayerController>();


            _earnedUpgrades = new List<int>();
            _earnedVirtues = new List<int>();
            if (PlayerController.equippedVirtues == null)
            {
                _earnedVirtues.Add(0);
                PlayerController.equippedVirtues = new List<int>();
                PlayerController.equippedVirtues.Add(0);
            }
            else
            {
                if (PlayerController.equippedVirtues.Contains(15) && !_earnedVirtues.Contains(15))
                {
                    _earnedVirtues.Add(15);
                }// check if marked is only, testing case
                if (PlayerController.equippedVirtues.Contains(15) && !_earnedVirtues.Contains(0))
                {
                    PlayerController.equippedVirtues.Add(0);
                    _earnedVirtues.Add(0);
                }
            }
            _collectedItems = new List<int>();
            healNums = new List<int>();
            laPickupNums = new List<int>();
            chargeNums = new List<int>();
            vpNums = new List<int>();
            _collectedKeyItems = new List<int>();
            _collectedItemCount = new List<int>();
            _openedDoors = new List<int>();
            _clearedWalls = new List<int>();
            equippedWeapons = pRef.equippedWeapons;
            equippedBuddies = pRef.equippedBuddies;
            subWeapons = pRef.subWeapons;

            scenesIveBeenTo = new List<int>();
            checkpointsReachedScenes = new List<int>();
            checkpointsReachedSpawns = new List<int>();

            unlockedWeapons = new List<PlayerWeaponS>();
            for (int i = 0; i < pRef.equippedWeapons.Count; i++)
            {
                if (i == 0)
                {
                    unlockedWeapons.Add(pRef.equippedWeapons[i]);
                }
                else
                {
                    if (!unlockedWeapons.Contains(pRef.equippedWeapons[i]))
                    {
                        unlockedWeapons.Add(pRef.equippedWeapons[i]);
                    }
                }
            }

            unlockedBuddies = new List<BuddyS>();
            for (int i = 0; i < pRef.equippedBuddies.Count; i++)
            {
                if (i == 0)
                {
                    unlockedBuddies.Add(pRef.equippedBuddies[i].GetComponent<BuddyS>());
                }
                else
                {
                    if (!unlockedBuddies.Contains(pRef.equippedBuddies[i].GetComponent<BuddyS>()))
                    {
                        unlockedBuddies.Add(pRef.equippedBuddies[i].GetComponent<BuddyS>());
                    }
                }
            }

            _earnedTech = new List<int>() { 0, 1, 2, 3, 10, 11, 12, 13, 14 };
            PlayerController.equippedTech = new List<int> { 0, 1, 2, 3, 4 };

            if (unlockForDemo)
            {
                DemoUnlocks(pRef);
            }

        }

        initialized = true;
    }

    public void ReinitializeForDemo()
    {
        PlayerController pRef = GameObject.Find("Player").GetComponent<PlayerController>();


        _earnedUpgrades = new List<int>();
        _earnedVirtues = new List<int>();
        _earnedVirtues.Add(0);
        PlayerController.equippedVirtues = new List<int>();
        PlayerController.equippedVirtues.Add(0);
        _collectedItems = new List<int>();
        healNums = new List<int>();
        laPickupNums = new List<int>();
        chargeNums = new List<int>();
        vpNums = new List<int>();
        _collectedKeyItems = new List<int>();
        _collectedItemCount = new List<int>();
        _openedDoors = new List<int>();
        _clearedWalls = new List<int>();
        equippedWeapons = pRef.equippedWeapons;
        equippedBuddies = pRef.equippedBuddies;
        subWeapons = pRef.subWeapons;

        scenesIveBeenTo = new List<int>();
        checkpointsReachedScenes = new List<int>();
        checkpointsReachedSpawns = new List<int>();

        unlockedWeapons = new List<PlayerWeaponS>();
        for (int i = 0; i < pRef.equippedWeapons.Count; i++)
        {
            if (i == 0)
            {
                unlockedWeapons.Add(pRef.equippedWeapons[i]);
            }
            else
            {
                if (!unlockedWeapons.Contains(pRef.equippedWeapons[i]))
                {
                    unlockedWeapons.Add(pRef.equippedWeapons[i]);
                }
            }
        }

        unlockedBuddies = new List<BuddyS>();
        for (int i = 0; i < pRef.equippedBuddies.Count; i++)
        {
            if (i == 0)
            {
                unlockedBuddies.Add(pRef.equippedBuddies[i].GetComponent<BuddyS>());
            }
            else
            {
                if (!unlockedBuddies.Contains(pRef.equippedBuddies[i].GetComponent<BuddyS>()))
                {
                    unlockedBuddies.Add(pRef.equippedBuddies[i].GetComponent<BuddyS>());
                }
            }
        }

        _earnedTech = new List<int>() { 0, 1, 2, 3, 10, 11, 12, 13, 14 };
        PlayerController.equippedTech = new List<int> { 0, 1, 2, 3, 4 };

        DemoUnlocks(pRef);

    }

    void DemoUnlocks(PlayerController pRef)
    {

        LevelUpHandlerS lHandler = GetComponent<LevelUpHandlerS>();
        lHandler.ResetUpgrades();
        _earnedTech.Add(5);
        _earnedTech.Add(6);
        PlayerController.equippedTech.Add(5);
        PlayerController.equippedTech.Add(6);
        for (int i = 0; i < buddiesToAddForDemo.Count; i++)
        {
            unlockedBuddies.Add(buddiesToAddForDemo[i].GetComponent<BuddyS>());
            if (i == 0 && pRef != null)
            {
                pRef.equippedBuddies.Add(buddiesToAddForDemo[i]);
            }
        }
        for (int i = 0; i < weaponsToAddForDemo.Count; i++)
        {
            unlockedWeapons.Add(weaponsToAddForDemo[i]);
            if (i == 0 && pRef != null)
            {
                pRef.equippedWeapons.Add(weaponsToAddForDemo[i]);
                pRef.subWeapons.Add(weaponsToAddForDemo[i]);
            }
        }


        PlayerInventoryS.I._earnedVirtues.Add(1);
        PlayerInventoryS.I._earnedVirtues.Add(3);
        PlayerInventoryS.I._earnedVirtues.Add(8);
        PlayerInventoryS.I._earnedVirtues.Add(12);
        PlayerInventoryS.I._earnedVirtues.Add(17);
        PlayerInventoryS.I._earnedVirtues.Add(19);
        PlayerInventoryS.I._earnedVirtues.Add(20);

        _collectedItems = new List<int>(2) { 0, 1 };
        healNums = new List<int>(3) { 0, 1, 2 };
        chargeNums = new List<int>(1) { 0 };
        _collectedItemCount = new List<int>(2) { 3, 1 };
        iManager.equippedInventory.Add(1);
        vpNums = new List<int>(3) { 0, 1, 2 };
    }

    public void AddOpenDoor(int i)
    {
        if (!_openedDoors.Contains(i))
        {
            _openedDoors.Add(i);
        }
    }

    public void RefreshRechargeables()
    {
        if (CheckForItem(0))
        {
            _collectedItemCount[_collectedItems.IndexOf(0)] = healNums.Count;
        }
        if (CheckForItem(1))
        {
            _collectedItemCount[_collectedItems.IndexOf(1)] = chargeNums.Count;
        }
        if (CheckForItem(2))
        {
            _collectedItemCount[_collectedItems.IndexOf(2)] = laPickupNums.Count;
        }
        _iManager.RefreshUI();
    }

    public void SaveLoadout(List<PlayerWeaponS> wepList, List<PlayerWeaponS> subList, List<int> equipBuds)
    {
        equippedWeapons = wepList;
        subWeapons = subList;
        equippedBuddies = new List<GameObject>();
        for (int i = 0; i < equipBuds.Count; i++)
        {
            equippedBuddies.Add(masterLoadoutList.masterBuddyList[equipBuds[i]].gameObject);
        }
    }
    public List<PlayerWeaponS> EquippedWeapons()
    {
        return equippedWeapons;
    }
    public List<PlayerWeaponS> SubWeapons()
    {
        return subWeapons;
    }
    public List<GameObject> EquippedBuddies()
    {
        return equippedBuddies;
    }

    public bool CheckHeal(int n)
    {
        return healNums.Contains(n);
    }
    public bool CheckCharge(int n)
    {
        return chargeNums.Contains(n);
    }
    public bool CheckStim(int n)
    {
        return laPickupNums.Contains(n);
    }
    public bool CheckVP(int v)
    {
        return vpNums.Contains(v);
    }

    public void NewGame(bool newGamePlus = false)
    {
        _dManager.ClearAllSaved();
        _collectedItems.Clear();
        _collectedItemCount.Clear();
        _collectedKeyItems.Clear();
        _clearedWalls.Clear();
        _openedDoors.Clear();

        scenesIveBeenTo.Clear();
        checkpointsReachedScenes.Clear();
        checkpointsReachedSpawns.Clear();

        skippableScenes.Clear();

        if (_iManager && !newGamePlus)
        {
            _iManager.equippedInventory.Clear();
            _iManager.equippedInventory.Add(0);
        }

        if (!newGamePlus)
        {
            CheckpointS.lastSavedTimeDotTime = 0f;
            healNums.Clear();
            chargeNums.Clear();
            vpNums.Clear();
            laPickupNums.Clear();
            if (unlockedWeapons.Count > 1)
            {
                unlockedWeapons.RemoveRange(1, unlockedWeapons.Count - 1);
            }
            if (unlockedBuddies.Count > 1)
            {
                unlockedBuddies.RemoveRange(1, unlockedBuddies.Count - 1);
            }
            SetUpStartTech();
            OverwriteReversionData(true);
        }
        PlayerStatsS.healOnStart = true;
        PlayerStatsS._currentDarkness = 0f;
        if (!newGamePlus)
        {
            PlayerCollectionS.currencyCollected = 0;
        }
        PlayerController._currentParadigm = 0;
        PlayerController.familiarUnlocked = false;
        SpawnPosManager.whereToSpawn = 0;
        GameOverS.revivePosition = 0;
        //CameraShakeS.SetTurbo();
        List<int> buddyList = new List<int>();
        if (!newGamePlus)
        {
            buddyList.Add(unlockedBuddies[0].buddyNum);
            equippedWeapons = new List<PlayerWeaponS> { unlockedWeapons[0] };
            subWeapons = new List<PlayerWeaponS> { unlockedWeapons[0] };
            LevelUpHandlerS lHandler = GetComponent<LevelUpHandlerS>();
            lHandler.ResetUpgrades();
        }
        _tvNum = Mathf.RoundToInt(UnityEngine.Random.Range(100, 999));
        if (unlockForDemo)
        {
            DemoUnlocks(null);
        }
        if (!newGamePlus)
        {
            SaveLoadout(equippedWeapons, subWeapons, buddyList);
            GameMenuS.ResetOptions();
        }
        else
        {
            DarknessPercentUIS.hasReached100 = true;
        }
        OverwriteInventoryData(eraseOnNewGame);

        // i want these to be unlocked forever
        //GameMenuS.unlockedChallenge = false;
        //GameMenuS.unlockedTurbo = false;

        PlayerController.killedFamiliar = false;

        if (newGamePlus)
        {
            CheckpointS.lastSavePointName = "sacrament i.";
        }
        else
        {
            CheckpointS.lastSavePointName = "Abandoned Faith";
        }
        CheckpointS.totalPlayTimeSeconds = 0;
        CheckpointS.totalPlayTimeMinutes = 0;
        CheckpointS.totalPlayTimeHours = 0;

    }

    public void OverwriteReversionData(bool includeDarkness = false){
        dManager.OverwriteReversionData();
        if (includeDarkness)
        {
            _revertDarknessNums = new List<float>();
        }
        _revertedOpenedDoors = new List<int>();
        _revertedClearedWalls = new List<int>();
        _revertedProgressNums = new List<int>();
        _revertedCollectedItems = new List<int>();
        _revertedScenesIveBeenTo = new List<int>();
        _revertedCollectedKeyItems = new List<int>();
        _revertedCollectedItemCount = new List<int>();
        _revertedCheckpointsReachedScenes = new List<int>();
        _revertedCheckpointsReachedSpawns = new List<int>();
    }

    public void LoadNewInventoryData(InventorySave newData)
    {

        inventoryData = newData;
        LoadInventoryData();
    }

    void SetUpStartTech()
    {

        _earnedUpgrades.Clear();
        if (PlayerController.equippedVirtues != null)
        {
            if (!PlayerController.equippedVirtues.Contains(0))
            {


                PlayerController.equippedVirtues.Add(0);

            }
            if (!_earnedVirtues.Contains(0))
            {
                _earnedVirtues.Add(0);
            }
            if (!_earnedVirtues.Contains(15) && PlayerController.equippedVirtues.Contains(15))
            {
                _earnedVirtues.Add(15);
            }
        }
        else
        {
            _earnedVirtues.Clear();

            _earnedVirtues.Add(0);
            PlayerController.equippedVirtues = new List<int>();
            PlayerController.equippedVirtues.Add(0);

        }
        PlayerController.equippedTech = new List<int> { 0, 1, 2, 3, 4 };
        _earnedTech = new List<int> { 0, 1, 2, 3, 4, 10, 11, 12, 13, 14 };
    }

    void LoadInventoryData()
    {
        _earnedUpgrades = inventoryData.earnedUpgrades;
        _collectedItems = inventoryData.collectedItems;
        healNums = inventoryData.healNums;
        vpNums = inventoryData.vpNums;
        laPickupNums = inventoryData.laPickupNums;
        chargeNums = inventoryData.chargeNums;
        _collectedKeyItems = inventoryData.collectedKeyItems;
        _collectedItemCount = inventoryData.collectedItemCount;
        _openedDoors = inventoryData.openedDoors;
        _clearedWalls = inventoryData.clearedWalls;
        _earnedVirtues = inventoryData.earnedVirtues;
        _earnedTech = inventoryData.earnedTech;

        AddEarnedTech(10, false);
        AddEarnedTech(11, false);
        AddEarnedTech(12, false);
        AddEarnedTech(13, false);
        AddEarnedTech(14, false);

        if (inventoryData.skippableScenes != null)
        {
            skippableScenes = inventoryData.skippableScenes;
        }

        scenesIveBeenTo = inventoryData.scenesIveBeenTo;

        _dManager.LoadCombatsCleared(inventoryData.combatClearedAtLeastOnce, inventoryData.combatRankingData, inventoryData.combatRankingGrades, inventoryData.specialCombatsCleared);

        checkpointsReachedScenes = inventoryData.checkpointsReachedScenes;
        checkpointsReachedSpawns = inventoryData.checkpointsReachedSpawns;

        CameraShakeS.SetTurbo(inventoryData.turboSetting);

        TextInputUIS.playerName = inventoryData.playerName;

        unlockedWeapons = new List<PlayerWeaponS>();
        equippedWeapons = new List<PlayerWeaponS>();
        subWeapons = new List<PlayerWeaponS>();
        unlockedBuddies = new List<BuddyS>();
        equippedBuddies = new List<GameObject>();

        for (int i = 0; i < inventoryData.unlockedWeapons.Count; i++)
        {
            unlockedWeapons.Add(masterLoadoutList.masterWeaponList[inventoryData.unlockedWeapons[i]]);
        }
        for (int i = 0; i < inventoryData.equippedWeapons.Count; i++)
        {
            equippedWeapons.Add(masterLoadoutList.masterWeaponList[inventoryData.equippedWeapons[i]]);
        }
        for (int i = 0; i < inventoryData.subWeapons.Count; i++)
        {
            subWeapons.Add(masterLoadoutList.masterWeaponList[inventoryData.subWeapons[i]]);
        }
        for (int i = 0; i < inventoryData.unlockedBuddies.Count; i++)
        {
            unlockedBuddies.Add(masterLoadoutList.masterBuddyList[inventoryData.unlockedBuddies[i]]);
        }
        for (int i = 0; i < inventoryData.equippedBuddies.Count; i++)
        {
            equippedBuddies.Add(masterLoadoutList.masterBuddyList[inventoryData.equippedBuddies[i]].gameObject);
        }

        PlayerController._currentParadigm = inventoryData.currentParadigm;
        if (inventoryData.familiarUnlocked != null)
        {
            PlayerController.familiarUnlocked = inventoryData.familiarUnlocked;
        }

        // make sure marked saves/loads appropriately
        if (PlayerController.equippedVirtues != null)
        {
            if (PlayerController.equippedVirtues.Contains(15) && !inventoryData.equippedVirtues.Contains(15))
            {
                inventoryData.equippedVirtues.Add(15);
            }
            if (PlayerController.equippedVirtues.Contains(15) && !inventoryData.earnedVirtues.Contains(15)) { inventoryData.earnedVirtues.Add(15); }
            if (PlayerController.equippedVirtues.Contains(15) && !_earnedVirtues.Contains(15)) { _earnedVirtues.Add(15); }
        }
        PlayerController.equippedVirtues = inventoryData.equippedVirtues;
        PlayerController.equippedTech = inventoryData.equippedTech;

        LevelUpHandlerS lHandler = GetComponent<LevelUpHandlerS>();
        List<LevelUpS> nLU = new List<LevelUpS>();
        List<LevelUpS> aLU = new List<LevelUpS>();
        List<LockedLevelUpS> lLU = new List<LockedLevelUpS>();
        for (int i = 0; i < inventoryData.nextLevelUpgrades.Count; i++)
        {
            nLU.Add(masterLoadoutList.levelUpList[inventoryData.nextLevelUpgrades[i]]);
        }
        for (int i = 0; i < inventoryData.availableUpgrades.Count; i++)
        {
            aLU.Add(masterLoadoutList.levelUpList[inventoryData.availableUpgrades[i]]);
        }
        for (int i = 0; i < inventoryData.lockedUpgrades.Count; i++)
        {
            lLU.Add(masterLoadoutList.lockedUpList[inventoryData.lockedUpgrades[i]]);
        }
        lHandler.LoadLists(nLU, aLU, lLU);

        if (inventoryData.tvNumber != null)
        {
            if (inventoryData.tvNumber < 100 || inventoryData.tvNumber == 999)
            {
                _tvNum = Mathf.FloorToInt(UnityEngine.Random.Range(100, 999));
                inventoryData.tvNumber = _tvNum;
            }
            else
            {
                _tvNum = inventoryData.tvNumber;
            }
        }
        else
        {
            _tvNum = Mathf.FloorToInt(UnityEngine.Random.Range(100, 999));
            inventoryData.tvNumber = _tvNum;
        }

        DarknessPercentUIS.hasReached100 = inventoryData.hasReached100;

        GameOverS.revivePosition = inventoryData.currentSpawnPoint;
        RefreshRechargeables();

        DifficultyS.SetDifficultiesFromInt(inventoryData.sinLevel, inventoryData.punishLevel);

        _iManager.LoadInventory(inventoryData.equippedInventory, inventoryData.currentSelection);
        _iManager.RefreshUI();

        GameMenuS.unlockedChallenge = inventoryData.unlockedChallenge;
        GameMenuS.unlockedTurbo = inventoryData.unlockedTurbo;
        PlayerController.killedFamiliar = inventoryData.killedFamiliar;

        ControlManagerS.savedKeyboardControls = inventoryData.savedKeyboardControls;
        ControlManagerS.savedGamepadControls = inventoryData.savedGamepadControls;
        ControlManagerS.savedKeyboardandMouseControls = inventoryData.savedMouseControls;

        CheckpointS.lastSavePointName = inventoryData.lastSavePointName;
        CheckpointS.totalPlayTimeSeconds = inventoryData.totalPlayTimeSeconds;
        CheckpointS.totalPlayTimeMinutes = inventoryData.totalPlayTimeMinutes;
        CheckpointS.totalPlayTimeHours = inventoryData.totalPlayTimeHours;

        ChapterTrigger.lastChapterTriggered = inventoryData.lastChapterName;

        CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = inventoryData.savedCameraShake;
        if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER > 1f)
        {
            CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 1f;
        }
        else if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER < 0)
        {
            CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 0;
        }

        // load reversion properties (local data = saved data) 
        if (inventoryData.revertedCollectedItems != null)
        {
            _revertedCollectedItems = inventoryData.revertedCollectedItems;
        }
        if (inventoryData.revertedCollectedKeyItems != null)
        {
            _revertedCollectedKeyItems = inventoryData.revertedCollectedKeyItems;
        }
        if (inventoryData.revertedCollectedItemCount != null)
        {
            _revertedCollectedItemCount = inventoryData.revertedCollectedItemCount;
        }
        if (inventoryData.revertedOpenedDoors != null)
        {
            _revertedOpenedDoors = inventoryData.revertedOpenedDoors;
        }
        if (inventoryData.revertedClearedWalls != null)
        {
            _revertedClearedWalls = inventoryData.revertedClearedWalls;
        }
        if (inventoryData.revertedCombatClearedAtLeastOnce != null)
        {
            _dManager.OverwriteRevertedCombatClearedAtLeastOnce(inventoryData.revertedCombatClearedAtLeastOnce);
        }
        if (inventoryData.revertedCombatRankingData != null)
        {
            _dManager.OverwriteRevertedCombatClearedRanks(inventoryData.revertedCombatRankingData);
        }
        if (inventoryData.revertedCombatRankingGrades != null)
        {
            _dManager.OverwriteRevertedCombatClearedGrades(inventoryData.revertedCombatRankingGrades);
        }
        if (inventoryData.revertedSpecialCombatsCleared != null)
        {
            _dManager.OverwriteSpecialConditionCombatCleared(inventoryData.revertedSpecialCombatsCleared);
        }

        if (inventoryData.revertDarknessNums != null)
        {
            _revertDarknessNums = inventoryData.revertDarknessNums;
        }

        if (inventoryData.revertedScenesIveBeenTo != null)
        {
            _revertedScenesIveBeenTo = inventoryData.revertedScenesIveBeenTo;
        }
        if (inventoryData.revertedCheckpointsReachedScenes != null)
        {
            _revertedCheckpointsReachedScenes = inventoryData.revertedCheckpointsReachedScenes;
        }
        if (inventoryData.revertedCheckpointsReachedSpawns != null)
        {
            _revertedCheckpointsReachedSpawns = inventoryData.revertedCheckpointsReachedSpawns;
        }
        if (inventoryData.revertedProgressNums != null)
        {
            _revertedProgressNums = inventoryData.revertedProgressNums;
        }
    }

    public void OverwriteInventoryData(bool erase = false, bool newGamePlus = false)
    {

        if (erase)
        {
            inventoryData = null;
        }
        else
        {
            inventoryData = new InventorySave();

            if (initialized)
            {
                // make sure marked saves/loads appropriately
                if (PlayerController.equippedVirtues != null)
                {
                    if (PlayerController.equippedVirtues.Contains(15) && !_earnedVirtues.Contains(15))
                    {
                        _earnedVirtues.Add(15);
                    }
                }
                inventoryData.playerName = TextInputUIS.playerName;
                inventoryData.earnedUpgrades = _earnedUpgrades;
                inventoryData.collectedItems = _collectedItems;
                inventoryData.healNums = healNums;
                inventoryData.laPickupNums = laPickupNums;
                inventoryData.chargeNums = chargeNums;
                inventoryData.vpNums = vpNums;
                inventoryData.collectedKeyItems = _collectedKeyItems;
                inventoryData.collectedItemCount = _collectedItemCount;
                inventoryData.openedDoors = _openedDoors;
                inventoryData.clearedWalls = _clearedWalls;
                inventoryData.earnedVirtues = _earnedVirtues;
                inventoryData.earnedTech = _earnedTech;

                inventoryData.scenesIveBeenTo = scenesIveBeenTo;
                inventoryData.checkpointsReachedScenes = checkpointsReachedScenes;
                inventoryData.checkpointsReachedSpawns = checkpointsReachedSpawns;
                inventoryData.currentSpawnPoint = GameOverS.revivePosition;

                inventoryData.skippableScenes = skippableScenes;

                if (_dManager.combatClearedAtLeastOnce != null)
                {
                    inventoryData.combatClearedAtLeastOnce = _dManager.combatClearedAtLeastOnce;
                    inventoryData.combatRankingData = _dManager.combatClearedRanks;
                    inventoryData.combatRankingGrades = _dManager.combatClearedRankGrades;
                }
                if (_dManager.specialConditionCombatCleared != null)
                {
                    inventoryData.specialCombatsCleared = _dManager.specialConditionCombatCleared;
                }


                for (int i = 0; i < unlockedWeapons.Count; i++)
                {
                    inventoryData.unlockedWeapons.Add(unlockedWeapons[i].weaponNum);
                }

                for (int i = 0; i < unlockedBuddies.Count; i++)
                {
                    inventoryData.unlockedBuddies.Add(unlockedBuddies[i].buddyNum);
                }
                for (int i = 0; i < equippedWeapons.Count; i++)
                {
                    inventoryData.equippedWeapons.Add(equippedWeapons[i].weaponNum);
                }
                for (int i = 0; i < subWeapons.Count; i++)
                {
                    inventoryData.subWeapons.Add(subWeapons[i].weaponNum);
                }

                inventoryData.equippedVirtues = PlayerController.equippedVirtues;
                inventoryData.equippedTech = PlayerController.equippedTech;

                for (int i = 0; i < equippedBuddies.Count; i++)
                {
                    inventoryData.equippedBuddies.Add(equippedBuddies[i].GetComponent<BuddyS>().buddyNum);
                }

                inventoryData.equippedInventory = _iManager.equippedInventory;
                inventoryData.currentSelection = _iManager.currentSelection;

                LevelUpHandlerS lHandler = GetComponent<LevelUpHandlerS>();
                inventoryData.availableUpgrades = new List<int>();
                for (int i = 0; i < lHandler.availableLevelUps.Count; i++)
                {
                    inventoryData.availableUpgrades.Add(lHandler.availableLevelUps[i].upgradeID);
                }
                inventoryData.nextLevelUpgrades = new List<int>();
                for (int i = 0; i < lHandler.nextLevelUps.Count; i++)
                {
                    inventoryData.nextLevelUpgrades.Add(lHandler.nextLevelUps[i].upgradeID);
                }
                inventoryData.lockedUpgrades = new List<int>();
                for (int i = 0; i < lHandler.lockedLevelUps.Count; i++)
                {
                    inventoryData.lockedUpgrades.Add(lHandler.lockedLevelUps[i].lockedUpgradeID);
                }

                inventoryData.familiarUnlocked = PlayerController.familiarUnlocked;
                inventoryData.currentParadigm = PlayerController._currentParadigm;

                inventoryData.sinLevel = DifficultyS.GetSinInt();
                inventoryData.punishLevel = DifficultyS.GetPunishInt();

                inventoryData.hasReached100 = DarknessPercentUIS.hasReached100;

                inventoryData.turboSetting = CameraShakeS.GetTurboInt();

                inventoryData.lastChapterName = ChapterTrigger.lastChapterTriggered;

                if (_tvNum < 100 || _tvNum == 999)
                {
                    _tvNum = Mathf.FloorToInt(UnityEngine.Random.Range(100, 999));
                }
                inventoryData.tvNumber = _tvNum;

                inventoryData.unlockedChallenge = GameMenuS.unlockedChallenge;
                inventoryData.unlockedTurbo = GameMenuS.unlockedTurbo;
                inventoryData.killedFamiliar = PlayerController.killedFamiliar;

                inventoryData.savedKeyboardControls = ControlManagerS.savedKeyboardControls;
                inventoryData.savedGamepadControls = ControlManagerS.savedGamepadControls;
                inventoryData.savedMouseControls = ControlManagerS.savedKeyboardandMouseControls;

                inventoryData.lastSavePointName = CheckpointS.lastSavePointName;
                inventoryData.totalPlayTimeSeconds = CheckpointS.totalPlayTimeSeconds;
                inventoryData.totalPlayTimeMinutes = CheckpointS.totalPlayTimeMinutes;
                inventoryData.totalPlayTimeHours = CheckpointS.totalPlayTimeHours;

                inventoryData.savedCameraShake = CameraShakeS.OPTIONS_SHAKE_MULTIPLIER;

                // overwrite reversion properties (saved data = local data) 
                if (_revertedCollectedItems != null)
                {
                    inventoryData.revertedCollectedItems = _revertedCollectedItems;
                }
                if (_revertedCollectedKeyItems != null)
                {
                    inventoryData.revertedCollectedKeyItems = _revertedCollectedKeyItems;
                }
                if (_revertedCollectedItemCount != null)
                {
                    inventoryData.revertedCollectedItemCount = _revertedCollectedItemCount;
                }
                if (_revertedOpenedDoors != null)
                {
                    inventoryData.revertedOpenedDoors = _revertedOpenedDoors;
                }
                if (_revertedClearedWalls != null)
                {
                    inventoryData.revertedClearedWalls = _revertedClearedWalls;
                }
                if (_dManager.revertedCombatClearedAtLeastOnce != null)
                {
                    inventoryData.revertedCombatClearedAtLeastOnce = _dManager.revertedCombatClearedAtLeastOnce;
                }
                if (_dManager.revertedCombatClearedRanks != null)
                {
                    inventoryData.revertedCombatRankingData = _dManager.revertedCombatClearedRanks;
                }
                if (_dManager.revertedCombatClearedRankGrades != null)
                {
                    inventoryData.revertedCombatRankingGrades = _dManager.revertedCombatClearedRankGrades;
                }
                if (_dManager.revertedSpecialConditionCombatCleared != null)
                {
                    inventoryData.revertedSpecialCombatsCleared = _dManager.revertedSpecialConditionCombatCleared;
                }

                if (_revertDarknessNums != null)
                {
                    inventoryData.revertDarknessNums = _revertDarknessNums;
                }

                if (_revertedScenesIveBeenTo != null)
                {
                    inventoryData.revertedScenesIveBeenTo = _revertedScenesIveBeenTo;
                }
                if (_revertedCheckpointsReachedScenes != null)
                {
                    inventoryData.revertedCheckpointsReachedScenes = _revertedCheckpointsReachedScenes;
                }
                if (_revertedCheckpointsReachedSpawns != null)
                {
                    inventoryData.revertedCheckpointsReachedSpawns = _revertedCheckpointsReachedSpawns;
                }
                if (_revertedProgressNums != null)
                {
                    inventoryData.revertedProgressNums = _revertedProgressNums;
                }
            }
        }
    }

    public void MergeRevertedData(){

        // add back in reverted items
        if (_revertedCollectedItems != null){
            for (int i = _revertedCollectedItems.Count-1; i > -1; i--){
                if (!_collectedItems.Contains(_revertedCollectedItems[i]))
                {
                    _collectedItems.Add(_revertedCollectedItems[i]);
                    _collectedItemCount.Add(_revertedCollectedItemCount[i]);
                    if (_revertedCollectedKeyItems.Contains(_revertedCollectedItems[i])){
                        _collectedKeyItems.Add(_revertedCollectedItems[i]);
                        _revertedCollectedKeyItems.RemoveAt(_revertedCollectedKeyItems.IndexOf(_revertedCollectedItems[i]));
                    }
                    _revertedCollectedItems.RemoveAt(i);
                    _revertedCollectedItemCount.RemoveAt(i);
                }
            }
            _revertedCollectedItems.Clear();
            _revertedCollectedKeyItems.Clear();
            _revertedCollectedItemCount.Clear();
        }

        // add back in reverted doors
        if (_revertedOpenedDoors != null)
        {
            for (int i = _revertedOpenedDoors.Count-1; i > -1; i--)
            {
                if (!_openedDoors.Contains(_revertedOpenedDoors[i]))
                {
                    _openedDoors.Add(_revertedOpenedDoors[i]);

                    _revertedOpenedDoors.RemoveAt(i);
                }
            }
            _revertedOpenedDoors.Clear();
        }

        // add back in reverted walls
        if (_revertedClearedWalls != null)
        {
            for (int i = _revertedClearedWalls.Count - 1; i > -1; i--)
            {
                if (!_clearedWalls.Contains(_revertedClearedWalls[i]))
                {
                    _clearedWalls.Add(_revertedClearedWalls[i]);

                    _revertedClearedWalls.RemoveAt(i);
                }
            }
            _revertedClearedWalls.Clear();
        }

        // add back in visited scenes
        if (_revertedScenesIveBeenTo != null)
        {
            for (int i = _revertedScenesIveBeenTo.Count - 1; i > -1; i--)
            {
                if (!scenesIveBeenTo.Contains(_revertedScenesIveBeenTo[i]))
                {
                    scenesIveBeenTo.Add(_revertedScenesIveBeenTo[i]);

                    _revertedScenesIveBeenTo.RemoveAt(i);
                }
            }
            _revertedScenesIveBeenTo.Clear();
        }
        if (_revertedCheckpointsReachedScenes != null)
        {
            for (int i = _revertedCheckpointsReachedScenes.Count - 1; i > -1; i--)
            {
                if (!checkpointsReachedScenes.Contains(_revertedCheckpointsReachedScenes[i]))
                {
                    checkpointsReachedScenes.Add(_revertedCheckpointsReachedScenes[i]);
                    checkpointsReachedSpawns.Add(_revertedCheckpointsReachedSpawns[i]);

                    _revertedCheckpointsReachedScenes.RemoveAt(i);
                    _revertedCheckpointsReachedSpawns.RemoveAt(i);
                }
            }
            _revertedCheckpointsReachedScenes.Clear();
            _revertedCheckpointsReachedSpawns.Clear();
        }

        // add back in progress nums
        if (_revertedProgressNums != null){
            for (int i = _revertedProgressNums.Count - 1; i > -1; i--){
                if (!StoryProgressionS.storyProgress.Contains(_revertedProgressNums[i])){
                    StoryProgressionS.storyProgress.Add(_revertedProgressNums[i]);
                    _revertedProgressNums.RemoveAt(i);
                }
            }
        _revertedProgressNums.Clear();
        }


        // call destruction manager to do the combat reversions
        _dManager.MergeRevertedData();
    }

    public void RevertDataForChapter(TrackRevertProgressScriptableObject trackRevert){

        int numToRevert = -1;
        for (int i = 0; i < trackRevert.revertProgressNums.Count; i++)
        {
            if (StoryProgressionS.storyProgress.Contains(trackRevert.revertProgressNums[i]))
            {
                numToRevert = StoryProgressionS.storyProgress.IndexOf(trackRevert.revertProgressNums[i]);
                _revertedProgressNums.Add(StoryProgressionS.storyProgress[numToRevert]);
                StoryProgressionS.storyProgress.RemoveAt(numToRevert);
            }
        }
        for (int i = 0; i < trackRevert.revertItemsFound.Count; i++)
        {
            if (_collectedItems.Contains(trackRevert.revertItemsFound[i]))
            {
                numToRevert = _collectedItems.IndexOf(trackRevert.revertItemsFound[i]);
                _revertedCollectedItems.Add(_collectedItems[numToRevert]);
                _revertedCollectedItemCount.Add(_collectedItemCount[numToRevert]);

                _collectedItems.RemoveAt(numToRevert);
                _collectedItemCount.RemoveAt(numToRevert);

                if (_collectedKeyItems.Contains(trackRevert.revertItemsFound[i])){
                    _revertedCollectedKeyItems.Add(trackRevert.revertItemsFound[i]);
                    _collectedKeyItems.RemoveAt(_collectedKeyItems.IndexOf(trackRevert.revertItemsFound[i]));
                }
            }
        }
        for (int i = 0; i < trackRevert.revertClearedWalls.Count; i++)
        {
            if (_clearedWalls.Contains(trackRevert.revertClearedWalls[i]))
            {
                numToRevert = _clearedWalls.IndexOf(trackRevert.revertClearedWalls[i]);
                _revertedProgressNums.Add(_clearedWalls[numToRevert]);
                _clearedWalls.RemoveAt(numToRevert);
            }
        }
        for (int i = 0; i < trackRevert.revertOpenedDoors.Count; i++)
        {
            if (_openedDoors.Contains(trackRevert.revertOpenedDoors[i]))
            {
                numToRevert = _openedDoors.IndexOf(trackRevert.revertOpenedDoors[i]);
                _revertedOpenedDoors.Add(_openedDoors[numToRevert]);
                _openedDoors.RemoveAt(numToRevert);
            }
        }

        for (int i = 0; i < trackRevert.revertSceneNums.Count; i++)
        {
            if (scenesIveBeenTo.Contains(trackRevert.revertSceneNums[i]))
            {
                numToRevert = scenesIveBeenTo.IndexOf(trackRevert.revertSceneNums[i]);
                _revertedScenesIveBeenTo.Add(scenesIveBeenTo[numToRevert]);
                scenesIveBeenTo.RemoveAt(numToRevert);

            }

                if (checkpointsReachedScenes.Contains(trackRevert.revertSceneNums[i]))
            {numToRevert = checkpointsReachedScenes.IndexOf(trackRevert.revertSceneNums[i]);
                _revertedCheckpointsReachedScenes.Add(checkpointsReachedScenes[numToRevert]);
                _revertedCheckpointsReachedSpawns.Add(checkpointsReachedSpawns[numToRevert]);
                checkpointsReachedScenes.RemoveAt(numToRevert);
                checkpointsReachedSpawns.RemoveAt(numToRevert);
            }
        }
        _dManager.RevertCombatData(trackRevert.revertSceneNums);
    }

}

[System.Serializable]
public class InventorySave {
	public List<int> earnedUpgrades;
	public List<int> earnedVirtues;
	public List<int> earnedTech;
	public List<int> collectedItems;
	public List<int> healNums;
	public List<int> laPickupNums;
	public List<int> chargeNums;
	public List<int> vpNums;
	public List<int> collectedKeyItems;
	public List<int> collectedItemCount;
	public List<int> openedDoors;
	public List<int> clearedWalls;

    /// reverted properties (to keep track when jumping around new game +)
    public List<int> revertedCollectedItems;
    public List<int> revertedCollectedKeyItems;
    public List<int> revertedCollectedItemCount;
    public List<int> revertedOpenedDoors;
    public List<int> revertedClearedWalls;
    public List<int> revertedCombatClearedAtLeastOnce;
    public List<int> revertedCombatRankingData;
    public List<string> revertedCombatRankingGrades;
    public List<int> revertedSpecialCombatsCleared;

	public List<int> combatClearedAtLeastOnce;
	public List<int> combatRankingData;
	public List<string> combatRankingGrades;
	public List<int> specialCombatsCleared;
	
	public List<int> unlockedWeapons;
	public List<int> unlockedBuddies;
	public List<int> equippedWeapons;
	public List<int> subWeapons;

	public List<int> equippedVirtues;
	public List<int> equippedTech;
	
	public List<int> equippedBuddies;
	public int currentParadigm;
	public bool familiarUnlocked = false;

	public List<int> equippedInventory;
	public int currentSelection = 0;

	public List<int> nextLevelUpgrades;
	public List<int> availableUpgrades;
	public List<int> lockedUpgrades;


	public List<int> scenesIveBeenTo;
	public List<int> checkpointsReachedSpawns;
	public List<int> checkpointsReachedScenes;

    public List<int> revertedScenesIveBeenTo;
    public List<int> revertedCheckpointsReachedSpawns;
    public List<int> revertedCheckpointsReachedScenes;
	public int currentSpawnPoint;

    public List<int> revertedProgressNums;

    public List<float> revertDarknessNums;

	public int turboSetting = 0;
	public int tvNumber;

	public int sinLevel;
	public int punishLevel;

	public string playerName;

	public bool hasReached100 = false;
    public List<int> skippableScenes;

    public bool killedFamiliar = false;
    public bool unlockedChallenge = false;
    public bool unlockedTurbo = false;
    public string lastChapterName = "Track -1 ~ Cradle";
    public string lastSavePointName = "Abandoned Faith";

    // saved Options
    public List<int> savedGamepadControls = new List<int>();
    public List<int> savedKeyboardControls = new List<int>();
    public List<int> savedMouseControls = new List<int>();

    public int totalPlayTimeSeconds = 0;
    public int totalPlayTimeMinutes = 0;
    public int totalPlayTimeHours = 0;

    public float savedCameraShake = 1f;

	public InventorySave(){
		playerName = "LUCAH";
		earnedUpgrades = new List<int>();
		earnedVirtues = new List<int>();
		earnedTech = new List<int>();
		collectedItems = new List<int>();
		healNums = new List<int>();
		laPickupNums = new List<int>();
		chargeNums = new List<int>();
		collectedKeyItems = new List<int>();
		collectedItemCount = new List<int>();
		openedDoors = new List<int>();
		clearedWalls = new List<int>();

		combatClearedAtLeastOnce = new List<int>();
		combatRankingData = new List<int>();
		combatRankingGrades = new List<string>();
		specialCombatsCleared = new List<int>();

        // reversion properties
        revertedCollectedItems = new List<int>();
        revertedCollectedKeyItems = new List<int>();
        revertedCollectedItemCount = new List<int>();
        revertedOpenedDoors = new List<int>();
        revertedClearedWalls = new List<int>();
        revertedCombatClearedAtLeastOnce = new List<int>();
        revertedCombatRankingData = new List<int>();
        revertedCombatRankingGrades = new List<string>();
        revertedSpecialCombatsCleared = new List<int>();

        revertedProgressNums = new List<int>();

        revertDarknessNums = new List<float>(1) { 0 };

		scenesIveBeenTo = new List<int>();
		checkpointsReachedScenes = new List<int>();
		checkpointsReachedSpawns = new List<int>();


        revertedScenesIveBeenTo = new List<int>();
        revertedCheckpointsReachedScenes = new List<int>();
        revertedCheckpointsReachedSpawns = new List<int>();

		unlockedWeapons = new List<int>();
		unlockedBuddies = new List<int>();
		equippedWeapons = new List<int>();
		subWeapons = new List<int>();
		
		equippedVirtues = new List<int>();
		equippedTech = new List<int>();
		
		equippedBuddies = new List<int>();

		currentSelection = 0;
		equippedInventory = new List<int>();
		currentParadigm = 0;
		currentSpawnPoint = 0;

        skippableScenes = new List<int>();

		availableUpgrades = new List<int>(){0,1,2,6};
		nextLevelUpgrades = new List<int>(){4,5,3};
		lockedUpgrades = new List<int>(){0,1, 2};
		familiarUnlocked = false;

		tvNumber = Mathf.FloorToInt(UnityEngine.Random.Range(100, 999));

		turboSetting = 0;

		punishLevel = 1;
		sinLevel = 1;

		hasReached100 = false;

        unlockedChallenge = false;
        unlockedTurbo = false;
        killedFamiliar = false;

            savedKeyboardControls = new List<int>(14) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        savedGamepadControls = new List<int>(14) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        savedMouseControls = new List<int>(14) { 14, 15, 16, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

       lastSavePointName = "Abandoned Faith";
        totalPlayTimeSeconds = 0;
    totalPlayTimeMinutes = 0;
        totalPlayTimeHours = 0;
    lastChapterName = "Track -1 ~ Cradle";

        savedCameraShake = 1f;

	}
}
