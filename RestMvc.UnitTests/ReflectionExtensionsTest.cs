using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using RestMvc.Attributes;

namespace RestMvc.UnitTests
{
    [TestFixture]
    public class ReflectionExtensionsTest
    {
        public class EmptyController {}

        public void Unannotated() { }

        [Get("test")]
        public void Index() { }

        [Post("Test")]
        public void Create() { }

        [Get("test/{id}")]
        public void Show() { }

        [Test]
        public void ControllerNameShouldBeTypeNameIfNoSuffix()
        {
            Assert.That(GetType().GetControllerName(), Is.EqualTo("ReflectionExtensionsTest"));
        }

        [Test]
        public void ControllerNameShouldStripOutSuffix()
        {
            Assert.That(typeof(EmptyController).GetControllerName(), Is.EqualTo("Empty"));
        }

        [Test]
        public void GetAttributeReturnsNullIfMissingAttribute()
        {
            var method = GetType().GetMethod("Unannotated");
            Assert.That(method.GetResourceActionAttribute(), Is.Null);
        }

        [Test]
        public void GetAttributeReturnsCorrectAttribute()
        {
            var method = GetType().GetMethod("Show");
            Assert.That(method.GetResourceActionAttribute(), Is.EqualTo(new GetAttribute("test/{id}")));
        }

        [Test]
        public void GetResourceActionAttributesReturnsAllAnnotatedMethods()
        {
            var actions = GetType().GetResourceActions().Select(action => action.Name).ToArray();
            Assert.That(actions, Is.EqualTo(new[] {"Index", "Create", "Show"}));
        }

        [Test]
        public void TypeWithNoAnnotationsShouldHaveNoResourceUris()
        {
            Assert.That(typeof(EmptyController).GetResourceUris(), Is.EqualTo(new string[0]));
        }

        [Test]
        public void ShouldIgnoreCaseWhenSelectingResourceUris()
        {
            Assert.That(GetType().GetResourceUris(), Is.EqualTo(new[] { "test", "test/{id}" }));
        }

        [Test]
        public void ShouldSupportNoMethodsForMissingResourceUri()
        {
            Assert.That(GetType().GetSupportedMethods(""), Is.EqualTo(new string[0]));
        }

        [Test]
        public void AllMethodsShouldBeUnsupportedForMissingResourceUri()
        {
            Assert.That(GetType().GetUnsupportedMethods(""),
                Is.EqualTo(new[] {"GET", "POST", "PUT", "DELETE"}));
        }

        [Test]
        public void ShouldDetectSetOfMethodsForResourceUri()
        {
            Assert.That(GetType().GetSupportedMethods("Test"), Is.EqualTo(new[] {"GET", "POST"}));
        }

        [Test]
        public void SupportedMethodsShouldBeCaseInsensitive()
        {
            Assert.That(GetType().GetSupportedMethods("test"), Is.EqualTo(new[] {"GET", "POST"}));
        }

        [Test]
        public void SupportedMethodsForSecondResourceUriOnType()
        {
            Assert.That(GetType().GetSupportedMethods("test/{id}"), Is.EqualTo(new[] {"GET"}));
        }
    }
}
