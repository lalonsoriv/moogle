using System;
using System.Text.RegularExpressions;
using System.Text;
namespace MoogleEngine;

class WordProcessor
{
    // Metodo que elimina las palabras de tamaño menor a 2 
    public static bool Processor(string word)
    {
        if (word.Length > 2)
        {
            return true;
        }
        return false;
    }
}

class Normalize
{
    // Metodo que normaliza las palabras de la query dejando solamente las letras y numeros con los operadores
    public static List<string> NormalizeQuery(string query)
    {
        string replace = @"[^\wáéúíóñü^!*]+";
        query = query.ToLower();
        query = query.Trim();
        List<string> queryListed = Regex.Split(query, replace).ToList();

        return queryListed;
    }

    // Metodo que normaliza el texto dejando solamente las letras y los numeros
    public static List<string> NormalizeDoc(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var sr = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
        string content = sr.ReadToEnd();
        string replace = @"[^\wáéúíóñü]+";
        string fileNormalize = content.ToLower();
        List<string> wordsFromFile = Regex.Split(fileNormalize, replace).ToList();

        return wordsFromFile;
    }

}
class LevenshteinMethod
{
    // Metodo que calcula la similitud entre la palabra de la query que no fue encontrada y las palabras que existe en el diccionario 
    public static string[] FindSimilarities(string query, Dictionary<string, Dictionary<int, WordDetails>> wordsDictionary)
    {
        string[] result = new string[4];

        double[] distance = { 101, 101, 101, 101 };

        List<string> words = wordsDictionary.Keys.ToList();

        var queryLenght = query.Length;

        var wordLenght = 0;

        foreach (var word in words)
        {
            wordLenght = word.Length;

            var matrix = new int[queryLenght + 1, wordLenght + 1];

            if (wordLenght <= queryLenght + 3 && wordLenght >= queryLenght - 3)
            {
                // Inicializacion de la matriz siendo el tamaño de las filas iguales a queryLenght y de las columnas iguales a wordLenght
                for (int i = 0; i <= queryLenght; matrix[i, 0] = i++) { }
                for (int j = 0; j <= wordLenght; matrix[0, j] = j++) { }

                // Calcula la distancia entre las filas y columnas 
                for (int i = 1; i <= queryLenght; i++)
                {
                    for (int j = 1; j <= wordLenght; j++)
                    {
                        // Si son iguales en posiciones equidistantes el peso es 0 de lo contrario el peso sera 1
                        var cost = (word[j - 1] == query[i - 1]) ? 0 : 1;

                        matrix[i, j] = Math.Min(Math.Min(matrix[i - 1, j] + 1,      // Eliminacion de un caracter
                                                matrix[i, j - 1] + 1),              // Insercion de un caracter
                                                matrix[i - 1, j - 1] + cost);       // Sustitucion de un caracter
                    }
                }
                // Calculamos la distancia de los cambios en la palabra.

                double qwDistance = 0;
                if (queryLenght > wordLenght)
                    qwDistance = ((double)matrix[queryLenght, wordLenght] / (double)queryLenght) * 100;
                else
                    qwDistance = ((double)matrix[queryLenght, wordLenght] / (double)wordLenght) * 100;
                // No necesitamos aquellas palabras cuya distancia sea mayor que 30
                if (qwDistance <= 30)
                {
                    // Asi guardamos en la lista result las 4 palabras mas similares (menor distancia)
                    for (int i = 0; i < distance.Length; i++)
                    {
                        if (distance[i] > qwDistance)
                        {
                            for (int j = distance.Length - 1; j > i; j--)
                            {
                                distance[j] = distance[j - 1];
                                result[j] = result[j - 1];
                            }
                            distance[i] = qwDistance;
                            result[i] = word;
                            break;
                        }
                    }
                }
            }
        }
        return result;
    }
}