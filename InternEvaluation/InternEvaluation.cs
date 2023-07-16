using InternEvaluation.Models;
using InternEvaluation.Services.Abstract;
using InternEvaluation.Services.Constant;

namespace InternEvaluation
{
    public class InternEvaluation
    {
        // İlgili koşullar için sabitler
        private const int minAge = 18;
        private const int autoAcceptedYearsOfExperience = 10;
        private List<string> TeckList = new() { "C#", "RabbitMQ", "Docker", "Microservice", "VisualStudio" };
        private readonly IIdentityValidator _iIdentityValidator;
        private bool disposedValue;

        public InternEvaluation(IIdentityValidator iIdentityValidator)
        {
            _iIdentityValidator = iIdentityValidator;
        }

        public ApplicatonResult Evaluate(InternApplicant form)
        {
            // Eğer parametre boş ise ArgumentNullException fırlatılmalıdır.
            if(form is null)
            {
                throw new ArgumentNullException(nameof(form));
            }
            
            // Eğer stajyerin yaş bilgisi 18'den küçük ise ApplicatonResult.AutoReject dönmelidir.
            if(form.Intern.Age < minAge)
            {
                return ApplicatonResult.AutoReject;
            }
            
            // Kimlik numarası geçerli değilse TransferredToHR dönmelidir.
            var validIdentity = _iIdentityValidator.IsValid(form.Intern.Identity);
            if (!validIdentity)
            {
                return ApplicatonResult.TransferredToHR;
            }

            // Eğer stajyer mezun ise TransferredToHR dönmelidir.
            if(form.Intern.isGraduate)
            {
                return ApplicatonResult.TransferredToHR;
            }

            // Eğer stajyer testten 70'den küçük puan almış ise AutoReject dönmelidir.
           if(form.QuizSkore < 70)
            {
                  return ApplicatonResult.AutoReject;
            }


            // Eğer yetenek benzerliği %25 'den küçük ise AutoReject dönmelidir.
            var similarTechStackCount = GetSimilarTechStackCount(form.TeckList);
            if(similarTechStackCount < 25)
            {
                  return ApplicatonResult.AutoReject;
            }


            // Eğer yetenek benzerliği %75' den büyük ise ve inceleme detaylı ise TransferredToCTO dönmelidir.
            var validationMode = form.Intern.Age > 25 ? ValidationMode.Detailed : ValidationMode.Quick;
            if (similarTechStackCount > 75 && validationMode is ValidationMode.Detailed)
            {
                return ApplicatonResult.TransferredToCTO;
            }

            // Eğer Ülkte Türkiye değilse AutoReject dönmelidir.
            if (_iIdentityValidator.CountryDataProvider.CountyData.Country != "TURKEY")
            {
                return ApplicatonResult.AutoReject;
            }

            return ApplicatonResult.AutoAccept;
        }


        // Listenin ne kadar benzer olduğunu yüzde olarak hesaplayan fonksiyon
        private int GetSimilarTechStackCount(List<string> List)
        {
            int count = List
                .Where(x => TeckList.Contains(x, StringComparer.OrdinalIgnoreCase))
                .Count();

            return (int)((double)(count / TeckList.Count) * 100);
        }
    }
}
