using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Filters;
using CBF_Testing.Domain.DTOs.Analysis;
using CBF_Testing.Domain.Entities;
using CBF_Testing.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Application.Analysis
{
    public class DecisionTreeAnalysisHandler(CBFTestingDbContext dbContext) : IRequestHandler<DecisionTreeAnalysis, DTResponse>
    {
        private readonly CBFTestingDbContext _dbContext = dbContext;

        public async Task<DTResponse> Handle(DecisionTreeAnalysis request, CancellationToken cancellationToken)
        {
            double percents = 0.7;
            var users = await _dbContext.Users.Take(100)
                                              .Include(e => e.RatingFeedbacks)
                                                .ThenInclude(e => e.Anime)
                                                    .ThenInclude(e=>e.Type)
                                              .Include(e=>e.RatingFeedbacks)
                                                .ThenInclude(e=>e.Anime)
                                                    .ThenInclude(e=>e.Genres)
                                              .ToListAsync(cancellationToken);

            var genres = await _dbContext.Genres.Select(e=>e.Name).ToArrayAsync(cancellationToken);

            List<double> RSME_All = new List<double>(users.Count);
            List<double> F1Score_All = new List<double>(users.Count);
            foreach (var user in users)
            {
                var ratings = user.RatingFeedbacks.ToList();
                if (ratings.Count <= 1) continue;
                ratings.Shuffle();
                int index = (int)(percents * ratings.Count);

                var trainedRatingsData = ratings.Take(index).ToList();
                var trainingData = trainedRatingsData.Select(e => e.Anime).ToList();

                int[] output = trainedRatingsData.Select(e => e.Rating).ToArray();

                bool[] allowedRatings = new bool[10];
                for (int i = 0; i < 10; i++)
                {
                    if (output.Contains(i + 1)) allowedRatings[i] = true;
                }

                List<string> possibleGenres = new List<string>();
                double[][] oneHotGenres = OneHotEncodeGenres(trainingData, possibleGenres);

                // Step 3: Extract other features
                double[][] otherFeatures = trainingData.Select(a => new double[]
                {
                    a.EpisodesCount,
                    a.Rating,
                    a.Members
                }).ToArray();


                // Step 4: Combine features (genres + other features)
                double[][] inputs = CombineFeatures(otherFeatures, oneHotGenres);

                // Step 5: Encode categorical features (e.g., Type)
                string[] types = trainingData.Select(a => a.Type.Name).ToArray();
                Codification codebook = new Codification("Type", types);
                int[] encodedTypes = codebook.Transform("Type", types);

                // Add encoded Type to inputs
                if (!encodedTypes.All(e => e == encodedTypes[0]))
                {
                    inputs = AddColumnToInputs(inputs, encodedTypes);
                }

                // Step 6: Train the C4.5 Decision Tree
                var teacher = new C45Learning()
                {
                    MaxHeight = 5,
                };

                for(int i = 0; i < inputs[0].Length; i++)
                {
                    var flag = true;
                    var f = inputs[0][i];
                    for(int j = 0; j < inputs.Length; j++)
                    {
                        if (inputs[j][i] != f)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if(flag)
                    {
                        Console.WriteLine("All elements in column are equal");
                    }
                }

                DecisionTree tree = teacher.Learn(inputs, output);

                // Step 7: Evaluate the model
                double error = new ZeroOneLoss(output).Loss(tree.Decide(inputs));
                Console.WriteLine($"Accuracy: {1.0 - error}");


                var testData = ratings.Skip(index).ToList();
                if (testData.Count <= 1) continue;
                var testAnime = testData.Select(e => e.Anime).ToList();

                int[] testOutput = testData.Select(e => e.Rating).ToArray();
                for (int i = 0; i < testOutput.Length; i++)
                {
                    if (!allowedRatings[testOutput[i] - 1])
                    {
                        testOutput[i] = AdjustRating(allowedRatings, testOutput[i]);
                    }
                    if (!types.Contains(testAnime[i].Type.Name))
                    {
                        testAnime[i].Type.Name = types[0];
                    }
                }

                double[][] testOneHotGenres = TestOneHotEncodeGenres(testAnime, possibleGenres);

                // Step 3: Extract other features
                double[][] testOtherFeatures = testAnime.Select(a => new double[]
                {
                    a.EpisodesCount,
                    a.Rating,
                    a.Members
                }).ToArray();

                // Step 4: Combine features (genres + other features)
                double[][] testInputs = CombineFeatures(otherFeatures, oneHotGenres);

                string[] testTypes = trainingData.Select(a => a.Type.Name).ToArray();
                Codification testCodebook = new Codification("Type", types);
                int[] testEncodedTypes = codebook.Transform("Type", types);

                // Add encoded Type to inputs
                testInputs = AddColumnToInputs(testInputs, testEncodedTypes);

                List<int> predictions = new List<int>(testData.Count);
                for (int i = 0; i < testAnime.Count; i++)
                {
                    predictions.Add(tree.Decide(testInputs[i]));
                }

                var actualCategories = testOutput.Select(r => ConvertRatingToCategory(r)).ToArray();
                var predictedCategories = predictions.Select(r => ConvertRatingToCategory(r)).ToArray();

                int[] rsmeActualCategories = testOutput.Select(r => ConvertRatingTo3(r)).ToArray();
                int[] rsmePredictedCategories = predictions.Select(r => ConvertRatingTo3(r)).ToArray();

                int sum = 0;
                for (int i = 0; i < predictions.Count; i++)
                {
                    int v = rsmeActualCategories[i] - rsmePredictedCategories[i];
                    v = v * v;
                    sum += v;
                }
                double RSME = Math.Sqrt((double)sum / (double)predictions.Count);
                RSME_All.Add(RSME);

                double f1Score = CalculateF1Score(actualCategories, predictedCategories);
                //double f1Score = CalculateF1FullScore(testOutput, predictions.ToArray());
                F1Score_All.Add(f1Score);
            }
            double totalRSME = RSME_All.Sum() / RSME_All.Count;
            double totalF1Score = F1Score_All.Sum() / F1Score_All.Count;
            return new DTResponse()
            {
                RSME = totalRSME,
                F1Score = totalF1Score
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
        public static int AdjustRating(bool[] allowedRatings, int rating)
        {
            if (allowedRatings == null || allowedRatings.Length != 10)
                throw new ArgumentException("allowedRatings must be a boolean array of size 10.");

            if (rating < 1 || rating > 10)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 10.");

            if (allowedRatings[rating - 1])
                return rating;

            int left = rating - 1;
            int right = rating - 1;

            while (left >= 0 || right < 10)
            {
                if (left >= 0 && allowedRatings[left])
                    return left + 1;

                if (right < 10 && allowedRatings[right])
                    return right + 1;

                left--;
                right++;
            }

            throw new InvalidOperationException("No allowed ratings available.");
        }
        // Convert Rating to Categories (Low, Medium, High)
        static string ConvertRatingToCategory(int rating)
        {
            if (rating >= 1 && rating <= 3)
                return "Low";
            if (rating >= 4 && rating <= 6)
                return "Medium";
            return "High";
        }

        // Calculate F1 Score for multiple categories (Low, Medium, High)
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
        // One-Hot Encode Genres
        double[][] OneHotEncodeGenres(List<Anime> animeData, List<string> possibleGenres)
        {
            List<string> genres = new List<string>();
            foreach (var anime in animeData)
            {
                foreach (var genre in anime.Genres)
                {
                    if (!genres.Contains(genre.Name))
                    {
                        genres.Add(genre.Name);
                    }
                }
            }
            double[][] res = new double[animeData.Count][];

            List<string> genresForDelete = new List<string>();
            foreach(var genre in genres)
            {
                bool flag = true;
                for(int i = 0; i < animeData.Count; i++)
                {
                    if (animeData[i].Genres.FirstOrDefault(e => e.Name == genre) == null)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag) genresForDelete.Add(genre);
            }

            foreach (var genre in genresForDelete) genres.Remove(genre);

            for (int i = 0; i < animeData.Count; i++)
            {
                res[i] = new double[genres.Count];
                foreach(var genre in animeData[i].Genres)
                {
                    var ind = genres.IndexOf(genre.Name);
                    if (ind >= 0)
                    {
                        res[i][ind] = 1;
                    }
                }
            }
            possibleGenres.AddRange(genres);
            return res;
        }

        double[][] TestOneHotEncodeGenres(List<Anime> animeData, List<string> possibleGenres)
        {
            double[][] res = new double[animeData.Count][];
            for (int i = 0; i < animeData.Count; i++)
            {
                res[i] = new double[possibleGenres.Count];
                foreach (var genre in animeData[i].Genres)
                {
                    var ind = possibleGenres.IndexOf(genre.Name);
                    if (ind >= 0)
                    {
                        res[i][ind] = 1;
                    }
                }
            }
            return res;
        }
        // Combine Features
        double[][] CombineFeatures(double[][] features1, double[][] features2)
        {
            return features1.Zip(features2, (f1, f2) => f1.Concat(f2).ToArray()).ToArray();
        }

        // Add Encoded Column to Inputs
        double[][] AddColumnToInputs(double[][] inputs, int[] column)
        {
            double[][] newInputs = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
            {
                newInputs[i] = inputs[i].Concat(new double[] { column[i] }).ToArray();
            }
            return newInputs;
        }
    }
}
