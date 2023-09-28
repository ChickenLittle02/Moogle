namespace MoogleEngine
{
    public class Ops
    {
        public static Dictionary<string, double> CleanDocs(Dictionary<string, Dictionary<string, double>> DocumentTF,
        string[] QueryDividido, string[] QueryClean, Dictionary<string, double> DocScores, Dictionary<string, string[]> DocumentoDividido)
        {

            //Este metodo recibe el diccionario grande de <documentos, Diccionario<palabras del documento,TF> que es DOcumentTF
            //Recibe un string[] con las palabras de la query sin caracteres pero con operadores QueryDividido
            //Recibe un string[] con las palabras de la query sin caracteres y sin operadores QueryClean
            //Recibe un Diccionario de <string,double> con <nombre de documento, score del documento> DocScores
            //Y recibe un diccionario de <string,string[]> con <nomre del documento, array de string con las palabras del documento en cada posicion

            //Este metodo va a aplicar los 4 operadores, !  ^  *  ~

            for (int i = 0; i < QueryDividido.Length; i++)
            {
                //Este for va recorriendo las palabras del query y verificando la existencia de los operadores

                //Operador ! es el operador que indica que esa palabra no debe aparecer en ningun documento
                //Ejemplo algoritmo !ordenacion, la palabra ordenacion no debe aparecer en ningun documento
                if (QueryDividido[i].StartsWith("!"))
                {
                    //Aqui me verifica si el operador ! existe en alguna palabra, si existe entro al foreach

                    foreach (var doc in DocScores)
                    {//Este me va recorriendo el diccionario de <documento,score>

                        if (DocumentTF[doc.Key].ContainsKey(QueryClean[i]))
                        {
                            //Comprueba si el diccionario grande de <documentos, Diccionario<palabras del documento,TF>, donde estan todas las palabras que tiene cada documento
                            //Si en ese documento existe la palabra que tenia delante el operador !

                            /* Poner el if en los resultados para cuando todas las palabras del quey tenga ! delante, en ese caso devolver normal los resultados
                             En el caso de que haya alguna palabra sin ! quitar las que tienen score 0 */
                            DocScores.Remove(doc.Key);

                        }
                    }
                    continue;

                }


                //Operador ^ es el operador que indica que esa palabra tiene que aparecer en todos los documentos devueltos 
                if (QueryDividido[i].StartsWith("^"))
                {
                    //Aqui me verifica si el operador ^ existe delante de alguna palabra, si existe entro al foreach

                    foreach (var doc in DocScores)
                    {//Este me va recorriendo el diccionario de <documento,score>

                        if (!DocumentTF[doc.Key].ContainsKey(QueryClean[i]))
                        {
                            //Comprueba si el diccionario grande de <documentos, Diccionario<palabras del documento,TF>, donde estan todas las palabras que tiene cada documento
                            //Si en ese documento existe la palabra que tenia delante el operador !

                            DocScores.Remove(doc.Key);

                        }
                    }
                    continue;
                }

                //ESte es el operador *
                if (QueryDividido[i].StartsWith("*"))
                {
                    //Aqui me verifica si el operador * existe delante de alguna palabra, si existe entro al foreach

                    //Tengo que contar las veces que aparece el operador *
                    int count = 1;
                    foreach (var charact in QueryDividido[i])
                    {
                        if (charact == '*')
                        {
                            count++;
                        }
                    }
                    //Count es la cantidad de veces que aparece el operador * mas una vez mas;

                    foreach (var doc in DocScores)
                    {//Este me va recorriendo el diccionario de <documento,score>

                        if (DocumentTF[doc.Key].ContainsKey(QueryClean[i]))
                        {
                            //Comprueba si el diccionario grande de <documentos, Diccionario<palabras del documento,TF>, donde estan todas las palabras que tiene cada documento
                            //Si en ese documento existe la palabra que tenia delante el operador *, entonces
                            //EN el diccionario de <documentos,score> el score lo multiplico por count

                            DocScores[doc.Key] = DocScores[doc.Key] * count;
                        }
                    }

                    continue;
                }

                if (QueryDividido[i].StartsWith("~"))
                {
                 
                    if (i == 0) break;
                    //EN este caso seria la primera palabra, y no tiene ninguna palabra anterior para verificar

                    foreach (var doc in DocScores)
                    {
                        if (DocumentTF[doc.Key].ContainsKey(QueryClean[i]) & DocumentTF[doc.Key].ContainsKey(QueryClean[i - 1]))
                        {
                                double distancia = distance(QueryClean[i], QueryClean[i - 1], DocumentoDividido[doc.Key]);


                                if (distancia == 0) DocScores[doc.Key] = 1 + DocScores[doc.Key];

                                else
                                {
                                    distancia = 1 / distancia;
                                    DocScores[doc.Key] = distancia + DocScores[doc.Key];
                                }
                            
                        }
                    }
                    continue;
                }


            }


            return DocScores;

        }
        static void Show(Dictionary<string, Dictionary<string, double>> DIccionario)
        {
            foreach (var item in DIccionario)
            {
                System.Console.WriteLine(item.Key);
                foreach (var word in item.Value)
                {
                    System.Console.WriteLine(word);
                }
            }
        }

        static int distance(string p1, string p2, string[] words)
        {//Este metodo recibe dos palabras y un array de palabras, y calcula la cantidad de palabras entre esas dos palabras


            int position = 0;

            for (int i = 0; i < words.Length; i++)
            {//COn este for itero por el array de palabras hasta encontrar p1 o p2, y guardar la posicion,
             //Si primero aparece P1 solo gurdo la posicion, pero si es P2, guardo la posicion y digo que ahora P1 es P2, y P2 P1, porque
             //la primera palabra que aparece es P2.

                if (words[i] == p1)
                {
                    position = i;
                    break;
                }
                else if (words[i] == p2)
                {

                    string temp = p1;
                    p1 = p2;
                    p2 = temp;

                    position = i;

                    break;
                }

            }

            int distance = int.MaxValue;
            int positionP1 = position;
            int positionP2 = 0;


            for (int i = position; i < words.Length; i++)
            {//Con este for voy calculando las distancias, si me encuentro la palabra P2, calculo la distancia y en distance guardo la
             //Menor distancia entre la que tenia guardada y la acabada de calcular, y entonces vuelvo a cambiar P2 por P1, y P1 por P2 porque
             //ahora volveria a calcular la distancia solo cuando me encuentre la otra palabra 
             //SI me encuentro la palabra p1 entonces cambio la posicion de P1, y sigo

                if (words[i] == p2)
                {
                    positionP2 = i;
                    distance = Math.Min(positionP2 - positionP1, distance);

                    string temp = p1;
                    p1 = p2;
                    p2 = temp;
                    positionP1 = positionP2;

                }
                else if (words[i] == p1)
                {
                    positionP1 = i;
                }

            }
            distance--;
            if(distance==-1) return 0; //este es en el caso en que esta buscando la misma palabra, p1==p2

            return distance;

        }

    }
}