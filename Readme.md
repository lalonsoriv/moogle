# Moogle!

Para poder el proyecto lo primero que tendrás que hacer para poder trabajar en este proyecto es instalar .NET Core 6.0. Luego, solo te debes parar en la carpeta del proyecto y ejecutar en la terminal de Linux:

```bash
make dev
```

Si estás en Windows, debes poder hacer lo mismo desde la terminal del WSL (Windows Subsystem for Linux). Si no tienes WSL ni posibilidad de instalarlo, el comando *ultimate* para ejecutar la aplicación es (desde la carpeta raíz del proyecto):

```bash
dotnet watch run --project MoogleServer
```

# Manual de uso de los operadores ('!', '^', '*'):
- Si se desea que una palabra no esté presente en todos los documentos que sean devueltos tras realizar la búsqueda se debe aplicar el operador '!'.
- Si se desea que una palabra esté presente en todos los documentos que sean devueltos tras realizar la búsqueda se debe aplicar el operador '^'.
- Si se desea que los documentos que contienen la palabra sean devueltos con mayor prioridad que los que no la contengan se debe aplicar el operador '*'. A mayor cantidad de repeticiones del operador más importante será la palabra.
- Si se añade el operador '^' después de '*' el programa entenderá que solo se están pididendo los documentos que contengan la palabra.
- Si después de los operadores '!' y '^' se añade cualquier otro el programa pedirá que se vuelva a introducir la búsqueda pues lo tomará como mal uso de dichos caracteres.

> Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.
> Laura Alonso Rivero C-122