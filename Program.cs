using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using Serilog;
using System.Linq;

namespace MLScraper
{
    class Program
    {
        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();
        static List<(string, string)> mainCategories = new List<(string, string)>();
        static List<(string, string)> subCategories = new List<(string, string)>();
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Init");
            InitScraping();
            Log.Information("End");
        }

        static void InitScraping()
        {
            mainCategories = GetMainCategoriesLinks("https://www.mercadolibre.com.ar/categorias#menu=categories");

            Log.Information("Sub category count:    {MainCategoriesCount}", mainCategories.Count);

            foreach (var tuple in mainCategories)
            {
                Log.Information(tuple.Item1);
            }

            Log.Information("Getting sub categories links...");

            foreach (var tuple in mainCategories)
            {
                var res = GetSubCategoriesLinks(tuple.Item1);
                subCategories.Concat(res);

                Log.Information("{TupleItem2}   ({ResCount} subcategories)", tuple.Item2, res.Count);

                foreach (var subCategory in res)
                {
                    Log.Information("{SubCategoryItem2}  {SubCategoryItem1}", subCategory.Item2, subCategory.Item1);
                }
            }

        }

        static List<(string, string)> GetMainCategoriesLinks(string url)
        {
            var mainCategoriesLinks = new List<(string, string)>();
            var html = GetHtml(url);

            if (html != null)
            {
                var links = html.CssSelect("h2.categories__title > a");

                foreach (var link in links)
                {
                    mainCategoriesLinks.Add((link.Attributes["href"].Value, link.InnerHtml));
                }
            }
            return mainCategoriesLinks;
        }

        static List<(string, string)> GetSubCategoriesLinks(string url)
        {
            var subCategoriesLinks = new List<(string, string)>();
            var html = GetHtml(url);

            if (html != null)
            {
                var links = html.CssSelect("li.category > a");

                foreach (var link in links)
                {
                    subCategoriesLinks.Add((link.Attributes["href"].Value, link.InnerHtml));
                }
            }

            return subCategoriesLinks;
        }
        static HtmlNode GetHtml(string url)
        {
            try
            {
                WebPage webPage = _scrapingBrowser.NavigateToPage(new Uri(url));
                return webPage.Html;
            }
            catch (Exception ex)
            {
                Log.Error("An error ocurred while trying to get the page: {Ex}", ex);
            }

            return null;
        }
    }
}
