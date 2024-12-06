using UnityEngine;
using System.Collections.Generic;

public class SkillData : MonoBehaviour 
{
	private static SkillData _instance;

	public static void Save(byte[] bytes)
	{
		if (bytes == null || bytes.Length < 1) return;

		if (_instance == null)
		{
			var go = new GameObject();
			_instance = go.AddComponent<SkillData>();
		}

		_instance._data.Enqueue(bytes);
	}

	private Queue<byte[]> _data = new Queue<byte[]>();
	private WWW _currentRequest = null;

	private string _url = "https://r47k6had49.execute-api.us-east-1.amazonaws.com/prod/devices";
	private string _key = "rZACPBvVvgz9zXNvBhSa3nJweuUbUaL2uGOlyk5e";
	private string _game = "GetDressed";
	private Dictionary<string, string> _headers = new Dictionary<string, string>();

	public void Awake()
	{
		DontDestroyOnLoad(this);

		_headers.Add("content-type", "application/json");
		_headers.Add("x-api-key", _key);
	}

	public void Start()
	{
		var device = SystemInfo.deviceUniqueIdentifier;

		var register = string.Format("{{\"Game\":\"{0}\",\"DeviceId\":\"{1}\"}}", _game, device);
		var registerBytes = System.Text.UTF8Encoding.UTF8.GetBytes(register);

		_currentRequest = new WWW(_url, registerBytes, _headers);

		_url = string.Format(@"{0}/{1}/{2}/events", _url, device, _game);
	}

	public void Update()
	{
		if (_currentRequest != null && _currentRequest.isDone)
		{
			if (_currentRequest.error != null) Debug.LogError(_currentRequest.error);
			_currentRequest = null;
		}

		if (_currentRequest != null || _data.Count < 1) return;

		_currentRequest = new WWW(_url, _data.Dequeue(), _headers);
	}
}