using System.Diagnostics;



namespace MoogleEngine
{
    public class Inicio
    {
        public static string[] dirFile;
        public static Dictionary<string, string[]> DocumentoDividido;
        public static Dictionary<string, Dictionary<string, double>> DocumentTF;
        public static Dictionary<string, double> DocumentIDF;
        public static Dictionary<string, Dictionary<string, double>> DocumentTF_IDF;

        public static void Main()
        {



            string folder = Directory.GetCurrentDirectory();
            //Aqui recibo donde se encuentra mi proyecto que estoy ejecutando

            // System.Console.WriteLine("folder sin cambiar    "+folder);
            folder = MoogleEngine.ModVec.ToPath(folder);
            //Aqui introduzco la direccion donde se encuentran mis documentos


            // System.Console.WriteLine("folder cambiada    "+folder);
            Inicio.dirFile = Directory.GetFiles(folder, "*.txt");
            //dirFile: matriz con los directorios para llegar a cada archivo ejemplo: /Content/habla.txt
            //GetFIles guarda en un array de string los path de acceso a cada archivo, en este caso
            //solo a los .txt porque son los que nos interesan 


            Dictionary<string, string> DocumentoInicial = MoogleEngine.ModVec.DocText(dirFile);
            //DocumentoInicial es un diccionario de <nombre de documento, Texto del documento>

            Inicio.DocumentoDividido = MoogleEngine.ModVec.ToTextDivide(DocumentoInicial);
            //DocumentoDividido es un diccionario de <nombre de documento, Palabras del documento dividido>

            Inicio.DocumentTF = MoogleEngine.ModVec.toTF(DocumentoDividido);
            //DOcument TF es un diccionario de <nombre de documento,DIccionario<palabras del documento,Veces que se repite la palabra en el documento>>



            Inicio.DocumentIDF = MoogleEngine.ModVec.toIDF(DocumentTF);
            //DOcumentIDF es un diccionario de <palabras, cantidad de documentos en que aparece>
            // System.Console.WriteLine("DocumentIDF    "+ DocumentIDF.Count);

            Inicio.DocumentTF_IDF = MoogleEngine.ModVec.toTF_IDF(DocumentTF, DocumentIDF);
            //DocumentTF_IDF es un diccionario de <nombre documento,Diccionario<palabra,valor TF_IDF de cada palabra>>



        }







    }
}