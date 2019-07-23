using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Newspapers
{
    public class WriteArticleViewModel
    {
        public int ID { get; set; }
        public NewspaperInfoViewModel Info { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Title { get; set; }
        public string PayOnlyContent { get; set; }
        [Required]
        [DisplayName("Short description")]
        public string ShortDescription { get; set; }
        public bool EnablePayOnly { get; set; }
        [Range(0.01, 10000000)]
        public double? Price { get; set; }
        public string PriceCurrencySymbol { get; set; }
        public HttpPostedFileBase ArticleImage { get; set; }
        public string ImgUrl { get; set; }
        public bool Edit { get; set; } = false;
        public bool? Publish { get; set; }
        public double NewspaperTax { get; set; }

        public WriteArticleViewModel(Newspaper newspaper, INewspaperService newspaperService)
        {
            Info = new NewspaperInfoViewModel(newspaper, newspaperService);
            PriceCurrencySymbol = Persistent.Countries.GetById(newspaper.CountryID).Currency.Symbol;
            NewspaperTax = (double)newspaper.Country.CountryPolicy.ArticleTax * 100.0;
        }

        public WriteArticleViewModel(Newspaper newspaper, Article article, INewspaperService newspaperService)
            :this(newspaper, newspaperService)
        {
            Content = article.Content;
            Title = article.Title;
            PayOnlyContent = article.PayOnlyContent;
            ShortDescription = article.ShortDescription;
            EnablePayOnly = article.Price.HasValue;
            Price = (double?)article.Price;
            ImgUrl = article.ImgURL;
            Edit = true;
            ID = article.ID;
            Publish = article.Published;
        }

        public WriteArticleViewModel()
        {

        }
    }
}