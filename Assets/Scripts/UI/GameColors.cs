using UnityEngine;
using System.Collections;

public class GameColors : MonoBehaviour
{
	public enum Uses
	{
		ChoiceButton = 0,
		ToolButton = 1,
		TimeCapsule = 2,
		Preparedness = 3,
		Stress = 4,
		ToolBar = 5,
		TimeText = 6,
		ConfirmGreen = 7,
		CancelRed = 8,
		OtherTaskBlue = 9,
		WarningOrange = 10
	}

	public static readonly Color CHOICE_BUTTON = new Color(0.47f, 0.69f, 0.27f);
	public static readonly Color TOOL_BUTTON = new Color(0.09f, 0.59f, 0.75f);
	public static readonly Color TIME_CAPSULE = new Color(0.42f, 0.8f, 0.92f);
	public static readonly Color PREPAREDNESS = new Color(1f, 0.52f, 0f);
	public static readonly Color STRESS = new Color(0.45f, 0.4f, 0.73f);
	public static readonly Color TOOL_BAR = new Color(0.18f, 0.18f, 0.18f);
	public static readonly Color TIME_TEXT = new Color(0f, 0.46f, 0.61f);
	public static readonly Color CONFIRM_GREEN = new Color(0.47f, 0.69f, 0.27f);
	public static readonly Color CANCEL_RED = new Color(0.85f, 0.31f, 0.31f);
	public static readonly Color OTHER_TASK_BLUE = new Color(0.09f, 0.59f, 0.75f);
	public static readonly Color WARNING_ORANGE = new Color(0.92f, 0.65f, 0.42f);

	private static Color[] _orderedColors;

	public static Color Darken(Color original)
	{
		var hsb = HSBColor.FromColor(original);

		float h = hsb.h;
		float s = hsb.s;
		float b = .7f * hsb.b;

		return new HSBColor(h, s, b).ToColor();
	}

	public static Color Lighten(Color original)
	{
		var hsb = HSBColor.FromColor(original);
		
		float h = hsb.h;
		float s = hsb.s;
		float b = 1.1f * hsb.b;

		if (b > 1) b = 1;
		
		return new HSBColor(h, s, b).ToColor();
	}

	public static Color ForUse(Uses use)
	{
		if (_orderedColors == null)	_orderedColors = new Color[]
			{ 
				CHOICE_BUTTON,
				TOOL_BUTTON,
				TIME_CAPSULE,
				PREPAREDNESS,
				STRESS,
				TOOL_BAR, 
				TIME_TEXT,
				CONFIRM_GREEN,
				CANCEL_RED,
				OTHER_TASK_BLUE,
				WARNING_ORANGE
			};

		return _orderedColors[(int)use];
	}
}

