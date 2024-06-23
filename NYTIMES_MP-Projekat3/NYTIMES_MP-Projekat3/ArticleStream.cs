using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace NYTIMES_MP_Projekat3
{
    public class ArticleStream : IObservable<Article>
    {
        private readonly Subject<Article> articleSubject;
        
        public ArticleStream()
        {
            articleSubject = new Subject<Article>();
        }
        public void GetArticles()
        {
            string apiKey = "pGnYUBa9Q2ASQpUYor0BTjgY5r3GcBpp";
            HttpClient client = new HttpClient();
            string url = $"https://api.nytimes.com/svc/mostpopular/v2/viewed/7.json?api-key={apiKey}";
            _ = Task.Run(async () =>
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();
                    dynamic articles = JsonConvert.DeserializeObject<dynamic>(content).results;
                    //Console.WriteLine($"{articles}");
                    foreach (var article in articles)
                    {
                        Article newArticle = new Article
                        {
                            Title = article.title,
                            Abstract = article.@abstract,
                            Author = article.byline,
                            Url = article.url
                        };
                        articleSubject.OnNext(newArticle);
                    }
                    //na ovaj nacin signaliziramo da su svi clanci pribavljeni
                    articleSubject.OnCompleted();
                }
                catch (Exception e)
                {
                    articleSubject.OnError(e);
                }
            });
        }
        public IDisposable Subscribe(IObserver<Article> observer)
        {
            return articleSubject.Subscribe(observer);
        }
    }
}
