﻿// <copyright file="BrowseAsyncTests.cs" company="JP Dillingham">
//     Copyright (c) JP Dillingham. All rights reserved.
//
//     This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as
//     published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
//     of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License along with this program. If not, see https://www.gnu.org/licenses/.
// </copyright>

namespace Soulseek.NET.Tests.Unit.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using Moq;
    using Soulseek.NET.Messaging.Messages;
    using Soulseek.NET.Messaging.Tcp;
    using Xunit;

    public class BrowseAsyncTests
    {
        [Trait("Category", "BrowseAsync")]
        [Theory(DisplayName = "BrowseAsync throws ArgumentException given bad username")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task BrowseAsync_Throws_ArgumentException_Given_Bad_Username(string username)
        {
            var s = new SoulseekClient();

            var ex = await Record.ExceptionAsync(async () => await s.BrowseAsync(username));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
        }

        [Trait("Category", "BrowseAsync")]
        [Fact(DisplayName = "BrowseAsync throws InvalidOperationException when not connected")]
        public async Task BrowseAsync_Throws_InvalidOperationException_When_Not_Connected()
        {
            var s = new SoulseekClient();

            var ex = await Record.ExceptionAsync(async () => await s.BrowseAsync("foo"));

            Assert.NotNull(ex);
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Contains("Connected", ex.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Trait("Category", "BrowseAsync")]
        [Fact(DisplayName = "BrowseAsync throws InvalidOperationException when not logged in")]
        public async Task BrowseAsync_Throws_InvalidOperationException_When_Not_Logged_In()
        {
            var s = new SoulseekClient();
            s.SetProperty("State", SoulseekClientStates.Connected);

            var ex = await Record.ExceptionAsync(async () => await s.BrowseAsync("foo"));

            Assert.NotNull(ex);
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Contains("logged in", ex.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Trait("Category", "BrowseAsync")]
        [Theory(DisplayName = "BrowseAsync returns expected response on success"), AutoData]
        public async Task BrowseAsync_Returns_Expected_Response_On_Success(List<Directory> directories)
        {
            var response = new BrowseResponse(directories.Count, directories);

            var waiter = new Mock<IWaiter>();
            waiter.Setup(m => m.WaitIndefinitely<BrowseResponse>(It.IsAny<WaitKey>(), null))
                .Returns(Task.FromResult(response));

            var conn = new Mock<IMessageConnection>();

            var s = new SoulseekClient("127.0.0.1", 1, messageWaiter: waiter.Object);
            s.SetProperty("State", SoulseekClientStates.Connected | SoulseekClientStates.LoggedIn);

            var result = await s.InvokeMethod<Task<BrowseResponse>>("BrowseInternalAsync", "foo", null, conn.Object);

            Assert.Equal(response, result);
        }
    }
}
