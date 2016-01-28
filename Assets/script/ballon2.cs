using UnityEngine;
using System.Collections;

public class ballon2 : MonoBehaviour {

	public static float micloudness;
	private string _device;
	public AudioClip _cliprecord;
	int _samplewindow = 128;
	bool is_initialized;

	// Use this for initialization
	void initMic() {
		Debug.Log("Hai");
		if(_device == null){
			_device = Microphone.devices[0];
			_cliprecord = Microphone.Start(_device, true, 999,44100);
		}
		Debug.Log("Hai");
	}

	void StopMic(){
		Microphone.End(_device);
	}

	float levelMax(){
		float levelmax = 0;
		float[] wavedata = new float[_samplewindow];
		int micposition = Microphone.GetPosition(null) - _samplewindow;//(int) Mathf.Ceil(_samplewindow+1);
		if(micposition<0){
			return 0;
		}
		_cliprecord.GetData(wavedata,micposition);
		for(int i=0; i< Mathf.RoundToInt(_samplewindow); i++){
			float wavepeak = wavedata[i]*wavedata[i];
			if(levelmax < wavepeak){
				levelmax = wavepeak;
			}

		}
		return levelmax;
	}
	
	// Update is called once per frame
	void Update () {
		micloudness = levelMax();
	}

	void OnEnable(){
		initMic();
		is_initialized = true;
	}

	void OnDisable(){
		StopMic();
	}

	void onDestroy(){
		StopMic();
	}

	void onAplicationFocus(bool focus){
		if(focus){
			Debug.Log("focused");
			if(!is_initialized){
				initMic();
				is_initialized = true;
			}
		}
		if(!focus){
			Debug.Log("Paused");
			StopMic();
			is_initialized = false;
		}
	}
}
