using System.Collections;

namespace Clothing
{
	public sealed class Category
	{
		private readonly int value;

		public static readonly Category HEAD = new Category(0);
		public static readonly Category TOP = new Category(1);
		public static readonly Category BOTTOM = new Category(2);
		public static readonly Category SHOES = new Category(3);
		public static readonly Category OTHER = new Category(4);
		static readonly string[] nameMap = new string[] {
			"Head", "Top", "Bottom", "Shoes", "Other", "Face"
		};

		private Category(int value)
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
		
		public static string GetName(CategoryEnum e)
		{
			return nameMap[(int)e];
		}

		public static string[] GetNames(Category[] categories)
		{
			string[] values =  new string[categories.Length];
			for(int i = 0; i < categories.Length; i++)
				values[i] = categories[i].ToString();
			return values;
		}

		public static string[] GetNames(Category.CategoryEnum[] categories)
		{
			string[] values =  new string[categories.Length];
			for(int i = 0; i < categories.Length; i++)
				values[i] = nameMap[(int)categories[i]];
			return values;
		}

		public static CategoryEnum GetEnum(string name)
		{
			if(name == "Head")
				return CategoryEnum.HEAD;
			if(name == "Top")
				return CategoryEnum.TOP;
			if(name == "Bottom")
				return CategoryEnum.BOTTOM;
			if(name == "Shoes")
				return CategoryEnum.SHOES;
			if(name == "Face")
				return CategoryEnum.FACE;

			return CategoryEnum.OTHER;
		}

		public enum CategoryEnum
		{
			HEAD, TOP, BOTTOM, SHOES, OTHER, FACE
		}
	}
}

