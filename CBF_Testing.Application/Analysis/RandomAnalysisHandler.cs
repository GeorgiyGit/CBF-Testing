using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Filters;
using CBF_Testing.Domain.DTOs.Analysis;
using CBF_Testing.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Application.Analysis
{
    public class RandomAnalysisHandler(CBFTestingDbContext dbContext) : IRequestHandler<RandomAnalysis, DTResponse>
    {
        private readonly CBFTestingDbContext _dbContext = dbContext;

        public async Task<DTResponse> Handle(RandomAnalysis request, CancellationToken cancellationToken)
        {
            Random rand = new Random();
            double percents = 0.7;
            var users = await _dbContext.Users.Take(100)
                                              .Include(e => e.RatingFeedbacks)
                                                .ThenInclude(e => e.Anime)
                                                    .ThenInclude(e => e.Type)
                                              .Include(e => e.RatingFeedbacks)
                                                .ThenInclude(e => e.Anime)
                                                    .ThenInclude(e => e.Genres)
                                              .ToListAsync(cancellationToken);

            var genres = await _dbContext.Genres.Select(e => e.Name).ToArrayAsync(cancellationToken);

            List<double> RSME_All = new List<double>(users.Count);
            List<double> RSME_10_All = new List<double>(users.Count);
            List<double> F1Score_All = new List<double>(users.Count);
            List<double> F110Score_All = new List<double>(users.Count);

            List<long> buildTimes = new List<long>();
            List<long> predictTimes = new List<long>();

            foreach (var user in users)
            {
                if (user.RatingFeedbacks.Count <= 1)
                {
                    continue;
                }
                int index = (int)(percents * user.RatingFeedbacks.Count);
                var testData = user.RatingFeedbacks.Skip(index).ToList();

                var ratings = user.RatingFeedbacks.Select(e => e.Rating).ToArray();



                int[] randRatings = new int[ratings.Length];
                for(int i = 0; i < ratings.Length; i++)
                {
                    randRatings[i] = rand.Next(1, 11);
                }

                var actualCategories = ratings.Select(r => ConvertRatingToCategory(r)).ToArray();
                var predictedCategories = randRatings.Select(r => ConvertRatingToCategory(r)).ToArray();

                int[] rsmeActualCategories = ratings.Select(r => ConvertRatingTo3(r)).ToArray();
                int[] rsmePredictedCategories = randRatings.Select(r => ConvertRatingTo3(r)).ToArray();

                int sum = 0;
                int sum10 = 0;
                for (int i = 0; i < testData.Count; i++)
                {
                    int v = rsmeActualCategories[i] - rsmePredictedCategories[i];
                    v = v * v;
                    sum += v;

                    int v2 = ratings[i] - randRatings[i];
                    v2 = v2 * v2;
                    sum10 += v2;
                }
                double RSME = Math.Sqrt((double)sum / (double)testData.Count);
                RSME_All.Add(RSME);

                double RSME10 = Math.Sqrt((double)sum10 / (double)testData.Count);
                RSME_10_All.Add(RSME10);

                double f1Score = CalculateF1Score(actualCategories, predictedCategories);
                double f110Score = CalculateF1FullScore(ratings, randRatings);
                F1Score_All.Add(f1Score);
                F110Score_All.Add(f110Score);
            }
            double totalRSME = RSME_All.Sum() / RSME_All.Count;
            double totalF1Score = F1Score_All.Sum() / F1Score_All.Count;

            double totalRSME10 = RSME_10_All.Sum() / RSME_10_All.Count;
            double totalF1Score10 = F110Score_All.Sum() / F110Score_All.Count;

            return new DTResponse()
            {
                RSME = totalRSME,
                F1Score = totalF1Score,
                F110Score = totalF1Score10,
                RSME10 = totalRSME10,
                BuildTime = new TimeSpan(0),
                PredictTime = new TimeSpan(1)
            };
        }

        public static int ConvertRatingTo3(int rating)
        {
            if (rating >= 1 && rating <= 3)
                return 0;
            if (rating >= 4 && rating <= 6)
                return 1;
            return 2;
        }
        static string ConvertRatingToCategory(int rating)
        {
            if (rating >= 1 && rating <= 3)
                return "Low";
            if (rating >= 4 && rating <= 6)
                return "Medium";
            return "High";
        }
        static double CalculateF1Score(string[] actual, string[] predicted)
        {
            // Calculate Precision and Recall for each category: Low, Medium, High
            string[] categories = { "Low", "Medium", "High" };

            double precisionSum = 0, recallSum = 0, f1Sum = 0;
            int totalCategories = categories.Length;

            List<int> categoryWeights = new List<int>(3);
            List<double> f1Scores = new List<double>(3);

            foreach (var category in categories)
            {
                categoryWeights.Add(actual.Count(e => e.Equals(category)) + predicted.Count(e => e.Equals(category)));
                int truePositives = actual.Zip(predicted, (a, p) => a == category && p == category ? 1 : 0).Sum();
                int falsePositives = predicted.Count(p => p == category) - truePositives;
                int falseNegatives = actual.Count(a => a == category) - truePositives;

                if (truePositives == 0)
                {
                    f1Scores.Add(0);
                    continue;
                }

                double precision = (truePositives + 0.0) / (truePositives + falsePositives);
                double recall = (truePositives + 0.0) / (truePositives + falseNegatives);
                double f1 = 2 * (precision * recall) / (precision + recall);

                precisionSum += precision;
                recallSum += recall;
                f1Scores.Add(f1);
            }

            for (int i = 0; i < f1Scores.Count; i++)
            {
                f1Sum += f1Scores[i] * (double)categoryWeights[i];
            }
            return f1Sum / categoryWeights.Sum(); // Average F1 Score for all categories
        }

        static double CalculateF1FullScore(int[] actual, int[] predicted)
        {
            // Calculate Precision and Recall for each category: Low, Medium, High
            int[] categories = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            double precisionSum = 0, recallSum = 0, f1Sum = 0;
            int totalCategories = categories.Length;

            List<int> categoryWeights = new List<int>(10);
            List<double> f1Scores = new List<double>(10);

            foreach (var category in categories)
            {
                categoryWeights.Add(actual.Count(e => e.Equals(category)) + predicted.Count(e => e.Equals(category)));
                int truePositives = actual.Zip(predicted, (a, p) => a == category && p == category ? 1 : 0).Sum();
                int falsePositives = predicted.Count(p => p == category) - truePositives;
                int falseNegatives = actual.Count(a => a == category) - truePositives;

                if (truePositives == 0)
                {
                    f1Scores.Add(0);
                    continue;
                }

                double precision = (truePositives + 0.0) / (truePositives + falsePositives);
                double recall = (truePositives + 0.0) / (truePositives + falseNegatives);
                double f1 = 2 * (precision * recall) / (precision + recall);

                precisionSum += precision;
                recallSum += recall;
                f1Scores.Add(f1);
            }

            for (int i = 0; i < f1Scores.Count; i++)
            {
                f1Sum += f1Scores[i] * (double)categoryWeights[i];
            }
            return f1Sum / categoryWeights.Sum(); // Average F1 Score for all categories
        }
    }
}
