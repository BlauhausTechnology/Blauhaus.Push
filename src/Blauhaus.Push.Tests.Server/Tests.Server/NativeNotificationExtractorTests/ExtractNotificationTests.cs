using System;
using System.Collections.Generic;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Server.Extractors;
using Blauhaus.Push.TestHelpers.MockBuilders;
using Blauhaus.Push.Tests.Tests.Server._Base;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Server.NativeNotificationExtractorTests
{
    [TestFixture]
    public class ExtractNotificationTests : BasePushNotificationsServerTest<NativeNotificationExtractor>
    {
        private IPushNotification _pushNotification;
        private readonly Guid _id = Guid.Parse("d76594fd-1cae-4f4f-9dfe-545102d20357");

        public override void SetUp()
        {
            base.SetUp();

            _pushNotification = new PushNotificationMockBuilder()
                .With(x => x.Name, "AllMinionsAlert")
                .With(x => x.Title, "Hear Ye!")
                .With(x => x.Body, "The king is dead. Long live the king")
                .With(x => x.DataProperties, new Dictionary<string, object>
                {
                    {"Id", _id },
                    {"Details", "He was stabbed in the bath" },
                    {"Number of stabs", 12 }
                })
                .Object;
        }

        [Test]
        public void SHOULD_extract_UWP_notification()
        {
            //Arrange
            const string expectedPayload =
                "<toast launch=\"{ " +
                "'Title' : 'Hear Ye!', " +
                "'Body' : 'The king is dead. Long live the king', " +
                "'Template_Name' : 'AllMinionsAlert', " +
                "'Id' : 'd76594fd-1cae-4f4f-9dfe-545102d20357', " +
                "'Details' : 'He was stabbed in the bath', " +
                "'Number of stabs' : '12' " +
                "}\">" +
                "<visual><binding template=\"ToastText01\">" +
                "<text id=\"1\">Hear Ye!</text>" +
                "<text id=\"2\">The king is dead. Long live the king</text></binding></visual></toast>";

            //Act
            var result = Sut.ExtractNotification(RuntimePlatform.UWP, _pushNotification);

            //Assert
            Assert.That(result.Value.Notification.Body, Is.EqualTo(expectedPayload));
        }

        [Test]
        public void SHOULD_extract_Ios_notification()
        {
            //Arrange
            const string expectedPayload =
                "{ \"aps\" : { \"alert\" : { \"title\" : \"Hear Ye!\", \"body\" : \"The king is dead. Long live the king\" }}, " +
                "\"Template_Name\" : \"AllMinionsAlert\", " +
                "\"Id\" : \"d76594fd-1cae-4f4f-9dfe-545102d20357\", " +
                "\"Details\" : \"He was stabbed in the bath\", " +
                "\"Number of stabs\" : \"12\" }";

            //Act
            var result = Sut.ExtractNotification(RuntimePlatform.iOS, _pushNotification);

            //Assert
            Assert.That(result.Value.Notification.Body, Is.EqualTo(expectedPayload));
        }

        [Test]
        public void SHOULD_extract_Android_notification()
        {
            //Arrange
            const string expectedPayload =
                "{ " +
                "\"notification\" : { \"title\" : \"Hear Ye!\", \"body\" : \"The king is dead. Long live the king\" }, " +
                "\"data\" : { " +
                "\"Template_Name\" : \"AllMinionsAlert\", " +
                "\"Title\" : \"Hear Ye!\", " +
                "\"Body\" : \"The king is dead. Long live the king\", " +
                "\"Id\" : \"d76594fd-1cae-4f4f-9dfe-545102d20357\", " +
                "\"Details\" : \"He was stabbed in the bath\", " +
                "\"Number of stabs\" : \"12\"" +
                " }" +
                " }";

            //Act
            var result = Sut.ExtractNotification(RuntimePlatform.Android, _pushNotification);

            //Assert
            Assert.That(result.Value.Notification.Body, Is.EqualTo(expectedPayload));
        }
    }
}