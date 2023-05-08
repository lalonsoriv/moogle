namespace MoogleEngine;
using System;
class Operators
{
    public string Word { get; set; }
    public int Include { get; set; }
    public int Exclude { get; set; }
    public int Weight { get; set; }

    // Metodo para identificar si la palabra contiene un operador retornando -1 cuando sea error, 1 cuando posea operador y 0 si no lo posee y modifica la clase Operators
    public int ExistOperator(string word)
    {
        char[] wildcards = { '!', '^', '*' };

        int cantWildcard = 0;

        // Sino contiene wildcard returna 0 
        if (!wildcards.Contains(word[0])) return 0;

        // si contiene ! como primer caracter
        if (word[0] == wildcards[0])
        {
            // si despues de ! siguen otros wildcards retorna -1
            if (word.Substring(1, word.Length - 1).Contains(wildcards[0]) || word.Substring(1, word.Length - 1).Contains(wildcards[1]) || word.Substring(1, word.Length - 1).Contains(wildcards[2])) return -1;
            // si no existen mas wildcards retorna 1 y modifica el valor de exclude 
            else
            {
                this.Exclude = 1;
                // modifica el valor de word en la clase por la palabra sin los operadores
                this.Word = word.Substring(1, word.Length - 1);
                return 1;
            }
        }

        // si contiene ^ como primer caracter
        else if (word[0] == wildcards[1])
        {
            // si despues de ^ siguen otros wildcards retorna -1
            if (word.Substring(1, word.Length - 1).Contains(wildcards[0]) || word.Substring(1, word.Length - 1).Contains(wildcards[1]) || word.Substring(1, word.Length - 1).Contains(wildcards[2])) return -1;
            // si no existen mas wildcards retorna 1 y modifica el valor de include 
            else
            {
                this.Include = 1;
                // modifica el valor de word en la clase por la palabra sin los operadores
                this.Word = word.Substring(1, word.Length - 1);
                return 1;
            }
        }

        // si contiene * como primer caracter
        else
        {
            // la cantidad de * que modificaran a la palabra sera 1
            cantWildcard = 1;
            // si despues de * sigue ! retorna -1
            if (word.Substring(1, word.Length - 1).Contains(wildcards[0])) return -1;
            // si despues de * sigue ^ modifica el valor de include y retorna 1
            else if (word.Substring(1, word.Length - 1).Contains(wildcards[1]))
            {
                this.Include = 1;
                // modifica el valor de word en la clase por la palabra sin los operadores
                this.Word = word.Substring(cantWildcard + 1, word.Length - cantWildcard - 1);
                return 1;
            }
            // de lo contrario aumentara el valor de la cantidad de *
            else
            {
                for (int i = 1; i < word.Length; i++)
                {
                    if (word.Substring(i, word.Length - i).Contains(wildcards[2])) cantWildcard++;
                    else break;
                }
                // modifica el valor de word en la clase por la palabra sin los operadores
                this.Word = word.Substring(cantWildcard, word.Length - cantWildcard);
            }
            // modifica el valor de weight multiplicando la cantidad de * por 1000 y retornara 1
            this.Weight = cantWildcard * 1000;
            return 1;

        }
    }

}
