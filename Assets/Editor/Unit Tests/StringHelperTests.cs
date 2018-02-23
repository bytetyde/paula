using System.Collections.Generic;
using Assets.Scripts.Common;
using NUnit.Framework;

namespace Assets.Editor.Unit_Tests
{
    public class StringHelperTests  {

        [Test]
        public void GetMatchingStringPartsInString_RegexCheck_StringListWithMatches()
        {
            string someString = "<test1><test2><test3><test4><test5><test23956@#4435hlsfdh@#^&$>";

            List<string> matchesInSomeString = StringHelper.GetMatchingStringPartsInString(someString, @"<([^<>].*?)>");

            Assert.That(matchesInSomeString.Count == 6  && matchesInSomeString[5] == "<test23956@#4435hlsfdh@#^&$>");
        }

        [Test]
        public void GetListEntriesAsString_NoApendixAndDelimiters_ConcatenatedString()
        {
            List<string> entries = new List<string>() {"test1", "test2" , "test3" , "test4" , "test5" , "test6" };

            string entriesAsString = StringHelper.GetListEntriesAsString(entries, "", "" ,"");

            Assert.That(entriesAsString.Length == 30);
        }

        [Test]
        public void GetListEntriesAsString_NoApendixAndSpaceDelimiter_ConcatenatedString()
        {
            List<string> entries = new List<string>() { "test1", "test2", "test3", "test4", "test5", "test6" };

            string entriesAsString = StringHelper.GetListEntriesAsString(entries, "", "", " ");

            Assert.That(entriesAsString.Length == 35 && entriesAsString == "test1 test2 test3 test4 test5 test6");
        }

        [Test]
        public void GetListEntriesAsString_WithStartAndEndApendixAndNoDelimiter_ConcatenatedString()
        {
            List<string> entries = new List<string>() { "test1", "test2", "test3", "test4", "test5", "test6" };

            string entriesAsString = StringHelper.GetListEntriesAsString(entries, ">", "<", "");

            Assert.That(entriesAsString.Length == 42);
        }

        [Test]
        public void ExceptCharsFromString_CharsCapitalXAndWhiteSpace_CleanedString()
        {
            string stringToTest = "XtestStringtestXStr ingtestStringXtest String";

            string modifiedString = StringHelper.ExceptCharsFromString(stringToTest, new char[] {'X', ' '});

            Assert.That(modifiedString == "testStringtestStringtestStringtestString");
        }

        [Test]
        public void SeparateStringIntoPartsBySingleDelimiter_DelimiterComma_ListWithDelimitedParts()
        {
            string stringToTest = "string1,string2,string3,string4,string5";

            List<string> stringInParts = StringHelper.SeparateStringIntoPartsBySingleDelimiter(stringToTest, ',');

            Assert.That(stringInParts.Count == 5);
        }

        [Test]
        public void ExtractIntValueFromString_Number13InString_NumberContainedInString()
        {
            string stringToTest = "string1to3test";

            int numberInString = StringHelper.ExtractIntValueFromString(stringToTest);

            Assert.That(numberInString == 13);
        }

        [Test]
        public void StringToLowerCase_Lower_AllCharsLowerCase()
        {
            string someString = "TeStsTrInG";

            string someModifiedString = StringHelper.StringToLowerCase(someString);

            Assert.That(someModifiedString == "teststring");
        }

        [Test]
        public void SeparateStringIntoPartsByDelimiterGroup_MultipleDelimiterChars_StringList()
        {
            string someString = "test1\\|test2\\|test3\\|test4";

            List<string> someStringParts = StringHelper.SeparateStringIntoPartsByDelimiterGroup(someString, new string[] {"\\|"});

            Assert.That(someStringParts.Count == 4 && someStringParts[0] == "test1");
        }

        [Test]
        public void HasTokenAttribute_OneAttribute_BoolValue()
        {
            string someToken = "<spawn-position stackable='yes'>";
            string anotherToken = "<spawn-position stackable='no'>";

            var someTokenHasAttribute = StringHelper.HasTokenAttribute(someToken, "stackable");
            var anotherTokenHasAttribute = StringHelper.HasTokenAttribute(anotherToken, "stackable");

            Assert.That(someTokenHasAttribute && !anotherTokenHasAttribute);
        }

        [Test]
        public void HasTokenAttributes_TwoAttributes_BoolValue()
        {
            string someToken = "<spawn-position stackable='yes' anotherTestAttribute='yes'>";
        
            var someTokenHasAttribute1 = StringHelper.HasTokenAttribute(someToken, "stackable");
            var someTokenHasAttribute2 = StringHelper.HasTokenAttribute(someToken, "anotherTestAttribute");

            Assert.That(someTokenHasAttribute1 && someTokenHasAttribute2);
        }

        [Test]
        public void TokenNameWithoutAttributes_OneAttribute_Tokenname()
        {
            string someToken = "<spawn-position stackable='yes'>";

            var someTokenHasAttribute = StringHelper.GetTokenNameWithoutAttributes(someToken);
        
            Assert.That(someTokenHasAttribute == "<spawn-position>");
        }

        [Test]
        public void TokenNameWithoutAttributes_ThreeAttributes_Tokenname()
        {
            string someToken = "<spawn-position stackable='yes' anotherTestAttribute='yes' lastAttribute='yes'>";

            var someTokenHasAttribute = StringHelper.GetTokenNameWithoutAttributes(someToken);

            Assert.That(someTokenHasAttribute == "<spawn-position>");
        }

        [Test]
        public void TokenAttributes_OneAttribute_TokenAttribute()
        {
            string someToken = "<spawn-position stackable='yes'>";
            var attribute = new KeyValueString("stackable", "yes");

            var tokenAttributes = StringHelper.GetTokenAttributes(someToken);
        
            Assert.That(tokenAttributes.Count == 1 && tokenAttributes[0].key == attribute.key && tokenAttributes[0].value == attribute.value);
        }

        [Test]
        public void TokenAttributes_TwoAttribute_TokenAttributes()
        {
            string someToken = "<spawn-position stackable='yes' anotherAttribute='no'>";
            var attribute1 = new KeyValueString("stackable", "yes");
            var attribute2 = new KeyValueString("anotherAttribute", "no");

            var tokenAttributes = StringHelper.GetTokenAttributes(someToken);

            Assert.That(tokenAttributes.Count == 2 && tokenAttributes[0].key == attribute1.key && tokenAttributes[0].value == attribute1.value
                        && tokenAttributes[1].key == attribute2.key && tokenAttributes[1].value == attribute2.value);
        }

        [Test]
        public void ChildTokenInToken_FirstDepth_TokenName()
        {
            string someToken = "{3*[20%<testtoken>]}";
        
            var childToken = StringHelper.GetChildTokenInToken(someToken);

            Assert.That(childToken == "[20%<testtoken>]");
        }

        [Test]
        public void ChildTokenInToken_SecondDepth_TokenName()
        {
            string someToken = "{3*[20%<testtoken>]}";

            var childToken = StringHelper.GetFirstTokenInString(someToken, 2);

            Assert.That(childToken == "<testtoken>");
        }
    }
}
