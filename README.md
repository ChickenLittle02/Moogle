# Moogle!

![](moogle.png)

> Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.

 Para incializar el buscador, copiamos los documentos de tipo .txt en la carpeta Content, y abrimos la consola en la carpeta del proyecto,
 si estamos en Windows ejecutar en la terminal de Windows el comando:

```bash
dotnet watch run --project MoogleServer
```
Y se abrirá un navegador en el cual ponemos la búsqueda que queremos realizar,

Podemos utilizar los operadores !,^,* delante de las palabras,
los cuales tendrán los siguientes efectos en los resultados de búsqueda:
SI la palabra tiene delante !, ejemplo !carro, se devolverán los documentos donde no aparezca la palabra carro
SI la palabra tiene delante ^, ejemplo ^carro, se devolverán solamente los documentos donde aparezca la palabra carro
SI la palabra tiene delante *, ejemplo *carro, los documentos que tengan la palabra carro tendrán mayor relevancia

También tenemos el operador "~ " que se utilizará entre dos palabras
asiento ~ carro
El cual dará una mayor relevancia a los documentos que tengan esas dos palabras, mientras más cerca mayor relevancia tendrá el documento
