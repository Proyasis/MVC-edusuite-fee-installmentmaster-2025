using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
namespace CITS.EduSuite.Business.Models.ViewModels
{
    public sealed class LessThanOrEqualToIfNotAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "{0} cannot be the same as {1}.";

        public string compareProperty { get; private set; }
        public string dependentProperty1 { get; private set; }
        public object dependentValue1 { get; private set; }
        public string dependentProperty2 { get; private set; }
        public object dependentValue2 { get; private set; }
        public LessThanOrEqualToIfNotAttribute(string CompareProperty, string DependentProperty1, object DependentValue1, string DependentProperty2, object DependentValue2)
            : base(DefaultErrorMessage)
        {
            compareProperty = CompareProperty;
            dependentProperty1 = DependentProperty1;
            dependentValue1 = DependentValue1;
            dependentProperty2 = DependentProperty2;
            dependentValue2 = DependentValue2;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, compareProperty);
        }

        protected override ValidationResult IsValid(object value,
                              ValidationContext validationContext)
        {
            if (value != null)
            {
                var CompareProperty = validationContext.ObjectInstance.GetType()
                                   .GetProperty(compareProperty);

                var ComparePropertyValue = (IComparable)CompareProperty
                              .GetValue(validationContext.ObjectInstance, null);

                var DependentProperty1 = validationContext.ObjectInstance.GetType()
                                  .GetProperty(dependentProperty1);

                var DependentPropertyValue1 = (IComparable)DependentProperty1
                              .GetValue(validationContext.ObjectInstance, null);

                var DependentProperty2 = validationContext.ObjectInstance.GetType()
                                  .GetProperty(dependentProperty2);

                var DependentPropertyValue2 = (IComparable)DependentProperty2
                              .GetValue(validationContext.ObjectInstance, null);
                var ValueThis = (IComparable)value;
                var check1 = DependentPropertyValue1 != null && dependentValue1 != null ? DependentPropertyValue1.ToString() != dependentValue1.ToString() : false;
                var check2 = DependentPropertyValue2 != null && dependentValue2 != null ? DependentPropertyValue2.ToString() != dependentValue2.ToString() : false;
                if (((ValueThis.CompareTo(ComparePropertyValue) > 0) && check1 && check2) && Convert.ToDecimal(ValueThis) != 0)
                {
                    return new ValidationResult(
                      FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "lessthanorequaltoif";
            compareRule.ValidationParameters.Add("comparepropertyname", compareProperty);
            compareRule.ValidationParameters.Add("dependentproperty1", dependentProperty1 != null ? dependentProperty1.ToString() : dependentProperty1);
            compareRule.ValidationParameters.Add("dependentvalue1", dependentValue1 != null ? dependentValue1.ToString() : dependentValue1);
            compareRule.ValidationParameters.Add("dependentproperty2", dependentProperty2 != null ? dependentProperty2.ToString() : dependentProperty2);
            compareRule.ValidationParameters.Add("dependentvalue2", dependentValue2 != null ? dependentValue2.ToString() : dependentValue2);
            yield return compareRule;

        }
    }

    public sealed class GreaterThanOrEqualToIfNotAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "{0} cannot be the same as {1}.";

        public string compareProperty { get; private set; }
        public string dependentProperty1 { get; private set; }
        public object dependentValue1 { get; private set; }
        public string dependentProperty2 { get; private set; }
        public object dependentValue2 { get; private set; }
        public GreaterThanOrEqualToIfNotAttribute(string CompareProperty, string DependentProperty1, object DependentValue1, string DependentProperty2, object DependentValue2)
            : base(DefaultErrorMessage)
        {
            compareProperty = CompareProperty;
            dependentProperty1 = DependentProperty1;
            dependentValue1 = DependentValue1;
            dependentProperty2 = DependentProperty2;
            dependentValue2 = DependentValue2;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, compareProperty);
        }

        protected override ValidationResult IsValid(object value,
                              ValidationContext validationContext)
        {
            if (value != null)
            {
                var CompareProperty = validationContext.ObjectInstance.GetType()
                                   .GetProperty(compareProperty);

                var ComparePropertyValue = (IComparable)CompareProperty
                              .GetValue(validationContext.ObjectInstance, null);

                var DependentProperty1 = validationContext.ObjectInstance.GetType()
                                  .GetProperty(dependentProperty1);

                var DependentPropertyValue1 = (IComparable)DependentProperty1
                              .GetValue(validationContext.ObjectInstance, null);

                var DependentProperty2 = validationContext.ObjectInstance.GetType()
                                  .GetProperty(dependentProperty2);

                var DependentPropertyValue2 = (IComparable)DependentProperty2
                              .GetValue(validationContext.ObjectInstance, null);
                var ValueThis = (IComparable)value;
                var check1 = DependentPropertyValue1 != null && dependentValue1 != null ? DependentPropertyValue1.ToString() != dependentValue1.ToString() : false;
                var check2 = DependentPropertyValue2 != null && dependentValue2 != null ? DependentPropertyValue2.ToString() != dependentValue2.ToString() : false;
                if ((ValueThis.CompareTo(ComparePropertyValue) < 0) && check1 && check2 && Convert.ToDecimal(ValueThis) != 0)
                {
                    return new ValidationResult(
                      FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "greaterthanorequaltoif";
            compareRule.ValidationParameters.Add("comparepropertyname", compareProperty);
            compareRule.ValidationParameters.Add("dependentproperty1", dependentProperty1 != null ? dependentProperty1.ToString() : dependentProperty1);
            compareRule.ValidationParameters.Add("dependentvalue1", dependentValue1 != null ? dependentValue1.ToString() : dependentValue1);
            compareRule.ValidationParameters.Add("dependentproperty2", dependentProperty2 != null ? dependentProperty2.ToString() : dependentProperty2);
            compareRule.ValidationParameters.Add("dependentvalue2", dependentValue2 != null ? dependentValue2.ToString() : dependentValue2);
            yield return compareRule;

        }
    }

    public sealed class FileTypeAttribute : ValidationAttribute, IClientValidatable
    {
        private List<string> _Extensions { get; set; }
        private const string _DefaultErrorMessage = "Only file types with the following extensions are allowed: {0}";
        public FileTypeAttribute(string fileExtensions)
        {
            _Extensions = fileExtensions.Split('|').Select(x => x.ToLower()).ToList();
            ErrorMessage = _DefaultErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;
            if (file != null)
            {
                var isValid = _Extensions.Any(e => file.FileName.ToLower().EndsWith(e));
                if (!isValid)
                {
                    return new ValidationResult(string.Format(ErrorMessageString, string.Join(", ", _Extensions)));
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "filetype",
                ErrorMessage = string.Format(ErrorMessageString, string.Join(", ", _Extensions))
            };
            rule.ValidationParameters.Add("extensions", string.Join(",", _Extensions));
            yield return rule;
        }
    }

    public sealed class FileSizeAttribute : ValidationAttribute, IClientValidatable
    {
        private float _maximumSize { get; set; }
        private double _maximumSizeByte { get; set; }
        private const string _DefaultErrorMessage = "Your Photo is too large, maximum allowed size is : {0} kb";
        public FileSizeAttribute()
        {
            _maximumSize = float.Parse(WebConfigurationManager.AppSettings["FileSizeKB"] != null ? WebConfigurationManager.AppSettings["FileSizeKB"].ToString() : "0");
            _maximumSizeByte = _maximumSize * 1024;
            ErrorMessage = _DefaultErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;
            if (file != null)
            {
                var isValid = file.ContentLength > _maximumSize;
                if (!isValid)
                {
                    return new ValidationResult(string.Format(ErrorMessageString, _maximumSize.ToString()));
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "filesize",
                ErrorMessage = string.Format(ErrorMessageString, _maximumSize.ToString())
            };
            rule.ValidationParameters.Add("maxsize", _maximumSize);
            yield return rule;
        }
    }

    public sealed class DateRestrictionAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "{0} cannot be the same as {1}.";

        public string compareProperty { get; private set; }
        public DateRestrictionAttribute(string CompareProperty)
            : base(DefaultErrorMessage)
        {
            compareProperty = CompareProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }

        protected override ValidationResult IsValid(object value,
                              ValidationContext validationContext)
        {
            if (value != null)
            {
                var CompareProperty = validationContext.ObjectInstance.GetType()
                                   .GetProperty(compareProperty);

                var ComparePropertyValue = (long)CompareProperty
                              .GetValue(validationContext.ObjectInstance, null);

                int DateValidationBefore = Convert.ToInt32(WebConfigurationManager.AppSettings["DateValidationBefore"]);
                DateTime TodayDate = DateTimeUTC.Now.Date;
                DateTime ValueThis = Convert.ToDateTime((IComparable)value).Date;
                var compare = (TodayDate - ValueThis).TotalDays;
                // if ((compare > DateValidationBefore || compare < 0) && ComparePropertyValue == 0)
                if ((compare > DateValidationBefore && ComparePropertyValue == 0) || compare < 0) // Edit Future date blocking
                {
                    return new ValidationResult(
                      FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "daterestriction";
            compareRule.ValidationParameters.Add("comparepropertyname", compareProperty);
            yield return compareRule;

        }
    }

    public sealed class BlockPartyConversionAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "{0} cannot be the same as {1}.";

        public string compareProperty { get; private set; }
        public string dependentProperty1 { get; private set; }
        public object dependentValue1 { get; private set; }
        public BlockPartyConversionAttribute(string CompareProperty, string DependentProperty1, object DependentValue1)
            : base(DefaultErrorMessage)
        {
            compareProperty = CompareProperty;
            dependentProperty1 = DependentProperty1;
            dependentValue1 = DependentValue1;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, compareProperty);
        }

        protected override ValidationResult IsValid(object value,
                              ValidationContext validationContext)
        {
            var CompareProperty = validationContext.ObjectInstance.GetType()
                               .GetProperty(compareProperty);

            var ComparePropertyValue = (IComparable)CompareProperty
                          .GetValue(validationContext.ObjectInstance, null);

            var DependentProperty1 = validationContext.ObjectInstance.GetType()
                              .GetProperty(dependentProperty1);

            var DependentPropertyValue1 = (IComparable)DependentProperty1
                          .GetValue(validationContext.ObjectInstance, null);

            var ValueThis = (IComparable)value;
            var check1 = DependentPropertyValue1 != null && dependentValue1 != null ? DependentPropertyValue1.ToString() == dependentValue1.ToString() : false;
            var check2 = ((ComparePropertyValue ?? "") == "" && (ValueThis ?? "") == "") || ((ComparePropertyValue ?? "") != "" && (ValueThis ?? "") != "") ? false : true;
            if (check1 && check2)
            {
                return new ValidationResult(
                  FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "blockpartyconversion";
            compareRule.ValidationParameters.Add("comparepropertyname", compareProperty);
            compareRule.ValidationParameters.Add("dependentproperty1", dependentProperty1 != null ? dependentProperty1.ToString() : dependentProperty1);
            compareRule.ValidationParameters.Add("dependentvalue1", dependentValue1 != null ? dependentValue1.ToString() : dependentValue1);
            yield return compareRule;

        }
    }

    public sealed class CustomRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "{0} is requied.";

        public string columnProperty { get; private set; }
        public string EnableProprety { get; set; }
        public CustomRequiredAttribute(string ColumnProperty)
            : base(DefaultErrorMessage)
        {
            columnProperty = ColumnProperty;
            EnableProprety = null;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }

        protected override ValidationResult IsValid(object value,
                              ValidationContext validationContext)
        {
            bool enabled = true;
            if (EnableProprety != null)
            {
                enabled = (bool)typeof(ApplicationSettingModel).GetProperty(EnableProprety).GetValue(typeof(ApplicationSettingModel));
            }
            bool required = true;


            required = enabled && (bool)typeof(ApplicationSettingModel).GetProperty(columnProperty).GetValue(typeof(ApplicationSettingModel));
            if (required && (value == null))
            {
                return new ValidationResult(
                  FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            bool enabled = true;
            if (EnableProprety != null)
            {
                enabled = (bool)typeof(ApplicationSettingModel).GetProperty(EnableProprety).GetValue(typeof(ApplicationSettingModel));
            }
            bool required = true;

            required = (bool)typeof(ApplicationSettingModel).GetProperty(columnProperty).GetValue(typeof(ApplicationSettingModel));

            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "customrequired";
            compareRule.ValidationParameters.Add("required", required);
            compareRule.ValidationParameters.Add("enabled", enabled);
            yield return compareRule;

        }
    }

    public sealed class CustomRequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "{0} is requied.";

        public string columnProperty { get; private set; }
        public string dependentProperty { get; private set; }
        public object dependentValue { get; private set; }
        public string EnableProprety { get; set; }
        public CustomRequiredIfAttribute(string ColumnProperty, string DependentProperty, object DependentValue)
            : base(DefaultErrorMessage)
        {
            columnProperty = ColumnProperty;
            dependentProperty = DependentProperty;
            dependentValue = DependentValue;
            EnableProprety = null;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }

        protected override ValidationResult IsValid(object value,
                              ValidationContext validationContext)
        {


            var DependentProperty = validationContext.ObjectInstance.GetType()
                              .GetProperty(dependentProperty);
            var DependentPropertyValue = (IComparable)DependentProperty
                          .GetValue(validationContext.ObjectInstance, null);






            var DependentValueCompare = (IComparable)DependentPropertyValue;

            Type DepenedentType = DependentProperty.PropertyType;
            if (DepenedentType.IsGenericType && DepenedentType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (dependentValue == null)
                {
                    return null;
                }

                DepenedentType = Nullable.GetUnderlyingType(DepenedentType);
            }

            dependentValue = Convert.ChangeType(dependentValue, DepenedentType);
            bool enabled = true;
            if (EnableProprety != null)
            {
                enabled = (bool)typeof(ApplicationSettingModel).GetProperty(EnableProprety).GetValue(typeof(ApplicationSettingModel));
            }
            bool required = true;

            required = enabled && (bool)typeof(ApplicationSettingModel).GetProperty(columnProperty).GetValue(typeof(ApplicationSettingModel));

            if (required && (value == null) && DependentValueCompare.CompareTo(dependentValue) == 0)
            {
                return new ValidationResult(
                  FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            bool enabled = true;
            if (EnableProprety != null)
            {
                enabled = (bool)typeof(ApplicationSettingModel).GetProperty(EnableProprety).GetValue(typeof(ApplicationSettingModel));
            }
            bool required = true;


            required = enabled && (bool)typeof(ApplicationSettingModel).GetProperty(columnProperty).GetValue(typeof(ApplicationSettingModel));

            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "customrequiredif";
            compareRule.ValidationParameters.Add("required", required);
            compareRule.ValidationParameters.Add("dependentproperty", dependentProperty);
            compareRule.ValidationParameters.Add("dependentvalue", dependentValue);
            compareRule.ValidationParameters.Add("enabled", enabled);
            yield return compareRule;

        }

    }

    public sealed class StringLengthDynamicAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "Enter a valid {0}.";       
        public string maxLengthProperty { get; private set; }
        public string minLengthProperty { get; private set; }
        public StringLengthDynamicAttribute(string MaxLengthProperty, string MinLengthProperty)
            : base(DefaultErrorMessage)
        {
            maxLengthProperty = MaxLengthProperty;
            minLengthProperty = MinLengthProperty;
        }
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }
        protected override ValidationResult IsValid(object value,
                             ValidationContext validationContext)
        {
            if (value != null)
            {
                var MaxLengthProperty = validationContext.ObjectInstance.GetType()
                                   .GetProperty(maxLengthProperty);

                var MaxLengthPropertyValue = (IComparable)MaxLengthProperty
                              .GetValue(validationContext.ObjectInstance, null);

                var MinLengthProperty = validationContext.ObjectInstance.GetType()
                                  .GetProperty(minLengthProperty);

                var MinLengthPropertyValue = (IComparable)MinLengthProperty
                              .GetValue(validationContext.ObjectInstance, null);

                //var ValueThis = (IComparable)value;
                int valulength = ((string)value).Length;

                if (valulength > Convert.ToInt64(MaxLengthPropertyValue) || valulength < Convert.ToInt64(MinLengthPropertyValue))
                {
                    return new ValidationResult(
                      FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "stringlengthdynamic";
            compareRule.ValidationParameters.Add("maxlengthproperty", maxLengthProperty != null ? maxLengthProperty.ToString() : maxLengthProperty);
            compareRule.ValidationParameters.Add("minlengthproperty", minLengthProperty != null ? minLengthProperty.ToString() : minLengthProperty);
            yield return compareRule;
        }
    }

}