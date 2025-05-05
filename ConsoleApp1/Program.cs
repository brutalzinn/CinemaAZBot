using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ConsoleApp1.Models;

public class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static string apiKey = "";
    private static string baseUrl = "https://api.themoviedb.org/3";
    public static async Task Main(string[] args)
    {

        IConfigurationRoot config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .AddEnvironmentVariables()
           .Build();

        apiKey = config["TmdbApiKey"] ?? "";  
        baseUrl = config["TmdbBaseUrl"] ?? "";

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "films")
                {
                    // dotnet run getfilms
                    Console.WriteLine("Getting Films");
                    List<Movie> allMovies = await GetAllMovies(); 
                    string json = JsonConvert.SerializeObject(allMovies);
                    Console.WriteLine(json); //print json
                }
                else if (args[0].ToLower() == "sql")
                {
                    //dotnet run 
                    Console.WriteLine("Generating Insert Statements");
                    string json = File.ReadAllText("movies.json");

                    List<Movie> movies = JsonConvert.DeserializeObject<List<Movie>>(json);
                    List<string> sqlStatements = GenerateSqlInsertStatements(movies);
                    foreach (string sql in sqlStatements)
                    {
                        Console.WriteLine(sql);
                    }
                    File.WriteAllLines("insert_movies.sql", sqlStatements);
                    Console.WriteLine("SQL INSERT statements written to insert_movies.sql");
                }
                else
                {
                    Console.WriteLine("Invalid argument. Use 'films' or 'sql'.");
                }
            }
            else
            {
                Console.WriteLine("Please provide an argument: 'films' or 'sql'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task<List<Movie>> GetAllMovies()
    {
        int page = 1;
        int totalPages = 5;
        List<Movie> allMovies = new List<Movie>();
        do
        {
            var uriBuilder = new UriBuilder($"{baseUrl}/discover/movie");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["include_adult"] = "false";
            query["include_video"] = "false";
            query["language"] = "pt-BR";
            query["page"] = page.ToString();
            query["sort_by"] = "popularity.desc";
            uriBuilder.Query = query.ToString();
            string requestUrl = uriBuilder.ToString();
            Console.WriteLine($"Requesting URL: {requestUrl}"); 
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode(); 
            string json = await response.Content.ReadAsStringAsync();
            MovieResponse movieResponse = JsonConvert.DeserializeObject<MovieResponse>(json);
            allMovies.AddRange(movieResponse.results);
            page++;

        } while (page <= totalPages);

        Console.WriteLine("Finished retrieving all movie pages.");
        return allMovies;
    }

    static List<string> GenerateSqlInsertStatements(List<Movie> movies)
    {
        List<string> sqlStatements = new List<string>();
        foreach (Movie movie in movies)
        {
            string overview = movie.overview?.Replace("'", "''");

            string sql = $"INSERT INTO filme (nome, sinopse, nota, ano_lancamento) VALUES (" +
                    
                         $"'{movie.title.Replace("'", "''")}', " +
                         $"'{overview}', " +
                         $"{movie.vote_average}, " +
                         $"'{DateTime.Parse(movie.release_date).Year}') " + 
                         $"ON CONFLICT (id) DO NOTHING;";
            sqlStatements.Add(sql);
        }
        return sqlStatements;
    }
}
