using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Track Revert ScriptableObject", menuName = "melessthanthree/Create Track Revert ScriptableObject", order = 1)]
public class TrackRevertProgressScriptableObject : ScriptableObject {

    public int trackNum = 0;
    public string revertSceneName = "Area00_03_Camp";
    public int revertSceneNameLoadNum = 0;
    public string chapterName;
    public string lastCheckpointName;
    public List<int> revertProgressNums;
    public List<int> revertItemsFound; // also handles key items and item count (everything except mantras, virtues, beads, etc)
    public List<int> revertCombatNums; // also needs to revert special conditions cleared, saved scores, and grades (except for trainer matches)
    public List<int> revertClearedWalls;
    public List<int> revertOpenedDoors;
    public List<int> revertSceneNums;

}
