using System.Diagnostics;



namespace MoogleEngine
{
    public class Inicio
    {
        public static string[] dirFile;
        public static Dictionary<string, string[]> DocumentoDividido;
        public static Dictionary<string, double> DocumentIDF;
        public static Dictionary<string, Dictionary<string, double>> DocumentTF_IDF;
 
        public static void Main()
        {
            string folder = Path.Join(@"..", "Content");
            //Aqui recibo donde se encuentra mi proyecto que estoy ejecutando, con .. sale de la carpeta donde esta corriendo el proyecto
            //Y despues entra a la carpeta Content

            Inicio.dirFile = Directory.GetFiles(folder, "*.txt");
            //dirFile: matriz con los directorios para llegar a cada archivo ejemplo: /Content/habla.txt
            //GetFIles guarda en un array de string los path de acceso a cada archivo, en este caso
            //solo a los .txt porque son los que nos interesan 

            Dictionary<string, string> DocumentoInicial = MoogleEngine.ModVec.DocText(dirFile);
            //DocumentoInicial es un diccionario de <nombre de documento, Texto del documento>

            Inicio.DocumentoDividido = MoogleEngine.ModVec.ToTextDivide(DocumentoInicial);
            //DocumentoDividido es un diccionario de <nombre de documento, Palabras del documento dividido>

            Inicio.DocumentTF_IDF = MoogleEngine.ModVec.toTF(DocumentoDividido);
            //DOcument TF es un diccionario de <nombre de documento,Diccionario<palabras del documento,Veces que se repite la palabra en el documento>>

            Inicio.DocumentIDF = MoogleEngine.ModVec.toIDF(DocumentTF_IDF);
            //DocumentIDF es un diccionario de <palabras, cantidad de documentos en que aparece>
            // System.Console.WriteLine("DocumentIDF    "+ DocumentIDF.Count);

            MoogleEngine.ModVec.toTF_IDF(DocumentTF_IDF, DocumentIDF);

        }


    }
}