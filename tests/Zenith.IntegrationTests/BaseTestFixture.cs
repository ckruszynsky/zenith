using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.IntegrationTests
{
    using static TestingFixture;

    [TestFixture]
    public abstract class BaseTestFixture
    {

        [SetUp]
        public async Task TestSetUp()
        {
            await ResetState();
        }
    }
}
