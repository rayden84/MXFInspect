#region license
//
// MXF - Myriadbits .NET MXF library. 
// Read MXF Files.
// Copyright (C) 2015 Myriadbits, Jochem Bakker
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// For more information, contact me at: info@myriadbits.com
//
#endregion

using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Myriadbits.MXF.Validators
{
    public class MXFValidatorUL : MXFValidator
    {
        public MXFValidatorUL(MXFFile file) : base(file)
        {
            Description = "SMPTE Registers/UL";
        }

        protected override async Task<List<MXFValidationResult>> OnValidate(IProgress<TaskReport> progress = null, CancellationToken ct = default)
        {
            List<MXFValidationResult> result = await Task.Run(() =>
            {

                var retval = new List<MXFValidationResult>();
                var klvsWithUnknownUL = this.File.Descendants()
                                                .OfType<MXFPack>()
                                                .Where(klv => klv.Key.SMPTEInformation == null
                                                    && klv is not MXFPackageMetaData
                                                    && klv is not MXFSystemMetaDataPack);

                foreach (var klv in klvsWithUnknownUL)
                {
                    if (klv.Key.IdentifiesPrivatelyRegisteredUL())
                    {
                        retval.Add(ValidationRules.CreateValidationResult(ValidationRuleIDs.ID_0718, klv, klv.Offset, klv.Key));
                    }
                    else
                    {
                        retval.Add(ValidationRules.CreateValidationResult(ValidationRuleIDs.ID_0717, klv, klv.Offset, klv.Key));
                    }
                }
                return retval;
            }, ct);
            return result;
        }
    }
}
