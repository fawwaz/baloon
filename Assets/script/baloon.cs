using UnityEngine;
using System.Collections;

public class baloon : MonoBehaviour {
	
	public int qsamples = 1024;
	public float refvalue = 0.1f;
	public float rmsvalue;
	public float dbvalue;
	public float volume = 90;
	int factor = 1000;
	AudioSource audio;
	ballon2 ba2;
	public GameObject b2;

	public float[] samples;

	// Use this for initialization
	IEnumerator Start () {
		samples = new float[qsamples];
		yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
		if(Application.HasUserAuthorization(UserAuthorization.Microphone)){
			initMic();
		}else{
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		getVolume();

		//transform.localScale = new Vector3(transform.localScale.x, volume * rmsvalue, transform.localScale.z);
		float size = volume * levelMax() *factor;
		transform.localScale = new Vector3(size, size, transform.localScale.z);

		//Debug.Log("Volume : "+volume);
		//Debug.Log("RMS : "+rmsvalue);
		Debug.Log("Levelmax : "+levelMax());
	}

	public void getVolume(){
		audio = GetComponent<AudioSource> ();
		audio.GetOutputData(samples, 0);
		float sum = 0;
		for(int i=0; i< qsamples; i++){
			sum += samples[i]*samples[i];
		}
		rmsvalue = Mathf.Sqrt(sum/qsamples);
		//rmsvalue += 1;
		dbvalue = 20 * Mathf.Log10(rmsvalue/refvalue);
		if(dbvalue<-160){
			dbvalue = -160;
		}
	}

	public static float micloudness;
	private string _device;
	public AudioClip _cliprecord;
	int _samplewindow = 128;
	bool is_initialized;

	// Use this for initialization
	void initMic() {
		Debug.Log("Hai");
		string[] devices = Microphone.devices;
		foreach(string devs in devices){
			Debug.Log(devs);
		}
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
