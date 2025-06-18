using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ConsoleApp1.Models;
using Models;
using System.CommandLine;
using System.Text.Json;
using System.CommandLine.Invocation;


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

        var rootCommand = new RootCommand("A CLI tool for movie data operations.");

        var filmsCommand = new Command("films", "Gets movie data from an API and saves to movies.json.");
        var totalPagesOption = new Option<int>(
            name: "--pages",
            description: "Number of pages to retrieve.",
            getDefaultValue: () => 5
        );
        filmsCommand.AddOption(totalPagesOption);

        filmsCommand.SetHandler(async (context) =>
        {
            var pages = context.ParseResult.GetValueForOption(totalPagesOption);
            Console.WriteLine($"Getting Films with {pages} pages...");
            List<Movie> allMovies = await GetAllMovies(pages);
            string json = JsonSerializer.Serialize(allMovies, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("Films data fetched and serialized.");
            await File.WriteAllTextAsync("movies.json", json);
            Console.WriteLine("Films data written to movies.json");
        });

        var genresCommand = new Command("genres", "Gets movie genres from an API and saves to genres.json.");
        genresCommand.SetHandler(async () =>
        {
            Console.WriteLine("Getting Genres...");
            List<Genre> allGenres = await GetAllGenres();
            string json = System.Text.Json.JsonSerializer.Serialize(allGenres, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("Genres data fetched and serialized.");
            await File.WriteAllTextAsync("genres.json", json);
            Console.WriteLine("Genres data written to genres.json");
        });

        var sqlCommand = new Command("sql", "Generates SQL INSERT statements.");

        var outputBaseNameOption = new Option<string>(
            name: "--output",
            description: "Base name or full path for the output SQL file(s). Defaults to 'insert_statements'. If both --movies and --genres are used, it acts as a prefix (e.g., 'yourname_movies.sql', 'yourname_genres.sql').",
            getDefaultValue: () => "insert_statements" // Default base name
        );

        var generateMoviesOption = new Option<bool>(
            name: "--movies",
            description: "Generate SQL INSERT statements for movie data (from movies.json).",
            getDefaultValue: () => false // Default to false
        );

        var generateGenresOption = new Option<bool>(
            name: "--genres",
            description: "Generate SQL INSERT statements for genre data (from genres.json).",
            getDefaultValue: () => false // Default to false
        );

  

        sqlCommand.AddOption(generateMoviesOption);
        sqlCommand.AddOption(generateGenresOption);
        sqlCommand.AddOption(outputBaseNameOption);

        sqlCommand.SetHandler(async (InvocationContext context) =>
        {
            bool generateMovies = context.ParseResult.GetValueForOption(generateMoviesOption);
            bool generateGenres = context.ParseResult.GetValueForOption(generateGenresOption);
            string outputBaseName = context.ParseResult.GetValueForOption(outputBaseNameOption);

            if (!generateMovies && !generateGenres)
            {
                Console.WriteLine("No SQL generation option specified. Use --movies or --genres.");
                context.ExitCode = 1;
                return;
            }

            Console.WriteLine("Generating SQL INSERT statements...");

            string movieSqlFile = "";
            string genreSqlFile = "";
            string movieGenreSqlFile = ""; // For the combined scenario

            if (generateMovies && generateGenres)
            {
                // Both are requested, generate three files: movies, genres, and movie_genres
                movieSqlFile = Path.Combine(Path.GetDirectoryName(outputBaseName) ?? "", Path.GetFileNameWithoutExtension(outputBaseName) + "_movies.sql");
                genreSqlFile = Path.Combine(Path.GetDirectoryName(outputBaseName) ?? "", Path.GetFileNameWithoutExtension(outputBaseName) + "_genres.sql");
                movieGenreSqlFile = Path.Combine(Path.GetDirectoryName(outputBaseName) ?? "", Path.GetFileNameWithoutExtension(outputBaseName) + "_movie_genres.sql");
                Console.WriteLine("Both --movies and --genres options detected. Generating separate files for movies, genres, and a combined file for movie-genre relationships.");
            }
            else if (generateMovies)
            {
                movieSqlFile = Path.Combine(Path.GetDirectoryName(outputBaseName) ?? "", Path.GetFileNameWithoutExtension(outputBaseName) + "_movies.sql");
                Console.WriteLine("--movies option detected. Generating movie SQL only.");
            }
            else if (generateGenres)
            {
                genreSqlFile = Path.Combine(Path.GetDirectoryName(outputBaseName) ?? "", Path.GetFileNameWithoutExtension(outputBaseName) + "_genres.sql");
                if (!genreSqlFile.EndsWith(".sql", StringComparison.OrdinalIgnoreCase)) genreSqlFile += ".sql";
                Console.WriteLine("--genres option detected. Generating genre SQL only.");
            }

            if (generateMovies)
            {
                Console.WriteLine($"Generating SQL for movies to {movieSqlFile}...");
                try
                {
                    string json = await File.ReadAllTextAsync("movies.json");
                    List<Movie> movies = JsonSerializer.Deserialize<List<Movie>>(json);
                    List<string> movieSqlStatements = GenerateMovieSQL(movies);
                    if (movieSqlStatements.Any())
                    {
                        await File.WriteAllLinesAsync(movieSqlFile, movieSqlStatements);
                        Console.WriteLine($"Movie SQL INSERT statements written to {movieSqlFile}");
                    }
                    else
                    {
                        Console.WriteLine("No movie SQL statements were generated.");
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Warning: movies.json not found. Skipping movie SQL generation.");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing movies.json: {ex.Message}. Skipping movie SQL generation.");
                }
                Console.WriteLine();
            }

            if (generateGenres)
            {
                Console.WriteLine($"Generating SQL for genres to {genreSqlFile}...");
                try
                {
                    string json = await File.ReadAllTextAsync("genres.json");
                    List<Genre> genres = JsonSerializer.Deserialize<List<Genre>>(json); // Directly deserialize to List<Genre>
                    List<string> genreSqlStatements = GenerateGenresSQL(genres);
                    if (genreSqlStatements.Any())
                    {
                        await File.WriteAllLinesAsync(genreSqlFile, genreSqlStatements);
                        Console.WriteLine($"Genre SQL INSERT statements written to {genreSqlFile}");
                    }
                    else
                    {
                        Console.WriteLine("No genre SQL statements were generated.");
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Warning: genres.json not found. Skipping genre SQL generation.");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing genres.json: {ex.Message}. Skipping genre SQL generation.");
                }
                Console.WriteLine();
            }

            if (generateMovies && generateGenres)
            {
                Console.WriteLine($"Generating SQL for movie-genre relationships to {movieGenreSqlFile}...");
                try
                {
                    string moviesJson = await File.ReadAllTextAsync("movies.json");
                    List<Movie> movies = JsonSerializer.Deserialize<List<Movie>>(moviesJson);

                    List<string> movieGenreSqlStatements = GenerateMoviesGenresSQL(movies);
                    if (movieGenreSqlStatements.Any())
                    {
                        await File.WriteAllLinesAsync(movieGenreSqlFile, movieGenreSqlStatements);
                        Console.WriteLine($"Movie-Genre relationship SQL INSERT statements written to {movieGenreSqlFile}");
                    }
                    else
                    {
                        Console.WriteLine("No movie-genre relationship SQL statements were generated.");
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"Warning: Missing file for movie-genre generation ({ex.FileName}). Skipping combined SQL generation.");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing JSON for movie-genre generation: {ex.Message}. Skipping combined SQL generation.");
                }
                Console.WriteLine();
            }
        });

        rootCommand.AddCommand(filmsCommand);
        rootCommand.AddCommand(genresCommand);
        rootCommand.AddCommand(sqlCommand);

        await rootCommand.InvokeAsync(args);
    }


    static async Task<List<Movie>> GetAllMovies(int totalPages = 5)
    {
        int page = 1;
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
            MovieResponse movieResponse = JsonSerializer.Deserialize<MovieResponse>(json);
            allMovies.AddRange(movieResponse.results);
            page++;

        } while (page <= totalPages);

        Console.WriteLine("Finished retrieving all movie pages.");
        return allMovies;
    }

    static async Task<DetailsResponse> GetMovieDetails(Movie movie)
    {
      
        var uriBuilder = new UriBuilder($"{baseUrl}/movie/{movie.id}?language=pt-BR");
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["language"] = "pt-BR";
        uriBuilder.Query = query.ToString();
        string requestUrl = uriBuilder.ToString();
        Console.WriteLine($"Requesting URL: {requestUrl}");
        HttpResponseMessage response = await client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        DetailsResponse detailsResponse = JsonSerializer.Deserialize<DetailsResponse>(json);
        Console.WriteLine($"Finished retrieving movie details for {movie.id} .");
        return detailsResponse;
    }

    static async Task<List<Genre>> GetAllGenres()
    {

        var uriBuilder = new UriBuilder($"{baseUrl}/genre/movie/list?language=pt-BR");
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["language"] = "pt-BR";
        uriBuilder.Query = query.ToString();
        string requestUrl = uriBuilder.ToString();
        Console.WriteLine($"Requesting URL: {requestUrl}");
        HttpResponseMessage response = await client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        GenreResponse genreResponse = JsonSerializer.Deserialize<GenreResponse>(json);
        Console.WriteLine($"Finished retrieving all genres.");
        return genreResponse.genres;
    }

    static List<string> GenerateMovieSQL(List<Movie> movies)
    {
        List<string> sqlStatements = new List<string>();
        foreach (Movie movie in movies) { 
            string overview = movie.overview?.Replace("'", "''");

            string sql = $"INSERT INTO Filmes (Id, Nome, Sinopse, Nota, AnoLancamento, BackgroundImgUrl, CoverImgUrl) VALUES (" +

                         $"'{movie.id}', " +
                         $"'{movie.title.Replace("'", "''")}', " +
                         $"'{overview}', " +
                         $"{movie.vote_average.ToString().Replace(",", ".")}, " +
                         $"{DateTime.Parse(movie.release_date).Year}," +
                         $"'https://image.tmdb.org/t/p/w500{movie.backdrop_path}'," +
                         $"'https://image.tmdb.org/t/p/w500{movie.poster_path}'" +
                         $");";
            sqlStatements.Add(sql);
        }
        return sqlStatements;
    }

    static List<string> GenerateGenresSQL(List<Genre> genres)
    {
        List<string> sqlStatements = new List<string>();
        foreach(Genre genre in genres)
        {
            string sql = $"INSERT INTO Genres (Id, Name) VALUES (" +
                      $"'{genre.id}'," +
                      $"'{genre.name}'" +
                      $");";
            sqlStatements.Add(sql);
        }

        return sqlStatements;
    }

    static  List<string> GenerateMoviesGenresSQL(List<Movie> movies)
    {
        List<string> sqlStatements = new List<string>();
        foreach (Movie movie in movies)
        {
            var genres = movie.genre_ids;
            foreach (var genre in genres)
            {
                string sql = $"INSERT INTO MovieGenres (GenresId, MoviesId) VALUES (" +
                        $"'{genre}'," +
                        $"'{movie.id}'" +
                        $");";
                sqlStatements.Add(sql);
            }
        }
        return sqlStatements;
    }
}
