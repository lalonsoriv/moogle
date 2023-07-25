#!/bin/bash

read -p "Escriba el comando que desea ejecutar a continuación: "  commands

case "$commands" in
run)
		cd ../
		dotnet watch run --project MoogleServer
		;;
    report)
		cd ../informe/
		pdfLatex informe.tex
		;;
    slides)
		cd ../presentacion/
		pdfLatex presentacion.tex
		;;
    show_report)
		if [ -f ../informe/informe.pdf ]
		then
			cd ../informe/
			start informe.pdf
		else
			cd ../informe/
			pdfLatex informe.tex
			cd ../informe/
			start informe.pdf
		fi
		;;	
    show_slides)
		if [ -f ../presentacion/presentacion.pdf ]
		then
			cd ../presentacion/
			start presentacion.pdf
		else
			cd ../presentacion/
			pdfLatex presentacion.tex
			cd ..presentacion/
			start presentacion.pdf
		fi
		;;
    clean) 
        rm -f ../informe/*.pdf
		rm -f ../informe/*.synctex.gz
		rm -f ../presentacion/*.pdf
		rm -f ../presentacion/*.synctex.gz	
	;;

	exit)
        break
	;;
    *)echo "Esta no es una opción válida."
    ;;

esac
