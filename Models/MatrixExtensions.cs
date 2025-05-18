using MathNet.Numerics.LinearAlgebra;
using OpenCvSharp;

namespace PLSImageProcessing.Models
{
    public static class MatrixExtensions
    {
        public static Mat ToOpenCv(this Matrix<double> matrix)
        {
            // Создаём Mat нужного размера и типа
            Mat mat = new Mat(matrix.RowCount, matrix.ColumnCount, MatType.CV_64FC1);

            // Преобразуем матрицу в одномерный массив
            double[] flatArray = matrix.ToRowMajorArray(); // Или matrix.ToArray() для column-major

            // Заполняем Mat данными
            mat.SetArray(flatArray);
            return mat;
        }

        public static Matrix<double> ToMathNet(this Mat mat)
        {
            // Проверяем тип матрицы
            if (mat.Type() != MatType.CV_64FC1)
                throw new ArgumentException("Mat must be of type CV_64FC1 (double)");

            // Получаем данные в виде одномерного массива
            double[] data = new double[mat.Rows * mat.Cols];
            mat.GetArray(out data);

            // Создаём матрицу MathNet (row-major)
            return Matrix<double>.Build.Dense(mat.Rows, mat.Cols, data);
        }
    }
}
