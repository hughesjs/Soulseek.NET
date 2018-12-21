﻿// <copyright file="ConnectionManager{T}Tests.cs" company="JP Dillingham">
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

namespace Soulseek.NET.Tests.Unit.Tcp
{
    using Moq;
    using Soulseek.NET.Tcp;
    using System.Collections.Concurrent;
    using System.Net;
    using Xunit;

    public class ConnectionManager_T_Tests
    {
        [Trait("Category", "Instantiation")]
        [Fact(DisplayName = "Instantiates properly")]
        public void Instantiates_Properly()
        {
            ConnectionManager<IConnection> c = null;

            var ex = Record.Exception(() => c = new ConnectionManager<IConnection>(1000));

            Assert.Null(ex);
            Assert.NotNull(c);

            Assert.Equal(1000, c.ConcurrentConnections);
            Assert.Equal(0, c.Active);
            Assert.Equal(0, c.Queued);
        }

        [Trait("Category", "Dispose")]
        [Fact(DisplayName = "Disposes without throwing")]
        public void Disposes_Without_Throwing()
        {
            var c = new ConnectionManager<IConnection>();

            var ex = Record.Exception(() => c.Dispose());

            Assert.Null(ex);
        }

        [Trait("Category", "Remove")]
        [Fact(DisplayName = "Removes does not throw on untracked connection")]
        public async void Removes_Does_Not_Throw_On_Untracked_Connection()
        {
            var mock = new Mock<IConnection>();
            mock.Setup(m => m.Key).Returns(new ConnectionKey(new System.Net.IPAddress(0x0), 1));

            var c = new ConnectionManager<IConnection>();

            var ex = await Record.ExceptionAsync(async () => await c.RemoveAsync(mock.Object));

            Assert.Null(ex);
        }

        [Trait("Category", "Remove")]
        [Fact(DisplayName = "Removes does not throw on null connection")]
        public async void Removes_Does_Not_Throw_On_Null_Connection()
        {
            var c = new ConnectionManager<IConnection>();

            var ex = await Record.ExceptionAsync(async () => await c.RemoveAsync(null));

            Assert.Null(ex);
        }

        [Trait("Category", "Remove")]
        [Fact(DisplayName = "Removes does not throw on null connection key")]
        public async void Removes_Does_Not_Throw_On_Null_Connection_Key()
        {
            var mock = new Mock<IConnection>();

            var c = new ConnectionManager<IConnection>();

            var ex = await Record.ExceptionAsync(async () => await c.RemoveAsync(mock.Object));

            Assert.Null(ex);
        }

        [Trait("Category", "Remove")]
        [Fact(DisplayName = "Removes does not dispose untracked connection")]
        public async void Removes_Does_Not_Dispose_Untracked_Connection()
        {
            var mock = new Mock<IConnection>();
            mock.Setup(m => m.Key).Returns(new ConnectionKey(new IPAddress(0x0), 1));

            var c = new ConnectionManager<IConnection>();

            await c.RemoveAsync(mock.Object);

            mock.Verify(m => m.Dispose(), Times.Never);
        }

        [Trait("Category", "Remove")]
        [Fact(DisplayName = "Removes removes given connection")]
        public async void Removes_Removes_Given_Connection()
        {
            var key = new ConnectionKey(new IPAddress(0x0), 1);

            var mock = new Mock<IConnection>();
            mock.Setup(m => m.Key).Returns(key);

            var c = new ConnectionManager<IConnection>();
            await c.AddAsync(mock.Object);

            var active = c.GetProperty<ConcurrentDictionary<ConnectionKey, IConnection>>("Connections");

            Assert.True(active.TryGetValue(mock.Object.Key, out var _), "Connection was added");

            await c.RemoveAsync(mock.Object);

            Assert.Empty(active);
            Assert.False(active.TryGetValue(mock.Object.Key, out var _), "Connection was removed");
            mock.Verify(m => m.Dispose(), Times.Once);
        }

        [Trait("Category", "Remove")]
        [Fact(DisplayName = "Removes removes given connection, then activates queued")]
        public async void Removes_Removes_Given_Connection_Then_Activates_Queued()
        {
            var key1 = new ConnectionKey(new IPAddress(0x1), 1);
            var mock1 = new Mock<IConnection>();
            mock1.Setup(m => m.Key).Returns(key1);

            var key2 = new ConnectionKey(new IPAddress(0x2), 2);
            var mock2 = new Mock<IConnection>();
            mock2.Setup(m => m.Key).Returns(key2);

            var c = new ConnectionManager<IConnection>(1);
            await c.AddAsync(mock1.Object);
            await c.AddAsync(mock2.Object);

            var active = c.GetProperty<ConcurrentDictionary<ConnectionKey, IConnection>>("Connections");
            var queued = c.GetProperty<ConcurrentQueue<IConnection>>("ConnectionQueue");

            // ensure connection 1 was added and immediately activated
            Assert.Single(active);
            Assert.True(active.TryGetValue(mock1.Object.Key, out var _), "Connection 1 was added");

            // ensure connection 2 was added and queued
            Assert.Single(queued);
            var peek = queued.TryPeek(out var peeked);
            Assert.True(peeked.Key == key2, "Connection 2 was queued");

            await c.RemoveAsync(mock1.Object);

            Assert.Empty(queued);
            Assert.Single(active);

            Assert.False(active.TryGetValue(mock1.Object.Key, out var _), "Connection 1 was removed");
            Assert.True(active.TryGetValue(mock2.Object.Key, out var _), "Connection 2 was activated");

            // ensure mock 1 was disposed when removed
            mock1.Verify(m => m.Dispose(), Times.Once);

            // ensure mock 2 was connected when activated
            mock2.Verify(m => m.ConnectAsync(), Times.Once);
        }
    }
}
