using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using Serilog;
using MLScraper.Models;
using MLScraper.Helpers;

namespace MLScraper
{
    class Program
    {
        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();
        static List<Category> _mainCategories = new List<Category>();
        static List<Category> _subCategories = new List<Category>();
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Init");
            Scrape();
            WriteToFile();
            Log.Information("End");
        }

        static void Scrape()
        {
            _mainCategories = GetMainCategoriesLinks("https://www.mercadolibre.com.ar/categorias#menu=categories");

            Log.Information("Sub category count:    {MainCategoriesCount}", _mainCategories.Count);

            foreach (var tuple in _mainCategories)
            {
                Log.Information(tuple.Name);
            }

            Log.Information("Getting sub categories links...");

            foreach (var tuple in _mainCategories)
            {
                var res = GetSubCategoriesLinks(tuple.Url);
                _subCategories.AddRange(res);

                Log.Information("{TupleItem2}   ({ResCount} subcategories)", tuple.Name, res.Count);

                foreach (var subCategory in res)
                {
                    Log.Information("{SubCategoryItem2}  {SubCategoryItem1}", subCategory.Name, subCategory.Url);
                }
            }
        }

        static void WriteToFile()
        {
            JSONSerializer.Instance.Serialize(_mainCategories, CategoryTypeEnum.MAINCATEGORIES);
            JSONSerializer.Instance.Serialize(_subCategories, CategoryTypeEnum.SUBCATEGORIES);
        }

        static List<Category> GetMainCategoriesLinks(string url)
        {
            var mainCategoriesLinks = new List<Category>();
            var html = GetHtml(url);

            if (html != null)
            {
                var links = html.CssSelect("h2.categories__title > a");

                foreach (var link in links)
                {
                    mainCategoriesLinks.Add(new Category
                    {
                        Name = link.InnerHtml,
                        Url = link.Attributes["href"].Value
                    });
                }
            }
            return mainCategoriesLinks;
        }

        static List<Category> GetSubCategoriesLinks(string url)
        {
            var subCategoriesLinks = new List<Category>();
            var html = GetHtml(url);

            if (html != null)
            {
                var links = html.CssSelect("li.category > a");

                foreach (var link in links)
                {
                    subCategoriesLinks.Add(new Category
                    {
                        Name = link.InnerHtml,
                        Url = link.Attributes["href"].Value
                    });
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
