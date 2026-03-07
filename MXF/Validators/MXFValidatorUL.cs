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

using Myriadbits.MXF.Identifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Myriadbits.MXF.Validators
{
    public class MXFValidatorUL : MXFValidator
    {
        public MXFValidatorUL(MXFFile file) : base(file)
        {
            Name = "SMPTE Registers/UL";
        }

        protected override async Task<List<MXFValidationResult>> OnValidate(IProgress<TaskReport> progress = null, CancellationToken ct = default)
        {
            List<MXFValidationResult> result = await Task.Run(() =>
            {

                var retval = new List<MXFValidationResult>();
                var mxfPacksWithUnknownUL = this.File.Descendants()
                                                .OfType<MXFPack>()
                                                .Where(klv => klv.Key.SMPTEInformation == null
                                                    && klv is not MXFPackageMetaData
                                                    && klv is not MXFSystemMetaDataPack);

                // lookup dictionary for most similar ULs  

                Dictionary<UL, UL> mostSimilarULDictionary = new Dictionary<UL, UL>();
                foreach (var pack in mxfPacksWithUnknownUL)
                {
                    if (!mostSimilarULDictionary.ContainsKey(pack.Key))
                    {
                        var mostSimilarUL = SMPTERegisters.FindMostSimilarUL(pack.Key);
                        mostSimilarULDictionary.Add(pack.Key, mostSimilarUL);
                    }
                }

                foreach (var pack in mxfPacksWithUnknownUL)
                {
                    if (pack.Key.IdentifiesPrivatelyRegisteredUL())
                    {
                        retval.Add(ValidationRules.CreateValidationResult(ValidationRuleIDs.ID_0718, pack, pack.Offset, pack.Key));
                    }
                    else
                    {
                        if (mostSimilarULDictionary.TryGetValue(pack.Key, out UL mostSimilarUL))
                        {
                            retval.Add(ValidationRules.CreateValidationResult(ValidationRuleIDs.ID_0717, pack, pack.Offset, pack.Key, mostSimilarUL));
                        }
                        else
                        {
                            retval.Add(ValidationRules.CreateValidationResult(ValidationRuleIDs.ID_0720, pack, pack.Offset, pack.Key));
                        }
                    }
                }
                return retval;
            }, ct);
            return result;
        }
    }
}
