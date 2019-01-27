﻿// <copyright file="DiagnosticMessageLevel.cs" company="JP Dillingham">
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

namespace Soulseek
{
    /// <summary>
    ///     Diagnostic message levels.
    /// </summary>
    public enum DiagnosticMessageLevel
    {
        /// <summary>
        ///     None.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Warning.
        /// </summary>
        Warning = 1,

        /// <summary>
        ///     Info.
        /// </summary>
        Info = 2,

        /// <summary>
        ///     Debug.
        /// </summary>
        Debug = 3,
    }
}
