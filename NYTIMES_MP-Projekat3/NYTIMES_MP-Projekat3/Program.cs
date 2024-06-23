using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NYTIMES_MP_Projekat3;

public class Program
{
    public static void Main()
    {
        ArticleStream articleStream = new ArticleStream();
        SentimentAnalyzer sentimentAnalyzer = new SentimentAnalyzer();

        ArticleObserver observer1 = new ArticleObserver("Observer 1", sentimentAnalyzer);
        IObservable<Article> filtriraniStream = articleStream;
        IDisposable subscription1 = filtriraniStream.Subscribe(observer1);

        articleStream.GetArticles();
        Console.ReadLine();

        subscription1.Dispose();

    }
}