using InternEvaluation.Services.Constant;

namespace InternEvaluation.Services.Abstract
{
    public interface IIdentityValidator
    {
        bool IsValid(string idNumber);

        ICountryDataProvider CountryDataProvider { get; }

        public ValidationMode ValidationMode { get; set; }
    }
}
