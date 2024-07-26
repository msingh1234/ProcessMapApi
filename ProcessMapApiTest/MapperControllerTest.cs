using Mapper;
using Moq;
using ProcessMapApi.Controllers;

namespace ProcessMapApiTest
{
    public class MapperControllerTest
    {
        private readonly Mock<IProcessor> _MockProcessor = null;
        public MapperControllerTest()
        {
            _MockProcessor = new Mock<IProcessor>();
        }

        [Fact]
        public void ShouldThrowExceptionIfParamIsNull()
        {
            // Arrange
            var expectedParamName = "processor";
            bool exceptionThrown = false;

            // Act
            try
            {
                new MapperController(null);
            }

            // Assert
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(expectedParamName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void ShouldReturnResponseIfAllValidParams()
        {
            // Arrange
            string reurnValue = "Sky is the limit";
            _MockProcessor.Setup(ap => ap.GenrateResponse(It.IsAny<string>())).Returns(reurnValue);
            var controller = new MapperController(_MockProcessor.Object);
            // Act
            var result = controller.Get(It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reurnValue, result);
        }
    }
}
