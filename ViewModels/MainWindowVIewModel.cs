using BaseTemplateForWPF;
using PLSImageProcessing.Models;
using System.Windows;
using System.Windows.Input;

namespace PLSImageProcessing.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
		private readonly string _photoDirectory = System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\photos";
		private readonly string _skethDirectory = System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\sketches";
        private bool _needupdatePhoto = true;
        public MainWindowViewModel()
        {
			LoadedCommand = new LambdaCommand(OnLoadedCommandExecute, CanLoadedCommandExecuted);
            ProcessImageCommand = new LambdaCommand(OnProcessImageCommandExecute, CanProcessImageCommandExecuted);
        }

        #region Title 

        private string _Title = "PLSImageProcessing";
		public string Title
        {
			get => _Title;
			set => Set(ref _Title, value);
		}

        #endregion

        #region LoadVisisbility 

        private Visibility _LoadVisisbility = Visibility.Hidden;
		public Visibility LoadVisisbility
        {
			get => _LoadVisisbility;
			set => Set(ref _LoadVisisbility, value);
		}

		#endregion

		#region ContentEnabled 

		private bool _ContentEnabled = true;
		public bool ContentEnabled
        {
			get => _ContentEnabled;
			set => Set(ref _ContentEnabled, value);
		}

		#endregion

		#region N5Result 

		private PLSResult _N5Result = new PLSResult();
		public PLSResult N5Result
        {
			get => _N5Result;
			set => Set(ref _N5Result, value);
		}

		#endregion

		#region N100Result 

		private PLSResult _N100Result = new PLSResult();
		public PLSResult N100Result
        {
			get => _N100Result;
			set => Set(ref _N100Result, value);
		}

		#endregion

		#region N188Result 

		private PLSResult _N188Result = new PLSResult();
		public PLSResult N188Result
        {
			get => _N188Result;
			set => Set(ref _N188Result, value);
		}

		#endregion

		#region Commands

		#region ProcessImageCommand
		public ICommand ProcessImageCommand { get; }
        private async void OnProcessImageCommandExecute(object p)
        {
			await Task.Run(() => 
			{
				TurnToLoadMode();
				var imagesX = ImageHelpers.LoadImages(_photoDirectory);
				var imagesY = ImageHelpers.LoadImages(_skethDirectory);
            
				N5Result = TwoDimensionalPLS.ProcessPLS(imagesX.Take(5).ToArray(), imagesY.Take(5).ToArray());
				N100Result = TwoDimensionalPLS.ProcessPLS(imagesX.Take(100).ToArray(), imagesY.Take(100).ToArray());
				N188Result = TwoDimensionalPLS.ProcessPLS(imagesX, imagesY);
				TurnToDefaultMode();
			});
        }
        private bool CanProcessImageCommandExecuted(object p) => true;
        #endregion

		#region LoadedCommand
        public ICommand LoadedCommand { get; }
		private async void OnLoadedCommandExecute(object p)
		{   
        }
		private bool CanLoadedCommandExecuted(object p) => true;
		#endregion

		#endregion

		#region Methods        

		private void TurnToLoadMode()
		{
			LoadVisisbility = Visibility.Visible;
			ContentEnabled = false;
		}

        private void TurnToDefaultMode()
        {
            LoadVisisbility = Visibility.Hidden;
            ContentEnabled = true;
        }       

        #endregion
    }
}
