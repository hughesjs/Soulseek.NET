﻿// <copyright file="RoomTicker.cs" company="JP Dillingham">
//     Copyright (c) JP Dillingham. All rights reserved.
//
//     This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
//     as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
//     of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License along with this program. If not, see https://www.gnu.org/licenses/.
// </copyright>

namespace Soulseek
{
    /// <summary>
    ///     A chat room ticker.
    /// </summary>
    public class RoomTicker
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoomTicker"/> class.
        /// </summary>
        /// <param name="name">The room name.</param>
        /// <param name="message">The ticker content.</param>
        public RoomTicker(string name, string message)
        {
            Name = name;
            Message = message;
        }

        /// <summary>
        ///     Gets the room name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the ticker content.
        /// </summary>
        public string Message { get; }
    }
}