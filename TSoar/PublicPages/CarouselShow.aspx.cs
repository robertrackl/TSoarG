using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace TSoar.PublicPages
{
    public partial class CarouselShow : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ObjectCache cache = MemoryCache.Default;
                try
                {
                    if (cache["CarouselInnerHtml"] != null && cache["CarouselIndicatorsHtml"] != null)
                    {
                        //use the cached html
                        ltlCarouselImages.Text = cache["CarouselInnerHtml"].ToString();
                        ltlCarouselIndicators.Text = cache["CarouselIndicatorsHtml"].ToString();
                    }
                    else
                    {
                        //get a list of images from the folder
                        const string imagesPath = "/i/CarouselShow/";
                        var dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~" + imagesPath));
                        //filtering to jpgs, but ideally not required
                        List<string> fileNames = (from flInfo in dir.GetFiles() where flInfo.Name.EndsWith(".jpg") select flInfo.Name).ToList();
                        if (fileNames.Count > 0)
                        {
                            var carouselInnerHtml = new StringBuilder();
                            var indicatorsHtml = new StringBuilder();
                            //loop through and build up the html for indicators + images
                            for (int i = 0; i < fileNames.Count; i++)
                            {
                                var fileName = fileNames[i];
                                carouselInnerHtml.AppendLine(i == 0 ? "<div class=\"item active\">" : "<div class=\"item\">");
                                carouselInnerHtml.AppendLine("    <img src=\".." + imagesPath + fileName + "\" alt=\"Slide #" + (i + 1) + "\">");
                                carouselInnerHtml.AppendLine("    <div style=\"text-align:center\" class=\"carousel-caption\">");
                                carouselInnerHtml.AppendLine("<h3>" + fileName + "</h3>");
                                carouselInnerHtml.AppendLine("    </div>");
                                carouselInnerHtml.AppendLine("</div>");
                                indicatorsHtml.AppendLine(i == 0 ? "<li data-target=\"#myCarousel\" data-slide-to=\"" + i +
                                    "\" class=\"active\"></li>" : "<li data-target=\"#myCarousel\" data-slide-to=\"" + i + "\"></li>");
                            }
                            //stick the html in the literal tags and the cache
                            cache["CarouselInnerHtml"] = ltlCarouselImages.Text = carouselInnerHtml.ToString();
                            cache["CarouselIndicatorsHtml"] = ltlCarouselIndicators.Text = indicatorsHtml.ToString();
                        }
                    }
                }
                catch (Exception)
                {
                    //something is dodgy so flush the cache
                    if (cache["CarouselInnerHtml"] != null)
                    {
                        Cache.Remove("CarouselInnerHtml");
                    }
                    if (cache["CarouselIndicatorsHtml"] != null)
                    {
                        Cache.Remove("CarouselIndicatorsHtml");
                    }
                }
            }
        }
    }
}