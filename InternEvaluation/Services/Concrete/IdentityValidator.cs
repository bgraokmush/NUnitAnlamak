using InternEvaluation.Services.Abstract;
using InternEvaluation.Services.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternEvaluation.Services.Concrete
{
    public class IdentityValidator : IIdentityValidator
    {
        public ICountryDataProvider CountryDataProvider => throw new NotImplementedException();

        public ValidationMode ValidationMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsValid(string idNumber)
        {
            return idNumber.Length == 11;
        }
    }
}
