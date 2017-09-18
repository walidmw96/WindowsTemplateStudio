// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;

using Xunit;

namespace Microsoft.Templates.Test
{
    [Collection("StyleCopCollection")]
    [Trait("Type", "CodeStyle")]
    [Trait("ExecutionSet", "Minimum")]
    public class CodeStyleEnforcementTests
    {
        [Fact]
        public void EnsureCSharpCodeDoesNotUseThis()
        {
            var result = CodeIsNotUsed("this.", ".cs");

            Assert.True(result.Item1, result.Item2);
        }

        [Fact]
        public void EnsureTemplatesDoNotUseTabsInWhitespace()
        {
            // Some of the merge functionality includes whitespace in string comparisons.
            // Ensuring all whitespace is spaces avoids issues where strings differ due to different whitespace (which can be hard to spot)
            void EnsureTabsNotUsed(string fileExtension)
            {
                var result = CodeIsNotUsed('\t'.ToString(), fileExtension);

                Assert.True(result.Item1, result.Item2);
            }

            EnsureTabsNotUsed("*.cs");
            EnsureTabsNotUsed("*.vb");
        }

        [Fact]
        public void EnsureCodeDoesNotUseOldTodoCommentIdentifier()
        {
            void EnsureUwpTemplatesNotUsed(string fileExtension)
            {
                var result = CodeIsNotUsed("UWPTemplates", fileExtension);

                Assert.True(result.Item1, result.Item2);
            }

            EnsureUwpTemplatesNotUsed("*.cs");
            EnsureUwpTemplatesNotUsed("*.vb");
        }

        [Fact]
        public void EnsureVisualbasicCodeDoesNotIncludeCommonPortingIssues()
        {
            // Build tests will fail if these are included but this test is quicker than building everything
            void CheckStringNotIncluded(string toSearchFor)
            {
                var result = CodeIsNotUsed(toSearchFor, ".vb");

                Assert.True(result.Item1, result.Item2);
            }

            CheckStringNotIncluded("Namespace Param_RootNamespace."); // Root namespace is included by default in VB
            CheckStringNotIncluded(";");
        }

        private Tuple<bool, string> CodeIsNotUsed(string textThatShouldNotBeinTheFile, string fileExtension)
        {
            // This is the relative path from where the test assembly will run from
            const string templatesRoot = "..\\..\\..\\..\\..\\Templates";

            foreach (var file in GetFiles(templatesRoot, fileExtension))
            {
                if (File.ReadAllText(file).Contains(textThatShouldNotBeinTheFile))
                {
                    // Throw an assertion failure here and stop checking other files.
                    // We don't need to check every file if at least one fails as this should just be a final verification.
                    return new Tuple<bool, string>(false, $"The file '{file}' contains '{textThatShouldNotBeinTheFile}' but based on our style guidelines it shouldn't.");
                }
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private IEnumerable<string> GetFiles(string directory, string extension = ".*")
        {
            foreach (var dir in Directory.GetDirectories(directory))
            {
                foreach (var file in Directory.GetFiles(dir, $"*{extension}"))
                {
                    yield return file;
                }

                foreach (var file in GetFiles(dir, extension))
                {
                    yield return file;
                }
            }
        }
    }
}