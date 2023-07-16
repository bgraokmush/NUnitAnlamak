using InternEvaluation.Models;
using InternEvaluation.Services.Constant;
using Moq;
using NUnit.Framework;
using InternEvaluation.Services.Abstract;
using FluentAssertions;

namespace InternEvaluation.Tests
{
    [TestFixture]
    public class InternEvaluationTests
    {
        //Mock objenin oluşturulması için metod
        private Mock<IIdentityValidator> InitialiseTestMock()
        {
            Mock<IIdentityValidator> mock = new Mock<IIdentityValidator>();
            mock.DefaultValue = DefaultValue.Mock;
            mock.Setup(i => i.CountryDataProvider.CountyData.Country).Returns("TURKEY");
            mock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            return mock;
        }

        private InternEvaluation InitialiseTestEvaluator(Mock<IIdentityValidator> mock)
        {
            InternEvaluation evaluator = new InternEvaluation(mock.Object);
            return evaluator;
        }

        // InternApplicant nesnesinin oluşturulması için metod
        private InternApplicant InitialiseTestInternApplicant()
        {
            var form = new InternApplicant();
            form.Intern = new Intern()
            {
                Id = 1,
                Name = "Test",
                Identity = "12345678901",
                Age = 18,
                isGraduate = false
            };
            form.TeckList = new List<string>() { "C#", "RabbitMQ", "Docker", "Microservice", "VisualStudio" };
            form.QuizSkore = 71;
            form.isInterviewSuccess = true;
            return form;
        }


        // Eğer form boş ise ArgumentNullException fırlatılmalıdır.
        [Test]
        public void Intern_WithNullForm_ShouldThrowArgumentNullException()
        {
            // Arrange
            Mock<IIdentityValidator> mock = InitialiseTestMock();
            var evaluator = InitialiseTestEvaluator(mock);
            InternApplicant form = null; // Test Case -> Null Form

            // Act
            Action act = () => evaluator.Evaluate(form);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        // 18 yaşından küçük ise AutoReject dönmeli
        [Test]
        public void Intern_WithUnderAge_SholdBeRejected()
        {
            // Arrange
            Mock<IIdentityValidator> mock = InitialiseTestMock();
            var evaluator = InitialiseTestEvaluator(mock);
            var form = InitialiseTestInternApplicant();
            form.Intern.Age = 17; // Test Case -> Under Age
            // Act
            var result = evaluator.Evaluate(form);

            // Assert
            result.Should().Be(ApplicatonResult.AutoReject);
        }

        // Identity geçersiz ise TransferredToHR dönmeli
        [Test]
        public void Intern_WithInvalidIdentity_ShouldBeTransferredToHR()
        {
            // Arrange
            Mock<IIdentityValidator> mock = InitialiseTestMock();
            mock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false); // test case -> invalid identity
            var evaluator = InitialiseTestEvaluator(mock);
            var form = InitialiseTestInternApplicant();
            // Act
            var result = evaluator.Evaluate(form);

            // Assert
            result.Should().Be(ApplicatonResult.TransferredToHR);
        }

        // Mezun ise TransferredToHR dönmeli
        [Test]
        public void Intern_WithGraduate_ShouldBeTransferredToHR()
        {
            // Arrange
            Mock<IIdentityValidator> mock = InitialiseTestMock();
            var evaluator = InitialiseTestEvaluator(mock);
            var form = InitialiseTestInternApplicant();
            form.Intern.isGraduate = true; // test case -> graduate

            // Act
            var result = evaluator.Evaluate(form);

            // Assert
            result.Should().Be(ApplicatonResult.TransferredToHR);
        }

        // Teknik yeterlilik puanı 70'den düşük ise AutoReject dönmeli
        [Test]
        public void Intern_WithLowTechScore_ShouldBeRejected()
        {
            // Arrange
            Mock<IIdentityValidator> mock = InitialiseTestMock();
            var evaluator = InitialiseTestEvaluator(mock);
            var form = InitialiseTestInternApplicant();
            form.QuizSkore = 69; // Test case -> low tech score

            // Act
            var result = evaluator.Evaluate(form);

            // Assert
            result.Should().Be(ApplicatonResult.AutoReject);
        }

        //Eğer yetenek benzerliği %25'den düşük ise AutoReject dönmeli
        [Test]
        public void Intern_WithLowTechSimilarity_ShouldBeRejected()
        {
            // Arrange
            Mock<IIdentityValidator> mock = InitialiseTestMock();
            var evaluator = InitialiseTestEvaluator(mock);
            var form = InitialiseTestInternApplicant();
            form.TeckList = new List<string>() { "C#" }; // Test case -> low tech similarity

            // Act
            var result = evaluator.Evaluate(form);

            // Assert
            result.Should().Be(ApplicatonResult.AutoReject);
        }

        // Eğer yetenek benzerliği %75' den büyük ise ve inceleme detaylı ise TransferredToCTO dönmelidir.
        [Test]
        public void Intern_WithHighTechSimilarityAndDetailedReview_ShouldBeTransferredToCTO()
        {
            // Arrange
            Mock<IIdentityValidator> mock = InitialiseTestMock();
            var evaluator = InitialiseTestEvaluator(mock);
            var form = InitialiseTestInternApplicant();
            form.TeckList = new List<string>() { "C#", "RabbitMQ", "Docker", "Microservice", "VisualStudio" }; // Test case -> high tech similarity
            form.Intern.Age = 26; // Test case -> detailed review


            // Act
            var result = evaluator.Evaluate(form);

            // Assert
            result.Should().Be(ApplicatonResult.TransferredToCTO);
        }

        // Eğer Ülkte Türkiye değilse AutoReject dönmelidir.
        [Test]
        public void Intern_WithNotTurkey_ShouldBeRejected()
        {
            // Arrange
            Mock<IIdentityValidator> mock = InitialiseTestMock();
            mock.Setup(i => i.CountryDataProvider.CountyData.Country).Returns("USA"); // Test case -> not turkey
            var evaluator = InitialiseTestEvaluator(mock);
            var form = InitialiseTestInternApplicant();

            // Act
            var result = evaluator.Evaluate(form);

            // Assert
            result.Should().Be(ApplicatonResult.AutoReject);
        }
    }
}