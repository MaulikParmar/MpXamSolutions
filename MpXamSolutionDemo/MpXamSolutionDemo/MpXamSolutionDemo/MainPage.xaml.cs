using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MpXamSolutionDemo
{
    public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            var vm = new TestViewModel();
            this.BindingContext = vm;
            vm.Validate();

            btn.Clicked += (sender, e) =>
            {
                vm.Validate();
            };
            
		}
	}

    public class TestViewModel : MpXamSolution.Helpers.ValidationBase, INotifyPropertyChanged
    {
        private string _firstname;
        [Required]
        [Display(Name = "First Name")]
        public string FirstName
        {
            get { return _firstname; }
            set
            {
                _firstname = value;
                OnPropertyChanged();
                ValidateProperty(value);                
            }
        }

        private string _lastName;
        [Required]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged();
                ValidateProperty(value);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
