using System.Diagnostics;

namespace MoogleEngine;

class SearchItemPLus : IComparable<SearchItemPLus>
{
    public SearchItemPLus(string title, string snippet, float score, List<string> WordsInDoc, int pathId)
    {
        this.Title = title;
        this.Snippet = snippet;
        this.Score = score;
        this.WordsInDoc = WordsInDoc;
        this.PathId = pathId;

    }

    public string Title { get; set; }
    public string Snippet { get; set; }
    public float Score { get; set; }
    public List<string> WordsInDoc { get; set; }
    public int PathId { get; set; }

    // Para poder ordenar los documentos por importancia en el ranking
    public int CompareTo(SearchItemPLus? other)
    {
        if (other == null) return 1;

        // Si el documento que se esta verificando posee menor cantidad de palabras de la query que el anterior se encontrara en una posicion inferior a este
        if (this.WordsInDoc.Count > other.WordsInDoc.Count) return -1;

        // Si el documento que se esta verificando posee mayor cantidad de palabras de la query que el anterior se encontrara en una posicion superior a este
        else if (this.WordsInDoc.Count < other.WordsInDoc.Count) return 1;

        // Si la cantidad de palabras de la query de ambos documentos es igual se verificara el score
        else
        {
            // Si el score del documento que se esta verificando es menor que el anterior se encontrara en una posicion inferior
            if (this.Score > other.Score) return -1;

            // Si el score del documento que se esta verificando es mayor que el anterior se encontrara en una posicion superior
            else if (this.Score < other.Score) return 1;

            // Si son iguales se mantienen en el oreden por defecto
            else return 0;
        }

    }

}


public static class Moogle
{
    static LoadData data = new LoadData();

    // Inicializa la base de datos antes de comenzar la busqueda
    public static bool dataloaded = false;
    public static void InitData()
    {
        if (!dataloaded)
        {
           Stopwatch timeMeasure = new Stopwatch();
           timeMeasure.Start();

            data.Initialize();
            dataloaded = true;
           
           timeMeasure.Stop();
           var time = timeMeasure.Elapsed.Duration();
           Console.WriteLine($"Tiempo:{timeMeasure.Elapsed.TotalMinutes} min");
        }

    }

    public static SearchResult Query(string query)
    {
        // Crea una lista de string con las palabras de la query normalizada y con los operadores si los posee
        List<string> queryListed = Normalize.NormalizeQuery(query);

        // Crea una lista de searcher que tendra el pathId, el nombre del archivo, el numero de ocurrencias de la palabra en el documento y su tfIdf en el documento
        List<Searcher> results = new List<Searcher>();

        // Crea una lista de SearchItemPLus que va a tener el tiulo del documento, un snippet, un score, la cantidad de palabras de la query que poseee y el pathId del documento
        List<SearchItemPLus> finalResults = new List<SearchItemPLus>();

        // Crea una lista de Operators que va a contener las palabras de la query que tenian operadores, si el operador es no debe aparecer o de debe aparecer y el peso de la palabra en el documento por la importancia que posee  
        List<Operators> wordsWithOperators = new List<Operators>();

        // Crea una lista de palabras a sugerir en caso de que las palabras de la query no tenga resultados
        List<string> suggestions = new List<string>();

        string title = "";
        float score = 0;
        string snippet = "";
        int pathId = 0;

        // Se buscara por cada palabra de la query 
        foreach (var queryWord in queryListed)
        {
            Operators myOperators = new Operators();

            string word = queryWord;

            // Verificar si existen operadores en la query
            switch (myOperators.ExistOperator(queryWord))
            {
                // En caso de que los operadores estan mal escritos dejara de buscar y enviara un mensaje en la pagina
                case -1:
                    List<SearchItem> mySearchItem = new List<SearchItem>();
                    mySearchItem.Add(new SearchItem("Error. Mal uso de los operadores", "Para usarlo correctamente debes poner ! o ^ delante de una palabra, pero nunca delante o detrás de otro operador", score));
                    return new SearchResult(mySearchItem.ToArray(), query);

                // En caso de que existan los operadores se devuelve la palabra sin estos
                case 1:
                    word = myOperators.Word;
                    wordsWithOperators.Add(myOperators);
                    break;

            }

            // Llena la lista results de Searcher llamando al metodo Search sobre la palabra
            results = data.Search(word);
            // Si no se encuentran documentos que contengan la query se buscan palabras que sean similares para sugerir al usuario
            if (results.Count == 0)
            {
                suggestions =  suggestions.Union(LevenshteinMethod.FindSimilarities(word, data.wordsDictionary).ToList()).ToList();
            }

            else
                // Recorre cada elemento de la lista y devuelve los datos de los documentos que contienen la palabra
                foreach (var item in results)
                {
                    title = item.fileName;

                    pathId = item.pathId;

                    Doc myDoc = new Doc();

                    myDoc.fileName = item.fileName;

                    string myFilePathstr = data.directoryNames.First(x => x.pathId == item.pathId).filePath;

                    snippet = myDoc.Snippet(word, myFilePathstr + "\\" + item.fileName + ".txt");

                    // Se toma el indice del documento y el camino
                    int index = finalResults.FindIndex(item => item.Title + item.PathId == title + pathId);
                    // Si el indice existe suma los tfIdf de la palabras de la query que aparecen en él y agrega el peso del operador * de existir y añade la palabra a la lista de palabras de la query que posee el documento 
                    if (index >= 0)
                    {
                        score += item.tfIdf + myOperators.Weight;
                        finalResults[index].Score = score;
                        finalResults[index].WordsInDoc.Add(word);

                    }
                    // Sino lo agrega, añade su tfIdf y agrega el peso del operador * de existir y añade la palabra a la lista de palabras de la query que posee
                    else
                    {
                        score = item.tfIdf + myOperators.Weight;
                        // Crea una lista de string con las palabras de la query que hay en cada documento
                        List<string> newWordsInDoc = new List<string>();
                        newWordsInDoc.Add(word);

                        finalResults.Add(new SearchItemPLus(title, snippet, score, newWordsInDoc, pathId));
                    }

                }

        }
        // Si no se encuentran documentos que contengan ninguna de las palabras de la query 
        if (finalResults.Count == 0)
        {
            List<SearchItem> mySearchItem = new List<SearchItem>();
            mySearchItem.Add(new SearchItem("Búsqueda nula", "Lea las sugerencias", score));
            return new SearchResult(mySearchItem.ToArray(), String.Join(", ", suggestions));
        }

        // Recorre las palabras con operadores
        foreach (var item in wordsWithOperators)
        {
            // Lista de SearchItemPLus para clonar el contenido de finalResults para poder recorrerlo y remover los documnetos segun el criterio del operador 
            List<SearchItemPLus> finalResultsTemp = new List<SearchItemPLus>(finalResults);
            foreach (var result in finalResultsTemp)
            {
                // Si la palabra poseia ! entonces se remueven todos los documentos que contienen la palabra
                if (item.Exclude == 1)
                {
                    if (result.WordsInDoc.Contains(item.Word))
                        finalResults.Remove(result);
                }
                // Si la palabra poseia ^ entonces se remueven todos los documentos que no contienen la palabra
                else if (item.Include == 1)
                {
                    if (!result.WordsInDoc.Contains(item.Word))
                        finalResults.Remove(result);
                }

            }
        }

        finalResults.Sort();

        // Lista de SearchItem para guardar el titulo, snippet y score de los documentos 
        List<SearchItem> end = new List<SearchItem>();
        foreach (var result in finalResults)
        {
            score = result.Score;
            title = result.Title;
            snippet = result.Snippet;
            end.Add(new SearchItem(title, snippet, score));
        }
        return new SearchResult(end.ToArray(), query);
    }

}