# PT 

Projeto para Fins Didáticos: Geração de Instruções SQL para Popular Banco de Dados de Filmes na PUC-MG
 Este projeto foi desenvolvido para a disciplina de Banco de Dados da Pontifícia Universidade Católica de Minas Gerais (PUC-MG).

Tecnologia Utilizada:

A aplicação utiliza a API do The Movie Database (TMDb) para obter os dados dos filmes e uma aplicação do tipo console em c#.

Objetivo:

O objetivo deste projeto é escavar uma api externa para popular uma base dados local de filmes

Gerar instruções SQL para inserir os dados em um banco de dados.

Uso:

Como usar
Configuração:

Configure sua chave da API do TMDb no arquivo appsettings.json. Exemplo:

    {
    "TmdbApiKey": "SUA_CHAVE_API_TMDB"
    }

    Opcionalmente, você pode especificar a URL base da API do TMDb no mesmo arquivo:

    {
    "TmdbBaseUrl": "https://api.themoviedb.org/3"
    }

Execução:

Abra o terminal na pasta onde o executável CinemaAZBot está localizado.

Para buscar filmes:

    CinemaAZBot films

Para gerar instruções SQL:

    CinemaAZBot sql


Saída
O comando films exibirá os dados dos filmes no terminal, em formato JSON.

O comando sql exibirá as instruções SQL INSERT no terminal e também as salvará em um arquivo chamado insert_movies.sql.

Este projeto foi desenvolvido exclusivamente para fins educacionais, no contexto da disciplina de Banco de Dados da PUC-MG e não possui nenhuma relação com https://www.themoviedb.org/.

# EN 

Consider "Educational Project: Generating SQL Instructions to Populate a Movie Database at PUC-MG
This project was developed for the Database course at the Pontifícia Universidade Católica de Minas Gerais (PUC-MG).

Technology Used:

The application uses The Movie Database (TMDb) API to obtain movie data and a C# console app.

Objective:

The objective of this project is to scrape an external API to populate a local movie database.

Generate SQL instructions to insert the data into a database.

Usage:

How to use
Configuration:

Configure your TMDb API key in the appsettings.json file. Example:

JSON

{
  "TmdbApiKey": "YOUR_TMDB_API_KEY"
}
Optionally, you can specify the base URL of the TMDb API in the same file:

JSON

{
  "TmdbBaseUrl": "https://api.themoviedb.org/3"
}
Execution:

Open the terminal in the folder where the CinemaAZBot executable is located.

To search for movies:

    CinemaAZBot films

To generate SQL instructions:

    CinemaAZBot sql


The sql command will display the SQL INSERT instructions in the terminal and also save them to a file named insert_movies.sql.

This project was developed exclusively for educational purposes within the context of the Database course at PUC-MG and we dont affiliate any with https://www.themoviedb.org/