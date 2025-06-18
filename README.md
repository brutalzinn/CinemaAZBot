---

# Projeto Educacional: Geração de SQL para Banco de Dados de Filmes

Este projeto foi desenvolvido para a **disciplina de Banco de Dados da Pontifícia Universidade Católica de Minas Gerais (PUC-MG)**. Seu objetivo é simples: **obter dados de filmes de uma API externa e gerar scripts SQL para popular um banco de dados local**.

---

## Tecnologias Utilizadas

A aplicação é um **aplicativo de console em C#** que interage com a **API do The Movie Database (TMDb)** para coletar as informações dos filmes.

---

## Objetivo Principal

O propósito central é **extrair dados de uma API externa (TMDb)** e **gerar instruções SQL INSERT** que permitam popular de forma eficiente um banco de dados local com informações de filmes, servindo como base didática para a compreensão do processo de ETL (Extract, Transform, Load).

---

## Como Usar

### Configuração Inicial

Antes de executar a aplicação, você precisa configurar sua **chave da API do TMDb** e, opcionalmente, a URL base da API.

1.  Crie um arquivo chamado `appsettings.json` na raiz do seu projeto (se ele já não existir).
2.  Adicione as seguintes chaves ao arquivo `appsettings.json`, substituindo `"SUA_CHAVE_API_TMDB"` pela sua chave real da API do TMDb:

    ```json
    {
      "TmdbApiKey": "SUA_CHAVE_API_TMDB",
      "TmdbBaseUrl": "https://api.themoviedb.org/3"
    }
    ```
    **Observação:** A `TmdbBaseUrl` é opcional, mas recomendada para garantir que a aplicação se conecte ao endpoint correto da API.

### Execução da Aplicação

Abra o **terminal** (ou Prompt de Comando/PowerShell no Windows, Terminal no macOS/Linux) e navegue até a pasta onde o executável `CinemaAZBot` (ou o arquivo `.dll` principal, se você estiver executando com `dotnet run`) está localizado.

A partir daí, você pode usar os seguintes comandos:

#### 1. Para buscar e salvar dados de filmes:

Este comando buscará filmes populares da API do TMDb e salvará os dados em formato JSON em um arquivo chamado `movies.json`.

```bash
CinemaAZBot films
```

* **Opção para número de páginas:** Você pode especificar quantas páginas de filmes deseja buscar:
    ```bash
    CinemaAZBot films --pages <numero_de_paginas>
    # Exemplo: CinemaAZBot films --pages 10
    ```

#### 2. Para buscar e salvar dados de gêneros:

Este comando buscará a lista de gêneros de filmes da API do TMDb e salvará os dados em formato JSON em um arquivo chamado `genres.json`.

```bash
CinemaAZBot genres
```

#### 3. Para gerar instruções SQL:

Este é o comando principal para gerar os scripts SQL a partir dos arquivos JSON (`movies.json` e `genres.json`) que você gerou anteriormente.

```bash
CinemaAZBot sql
```

* **Opção para gerar SQL de filmes:**
    ```bash
    CinemaAZBot sql --movies
    # Isso criará um arquivo SQL com instruções INSERT para a tabela de filmes.
    ```

* **Opção para gerar SQL de gêneros:**
    ```bash
    CinemaAZBot sql --genres
    # Isso criará um arquivo SQL com instruções INSERT para a tabela de gêneros.
    ```

* **Opção para gerar SQL de filmes E gêneros (incluindo a tabela de relacionamento):**
    ```bash
    CinemaAZBot sql --movies --genres
    # Isso criará TRÊS arquivos SQL:
    # 1. Um para a tabela de filmes.
    # 2. Um para a tabela de gêneros.
    # 3. Um para a tabela de relacionamento entre filmes e gêneros (muitos-para-muitos).
    ```

* **Opção para especificar o nome do arquivo de saída:**
    Você pode usar a opção `--output` para definir o nome base dos arquivos SQL gerados.

    * **Para um único tipo de SQL (filmes OU gêneros):**
        ```bash
        CinemaAZBot sql --movies --output meus_filmes
        # Isso criará um arquivo chamado meus_filmes.sql
        ```
    * **Para ambos (filmes E gêneros):**
        ```bash
        CinemaAZBot sql --movies --genres --output meu_projeto
        # Isso criará:
        #   meu_projeto_movies.sql
        #   meu_projeto_genres.sql
        #   meu_projeto_movie_genres.sql
        ```

---

## Saída dos Comandos

* O comando `films` exibirá os dados dos filmes no terminal em formato JSON e os salvará no arquivo `movies.json`.
* O comando `genres` exibirá os dados dos gêneros no terminal em formato JSON e os salvará no arquivo `genres.json`.
* O comando `sql` exibirá mensagens no terminal sobre o progresso da geração e **salvará as instruções SQL INSERT em arquivos `.sql`**, conforme as opções `--movies`, `--genres` e `--output` utilizadas.

---

**Importante:** Este projeto foi desenvolvido exclusivamente para fins educacionais no contexto da disciplina de Banco de Dados da PUC-MG e não possui nenhuma afiliação ou relação oficial com The Movie Database (TMDb) ou `https://www.themoviedb.org/`.

---