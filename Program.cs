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
        static List<string> mainCategories = new List<string>();
        static List<string> subCategories = new List<string>();
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            log.Information("Init");
            log.Information("Hello fellow MercadoTracker!");
            log.Information("Getting main categories links...");

            mainCategories = GetMainCategoriesLinks("https://www.mercadolibre.com.ar/categorias#menu=categories");

            log.Information("Sub category count:    {MainCategoriesCount}", mainCategories.Count);

            foreach (var cat in mainCategories)
            {
                log.Information(cat);
            }

            log.Information("Getting sub categories links...");

            foreach (var mainCategory in mainCategories)
            {
                var res = GetSubCategoriesLinks(mainCategory);
                subCategories.Concat(res);

                log.Information("Sub category count:    {ResCount}", res.Count);
                foreach (var subCategory in res)
                {
                    log.Information(subCategory);
                }
            }

            log.Information("End");
        }

        static List<string> GetMainCategoriesLinks(string url)
        {
            var mainCategoriesLinks = new List<string>();
            var html = GetHtml(url);

            if (html != null)
            {
                var links = html.CssSelect("h2.categories__title > a");

                foreach (var link in links)
                {
                    mainCategoriesLinks.Add(link.Attributes["href"].Value);
                }
            }
            return mainCategoriesLinks;
        }

        static List<string> GetSubCategoriesLinks(string url)
        {
            var subCategoriesLinks = new List<string>();
            var html = GetHtml(url);

            if (html != null)
            {
                var links = html.CssSelect("li.category > a");

                foreach (var link in links)
                {
                    subCategoriesLinks.Add(link.Attributes["href"].Value);
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
