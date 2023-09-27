﻿/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO;

namespace Finos.Fdc3.Backplane.Client
{
    /// <summary>
    /// Backplane client initialization parameters
    /// </summary>
    public class InitializeParams
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="desktopAgentIdentifier"></param>
        public InitializeParams(AppIdentifier desktopAgentIdentifier)
        {
            AppIdentifier = desktopAgentIdentifier;
        }

        /// <summary>
        /// App/Desktop agent identifier
        /// </summary>
        public AppIdentifier AppIdentifier { get; }

    }
}
