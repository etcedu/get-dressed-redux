using System;
using Newtonsoft.Json;
using UnityEngine;

public class TransformConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var trans = (Transform)value;
		writer.WriteStartObject();
		writer.WritePropertyName("localPosition.x");
		writer.WriteValue(trans.localPosition.x);
		writer.WritePropertyName("localPosition.y");
		writer.WriteValue(trans.localPosition.y);
		writer.WritePropertyName("localPosition.z");
		writer.WriteValue(trans.localPosition.z);
		writer.WritePropertyName("localRotation.x");
		writer.WriteValue(trans.localRotation.x);
		writer.WritePropertyName("localRotation.y");
		writer.WriteValue(trans.localRotation.y);
		writer.WritePropertyName("localRotation.z");
		writer.WriteValue(trans.localRotation.z);
		writer.WritePropertyName("localRotation.w");
		writer.WriteValue(trans.localRotation.w);
		writer.WritePropertyName("localScale.x");
		writer.WriteValue(trans.localScale.x);
		writer.WritePropertyName("localScale.y");
		writer.WriteValue(trans.localScale.y);
		writer.WritePropertyName("localScale.z");
		writer.WriteValue(trans.localScale.z);
		writer.WriteEndObject();
	}
	
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Transform);
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