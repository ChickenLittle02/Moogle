namespace MoogleEngine
{
    public class Matrices
    {
        static double[,] Suma(double[,] x, double[,] y)
        {//EN caso de no poderse realizar la suma se va a devolver una matriz nula de 1x1,
         //comprobar que funciona en la multiplicacion y aqui en la suma
         //Terminar producto por un escalar, y resta, y comprobar que funcione la suma
            double[,] suma;
            if (x.GetLength(0) != y.GetLength(0) || x.GetLength(1) != y.GetLength(1))
            {
                suma = new double[1, 1];
                return suma;
            }
            suma = new double[x.GetLength(0), x.GetLength(1)];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    suma[i, j] = x[i, j] + y[i, j];

                }
            }
            return suma;
        }


        static double[,] Resta(double[,] x, double[,] y)
        {//EN caso de no poderse realizar la resta se va a devolver una matriz nula de 1x1,
         //comprobar que funciona en la multiplicacion y aqui en la suma
         //Terminar producto por un escalar, y resta, y comprobar que funcione la suma
            double[,] resta;
            if (x.GetLength(0) != y.GetLength(0) || x.GetLength(1) != y.GetLength(1))
            {
                resta = new double[1, 1];
                return resta;
            }
            resta = new double[x.GetLength(0), x.GetLength(1)];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    resta[i, j] = x[i, j] - y[i, j];

                }
            }
            return resta;
        }

        static double[,] Producto(double[,] x, double[,] y)
        {//En eL producto de dos matrices, tiene que cumplirse la condicion de que x tenga tantas columnas
         //como filas tiene y; la matriz resultante queda con tantas filas como x, y tantas columnas como y
         //Aqui queda incluido el caso de una matriz por un vector, y a la hora de multiplicarse tienen que cumplir las mismas condiciones
         //que el producto entre matrices
            double[,] producto;
            if (x.GetLength(1) != y.GetLength(0))
            {
                producto = new double[1, 1];
                return producto;
            }

            producto = new double[x.GetLength(0), y.GetLength(1)];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < y.GetLength(1); j++)
                {
                    for (int k = 0; k < y.GetLength(0); k++)
                    {
                        producto[i, j] += x[i, k] * y[k, j];
                    }
                }
            }

            return producto;
        }



        static double[,] ProductoWithScalar(double x, double[,] matrix)
        {
            double[,] product = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    product[i, j] = x * matrix[i, j];
                }
            }

            return product;
        }


    }
}