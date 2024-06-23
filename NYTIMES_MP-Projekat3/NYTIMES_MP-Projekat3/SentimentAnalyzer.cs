using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using static System.Net.Mime.MediaTypeNames;

namespace NYTIMES_MP_Projekat3
{
    public class SentimentData
    {
        [LoadColumn(0), ColumnName("Label")]
        public bool Sentiment { get; set; }

        [LoadColumn(1)]
        public string SentimentTitle{ get; set; }

        [LoadColumn(2)]
        public string SentimentAbstract{ get; set; }
    }

    public class SentimentPrediction : SentimentData
    {
        [ColumnName("PredictedLabel")]
        public bool Sentiment { get; set; }
    }

    public class SentimentAnalyzer
    {
        private readonly MLContext mlContext;
        private readonly ITransformer model;
        DataOperationsCatalog.TrainTestData splitData;
        public SentimentAnalyzer()
        {
            mlContext = new MLContext();
            model = TrainModel();
        }

        public ITransformer TrainModel()
        {
            var dataView = mlContext.Data.LoadFromTextFile<SentimentData>(
                path: "..\\..\\..\\sentiment.txt",
                hasHeader: false,
                separatorChar: ';');

            splitData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentTitle))
                .Append(mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentAbstract)))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(splitData.TrainSet);

            return model;
        }


        public SentimentPrediction Predict(string title, string @abstract)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            var result = predictionEngine.Predict(new SentimentData { SentimentTitle = title, SentimentAbstract = @abstract });
            return result;
        }
    }
}
