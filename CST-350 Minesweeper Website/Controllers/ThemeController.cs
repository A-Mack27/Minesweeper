using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.Drawing;
using CST_350_Minesweeper_Website.Models;
using Microsoft.AspNetCore.Http;

namespace CST_350_Minesweeper_Website.Controllers
{
	public class ThemeController : Controller
	{
		public static List<ThemeModel> Themes;

        /// <summary>
        /// Index view of the theme controller
        /// </summary>
        /// <returns></returns>
		public IActionResult Index()
		{
            PopulateThemeList();
            return View(Themes);
		}

        /// <summary>
        /// View to apply a theme
        /// </summary>
        /// <param name="themeIndex"></param>
        /// <returns></returns>
		public IActionResult ApplyTheme(string themeIndex)
		{
            if (HttpContext.Session.GetString("User") == null)
                return RedirectToAction("Index", "Login");
            // Parse the theme index and get the selected theme
            int theme = int.Parse(themeIndex);
            var selectedTheme = Themes[theme];

            // Store each individual color in session
            HttpContext.Session.SetString("SelectedTheme", selectedTheme.Name);
            HttpContext.Session.SetString("FormColor", selectedTheme.ConvertColorToCSS(selectedTheme.FormColor));
            HttpContext.Session.SetString("VisitedColor1", selectedTheme.ConvertColorToCSS(selectedTheme.VisitedColor1));
            HttpContext.Session.SetString("VisitedColor2", selectedTheme.ConvertColorToCSS(selectedTheme.VisitedColor2));
            HttpContext.Session.SetString("UnvisitedColor1", selectedTheme.ConvertColorToCSS(selectedTheme.UnvisitedColor1));
            HttpContext.Session.SetString("UnvisitedColor2", selectedTheme.ConvertColorToCSS(selectedTheme.UnvisitedColor2));
            HttpContext.Session.SetString("TextColor", selectedTheme.ConvertColorToCSS(selectedTheme.TextColor));

            // Refresh the page
            if (HttpContext.Session.GetString("GameStarted") == "true")
                return RedirectToAction("Board", "Game");
            return RedirectToAction("Configure", "Game");
		}

        /// <summary>
        /// Populates the list of themes
        /// </summary>
		public void PopulateThemeList()
		{
            // Create all the themes
            Themes = new List<ThemeModel>
            {
                new ThemeModel("Default Light",
                                Color.FromArgb(255, 255, 255),
                                Color.FromArgb(219, 219, 219),
                                Color.FromArgb(200, 200, 200),
                                Color.FromArgb(131, 131, 131),
                                Color.FromArgb(115, 115, 115),
                                Color.FromArgb(10,10,10)),

                new ThemeModel("Default Dark",
                                Color.FromArgb(43, 43, 43),
                                Color.FromArgb(0, 69, 118),
                                Color.FromArgb(0, 82, 140),
                                Color.FromArgb(67, 67, 67),
                                Color.FromArgb(74, 74, 74),
                                Color.FromArgb(230,230,230)),

                new ThemeModel("Ocean Sunset",
                                Color.FromArgb(255,255,255),
                                Color.FromArgb(69,188,238),
                                Color.FromArgb(0, 82, 140),
                                Color.FromArgb(239,144,7),
                                Color.FromArgb(246,211,23),
                                Color.FromArgb(0,0,0)),

                new ThemeModel("Lavender",
                                Color.FromArgb(155, 104, 204),
                                Color.FromArgb(186, 161, 255),
                                Color.FromArgb(204, 186, 255),
                                Color.FromArgb(255, 222, 127),
                                Color.FromArgb(255, 232, 163),
                                Color.FromArgb(0,0,0)),

                new ThemeModel("Moss",
                                Color.FromArgb(66, 152, 216),
                                Color.FromArgb(107, 76, 28),
                                Color.FromArgb(138, 99, 38),
                                Color.FromArgb(94, 135, 0),
                                Color.FromArgb(109, 157, 0),
                                Color.FromArgb(0,0,0)),

                new ThemeModel("Tide",
                                Color.FromArgb(48, 224, 216),
                                Color.FromArgb(215, 182, 133),
                                Color.FromArgb(195, 162, 113),
                                Color.FromArgb(68, 244, 236),
                                Color.FromArgb(18, 194, 186),
                                Color.FromArgb(0, 0, 0)),

                new ThemeModel("Iron Man",
                                Color.FromArgb(90,90,90),
                                Color.FromArgb(174,0,1),
                                Color.FromArgb(116,0,1),
                                Color.FromArgb(238,186,48),
                                Color.FromArgb(211,166,37),
                                Color.FromArgb(0,0,0)),

                new ThemeModel("Coffee",
                                Color.FromArgb(255,219,172),
                                Color.FromArgb(161,105,56),
                                Color.FromArgb(198,134,66),
                                Color.FromArgb(241,194,125),
                                Color.FromArgb(253,214,155),
                                Color.FromArgb(0,0,0)),

                new ThemeModel("Green Apple",
                                Color.FromArgb(210,253,186),
                                Color.FromArgb(99,224,26),
                                Color.FromArgb(145,235,93),
                                Color.FromArgb(234,231,198),
                                Color.FromArgb(214,211,178),
                                Color.FromArgb(0,0,0)),

                new ThemeModel("Blueberry",
                                Color.FromArgb(81,29,58),
                                Color.FromArgb(182,221,251),
                                Color.FromArgb(152,191,221),
                                Color.FromArgb(69,47,79),
                                Color.FromArgb(99,77,109),
                                Color.FromArgb(255,255,255)),

                new ThemeModel("Perry",
                                Color.FromArgb(125,70,50),
                                Color.FromArgb(0,177,177),
                                Color.FromArgb(0,147,140),
                                Color.FromArgb(252,176,52),
                                Color.FromArgb(222,121,27),
                                Color.FromArgb(0,0,0)),
            };
        }

		/// <summary>
		/// Default constructor
		/// </summary>
		public ThemeController()
		{
		}
	}
}
