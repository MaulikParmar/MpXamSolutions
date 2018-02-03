using MpXamSolution.Extentions;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace MpXamSolution.Controls
{
    public class MpLabel : Label, IControlValidation
    {
        #region Properties
        private string BindingPath = "";
        private INotifyScrollToProperty _NotifyScroll;
        private INotifyDataErrorInfo _NotifyErrors;
        #endregion

        #region Bindable Property

        #region Has Error
        public static readonly BindableProperty HasErrorProperty =
            BindableProperty.Create("HasError", typeof(bool), typeof(MpLabel), false, defaultBindingMode: BindingMode.TwoWay);

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            private set { SetValue(HasErrorProperty, value); }
        }
        #endregion

        #region ErrorMessage

        public static readonly BindableProperty ErrorMessageProperty =
           BindableProperty.Create("ErrorMessage", typeof(string), typeof(MpLabel), string.Empty);

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }
        #endregion

        #region ShowErrorMessage

        public static readonly BindableProperty ShowErrorMessageProperty =
           BindableProperty.Create("ShowErrorMessage", typeof(bool), typeof(MpLabel), false, defaultBindingMode: BindingMode.TwoWay);

        
        public bool ShowErrorMessage
        {
            get { return (bool)GetValue(ShowErrorMessageProperty); }
            set { SetValue(ShowErrorMessageProperty, value); }
        }
        #endregion

        #region BindingProperty
        public string BindingProperty
        {
            get { return (string)GetValue(BindingPropertyProperty); }
            set { SetValue(BindingPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindingProperty.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty BindingPropertyProperty =
            BindableProperty.Create("BindingProperty", typeof(string), typeof(MpLabel), "");
        #endregion

        #endregion

        #region Constructions
        public MpLabel()
        {
            IsVisible = false;
            this.ShowErrorMessage = true;
        }
        #endregion

        #region Actions / Methods
        private void SetPrivateFields(bool _hasError, string _errorMessage)
        {
            this.HasError = _hasError;
            this.ErrorMessage = _errorMessage;
            this.Text = _errorMessage;
            IsVisible = !string.IsNullOrEmpty(_errorMessage);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            this.CheckValidation();
        }

        /// <summary>
        /// Method will subscibe and unsubsribe Error changed event
        /// Get bindable property path of text property
        /// </summary>
        public void CheckValidation()
        {
            // Reset variables values
            // Set 'HasError' = false
            // Set 'ErrorMessage' = "";
            SetPrivateFields(false, "");

            BindingPath = "";
            //this.Placeholder = "";

            if (_NotifyErrors != null)
            {
                // Unsubscribe event
                _NotifyErrors.ErrorsChanged += _NotifyErrors_ErrorsChanged;
                _NotifyErrors = null; // Set null value on binding context change          
            }

            // Remove notify scroll to property
            if (_NotifyScroll != null)
            {
                _NotifyScroll.ScrollToProperty -= NotifyScroll_ScrollToProperty;
                _NotifyScroll = null;
            }

            if (string.IsNullOrEmpty(this.BindingProperty))
                return;

            // Set binding path
            BindingPath = this.BindingProperty;

            if (this.BindingContext != null && this.BindingContext is INotifyDataErrorInfo)
            {
                // Get 
                _NotifyErrors = this.BindingContext as INotifyDataErrorInfo;

                // Return do nothing for your object
                if (_NotifyErrors == null)
                    return;

                // Remove notify scroll to property
                if (this.BindingContext is INotifyScrollToProperty)
                {
                    _NotifyScroll = this.BindingContext as INotifyScrollToProperty;
                    _NotifyScroll.ScrollToProperty += NotifyScroll_ScrollToProperty;
                }                

                // Subscribe event
                _NotifyErrors.ErrorsChanged += _NotifyErrors_ErrorsChanged;

                // get property name for windows and other operating system
                // for windows 10 property name will be : properties
                // And other operation system its value : _properties
                string condition = "properties";

                // Get bindable properties
                var _propertiesFieldInfo = typeof(BindableObject)
                           .GetRuntimeFields()
                           .Where(x => x.IsPrivate == true && x.Name.Contains(condition))
                           .FirstOrDefault();                
            }
        }

        // Scroll to control when request for scroll to property
        private void NotifyScroll_ScrollToProperty(string PropertyName)
        {
            // If property is requested property
            if (this.BindingPath.Equals(PropertyName))
            {
                // Get scroll 
                ScrollView _scroll = Utility.GetParentControl<ScrollView>(this as Element);
                _scroll?.ScrollToAsync(this as Element, ScrollToPosition.Center, true);
            }
        }

        /// <summary>
        /// Method will fire on property changed
        /// Check validation of text property
        /// Set validation if any validation message on property changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _NotifyErrors_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            // Error changed
            if (e.PropertyName.Equals(this.BindingPath))
            {
                // Get errors
                string errors = _NotifyErrors
                            .GetErrors(e.PropertyName)
                            ?.Cast<string>()
                            .FirstOrDefault();

                // If has error
                // assign validation values
                if (!string.IsNullOrEmpty(errors))
                {
                    // HasError = true; //set has error value to true
                    // ErrorMessage = errors; // assign error
                    this.SetPrivateFields(true, errors);
                }
                else
                {
                    // reset error message and flag
                    // HasError = false;
                    //  ErrorMessage = "";
                    this.SetPrivateFields(false, "");
                }
            }
        }

        private void SetPlaceHolder()
        {
            if (!string.IsNullOrEmpty(BindingPath) && this.BindingContext != null)
            {
                // Get display attributes
                var _attributes = this.BindingContext.GetType()
                    .GetRuntimeProperty(BindingPath)
                    .GetCustomAttribute<DisplayAttribute>();

                // Set place holder
                if (_attributes != null)
                {
                    // this.Placeholder = _attributes.Name; // assign placeholder property
                }
            }
        }
        #endregion
    }
}
