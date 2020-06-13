using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using Serilog;

namespace MLScraper
{
    class Program
    {
        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            log.Information("Init");
            log.Information("Hello fellow MercadoTracker!");
            log.Information("Getting main categories links...");
            var mainCategories = GetMainCategoriesLinks("https://www.mercadolibre.com.ar/categorias#menu=categories");

            foreach (var cat in mainCategories)
            {
                log.Information(cat);
            }

            log.Information("Getting sub categories links...");
            var subCategories = GetSubCategoriesLinks("https://home.mercadolibre.com.ar/computacion/#menu=categories");

            foreach (var cat in subCategories)
            {
                log.Information(cat);
            }

            log.Information("End");
        }

        static List<string> GetMainCategoriesLinks(string url)
        {
            var mainCategoriesLinks = new List<string>();
            var html = GetHtml(url);
            var links = html.CssSelect("li");

            foreach (var link in links)
            {
                var mainCategoriesNodes = link.SelectNodes("//li[contains(@class, 'categories__item')]");

                if (mainCategoriesNodes.Count > 0)
                {
                    foreach (var node in mainCategoriesNodes)
                    {
                        var anchorElement = node.SelectNodes("a")[0];

                        mainCategoriesLinks.Add(anchorElement.Attributes["href"].Value);
                    }

                }
            }
            return mainCategoriesLinks;
        }

        static List<string> GetSubCategoriesLinks(string url)
        {
            var subCategoriesLinks = new List<string>();
            var html = GetHtml(url);
            var links = html.CssSelect("li");

            foreach (var link in links)
            {
                var subCategoriesNodes = link.SelectNodes("//li[contains(@class, 'category')]");

                if (subCategoriesNodes.Count > 0)
                {
                    foreach (var node in subCategoriesNodes)
                    {
                        var anchorElement = node.SelectNodes("a")[0];

                        subCategoriesLinks.Add(anchorElement.Attributes["href"].Value);
                    }

                }
            }
            return subCategoriesLinks;
        }
        static HtmlNode GetHtml(string url)
        {
            WebPage webPage = _scrapingBrowser.NavigateToPage(new Uri(url));
            return webPage.Html;
        }
    }
}
