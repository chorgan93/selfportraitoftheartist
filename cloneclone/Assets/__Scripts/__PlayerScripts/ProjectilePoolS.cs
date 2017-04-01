using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectilePoolS : MonoBehaviour {

	private List<ProjectileS> allSavedProjectiles = new List<ProjectileS>();
	public List<ProjectileS> projectilePool { get { return allSavedProjectiles; } }

	public void AddProjectile(ProjectileS newP){
		allSavedProjectiles.Add(newP);
		newP.gameObject.SetActive(false);
	}

	public bool ContainsProjectileID(int idCheck){
		bool hasID = false;
		for (int i = 0; i < allSavedProjectiles.Count; i++){
			if (allSavedProjectiles[i].projectileID == idCheck){
				hasID = true;
			}
		}
		return hasID;
	}

	public ProjectileS GetProjectile(int idCheck, Vector3 spawnPos, Quaternion spawnRot){
		ProjectileS returnProjectile = null;
		int projectileNum = -1;
		for (int i = 0; i < allSavedProjectiles.Count; i++){
			if (allSavedProjectiles[i].projectileID == idCheck && projectileNum == -1){
				returnProjectile = allSavedProjectiles[i];
				returnProjectile.gameObject.SetActive(true);
				projectileNum = i;
			}
		}
		allSavedProjectiles.RemoveAt(projectileNum);
		returnProjectile.transform.position = spawnPos;
		returnProjectile.transform.rotation = spawnRot;
		return returnProjectile;
	}
}
