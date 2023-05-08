using System;
namespace MoogleEngine;

// Esta clase fue implementada como requisito de la asignartura Álgebra I pero no posee utilidad en el proyecto 
public class Matrix
{
    private double[,] matrix;
    public Matrix(double[,] matrix) { this.matrix = matrix; }
    public Matrix(int rows, int columns) { this.matrix = new double[rows, columns]; }
    public int Rows { get { return matrix.GetLength(0); } }
    public int Columns { get { return matrix.GetLength(1); } }


    public double this[int i, int j]
    {
        get
        {
            if (i < 0 || i >= Rows) throw new ArgumentOutOfRangeException("i");
            if (j < 0 || j >= Columns) throw new ArgumentOutOfRangeException("j");
            return matrix[i, j];
        }
        set
        {
            if (i < 0 || i >= Rows) throw new ArgumentOutOfRangeException("i");
            if (j < 0 || j >= Columns) throw new ArgumentOutOfRangeException("j");
            matrix[i, j] = value;
        }
    }

    public bool Equal(Matrix a)
    {
        // Verifica si las matrices son iguales
        if(a == null) throw new ArgumentNullException("a");
        if(a.Rows == this.Rows && a.Columns == this.Columns)
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    if (a[i,j] != this.matrix[i,j]) return false;
                }
            }
            return true;
        }
        else throw new InvalidOperationException("No se puede calcular las matrices son incopamtibles");
    }

    public Matrix Transposed()
    {
        if(this.Columns != this.Rows) throw new InvalidOperationException ("La matriz no es cuadrada");
        // Crear el array para el resultado
        Matrix transposed = new Matrix(this.Columns, this.Rows);

        // Recorrer las columnas de la matrix
        for (int i = 0; i < Columns; i++)
        {
            // Recorrer las filas de la matrix
            for (int j = 0; j < Rows; j++)
            {
                transposed[j, i] = this[i, j];
            }

        }
        return transposed;
    }

    public Matrix MatrixSum(Matrix a)
    {
        if (a == null) throw new ArgumentNullException("a");
        if (a.Rows != this.Rows || this.Columns != a.Columns) throw new ArgumentException("No se puede calcular las matrices son incompatibles");

        Matrix result = new Matrix(this.Rows, this.Columns);
        // Recorrer las filas de la matriz original
        for (int i = 0; i < Rows; i++)
        {
            // Recorrer las columnas de la matriz original
            for (int j = 0; i < Columns; i++)
            {
                result[i, j] = this[i, j] + a[i, j];
            }
        }
        return result;
    }

    public Matrix Mult(Matrix a)
    {
        Matrix result = new Matrix(this.Rows, a.Columns);

        if (a == null) throw new ArgumentNullException("a");
        if (this.Columns != a.Rows) throw new ArgumentException("No se puede calcular las matrices son incompatibles");

        // Recorrer las filas de result
        for (int i = 0; i < result.Rows; i++)
        {
            // Recorrer las columnas de result
            for (int j = 0; i < result.Columns; i++)
            {
                for (int k = 0; k < a.Rows; k++)
                {
                    result[i, j] += this[i, k] * a[k, j];
                }
            }
        }
        return result;
    }

    public Matrix ScalarProd(double scalar)
    {
        Matrix result = new Matrix(this.Rows, this.Columns);

        // Recorrer las filas de la matriz
        for (int i = 0; i < Rows; i++)
            // Recorrer las columnas de la matriz
            for (int j = 0; j < Columns; j++)
                // Operar el producto escalar
                result[i, j] = this[i, j] * scalar;

        return result;
    }
}