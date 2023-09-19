using System.Text;
using System.Diagnostics;


namespace MoogleEngine;


public static class Moogle
{
    public static void Show(string[] input){
        foreach (var item in input)
        {
            System.Console.WriteLine(item);
        }

    }
    public static SearchResult Query(string query)
    {
        string[] QueryDividido = Busqueda.DivideQuery(query);
        //El query dividido y en minuscula, pero con signos de puntuacion y con operadores
        Show(QueryDividido);

        string[] QueryWithout = Busqueda.Clean(QueryDividido);
        //Un array con todas las palabras del query sin los signos de puntuacion, pero con operadores

        //Para hacer la sugerencia vamos a dar los resultados en minuscula, entonces cogemos el query, lo llevamos a minuscula y lo separamos
        // le quitamos los signos de puntuacion, despues le quitamos los operadores, despues lo comparamos con la distancia levenshtein
        // Si antes de la distancia levenshtein esta palabra es distinta a la palabra despues de la levenshtein, 
        //se sutituye la palabra de antes de la distancia por la de despues de la distancia, en la query que tiene signos de puntuacion

        string[] LowerQueryWithout = Busqueda.LowerString(QueryWithout);
        //Un array con todas las palabras del query sin operadores y en minuscula

        string[] QueryClean = Busqueda.SearchTheOne(LowerQueryWithout, Inicio.DocumentIDF);
        //Este es un array con las palabras de la query que no se encontraban en el IDF sustituidas por la palabra que menor distancia levenshtein tenga
        //con respecto a las palabras del IDF

        string suggestion = Busqueda.Change(QueryDividido, LowerQueryWithout, QueryClean);
        //Aqui se guarda la sugerencia, que seria la query con las palabras mal escritas o que no se encontraron cambiadas

        Dictionary<string, double> QueryTF = ModVec.ToQueryTF(LowerQueryWithout);
        //QueryTF es un diccionario de <palabras del query,TF de la palabra>

        Dictionary<string, double> QueryTF_IDF = ModVec.toQueryTF_IDF(QueryTF, Inicio.DocumentIDF);
        //QueryTF_IDF es un diccionario de <palabras, valor TF_IDF de cada palabra>

        Dictionary<string, double> DocScores = ModVec.SearchScores(QueryTF_IDF, Inicio.DocumentTF_IDF);
        //DocScores es un diccionario de <nombre de documentos, score>
        //Hay que arreglar en la similitud que si el denominador es 0, el score es 0

        Dictionary<string, double> DocScoresClean = Ops.CleanDocs(Inicio.DocumentTF_IDF, QueryDividido, QueryClean, DocScores, Inicio.DocumentoDividido);
        //Resultados modificados segun los operadores, ver si se puede cambiar dividequery por QueryDIvidido sin problemas

        List<KeyValuePair<string, double>> SortDocScores = ModVec.SortScores(DocScoresClean);


        //comprobar que todas las palabras de la query no comiencen con !

        int count = 0;

        foreach (var word in QueryDividido)
        {
            if (word.StartsWith("!"))   count++;
        }

        SearchItem[] items;



        //Aqui verificamos si todas las palabras tienen negacion !
        int final;

        if (QueryDividido.Length != 0 && count == QueryDividido.Length)
        {

            if (SortDocScores.Count == 0)
            {

                items = new SearchItem[1]{
                    new SearchItem("", "No hay resultados para su busqueda",0)
                };

                return new SearchResult(items, suggestion);

            }
            if (SortDocScores.Count < 5)
            {
                items = new SearchItem[SortDocScores.Count];

                for (int i = 0; i < SortDocScores.Count; i++)
                {
                    items[i] = new SearchItem(SortDocScores[i].Key, NotContainSnip(SortDocScores[i].Key, Inicio.dirFile, LowerQueryWithout), SortDocScores[i].Value);
                }

                return new SearchResult(items, suggestion);
            }
            else
            {
                items = new SearchItem[5];

                for (int i = 0; i < 5; i++)
                {
                    items[i] = new SearchItem(SortDocScores[i].Key, NotContainSnip(SortDocScores[i].Key, Inicio.dirFile, LowerQueryWithout), SortDocScores[i].Value);
                }

                return new SearchResult(items, suggestion);

            }


        }

        double cantDoc = SortDocScores.Count;

        //Hay que eliminar los que tengan score 0
        SortDocScores = delete0(SortDocScores);

        if (SortDocScores.Count == 0)
        {

            items = new SearchItem[1]{
                    new SearchItem("", "No hay resultados para su busqueda",0)
                };

            return new SearchResult(items, suggestion);

        }
        else if (SortDocScores.Count < 5)
        {
            items = new SearchItem[SortDocScores.Count];

            for (int i = 0; i < SortDocScores.Count; i++)
            {
                items[i] = new SearchItem(SortDocScores[i].Key, Snip(SortDocScores[i].Key, Inicio.dirFile, QueryTF_IDF), SortDocScores[i].Value);
            }

            return new SearchResult(items, suggestion);
        }
        else
        {
            items = new SearchItem[5];

            for (int i = 0; i < 5; i++)
            {
                items[i] = new SearchItem(SortDocScores[i].Key, Snip(SortDocScores[i].Key, Inicio.dirFile, QueryTF_IDF), SortDocScores[i].Value);
            }

            return new SearchResult(items, suggestion);
        }
    }

    public static string Snip(string NameDoc, string[] dirFile, Dictionary<string, double> QueryTF_IDF)
    {
        //Este metodo recibe un string con el nombre del documento
        //Y recibe un string[] con los path de cada documento
        //Y recibe un diccionario de <palabras de la query, peso de la palabra>
        //Y devuelve un string con un snippet
        string[] texto = new string[0];
        string snip = "";

        foreach (var item in dirFile)
        {
            if (item.Contains(NameDoc + ".txt"))
            {
                texto = File.ReadAllLines(item);
                break;
            }
        }

        int count = 0;

        foreach (var lineas in texto)
        {
            foreach (var item in QueryTF_IDF)
            {

                if (lineas.Contains(item.Key))
                {
                    snip = lineas;
                    count++;
                    break;
                }

            }
        }
        if (count == 0)
        {
            snip = "";
        }

        return snip;

    }

    static string NotContainSnip(string NameDoc, string[] dirFile, string[] LowerQueryWithout)
    {
        string[] texto = new string[0];
        string snip = "";

        foreach (var item in dirFile)
        {
            if (item.Contains(NameDoc + ".txt"))
            {
                texto = File.ReadAllLines(item);
                break;
            }
        }

        foreach (var lineas in texto)
        {
            foreach (var word in LowerQueryWithout)
            {
                if (!lineas.Contains(word))
                {
                    snip = lineas;
                    break;
                }
            }
        }


        return snip;
    }

    public static List<KeyValuePair<string, double>> delete0(List<KeyValuePair<string, double>> SortDocScores)
    {
        int cantDoc = SortDocScores.Count;

        for (int i = 0; i < cantDoc; i++)
        {
            for (int h = 0; h < SortDocScores.Count; h++)
            {
                if (SortDocScores[h].Value == 0)
                {
                    SortDocScores.Remove(SortDocScores[h]);
                    break;
                }
            }

        }


        return SortDocScores;

    }

}