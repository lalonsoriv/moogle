while [ true  ]
do

echo "Escriba options para poder ver la lista de opciones que se ofrecen"

read input

case $input in 
	options)
        echo "Sus opciones son: run, clean, report, slides, show_report, show_slides"
        ;;
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
		rm -f ../presentacion/*.synctex.gzog	
	;;

    *)echo "Esta no es una opción válida."
    ;;

esac
done  