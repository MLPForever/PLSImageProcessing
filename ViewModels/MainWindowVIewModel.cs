using BaseTemplateForWPF;
using OpenCvSharp;

namespace PLSImageProcessing.ViewModels
{
    public class MainWindowVIewModel : BaseViewModel
    {

		#region Title 

		private string _Title = "PLSImageProcessing";
		public string Title
        {
			get => _Title;
			set => Set(ref _Title, value);
		}

		#endregion


		#region Photo 

		private Mat _Photo;
		public Mat Photo
        {
			get => _Photo;
			set => Set(ref _Photo, value);
		}

		#endregion
	}
}
