﻿using UnityEngine;
using System.Collections;

public class CameraShakeS : MonoBehaviour {

	//_______________________________________CLASS VARIABLES

	private float 				_smallShakeIntensity = 0.22f;
	private float 				_smallShakeDuration = 0.2f;

	private float 				_largeShakeIntensity = 1.2f;
	private float 				_largeShakeDuration = 0.38f;

	private float 				_delayShakeTime;

	private float 				_smallSleepTime = 0.06f;
	private float 				_bigSleepTime = 0.14f;

	private float 				_startDeathTimeScale = .3f;
	private float 				_addDeathTimeScale = 0f;
	private float 				_delayTimeIncreaseMax = 0.8f;
	private float 				_delayTimeIncreaseCountdown;
	private float 				_increaseTimeRate = 0.5f;

	private int					_shakePriority = 0;

	public static float 		OPTIONS_SHAKE_MULTIPLIER = 1f;

	public static float 		turboMultiplier = 0.91f;
	private float turboOff = 0.91f;
	private float turboOn = 1f;
	private float superTurbo = 1.09f;

	private float debugScale = 0.91f;
	

	//_______________________________________INSTANCE VARIABLES

	
	private Vector3				_originPosition;
	private float				_shake_intensity;
	private float				_shake_decay;

	private bool				_isShaking;
	private Vector3				_shakeOffset;

	private bool 				_isSleeping;
	private float 				_prevMult;

	private float 				_sleepTimeAmount;

	private float 				_slowTimeAmount;
	private bool				_sloMoOn;
	private bool 				_dodgeSloMoStart;
	private float 				_dodgeSloTime;
	private bool 				dodgeSloMo;
	private bool 				doDeathTime = false;

	private bool 				timePaused = false;
	private float 				capturedTimeScale = 1f;

	[HideInInspector]
	public bool lockXShake = false;

	//_________________________________________________GETTERS AND SETTERS

	public bool	isShaking				{ get { return _isShaking; } }
	public bool isSleeping				{ get { return _isSleeping; } }
	public bool isSloMo					{ get { return _sloMoOn; } }

	public Vector3 shakeOffset			{ get { return _shakeOffset; } }


	//____________________________________SINGLETON

	public static CameraShakeS C;

	//_____________________________________UNITY METHODS

	void Awake () {

		C = this;

		#if UNITY_EDITOR || UNITY_EDITOR_OSX
		Time.timeScale = turboMultiplier = debugScale;
		#else
			Time.timeScale = turboMultiplier;
		#endif

	}

	void Update () {

		if (!timePaused){
			ExecuteSleep();
			ExecuteShake();
		}

	}
	


	//_________________________________________PRIVATE METHODS

	private void StartShake ( float newIntensity, float newDuration, int priority){

		if ( _shakePriority <= priority){

			_shake_intensity = newIntensity;
			_shake_decay = _shake_intensity/newDuration;
	
			_isShaking = true;

			_shakePriority = priority;
		}

	}

	private void ExecuteSleep (){

		if (_isSleeping){
	
			Time.timeScale = 0f;

			_sleepTimeAmount -= Time.unscaledDeltaTime;
			if (_sleepTimeAmount <= 0){
				_isSleeping = false;
			}
		}else if (doDeathTime){

			Time.timeScale = _startDeathTimeScale+_addDeathTimeScale;

			_delayTimeIncreaseCountdown -= Time.unscaledDeltaTime;
			if (_delayTimeIncreaseCountdown <= 0){
				_addDeathTimeScale += _increaseTimeRate*Time.unscaledDeltaTime;
			}

			if (Time.timeScale >= turboMultiplier){
				Time.timeScale = turboMultiplier;
				doDeathTime = false;
			}

		}
		else{
			if (_sloMoOn){

				if (_dodgeSloMoStart){
					Time.timeScale = 0.4f*turboMultiplier;
					_dodgeSloTime -= Time.unscaledDeltaTime;
					if (_dodgeSloTime <= 0){
						_dodgeSloMoStart = false;
					}
				}
				else if (dodgeSloMo){
					Time.timeScale = 0.7f*turboMultiplier;
					_slowTimeAmount -= Time.unscaledDeltaTime;
				}else{
					Time.timeScale = 0.5f*turboMultiplier;
					_slowTimeAmount -= Time.unscaledDeltaTime;
				}

				if (_slowTimeAmount <= 0){
					_sloMoOn = false;
				}
			}
			else{
				Time.timeScale = turboMultiplier;
			}
		}

	}

	private void ExecuteShake (){

		if (_isShaking && (!doDeathTime || (doDeathTime && !_isSleeping))){

			if (_delayShakeTime > 0){
				_delayShakeTime -= Time.unscaledDeltaTime;
			}
			else{
				_shakeOffset = Random.insideUnitSphere*_shake_intensity*OPTIONS_SHAKE_MULTIPLIER;
				if (lockXShake){
					_shakeOffset.x = 0f;
				}
				_shakeOffset.y *= 0.7f;
				_shakeOffset.z = 0;
	
				if (doDeathTime){
					_shake_intensity -= _shake_decay * Time.deltaTime;
				}else{
					_shake_intensity -= _shake_decay * Time.unscaledDeltaTime;
				}
	
				if (_shake_intensity <= 0){
					EndShake();
				}
			}

		}

	}

	private void EndShake () {

		_isShaking = false;
		_shakePriority = -1;

	}

	private void EndSleep(){

		_isSleeping = false;

	}

	//_________________________________________SHAKE METHODS

	public void ShootShake(){
		
		StartShake (_smallShakeIntensity/2f, _smallShakeDuration/2f, 0);
		
	}

	public void MicroShake(float shakeAlt = 1f){
		
		StartShake (_smallShakeIntensity/2f*shakeAlt, _smallShakeDuration/2f, 1);
		
	}

	public void SmallShake(){

		StartShake (_smallShakeIntensity, _smallShakeDuration, 2);

	}

	public void SmallShakeCustomDuration(float newDuration){

		StartShake (_smallShakeIntensity, newDuration, 2);

	}

	public void LargeShake(){
		
		StartShake (_largeShakeIntensity, _largeShakeDuration, 3);
		
	}

	public void CancelSloMo(){
		_dodgeSloMoStart = dodgeSloMo = _sloMoOn = false;
		_slowTimeAmount = _dodgeSloTime = 0f;
	}

	public void SpecialAttackShake(){
		
		StartShake (_largeShakeIntensity*0.75f, _largeShakeDuration*0.5f, 3);
		
	}

	public void LargeShakeCustomDuration(float newDuration){
		
		StartShake (_largeShakeIntensity, newDuration, 3);
		
	}

	//_________________________________________SLEEP METHODS

	public void TimeSleep(float sleepTime, bool doPunch = false){

		if (_isSleeping){
			_sleepTimeAmount += sleepTime/2f;
		}
		else{
			_sleepTimeAmount += sleepTime;
		}

			_isSleeping = true;
				
			Time.timeScale = 0;
	
	
			if (doPunch){
				GetComponent<CameraFollowS>().PunchIn();
			}


	}

	public void TimeSleepEndCombat(float sleepTime){
		
		if (_isSleeping){
			_sleepTimeAmount += sleepTime/2f;
		}
		else{
			_sleepTimeAmount += sleepTime;
		}
		
		_isSleeping = true;
		
		Time.timeScale = 0;
		

		GetComponent<CameraFollowS>().PunchInCustom(0.56f, 0.1f);

		
		
	}

	public void TimeSleepBigPunch(float sleepTime){

		
			_isSleeping = true;
		
			Time.timeScale = 0;
			
			_sleepTimeAmount += sleepTime;
			
	
			GetComponent<CameraFollowS>().PunchInBig();

		
		
	}

	public void DodgeSloMo(float slowTime, float sleepTime, float punchMult, float punchHangTime){
		
		_dodgeSloMoStart = dodgeSloMo = true;
		_dodgeSloTime = sleepTime;
		SloMo(slowTime);
		GetComponent<CameraFollowS>().PunchInCustom(punchMult, punchHangTime);
	}

	public void SloAndPunch(float slowTime, float punchMult, float hangTime){
		dodgeSloMo = true;
		SloMo(slowTime);
		GetComponent<CameraFollowS>().PunchInCustom(punchMult, hangTime);
	}

	public void SloMo(float slowTime){

		_slowTimeAmount = slowTime;
		_sloMoOn = true;

	}

	public void SmallSleep(){

		TimeSleep(_smallSleepTime);

	}



	public void BigSleep(bool doPunch = false){
		
		TimeSleep(_bigSleepTime, doPunch);
		
	}

	public void DeathTimeEffect(){
		CameraEffectsS.E.VignetteDeathEffect();
		doDeathTime = true;
		_delayTimeIncreaseCountdown = _delayTimeIncreaseMax;
		_addDeathTimeScale = 0f;

	}

	public void PauseGame(){
		if (!timePaused){
			capturedTimeScale = Time.timeScale;
			Time.timeScale = 0f;
			timePaused = true;
		}
	}

	public void UnpauseGame(){
		if (timePaused){
			Time.timeScale = capturedTimeScale;
			timePaused = false;
		}
	}



}
