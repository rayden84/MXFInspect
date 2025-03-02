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

using Myriadbits.MXF.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Myriadbits.MXF.Validators
{
    public class MXFValidatorFile : MXFValidator
    {

        public MXFValidatorFile(MXFFile file) : base(file)
        {
            Description = "General/File";
        }

        protected override async Task<List<MXFValidationResult>> OnValidate(IProgress<TaskReport> progress = null, CancellationToken ct = default)
        {
            List<MXFValidationResult> resultList = await Task.Run(() =>
            {
                var retval = new List<MXFValidationResult>();

                // add exceptions as validation results
                foreach (var ex in File.Exceptions)
                {
                    ct.ThrowIfCancellationRequested();

                    switch (ex)
                    {
                        case EndOfKLVStreamException eofEx:
                            retval.Add(new MXFValidationResult(
                                MXFValidationCategory.KLVStream,
                                MXFValidationSeverity.Error,
                                eofEx.TruncatedObject,
                                eofEx.Offset,
                                eofEx.Message
                            ));
                            break;

                        case UnparseablePackException upEx:
                            retval.Add(new MXFValidationResult(
                                MXFValidationCategory.KLVStream,
                                MXFValidationSeverity.Error,
                                upEx.UnparseablePack,
                                upEx.Offset,
                                upEx.Message
                            ));
                            break;

                        case KLVParsingException pEx:
                            retval.Add(new MXFValidationResult(
                                MXFValidationCategory.KLVStream,
                                MXFValidationSeverity.Error,
                                File.Descendants().Where(o => o.Offset == pEx.Offset).FirstOrDefault(),
                                pEx.Offset,
                                pEx.InnerException?.Message ?? ex.Message
                            ));
                            break;

                        default:
                            retval.Add(new MXFValidationResult(
                                MXFValidationCategory.General,
                                MXFValidationSeverity.Error,
                                null,
                                null,
                                ex.InnerException?.Message ?? ex.Message
                            ));
                            break;
                    }
                }
                return retval;
            }, ct);

            return resultList;
        }
    }
}
