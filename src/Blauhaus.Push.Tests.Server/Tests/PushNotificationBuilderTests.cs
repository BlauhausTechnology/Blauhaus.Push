using System;
using System.Collections.Generic;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Server.Tests
{
    public class PushNotificationBuilderTests
    {

        private PushNotificationTemplate _visibleTemplate;

        [SetUp]
        public void Setup()
        {
            _visibleTemplate = new PushNotificationTemplate("Visible Template", "DefaultTitle", "DefaultBody", new List<string>
            {
                "PropertyOne",
                "PropertyTwo"
            });
            
        }

        [Test]
        public void With_DataProperty_SHOULD_populate_properties_for_data_template()
        {
            //Arrange
            var sut = new PushNotificationBuilder(_visibleTemplate);

            //Act
            var result = sut
                .WithDataProperty("PropertyOne", "ValueOne")
                .WithDataProperty("PropertyTwo", "ValueTwo")
                .Create();

            //Assert
            Assert.AreEqual("ValueOne", result.DataProperties["PropertyOne"]);
            Assert.AreEqual("ValueTwo", result.DataProperties["PropertyTwo"]);
        }
        
        [Test]
        public void WithContent_SHOULD_set_Title_and_Body_for_visible_template()
        {
            //Arrange
            var sut = new PushNotificationBuilder(_visibleTemplate);

            //Act
            var result = sut
                .WithContent("Title", "Body").Create();

            //Assert
            Assert.AreEqual("Title", result.Title);
            Assert.AreEqual("Body", result.Body);
        }

        [Test]
        public void WithContent_SHOULD_set_Title_and_Body_to_default_if_not_provided()
        {
            //Arrange
            var sut = new PushNotificationBuilder(_visibleTemplate);

            //Act
            var result1 = sut.WithContent("", "Body").Create();
            var result2 = sut.WithContent("Title", "").Create();

            //Assert
            Assert.AreEqual("DefaultTitle", result1.Title);
            Assert.AreEqual("Body", result1.Body);
            Assert.AreEqual("Title", result2.Title);
            Assert.AreEqual("DefaultBody", result2.Body);
        }
        
        [Test]
        public void Create_SHOULD_set_Title_and_Body_to_default_if_not_provided()
        {
            //Arrange
            var sut = new PushNotificationBuilder(_visibleTemplate);

            //Act
            var result = sut.Create();

            //Assert
            Assert.AreEqual("DefaultTitle", result.Title);
            Assert.AreEqual("DefaultBody", result.Body);
        }
        
        [Test]
        public void With_DataProperty_SHOULD_throw_exception_if_property_does_not_exist_on_template()
        {
            //Arrange
            var sut1 = new PushNotificationBuilder(_visibleTemplate);

            //Act
            Assert.Throws<ArgumentException>(() => sut1.WithDataProperty("PropertyThree", "ValueThree").Create());

        }
    }
}