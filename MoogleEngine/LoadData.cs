using System;
using System.Text.RegularExpressions;
namespace MoogleEngine;
class Doc
{
    public int pathId;
    public string fileName = "";
    public int wordsNumber;

    // Metodo para calcular el substring que contendra el snippet
    public string Snippet(string word, string filePath)
    {
        // Se lee el documento, se llevan a miniscula las letras y se busca el indice de la palabra q necesitamos q este en el snippet
        string content = File.ReadAllText(filePath);
        string contentLower = content.ToLower();
        
        // Se busca el indice de la palabra aisalada en el documento
        int index = Regex.Match(contentLower, @"\b"+word+ @"\b").Index;
        
        int start = 0;
        int end = 0;
        int distance = 50;

        // Si la palabra existe en el documento se procede a buscar el contenido del texto a la derecha e izquierda de esta
        if (index >= 0)
        {
            // Si la palabra se encuentra a en una posicion inferior a la distancia que queremos recorrer entonces se leera el documento desde la primera posicion
            if (index < distance) start = 0;
            //  De lo contrario se comienza a leer el documento a partir de la diferecia entre la posicion de la palabra y la distancia que se desea
            else start = index - distance;
            // Si la palabra se encuentra en una posicion q al sumarla con su tamaño y la distancia deseada sea mayor q el tamaño del documento entonces se leera hasta el final del documento
            if (word.Length - 1 + index + distance > content.Length) end = content.Length;
            // De lo contrario se leera hasta la suma de la distacia, la posicion y el tamaño de la palabra
            else end = word.Length - 1 + index + distance;
        }

        return content.Substring(start, end - start);
    }

}
class directoryName
{
    public int pathId;
    public string filePath = "";
}
class Searcher
{
    public int pathId;
    public string fileName = "";
    public int numberOfOccurrences;
    public float tfIdf;
}
class WordDetails
{
    public int numberOfOccurrences;
    public float tfIdf;
}
class LoadData
{
    //Obtener la lista de los nombres de los documentos por su ruta
    List<string> fileNamePaths = new List<string>();

    //Diccionario de una palabra y un diccionario de WordDetails
    public Dictionary<string, Dictionary<int, WordDetails>> wordsDictionary = new Dictionary<string, Dictionary<int, WordDetails>>();

    //Diccionario de WordDetails q va a tener la id de un doc y una clase con el # de veces q aparece una palabra, la tf y la idf
    public Dictionary<int, WordDetails> docsDictionary = new Dictionary<int, WordDetails>();

    //Diccionario de Doc q va a tener un indice con su id y una clase con el id de su path, el nombre y el numero de palabras
    public Dictionary<int, Doc> documents = new Dictionary<int, Doc>();

    // Lista de directoryName que va a tener el pathId y su path
    public List<directoryName> directoryNames = new List<directoryName>();

    //Obtener la ruta de los documentos
    string GetMainPath()
    {
        var outPutDirectory = AppContext.BaseDirectory;
        string icon_path = Path.Combine(outPutDirectory, "..\\..\\..\\..\\Content");
        return icon_path;
    }

    //Obtener los nombres de los documentos sin extension
    string GetFileName(string fileNamePath)
    {
        string fileName = "";

        fileName = Path.GetFileNameWithoutExtension(fileNamePath);

        return fileName;
    }

    // Inicializa la data
    public void Initialize()
    {
        // Obtener en una lista las rutas de los nombres de los documnetos con su extensio txt
        fileNamePaths = Directory.GetFiles(GetMainPath(), "*.txt", SearchOption.AllDirectories).ToList();

        int docId = 0;

        int docsNumber = 0;

        int currentFileNameID = 0;

        string currentFilePath = "";

        // Recorre cada ruta de los nombres de los documentos
        foreach (var fileNamePath in fileNamePaths)
        {
            // Obtiene la ruta del documento
            string filePath = Path.GetDirectoryName(fileNamePath) ?? "\\";

            // Siempre que este sea diferente a los que ya estan en la lista se modifica la lista directoryNames con su pathId y path
            if (currentFilePath != filePath)
            {
                currentFileNameID++;
                currentFilePath = filePath;
                directoryName currentDirectoryName = new directoryName();
                currentDirectoryName.pathId = currentFileNameID;
                currentDirectoryName.filePath = currentFilePath;
                directoryNames.Add(currentDirectoryName);
            }

            //Obtener el texto de cada documento normalizado
            List<string> wordsFromFile = Normalize.NormalizeDoc(fileNamePath);

            Doc currentDoc = new Doc();
            //Obtener valores de la clase por cada doc
            currentDoc.pathId = currentFileNameID;
            currentDoc.fileName = GetFileName(fileNamePath);
            currentDoc.wordsNumber = wordsFromFile.Count;

            //Agregando a la diccionario documents el identificador del documento
            documents.Add(docId, currentDoc);

            //Obtener cuantos docs hay 
            docsNumber = documents.Count;

            foreach (var word in wordsFromFile)
            {
                if (WordProcessor.Processor(word) == true)
                {
                    //Agregando o modificando en el diccinario la palabra
                    if (wordsDictionary.ContainsKey(word))
                    {
                        if (wordsDictionary[word].ContainsKey(docId))
                        {
                            wordsDictionary[word][docId].numberOfOccurrences += 1;
                        }
                        else
                        {
                            wordsDictionary[word].Add(docId, new WordDetails());
                            wordsDictionary[word][docId].numberOfOccurrences = 1;
                        }
                    }
                    else
                    {
                        wordsDictionary.Add(word, new Dictionary<int, WordDetails>());
                        wordsDictionary[word].Add(docId, new WordDetails());
                        wordsDictionary[word][docId].numberOfOccurrences = 1;
                    }
                }
            }
            docId += 1;
        }

        //Calculando tf-idf por cada palabra 
        var wordsDictionary2 = new Dictionary<string, Dictionary<int, WordDetails>>();
        foreach (var word in wordsDictionary)
        {
            docsDictionary = word.Value;

            foreach (var item in word.Value)
            {
                float tf = (float)item.Value.numberOfOccurrences / documents[item.Key].wordsNumber;
                float idf = 0;
                // Si la palabra aparecen en mas del 80% de los documentos se le considerará una palabra irrelevante por lo tanto su idf sera 0
                if (word.Value.Count >= docsNumber * 4 / 5) idf = 0;

                else idf = (float)Math.Log(docsNumber / word.Value.Count);

                float tfIdf = tf * idf * 100;

                WordDetails temp = new WordDetails();
                temp = item.Value;
                temp.tfIdf = tfIdf;
                docsDictionary[item.Key] = temp;
            }
            wordsDictionary2.Add(word.Key, docsDictionary);
        }

    }


    // Metodo para listar los documentos que contienen la palabra deseada
    public List<Searcher> Search(string word)
    {
        List<Searcher> searchResults = new List<Searcher>();
        if (wordsDictionary.ContainsKey(word))
        {
            foreach (var item in wordsDictionary[word])
            {
                // Siempre que su tfIdf no sea 0 buscara la palabra y añadira a la lista el nombre del documento, el pathId, el numero de ocurrencia de la palabra, y su tfIdf
                if (item.Value.tfIdf != 0)
                {
                    Searcher currentWord = new Searcher();
                    currentWord.fileName = documents[item.Key].fileName;
                    currentWord.pathId = documents[item.Key].pathId;
                    currentWord.numberOfOccurrences = item.Value.numberOfOccurrences;
                    currentWord.tfIdf = item.Value.tfIdf;
                    searchResults.Add(currentWord);

                }

            }

        }

        return searchResults;
    }

}