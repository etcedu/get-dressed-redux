using System.Collections;
using UnityEngine;

namespace Clothing
{
	public sealed class Tier
	{
		private readonly int value;
		
		public static readonly Tier INFORMAL = new Tier(0);
		public static readonly Tier CASUAL = new Tier(1);
		public static readonly Tier BUSINESS_CASUAL = new Tier(2);
		public static readonly Tier BUSINESS_PROFESSIONAL = new Tier(3);
		static string[] nameMap = new string[]
		{
			"Informal", "Casual", "Business Casual", "Business Professional"
		};
		static readonly int[,] scoreMap = new int[,] {
			{100,	90,		80,		70},
			{40,	100,	90,		80},
			{10,	40,		100,	90},
			{0,		20,		40,		100}
		};
		
		private Tier(int value)
		{
			this.value = value;
		}
		
		public int Value()
		{
			return value;
		}

		public override string ToString ()
		{
			return nameMap[value];
		}

		public static string GetName(TierEnum e)
		{
			return nameMap[(int)e];
		}
		
		public static string[] GetNames(Tier[] tiers)
		{
			string[] values =  new string[tiers.Length];
			for(int i = 0; i < tiers.Length; i++)
				values[i] = tiers[i].ToString();
			return values;
		}
		
		public static string[] GetNames(Tier.TierEnum[] tiers)
		{
			string[] values =  new string[tiers.Length];
			for(int i = 0; i < tiers.Length; i++)
				values[i] = nameMap[(int)tiers[i]];
			return values;
		}

		public static int Score(Tier clothesTier, Tier interviewTier)
		{
			return scoreMap[interviewTier.value, clothesTier.value];
		}

		public static int Score(TierEnum clothesTier, TierEnum interviewTier)
		{
			return scoreMap[(int)interviewTier,(int)clothesTier];
		}
		
		public static TierEnum GetEnum(string name)
		{
			Debug.Log("Name: " + name);
			if(name == "Casual")
				return TierEnum.CASUAL;
			if(name == "Business Casual")
				return TierEnum.BUSINESS_CASUAL;
			if(name == "Business Professional")
				return TierEnum.BUSINESS_PROFESSIONAL;
			return TierEnum.INFORMAL;
		}

		public enum TierEnum 
		{
			INFORMAL = 0, CASUAL = 1, BUSINESS_CASUAL = 2, BUSINESS_PROFESSIONAL = 3
		}
	}
}

