using NUnit.Framework;
using Moq;
using ModLocalization;
using System.IO;
using System.Reflection;
using ModLocalization.Record;

namespace ModLocalization.Tests
{
    [TestFixture]
    public class TranslationUpdaterTests
    {
        private TranslationUpdater translationUpdater;
        private string tempCsvFilePath;

        [SetUp]
        public void Setup()
        {
            translationUpdater = new TranslationUpdater();

            // Create a temporary CSV file and write test data to it
            tempCsvFilePath = Path.GetTempFileName();
            WriteTestDataToCsv(tempCsvFilePath);
        }

        [TearDown]
        public void TearDown()
        {
            // Delete the temporary CSV file
            if (File.Exists(tempCsvFilePath))
            {
                File.Delete(tempCsvFilePath);
            }
        }

        [Test]
        public void UpdateTranslations_FileExists_ReturnsTrue()
        {
            // Act
            bool result = translationUpdater.UpdateTranslations(tempCsvFilePath);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void UpdateTranslations_TranslationsAddedToFile_ReturnsTrue()
        {
            // Arrange
            var translationUpdater = new TranslationUpdater();

            // Act
            bool result = translationUpdater.UpdateTranslations(tempCsvFilePath);

            // Assert
            Assert.IsTrue(result);

            // Read the contents of the updated CSV file
            string[] fileContents = File.ReadAllLines(tempCsvFilePath);

            // Verify the header
            string header = fileContents[0];
            string[] headerColumns = header.Split(',');

            // Get the expected column count dynamically using reflection
            int expectedColumnCount = GetPropertyCount<LocalizationRecord>();

            Assert.That(headerColumns.Length, Is.EqualTo(expectedColumnCount), "Number of header columns does not match the expected count.");

            // Verify the first data row
            string firstDataRow = fileContents[1];
            string[] dataRowColumns = firstDataRow.Split(',');

            // Check if the number of columns in the data row matches the header
            Assert.That(dataRowColumns.Length, Is.EqualTo(expectedColumnCount), "Number of data row columns does not match the expected count.");

            // Check if each column in the data row has a non-empty value
            foreach (string columnValue in dataRowColumns)
            {
                Assert.That(string.IsNullOrEmpty(columnValue), Is.False, "Data row contains empty column value.");
            }

            // ... Additional assertions if needed ...
        }
        private int GetPropertyCount<T>()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            return properties.Length;
        }

        // Helper method to write test data to the CSV file
        // Helper method to write test data to the CSV file
        private void WriteTestDataToCsv(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Key,File,Type,English");
                writer.WriteLine("jcphluxBlockTest,UI,Menu,\"This is a {0} test {1}\"");
            }
        }

    }
}
