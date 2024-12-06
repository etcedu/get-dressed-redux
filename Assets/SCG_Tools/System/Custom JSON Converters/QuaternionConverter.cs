using System;
using Newtonsoft.Json;
using UnityEngine;

public class QuaternionConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var quat = (Quaternion)value;
		writer.WriteStartObject();
		writer.WritePropertyName("w");
		writer.WriteValue(quat.w);
		writer.WritePropertyName("x");
		writer.WriteValue(quat.x);
		writer.WritePropertyName("y");
		writer.WriteValue(quat.y);
		writer.WritePropertyName("z");
		writer.WriteValue(quat.z);
		writer.WriteEndObject();
	}
	
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Quaternion);
	}
	
	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
	}
	
	public override bool CanRead
	{
		get { return false; }
	}
}