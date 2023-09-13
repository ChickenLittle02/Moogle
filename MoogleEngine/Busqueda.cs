using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
namespace MoogleEngine
{
    public class Busqueda
    {
        public static string[] DivideQuery(string query)
        {
            //Este metodo recibe la query y devuelve un array de string
            //con las palabras divididas y sin los signos de puntuacion menos los operadores
            for(int i=0; i<query.Length; i++)
            {
                if(query[i]=='~'){
                    if(i==query.Length-1){
                        query = CopyQuery(query,i);
                    }else if(query[i+1]!=' '){
                        query = CopyQuery(query,i);
                    }
                }
            }
            
            query = Regex.Replace(query, "~ ", "~");
            
            string LowerText = query.ToLower();
            string space = " ";
            string[] sinCarac = LowerText.Split(space, StringSplitOptions.RemoveEmptyEntries);
            //Aqui divide el string query en palabras segun los espacios

            return sinCarac;
        }

        public static string CopyQuery(string query, int i){
            string query_COpy = "";
            if(i==query.Length-1){
                for(int j = 0; j<query.Length-1;j++){
                    query_COpy+= query[j];
                }
            }else{
                for(int j = 0; j<query.Length;j++){

                    if(j!=i){
                    query_COpy+= query[j];
                    }
                }
                
            }

            return query_COpy;
        }


        public static string[] Clean(string[] query)
        {
            //Este es un metodo que recibe un array de string y por cada posicion de ese array me quita todos los caracteres distintos de
            //\p{L} letras, \d digitos, \s espacios en blanco, y distintos de los operadores ^, !, ~, *

            string[] Without = new string[query.Length];

            for (int i = 0; i < Without.Length; i++)
            {
                //Este for me va iterando por cada posicion del array query y me va guardando en cada posición 
                Without[i] = Regex.Replace(query[i], @"[^\p{L}\d\s^!~*]", "");
            }

            return Without;

        }




        public static string[] LowerString(string[] DivideQuery)
        {
            //Este es un metodo que recibe un array de string con las palabras de la query con operadores, y me elimina por cada palabra
            //sus operadores
            string[] Without = new string[DivideQuery.Length];

            for (int i = 0; i < Without.Length; i++)
            {
                //Este for me va iterando por cada posicion del array, y me remplaza por cada palabra los operadores por ""
                Without[i] = Regex.Replace(DivideQuery[i], @"[!~^*]", "");

            }


            return Without;
        }





        public static string[] SearchTheOne(string[] query, Dictionary<string, double> Doc_IDF)
        {
            /*Este metodo recibe el array de string con las palabras de la query divididas y sin ningun signo de puntuacion
            yield verifica si la palabra esta o no entre las palabras de los documentos, si no esta, compara con la distancia Levenshtein
            la palabra que mas se le parezca y la sustituye por esa en la query*/

            string[] queryCopy = new string[query.Length];
            Array.Copy(query, queryCopy, query.Length);

            for (int i = 0; i < queryCopy.Length; i++)
            {//Este for va recorriendo las palabras de la query y verificando si no aparecen el el diccionario IDF

                if (!Doc_IDF.ContainsKey(queryCopy[i]) && !(queryCopy[i] == "")) queryCopy[i] = PalabraPreferida(queryCopy[i], Doc_IDF);

                // si esa palabra == "" quiere decir que era un operador o un signo de puntuacion solamente, o 
                //una combinacion de las dos, entonces no es una palabra por tsnto no hay que buscarle ninguna palabra para cambiarla,
                //Entonces habria que verificar si el diccionario IDF no contiene la palabra, 
                //lo que quiere decir que no es ninguna palabra de algun documento,
                //y //en cualquier otro caso tengo que comparar la levenshtein con cada palabra del diccionario
                // y quedarme con la que menor distancia tenga, y esa sustituirla por la palabra que estaba mal               
            }

            return queryCopy;

        }




        private static string PalabraPreferida(string word, Dictionary<string, double> Doc_IDF)
        {
            //Este metodo recibe una palabra y el diccionario IDF y compara la palabra con cada palabra buscando la que menor distancia Levenshtein tenga 
            //y esa es la palabra que devuelve
            int distanciaFavorita = int.MaxValue;
            int distancia;

            string Favorita = word;


            foreach (var palabra in Doc_IDF)
            {
                //Este foreach va recorriendo cada palabra del IDF y va calculando la distancia levenshtein de la palabra de la query
                //con la palabra del IDF, si la distancia es menor que la distancia ya guardada, entonces se cambia la distancia guardada
                //Por esta que es menor y la palabra se cambia por esta que tiene menor distancia, asi va quedando guardada la palabra
                //con menor distancia

                distancia = levenshtein(palabra.Key, word);

                if (distancia < distanciaFavorita)
                {
                    distanciaFavorita = distancia;
                    Favorita = palabra.Key;
                }
            }

            return Favorita;

        }


        private static Int32 levenshtein(String a, String b)
        {       //Este metodo recibe dos palabras y me devuelve la distancia levenshtein de ellas dos

            if (string.IsNullOrEmpty(a))
            {//Comprueba el caso de que una de las palabras sea null
                if (!string.IsNullOrEmpty(b))

                {//En el caso de que la otra sea null, la distancia es el length de la otra palabra porque tiene que agregar esa misma cantidad de letras

                    return b.Length;
                }

                //En caso de que la otra sea null, pues la distancia es 0 porque son iguales
                return 0;
            }

            if (string.IsNullOrEmpty(b))
            {
                if (!string.IsNullOrEmpty(a))
                {
                    return a.Length;
                }
                return 0;
            }

            Int32 cost;
            Int32[,] d = new int[a.Length + 1, b.Length + 1];
            Int32 min1;
            Int32 min2;
            Int32 min3;

            for (Int32 i = 0; i <= d.GetUpperBound(0); i += 1)
            {
                d[i, 0] = i;
            }

            for (Int32 i = 0; i <= d.GetUpperBound(1); i += 1)
            {
                d[0, i] = i;
            }

            for (Int32 i = 1; i <= d.GetUpperBound(0); i += 1)
            {
                for (Int32 j = 1; j <= d.GetUpperBound(1); j += 1)
                {
                    cost = Convert.ToInt32(!(a[i - 1] == b[j - 1]));

                    min1 = d[i - 1, j] + 1;
                    min2 = d[i, j - 1] + 1;
                    min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];

        }



        public static string Change(string[] QueryDividido, string[] LowerQueryWithout, string[] QueryClean)
        {
            // Este metodo es para hacer la sugerencia, recibe
            // un array de string con las palabras de la query divididas y con las letras en minuscula, QueryDividido
            //recibe un array de string con las palabras de la query divididas, en minuscula, y sin operadores LowerQueryWithout
            // un array de string con las palabras de la query divididas, sin operadores, con la letra minuscula y modificadas por la levenshtein QueryClean


            string[] QueryDividido2 = new string[QueryDividido.Length];
            Array.Copy(QueryDividido, QueryDividido2, QueryDividido.Length);

            for (int i = 0; i < QueryDividido2.Length; i++)
            {
                //COn este for voy recorriendo el array de palabra de la query sin signos, y con letra normal

                if (!(LowerQueryWithout[i] == QueryClean[i]))
                {
                    //Por cada posicion compruebo si la palabra en minuscula antes de cambiar con la levenshtein es diferente a la palabra
                    //despues de cambiada, en caso de serlo, compruebo si la palabra cambiada existe en esa posicion de las palabras de la query divididas
                    //y con signos de puntuacion, 
                    //En caso de no existir sustituyo en la query la palabra con letra normal por la palabra cambiada con la levenshtein

                    string Bad = LowerQueryWithout[i];
                    string Good = QueryClean[i];
                    QueryDividido2[i] = QueryDividido2[i].Replace(Bad, Good);

                }
            }

            string suggestion = String.Join(' ', QueryDividido2);

            return suggestion;

        }

    }
}