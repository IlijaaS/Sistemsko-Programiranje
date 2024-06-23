using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYTIMES_MP_Projekat3
{
    public class ArticleObserver : IObserver<Article>
    {
        private readonly string name;
        private readonly SentimentAnalyzer sentimentAnalyzer;
        public ArticleObserver(string name, SentimentAnalyzer sentimentAnalyzer)
        {
            this.name = name;
            this.sentimentAnalyzer = sentimentAnalyzer; 
        }
        public void OnNext(Article article)
        {
            var Sentiment = sentimentAnalyzer.Predict(article.Title, article.Abstract);
            Console.WriteLine($"{name}: {article.Title}! \n Sentiment {(Sentiment.Sentiment ? "Positive" : "Negative")}.\n");
        }
        public void OnError(Exception e)
        {
            Console.WriteLine($"{name}: Doslo je do greske: {e.Message}");
        }
        public void OnCompleted()
        {
            Console.WriteLine($"{name}: Uspesno vraceni svi clanci.");
        }
    }
}
