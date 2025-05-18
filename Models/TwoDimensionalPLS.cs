using OpenCvSharp;

namespace PLSImageProcessing.Models
{
    public static class TwoDimensionalPLS
    {
        private static int[] _dValues = new int[] { 10, 50, 100, 200 };
        public static PLSResult ProcessPLS(Mat[] imagesX, Mat[] imagesY)
        {
            Mat meanX = new Mat(new Size(imagesX[0].Width, imagesX[0].Height), MatType.CV_64FC1);
            Mat meanY = new Mat(new Size(imagesY[0].Width, imagesY[0].Height), MatType.CV_64FC1);

            foreach (var image in imagesX)
            {
                Cv2.Add(meanX, image, meanX);
            }
            Cv2.Divide(meanX, imagesX.Length, meanX);

            foreach (var image in imagesY)
            {
                Cv2.Add(meanY, image, meanY);
            }
            Cv2.Divide(meanY, imagesY.Length, meanY);


            List<Mat> centeredX = new List<Mat>();
            List<Mat> centeredY = new List<Mat>();

            foreach (var image in imagesX)
            {
                centeredX.Add(image - meanX);
            }

            foreach (var image in imagesY)
            {
                centeredY.Add(image - meanY);
            }


            Mat CRxy = new Mat(new Size(200, 200), MatType.CV_64FC1);
            Mat CRyx = new Mat(new Size(200, 200), MatType.CV_64FC1);


            for (int i = 0; i < centeredX.Count; i++)
            {
                Cv2.Add(CRxy, centeredX[i] * centeredY[i].T(), CRxy);
            }
            CRyx = CRxy.T().ToMat();

            Mat CCxy = new Mat(new Size(200, 200), MatType.CV_64FC1);
            Mat CCyx = new Mat(new Size(200, 200), MatType.CV_64FC1);


            for (int i = 0; i < centeredX.Count; i++)
            {
                Cv2.Add(CCxy, centeredX[i].T() * centeredY[i], CCxy);
            }
            CCyx = CCxy.T().ToMat();

            Mat S1r = CRxy * CRyx;
            Mat S2r = CRyx * CRxy;
            Mat S1c = CCxy * CCyx;
            Mat S2c = CCyx * CCxy;


            Mat eigenValuesS1r = new Mat();
            Mat Wx1 = new Mat();
            Cv2.Eigen(S1r, eigenValuesS1r, Wx1);
            Mat eigenValuesS2r = new Mat();
            Mat Wx2 = new Mat();
            Cv2.Eigen(S1c, eigenValuesS2r, Wx2);
            Mat eigenValuesS1c = new Mat();
            Mat Wy1 = new Mat();
            Cv2.Eigen(S2r, eigenValuesS1c, Wy1);
            Mat eigenValuesS2c = new Mat();
            Mat Wy2 = new Mat();
            Cv2.Eigen(S2c, eigenValuesS2c, Wy2);

            var plsResult = new PLSResult();

            var xIO = new Mat();
            var yIO = new Mat();

            centeredX[0].ConvertTo(xIO, MatType.CV_8UC1);
            centeredY[0].ConvertTo(yIO, MatType.CV_8UC1);

            plsResult.OrigImagePhoto = ImageHelpers.MatToBitmapImage(xIO);
            plsResult.OrigImageSketch = ImageHelpers.MatToBitmapImage(yIO);

            foreach (var d in _dValues)
            {
                List<Mat> U = new List<Mat>();
                List<Mat> V = new List<Mat>();

                Mat Wx1d = RRPPRow(Wx1.T(), d);
                Mat Wx2d = RRPPCol(Wx2, d);
                Mat Wy1d = RRPPRow(Wy1.T(), d);
                Mat Wy2d = RRPPCol(Wy2, d);
                
                U.Add(Wx1d * centeredX[0] * Wx2d);
                V.Add(Wy1d * centeredY[0] * Wy2d);

                var xI = (Wx1d.T() * U[0] * Wx2d.T()).ToMat();
                var yI = (Wy1d.T() * V[0] * Wy2d.T()).ToMat();

                xI.ConvertTo(xI, MatType.CV_8UC1);
                yI.ConvertTo(yI, MatType.CV_8UC1);

                if (d == 10)
                {
                    plsResult.D10ImagePhoto = ImageHelpers.MatToBitmapImage(xI);
                    plsResult.D10ImageSketch = ImageHelpers.MatToBitmapImage(yI);
                }
                if (d == 50)
                {
                    plsResult.D50ImagePhoto = ImageHelpers.MatToBitmapImage(xI);
                    plsResult.D50ImageSketch = ImageHelpers.MatToBitmapImage(yI);
                }
                if (d == 100)
                {
                    plsResult.D100ImagePhoto = ImageHelpers.MatToBitmapImage(xI);
                    plsResult.D100ImageSketch = ImageHelpers.MatToBitmapImage(yI);
                }
                if (d == 200)
                {
                    plsResult.D200ImagePhoto = ImageHelpers.MatToBitmapImage(xI);
                    plsResult.D200ImageSketch = ImageHelpers.MatToBitmapImage(yI);

                }
            }

            return plsResult;
        }

        private static Mat RRPPRow(Mat mat, int d)
        {            

            var rowSums = Enumerable.Range(0, mat.Rows)
            .Select(i => (Index: i, Sum: mat.Row(i).Sum().Val0 / mat.Rows))
            .OrderByDescending(x => x.Sum)
            .Take(d);

            var result = new Mat(d, mat.Cols, mat.Type());
            int j = 0;
            foreach (var row in rowSums)
                mat.Row(row.Index).CopyTo(result.Row(j++));

            return result;
        }

        private static Mat RRPPCol(Mat mat, int d)
        {            

            var colSums = Enumerable.Range(0, mat.Cols)
            .Select(i => (Index: i, Sum: mat.Col(i).Sum().Val0 / mat.Cols))
            .OrderByDescending(x => x.Sum)
            .Take(d);

            var result = new Mat(mat.Cols, d, mat.Type());
            int j = 0;
            foreach (var col in colSums)
                mat.Col(col.Index).CopyTo(result.Col(j++));

            return result;
        }
    }
}
