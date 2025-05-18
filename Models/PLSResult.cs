using System.Windows.Media.Imaging;

namespace PLSImageProcessing.Models
{
    public class PLSResult
    {
        public BitmapImage OrigImagePhoto { get; set; }
        public BitmapImage OrigImageSketch { get; set; }
        public BitmapImage D10ImagePhoto { get; set; }
        public BitmapImage D10ImageSketch { get; set; }
        public BitmapImage D50ImagePhoto { get; set; }
        public BitmapImage D50ImageSketch { get; set; }
        public BitmapImage D100ImagePhoto { get; set; }
        public BitmapImage D100ImageSketch { get; set; }
        public BitmapImage D200ImagePhoto { get; set; }
        public BitmapImage D200ImageSketch { get; set; }
    }
}
