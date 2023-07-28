echo "Debe elegir una de las siguientes opciones para ejecutar el script"
echo run
echo report
echo show_report
echo slides
echo show_slides
echo clean
 read Var
 if [ "$Var" == -ne ];
then 
 make dev
fi
 if [ "$Var" == "report" ];
then 
 cd ..
 cd informe 
 pdflatex informe.tex
fi
 if [ "$Var" == "show_report" ];
then 
 cd ..
 cd informe
    if [ ! -f  "informe.pdf" ];
    then 
 pdflatex informe.tex
 clear
    fi
   cd ..
   cd informe
   archivo=$(pwd)/informe.pdf
   xdg-open "$archivo"
fi
 if [ "$Var" == "slides" ];
then 
 echo
fi
 if [ "$Var" == "show_slides" ];
then 
 echo 
fi

 if [ "$Var" == "clean" ];
 then
 cd ..
 cd informe
 carpeta=$(pwd)
 find "$carpeta" ! -name '*.pdf' ! -name '*.tex' ! -name '*.jpg' -type f -delete
 cd ..
 cd presentacion
 find "$carpeta" ! -name '*.pdf' ! -name '*.tex' ! -name '*.jpg' -type f -delete
 fi
 