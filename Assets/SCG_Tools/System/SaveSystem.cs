using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;
#if !UNITY_WEBPLAYER
using Newtonsoft.Json.Bson;
using System.IO;
#endif

/// <summary>
/// A system capable of saving pretty much anything you throw at it.
/// Created by Matt Becker
/// </summary>

public class SaveSystem<T>
{
	string _id = "";
	string _fileLocation = Application.persistentDataPath + "/";
	Dictionary<string, T> _entries = new Dictionary<string, T>(){};
	Func<T> _defaultValue;
	JsonConverter[] _converters;

	/// <summary>
	/// Initializes the save system. ID must be unique.
	/// DefaultValue should be setup like this:
	/// 	(Func<T>)(()=>{return default value;})
	/// </summary>
	public SaveSystem(string ID, Func<T> DefaultValue, params JsonConverter[] Converters)
	{
		_id = ID;
		_defaultValue = DefaultValue;
		_converters = Converters;
		initializedCheck();
	}


	public T GetValue(string key)
	{
		initializedCheck();
		if(_entries.ContainsKey(key))
			return _entries[key];
		else
			return _defaultValue();
	}

	public void GetValue(string key, ref T target)
	{
		initializedCheck();
		if(_entries.ContainsKey(key))
			target = _entries[key];
		else
			target = _defaultValue();
	}
	
	public T GetValue(string key, T defaultValue)
	{
		initializedCheck();
		if(_entries.ContainsKey(key))
			return _entries[key];
		else
			return defaultValue;
	}

	public void GetValue(string key, ref T target, T defaultValue)
	{
		initializedCheck();
		if(_entries.ContainsKey(key))
			target = _entries[key];
		else
			target = defaultValue;
	}

	public bool ContainsKey(string key)
	{
		initializedCheck();
		return _entries.ContainsKey(key);
	}

	public void SetValue(string key, T value)
	{
		initializedCheck();
		if(_entries.ContainsKey(key))
			_entries[key] = value;
		else
			_entries.Add(key, value);
	}

	public bool RemoveValue(string key)
	{
		initializedCheck();
		return _entries.Remove(key);
	}

	bool _initialized = false;
	void initializedCheck()
	{
		if(!_initialized)
		{
			_initialized = true;
#if !UNITY_WEBPLAYER
			if(!File.Exists(_fileLocation + _id))
				Save();
#endif
			load();
		}
	}

	void load()
	{
		_entries.Clear();
#if !UNITY_WEBPLAYER
		using(var memory  = new MemoryStream(File.ReadAllBytes(_fileLocation + _id)))
		{
			using (BsonReader reader = new BsonReader(memory))
				_entries = new JsonSerializer().Deserialize<Dictionary<string, T>>(reader);
		}
#else
		if(PlayerPrefs.HasKey(_id))
		{
			_entries = JsonConvert.DeserializeObject<Dictionary<string, T>>(PlayerPrefs.GetString(_id), _converters);
		}
#endif
	}

	/// <summary>
	/// Save the entry data.
	/// </summary>
	public void Save()
	{
#if !UNITY_WEBPLAYER
		using(var memory = new MemoryStream())
		{
			using (BsonWriter writer = new BsonWriter(memory))
			{
				new JsonSerializer().Serialize(writer, _entries);
				using(var fileStream = new FileStream(_fileLocation + _id, FileMode.Create))
					memory.WriteTo(fileStream);
			}

		}
#else
		PlayerPrefs.SetString(_id, JsonConvert.SerializeObject(_entries, _converters));
#endif
	}

	/// <summary>
	/// Delete the entry data.
	/// </summary>
	public void Delete()
	{
		_initialized = false;
#if !UNITY_WEBPLAYER
		if(File.Exists(_fileLocation + _id))
			File.Delete(_fileLocation + _id);
#else
		PlayerPrefs.DeleteKey(_id);
#endif
	}
}
