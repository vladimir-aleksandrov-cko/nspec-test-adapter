using System;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using NSpec.Domain;

namespace NSpec.TestAdapter
{
    public static class Extensions
    {
        public static TestCase ToTestCase(this ExampleBase example, string source, DiaSession diaSession, IMessageLogger logger)
        {

            var methodInfo = example.BodyMethodInfo;
            var specClassName = methodInfo.DeclaringType.FullName;
            string exampleMethodName = methodInfo.Name;

            logger.SendMessage(TestMessageLevel.Warning, $"SpecClassName: {specClassName ?? "Null"}");
            logger.SendMessage(TestMessageLevel.Warning, $"ExampleMethodName: {exampleMethodName ?? "Null"}");
            logger.SendMessage(TestMessageLevel.Warning, $"Spec: {example.Spec ?? "Null"}");


            var navigationData = diaSession.GetNavigationData(specClassName, exampleMethodName);

            return new TestCase
            {
                FullyQualifiedName = example.FullName(),
                ExecutorUri = new Uri(Constants.ExecutorUriString),
                Source = source,
                DisplayName = example.FullName().BeautifyForDisplay(),
                CodeFilePath = navigationData.FileName,
                LineNumber = navigationData.MinLineNumber,
            };
        }

        public static MethodInfo GetExampleBodyInfo(ExampleBase baseExample) => baseExample.BodyMethodInfo;

        public static string BeautifyForDisplay(this string fullName)
        {
            string displayName;

            // chop leading, redundant 'nspec. ' context

            const string nspecPrefix = @"nspec. ";
            const int prefixLength = 7;

            displayName = fullName.StartsWith(nspecPrefix) ? fullName.Substring(prefixLength) : fullName;

            // replace context separator

            const string originalSeparator = @". ";
            const string displaySeparator = @" › ";

            displayName = displayName.Replace(originalSeparator, displaySeparator);

            return displayName;
        }
    }
}
