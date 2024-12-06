using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Clothing
{
	[System.Serializable]
	public class Info
	{
		// Identifier
		[SerializeField]
		private string tag;
		public string Tag
		{
			get{ return tag; }
			set{ tag = value; }
		}
		// Display name
		[SerializeField]
		private string displayTag;
		public string DisplayTag
		{
			get{ return displayTag; }
			set{ displayTag = value;}
		}
		// Button icon
		[SerializeField]
		private Texture image;
		public Texture Image
		{
			get{ return image; }
			set{ image = value; }
		}
		// Renderer sprites
		[SerializeField]
		private Texture[] pieces = new Texture[0];
		public Texture[] Pieces
		{
			get{ return pieces; }
			set{ pieces = value; }
		}
		// Clothing tier
		[SerializeField]
		private List<Clothing.Tier.TierEnum> tiers = new List<Clothing.Tier.TierEnum>();
		public List<Clothing.Tier.TierEnum> Tiers
		{
			get{ return tiers; }
			set{ tiers = value; }
		}
		// Gender class
		[SerializeField]
		private GenderClass gender = GenderClass.EITHER;
		public GenderClass Gender
		{
			get{ return gender; }
			set{ gender = value; }
		}
		// Color
		[SerializeField]
		private string[] colors = new string[0];
		public string[] Colors 
		{
			get{ return colors; }
			set{ colors = value; }
		}
		[SerializeField]
		private string[] matchingColors = new string[0];
		public string[] MatchingColors
		{
			get{ return matchingColors; }
			set{ matchingColors = value; }
		}
		// Design
		[SerializeField]
		private string[] designs = new string[0];
		public string[] Designs 
		{
			get{ return designs; }
			set{ designs = value; }
		}
		[SerializeField]
		private string[] matchingDesigns = new string[0];
		public string[] MatchingDesigns
		{
			get{ return matchingDesigns; }
			set{ matchingDesigns = value; }
		}
		// Good items to pair with
		[SerializeField]
		private string[] goodMatches = new string[0];
		public string[] GoodMatches
		{
			get{ return goodMatches; }
			set{ goodMatches = value; }
		}
		// Bad items to pair with
		[SerializeField]
		private string[] badMatches = new string[0];
		public string[] BadMatches
		{
			get{ return badMatches; }
			set{ badMatches = value; }
		}

		[System.NonSerialized]
		private HashSet<string> goodTags;
		[System.NonSerialized]
		private HashSet<string> badTags;

		public enum GenderClass
		{
			EITHER, MALE, FEMALE
		}
	}
}

