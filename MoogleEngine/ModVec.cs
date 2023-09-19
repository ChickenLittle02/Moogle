using System.Text.RegularExpressions;
namespace MoogleEngine
{
    public class ModVec
    {


        public static Dictionary<string, string> DocText(string[] paths)
        {
            //Metodo que recibe un array de strings con los paths de cada documento (paths) y 
            //devuelve un diccionario con <Nombre de documento, Texto de DOcumento> (Documentos)


            string TextName;
            string[] Text;
            string Texto;
            Dictionary<string, string> Documentos = new Dictionary<string, string>();

            for (int i = 0; i < paths.Length; i++)
            {//Este for va recorriendo el array de paths, y por cada path busca el nombre del documento, y guarda el texto del documento concatenado

                TextName = ToName(paths[i]);

                Text = File.ReadAllLines(paths[i]);
                //File.ReadAllLines es un metodo que recibe un path y devuelve cada linea del texto del documento de ese path guardada en una
                //posicion de un array de string

                Texto = string.Join(" ", Text);
                //Join concatena todos los string de un array de string separandolos por el operador que se defina, en este caso es un espacio

                Documentos[TextName] = Texto.ToLower();
                //ToLower es para poner en un string todas las letras en minuscula

            }

            return Documentos;

        }

        private static string ToName(string path)
        {
            //Metodo que recibe path y devuelve nombre del documento

            //Posicion me guarda la ultima vez que aparece el string   \   , hay que ponerle el @ delante porque \ es un caracter restringido de vsc 
            int posicion = path.LastIndexOf(@"\");

            posicion++;
            //Hay que sumarle 1 a posicion para que empiece despues del caracter \

            int PointPosition = path.LastIndexOf('.');
            //EL largo de la palabra va siempre desde la ultima vez que aparece   \   hasta la ultima vez que aparece   .  

            //EL metodo substring recibe (posicion de donde empieza el substring, largo del texto que va a guardar)
            //Y devuelve un string que empieza en la posicion y tiene ese largo
            string name = path.Substring(posicion, PointPosition - (posicion));

            return name;
        }


        public static Dictionary<string, string[]> ToTextDivide(Dictionary<string, string> LowerText)
        {
            //Recibe un dictionary de <nombre de documento, texto del documento en minuscula
            //Devuelve un DIctionary de <nombre de documento, array de string con palabras del documento> que seria DocumentoDIvidido

            Dictionary<string, string[]> DocumentoDividido = new Dictionary<string, string[]>();

            string[] texto;

            foreach (var item in LowerText)
            {//Con este foreach vamos iterando por el texto de cada documento para limpiarlo de signos de puntuacion y separar todas las palabras del documento

                texto = RemoveChar(item.Value);
                DocumentoDividido[item.Key] = texto;

            }

            return DocumentoDividido;

        }




        private static string[] RemoveChar(string full)
        {
            //Metodo para quitar signos de puntuacion y demas caracteres que afectan la comprension por la pc

            full = Regex.Replace(full, @"[^\p{L}\d\s]", "");
            //Regex.Replace está remplaxando todos los caracteres que no son \p{L} letras, que no son \d digitos decimales y que no son
            // espacios en blanco \s, por el caracter vacío ""

            string[] sinCarac = full.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            //Split divide un string en partes cuando encuentre el caracter "", y con StringSplitOptions.RemoveEmptyEntries
            //hacemos que se omitan las posiciones del array que sean solo espacios en blanco


            return sinCarac;

        }




        public static Dictionary<string, Dictionary<string, double>> toTF(Dictionary<string, string[]> DocumentoDividido)
        {
            //Este metodo recibe un diccionario de <nombre de documento, array de string con palabras del documento>
            //Y devuelve un diccionario de <nombre de documento, Diccionario<palabra del documento, TF de la palabra>
            //La formula de TF que se utilizo fue en un documento(freq/maxFreq)
            //veces que se repite la palabra en el documento (freq)
            //Frecuencia de la palabra que mas se repite en el documento (maxFreq)



            Dictionary<string, Dictionary<string, double>> DocumentTF = new Dictionary<string, Dictionary<string, double>>();
            bool exist;


            foreach (var item in DocumentoDividido)
            {//Este foreach lo que va recorriendo cada documento y por cada uno creo un diccionario de <palabras del documento, TF de la palabra>
             // Despues voy iterando por el array de palabras de ese documento y agregando las palabras al Diccionario TF,
             // y va contando las veces que aparece esa palabra en el documento,
             // si la palabra no aparece se agrega nueva y se le pone como double 1, porque aparece una vez,
             //Si esa palabra aparece en el diccionario entonces se le suma uno al valor de double

                double max = 0;
                //Max va a ser el valor de la cantidad de veces que mas se repite una palabra en el documento

                Dictionary<string, double> Documento = new Dictionary<string, double>();
                //Documento es un diccionario de <palabras del documento item, veces que se repiten las palabras en el documento>

                for (int i = 0; i < item.Value.Length; i++)
                {
                    exist = Documento.ContainsKey(item.Value[i]);

                    if (exist) Documento[item.Value[i]] = ++Documento[item.Value[i]];
                    else Documento[item.Value[i]] = 1;

                    max = Math.Max(max, Documento[item.Value[i]]);

                }

                //Hay que normalizar el TF, para que los valores sean menores que 1 y no sean demasiado grandes,
                // para eso si dividen los valores por la frecuencia mas alta

                foreach (var word in Documento)
                {
                    Documento[word.Key] = Documento[word.Key] / max;
                }

                DocumentTF[item.Key] = Documento;
                //Aqui guardo en el documento las palabras y las veces que se repiten en el

            }

            return DocumentTF;
        }





        public static Dictionary<string, double> toIDF(Dictionary<string, Dictionary<string, double>> DocumentTF)
        {
            //EL metodo recibe un Diccionario de <nombre de documento,<palabra del documento, TF de la palabra>>
            //Y devuelve un Diccionario de <palabra de todos los documentos, IDF de esa palabra>
            //Metodo para tener IDF, que seria log de total de documentos entre la cantidad de documentos donde aparece la palabra

            bool exist;
            //Guarda true si la palabra existe en el key del Diccionario

            Dictionary<string, double> DocumentIDF = new Dictionary<string, double>();
            //DocumentIDF va a ser un diccionario de <palabras, cantidad de documentos donde aparece>

            double TotalDocs = DocumentTF.Count;
            // TotalDOcs es la cantidad de documentos que hay, que serian la cantidad de <Key,Value> del Diccionario DocumentTF

            foreach (var item in DocumentTF)
            {//Con este foreach voy iterando por los documentos, y despues voy iterando por las palabras de cada uno
             //Verifico si la palabra ya existe en el IDF le sumo uno a la cantidad, si
             //no existe la agrego y le pongo que aparece una vez

                foreach (var word in item.Value)
                {
                    exist = DocumentIDF.ContainsKey(word.Key);

                    if (exist) DocumentIDF[word.Key] = ++DocumentIDF[word.Key];
                    else DocumentIDF[word.Key] = 1;

                }
            }

            foreach (var item in DocumentIDF)
            {
                DocumentIDF[item.Key] = Math.Log(TotalDocs / DocumentIDF[item.Key]);

            }

            return DocumentIDF;

        }






        public static void toTF_IDF(Dictionary<string, Dictionary<string, double>> DOcumentTF, Dictionary<string, double> DOcumentIDF)
        {
            //Este metodo recibe un diccionario DOcumentTF con los TF de cada palabra en cada documento, y un diccionario con los IDF
            //Y convierte el diccionario DOcumentTF en un diccionario<nombre de documento,Diccionario<palabras del documento, valores TF_IDF>>
            //EL TF_IDF es el valor TF de la palabra en ese documento por el valor IDF de esa palabra

            bool exist;
            //Es una variable bool que va a guardar true si una determinada palabra pertenece

            double TF;
            double IDF;

            Dictionary<string, Dictionary<string, double>> TF_IDF = new Dictionary<string, Dictionary<string, double>>();

            foreach (var document in DOcumentTF)
            {//Con este foreach voy iterando por cada documento, 
             //y despues voy iterando por todos los elementos del IDF y comprobando si estan en las palabras,
             //SI estan, guardo el value de la palabra que sería el valor de tf y lo multiplico por el valor IDF, y ese producto que es el TF_IDF, 
             //lo guardo como value de la palabra en el diccionario DocumentTF
             //EN caso de no estar agregada la palabra entonces la omitimos, porque siempre va a tener valor de TF_IDF 0,
             //pues como no aparece en el documento su TF sería 0


                foreach (var word in DOcumentIDF)
                {
                    exist = (document.Value).ContainsKey(word.Key);


                    if (exist)
                    {
                        TF = document.Value[word.Key];
                        IDF = word.Value;
                        document.Value[word.Key] = TF * IDF;
                    }

                }
            }
        }


        //A partir de aqui estos metodos son para terminar el modelo vectorial con la query

        public static Dictionary<string, double> ToQueryTF(string[] query)
        {
            //Este es un metodo que dada un array de string con las palabras del query, que ya serian las de la sugerencia
            //me devuelve un diccionario de <palabras del query, TF de la palabra>

            Dictionary<string, double> QueryTF = new Dictionary<string, double>();
            //Dicionario de <palabras del query, TF de la palabra>

            bool exist;
            //Si la palabra existe guarda true

            for (int i = 0; i < query.Length; i++)
            {
                // Este for va iterando por el array de palabras de la query, y va verificando si estas palabras estan 
                //en el Diccionario de palabras de la query, en caso de no estar las agrega con frecuencia 1, porque aparecen una sola vez
                //y en el caso de estar le suma uno a la frecuencia

                exist = QueryTF.ContainsKey(query[i]);

                if (exist) QueryTF[query[i]] = ++QueryTF[query[i]];
                else QueryTF[query[i]] = 1;

            }

            double max = 0;
            //Max va a ser la mayor cantidad de veces que se repite una palabra

            foreach (var item in QueryTF)
            {
                max = Math.Max(max, item.Value);
            }

            //Aqui normalizo el TF
            foreach (var item in QueryTF)
            {
                QueryTF[item.Key] = item.Value / max;
            }

            return QueryTF;
        }


        public static Dictionary<string, double> toQueryTF_IDF(Dictionary<string, double> QueryTF, Dictionary<string, double> DocumentIDF)
        {
            //Este metodo recibe un diccionario de <palabras del query, TF de cada palabra>
            // y un diccionario de <palabras de todos los documentos, IDF de cada palabra>
            //Y devuelve un diccionario de <palabras de los documentos, TF_IDF de cada palabra

            Dictionary<string, double> QueryTF_IDF = new Dictionary<string, double>();
            //QueryTF_IDF es un diccionario de <palabras de los documentos, TF_IDF de cada palabra

            bool exist;
            double TF;
            double IDF;
            double a = 0.5;

            foreach (var word in DocumentIDF)
            {
                //Con este for voy recorriendo el array de palabras del Diccionario IDF, en caso de no aparecer la palabra quiere decir 
                //que el TF de esa palabra en la query es 0, por tanto TF*IDF de esa palabra es 0, en caso de aparecer esa palabra, calculo
                //su peso por la formula (a+(1-a)*TF)*IDF donde a es un valor entre 0,4 y 0,5 

                exist = QueryTF.ContainsKey(word.Key);

                //SI la palabra no existe
                if (!exist) QueryTF_IDF[word.Key] = 0;
                else
                {
                    TF = QueryTF[word.Key];
                    IDF = word.Value;
                    QueryTF_IDF[word.Key] = (a + (1 - a) * TF) * IDF;
                }

            }

            return QueryTF_IDF;
        }


        public static Dictionary<string, double> SearchScores(Dictionary<string, double> QueryTF_IDF, Dictionary<string, Dictionary<string, double>> DocumentTF_IDF)
        {
            //Este metodo recibe un diccionario de <string, double> con <palabras de la query, tf-idf de cada palabra>
            // Y un diccionario de <string,Diccionario<string,double>> que tiene <nombre de documento, palabras del documento, tf-idf de cada palabra>>
            //Y devuelve un diccionario de <string, double> con <nombre del documento, score del documento> 

            Dictionary<string, double> DocScores = new Dictionary<string, double>();

            foreach (var doc in DocumentTF_IDF)
            {//Este foreach va recorriendo cada documento y va calculando la similitud entre las palabras de ese documento y las palabras de la query
             //Y esa similitud es el score

                DocScores[doc.Key] = CosineSImilitude(QueryTF_IDF, doc.Value);
            }

            return DocScores;
        }

        private static double CosineSImilitude(Dictionary<string, double> QueryTF_IDF, Dictionary<string, double> DocumentTF_IDF)
        {
            //Este metodo recibe un diccionario <string,double> con las forma < palabras de la query, TF_IDF de cada palabra>
            //Y otro diccionario <string,double> con la forma < palabras de un documento, TF_IDF de cada palabra>
            //Y devuelve el valor double de similitud de ambos diccionarios, seria el score de ese documento
            //La formula de Similitud que se usa aqui es
            //numerador = Sumatoria de el producto de el peso de la palabra en la query por el peso de la palabra en el documento
            //denominador = Raiz cuadrada de SUmatoria A por Raiz cuadrada de SUmatoria B
            //SUmatoria A = Sumatoria de los cuadrados de los pesos de cada palabra de la query
            //SUmatoria B = SUmatoria de los cuadrados de los pesos de cada palabra del documento

            double numerador = 0;
            double numeradorProducto;

            double PowerQuery = 0;
            double AddPowerQuery;
            double PowerDocument = 0;
            double AddPowerDocument;

            foreach (var wordQuery in QueryTF_IDF)
            {   //Con este for voy iterando por los pesos de cada palabra de la query
                // Y por cada palabra de la query veo tambien el peso de esa palabra en el documento
                //SI esa palabra aparece en el documento va a tener un peso, y en caso de que no aparezca, va a tener valor TF_IDF 0,
                //Por tanto en caso de que una palabra de la query no aparezca en el documento se puede omitir buscar la palabra en el documento, pues no aporta nada
                //Asi en caso de que la palabra exista en la query y en el documento
                // calculo el producto de ambos pesos y los voy sumando con los de la siguiente palabra y asi sucesivamente
                //Tambien voy calculando el cuadrado de cada peso de la query y se lo sumo al cuadrado del peso de la siguiente palabra de la query y asi ...
                //voy calculando el cuadrado de cada  del documento y se lo sumo al cuadrado del peso de la siguiente palabra del documento y asi ...
                if (DocumentTF_IDF.ContainsKey(wordQuery.Key))
                {
                    numeradorProducto = wordQuery.Value * DocumentTF_IDF[wordQuery.Key];
                    numerador += numeradorProducto;


                    AddPowerDocument = Math.Pow(DocumentTF_IDF[wordQuery.Key], 2);
                    PowerDocument += AddPowerDocument;
                }

                AddPowerQuery = Math.Pow(wordQuery.Value, 2);
                PowerQuery += AddPowerQuery;

            }

            double denominador = Math.Sqrt(PowerQuery) * Math.Sqrt(PowerDocument);
            double Similitude;

            if (denominador == 0)
            {
                Similitude = 0;
                return Similitude;
            }

            Similitude = numerador / denominador;
            return Similitude;

        }


        public static List<KeyValuePair<string, double>> SortScores(Dictionary<string, double> DocScores)
        {
            //EL metodo recibe un diccionario de <nombre del documento, score del documento>, y devuelve una lista compuesta por 

            // using System.Linq;

            //Aqui usando Linq se ordena el diccionario segun sus value y se agregan los IOrderedEnumerable a una lista
            IOrderedEnumerable<KeyValuePair<string, double>> items = from pair in DocScores
                                                                     orderby pair.Value descending
                                                                     select pair;
            List<KeyValuePair<string, double>> ordered = new List<KeyValuePair<string, double>>();

            foreach (var pair in items)
            {
                ordered.Add(pair);

            }

            return ordered;

        }

    }
}