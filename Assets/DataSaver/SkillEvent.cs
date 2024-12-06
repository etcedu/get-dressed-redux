using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class SkillEvent
{
	private Dictionary<string, string> _things = new Dictionary<string, string>();

	public void AddString(string name, string value) 
	{
		if (_things.ContainsKey(name)) return;

		var stringValue = string.Format("\"{0}\"", value);

		_things.Add(name, stringValue);
	}

	public void AddNumber(string name, float number)
	{
		if (_things.ContainsKey(name)) return;

		var stringValue = number.ToString();

		_things.Add(name, stringValue);
	}

	public void AddNumber(string name, int number)
	{
		if (_things.ContainsKey(name)) return;
		
		var stringValue = number.ToString();
		
		_things.Add(name, stringValue);
	}

	public void AddFlag(string name, bool value)
	{
		if (_things.ContainsKey(name)) return;

		var stringValue = value ? "true" : "false";

		_things.Add(name, stringValue);
	}

	public void Save()
	{
		if (_things.Count < 1) return;

		AddNumber("GameTime", Time.fixedTime);

		SkillData.Save(GetBytes());
	}

	public byte[] GetBytes()
	{
		if (_things.Count < 1) return new byte[0];

		var builder = new StringBuilder();

		builder.Append('{');
		bool needComma = false;

		foreach (var kvp in _things)
		{
			if (needComma) builder.Append(',');
			builder.AppendFormat("\"{0}\":{1}", kvp.Key, kvp.Value);
			needComma = true;
		}

		builder.Append('}');

		return UTF8Encoding.UTF8.GetBytes(builder.ToString());
	}
}

