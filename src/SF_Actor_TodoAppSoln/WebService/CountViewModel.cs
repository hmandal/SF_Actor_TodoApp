// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using ActorBackendService.Interfaces;
using System.Collections.Generic;

namespace WebService
{
    public class DeviceViewModel
    {
        public IGetDeviceInfo DeviceInfo { get; set; }
    }
    public class DevicesViewModel
    {
        public IEnumerable<IGetDeviceInfo> DevicesInfo { get; set; }
    }
}
