using System.Drawing;

namespace CST_350_Minesweeper_Website.Models
{
	public class ThemeModel
	{
		public string Name { get; set; }
		public Color FormColor { get; set; }
		public Color UnvisitedColor1 { get; set; }
		public Color UnvisitedColor2 { get; set; }
		public Color VisitedColor1 { get; set; }
		public Color VisitedColor2 { get; set; }
		public Color TextColor { get; set; }

		/// <summary>
		/// Converts the ARGB color to just RGB for CSS compatibility
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public string ConvertColorToCSS(Color color)
		{
			return $"rgb({color.R}, {color.G}, {color.B})";
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="formColor"></param>
		/// <param name="unvisitedColor1"></param>
		/// <param name="unvisitedColor2"></param>
		/// <param name="visitedColor1"></param>
		/// <param name="visitedColor2"></param>
		public ThemeModel(string name, Color formColor, Color unvisitedColor1, Color unvisitedColor2, Color visitedColor1, Color visitedColor2, Color textColor)
		{
			Name = name;
			FormColor = formColor;
			UnvisitedColor1 = unvisitedColor1;
			UnvisitedColor2 = unvisitedColor2;
			VisitedColor1 = visitedColor1;
			VisitedColor2 = visitedColor2;
			TextColor = textColor;
		}
	}
}
