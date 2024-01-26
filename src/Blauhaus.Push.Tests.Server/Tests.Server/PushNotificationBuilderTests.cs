using System;
using System.Collections.Generic;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Server
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
            Assert.That(result.DataProperties["PropertyOne"], Is.EqualTo("ValueOne"));
            Assert.That(result.DataProperties["PropertyTwo"], Is.EqualTo("ValueTwo"));
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
            Assert.That(result.Title, Is.EqualTo("Title"));
            Assert.That(result.Body, Is.EqualTo("Body"));
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
            Assert.That(result1.Title, Is.EqualTo("DefaultTitle"));
            Assert.That(result1.Body, Is.EqualTo("Body"));
            Assert.That(result2.Title, Is.EqualTo("Title"));
            Assert.That(result2.Body, Is.EqualTo("DefaultBody"));
        }
        
        [Test]
        public void Create_SHOULD_set_Title_and_Body_to_default_if_not_provided()
        {
            //Arrange
            var sut = new PushNotificationBuilder(_visibleTemplate);

            //Act
            var result = sut.Create();

            //Assert
            Assert.That(result.Title, Is.EqualTo("DefaultTitle"));
            Assert.That(result.Body, Is.EqualTo("DefaultBody"));
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