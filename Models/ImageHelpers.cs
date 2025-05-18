using OpenCvSharp;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace PLSImageProcessing.Models
{
    public class ImageHelpers
    {
        public static Mat[] LoadImages(string folderPath, int width = 200, int height = 200)
        {
            var files = Directory.GetFiles(folderPath, "*.jpg");
            Mat[] images = new Mat[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                Mat img = Cv2.ImRead(files[i], ImreadModes.Grayscale);
                Cv2.Resize(img, img, new OpenCvSharp.Size(width, height));
                img.ConvertTo(img, MatType.CV_64FC1);
                images[i] = img;
            }

            return images;
        }

        public static List<Matrix<double>> LoadImagesAsMatrix(string folderPath, int width = 200, int height = 250)
        {
            var files = Directory.GetFiles(folderPath, "*.jpg");
            var images = new List<Matrix<double>>(files.Length);

            foreach (var file in files)
            {
                // 1. Загрузка и преобразование в grayscale
                using var img = Cv2.ImRead(file, ImreadModes.Grayscale);

                if (img.Empty())
                {
                    Console.WriteLine($"Warning: Could not load image {file}");
                    continue;
                }

                // 2. Изменение размера
                using var resizedImg = new Mat();
                Cv2.Resize(img, resizedImg, new OpenCvSharp.Size(width, height));

                // 3. Нормализация и преобразование в Matrix<double>
                var matrix = new DenseMatrix(height, width);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        // Получаем значение пикселя и нормализуем в [0, 1]
                        matrix[i, j] = resizedImg.At<byte>(i, j) / 255.0;
                    }
                }

                images.Add(matrix);
            }

            if (images.Count == 0)
            {
                throw new InvalidOperationException($"No valid images found in {folderPath}");
            }

            return images;
        }

        public static Mat MatrixToMat(Matrix<double> matrix)
        {
            // Конвертация Matrix<double> в Mat (диапазон 0-1 -> 0-255)
            var mat = new Mat(matrix.RowCount, matrix.ColumnCount, MatType.CV_8UC1);
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    mat.Set(i, j, (byte)(matrix[i, j] * 255));
                }
            }
            return mat;
        }


        public static Mat BitmapImageToMat(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                using (var bmp = new System.Drawing.Bitmap(outStream))
                {
                    return OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
                }
            }
        }

        public static BitmapImage MatToBitmapImage(Mat mat)
        {
            Cv2.Normalize(mat, mat, 0, 255, NormTypes.MinMax);
            mat.ConvertTo(mat, MatType.CV_8UC1);

            Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memory;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
    }
}
