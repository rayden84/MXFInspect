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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Serilog;

namespace Myriadbits.MXF.Validators
{
    public class MXFFileValidationEngine
    {
        public static async Task<MXFValidationReport> Validate(IList<MXFValidator> validators, IProgress<TaskReport> progress = null, CancellationToken ct = default)
        {
            List<MXFValidationResult> resultsList = new List<MXFValidationResult>();
            List<Exception> exceptions = new List<Exception>();

            // execute validators
            var sw = Stopwatch.StartNew();
            foreach (MXFValidator validator in validators)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    int currentPercentage = (int)(validators.IndexOf(validator) + 1 * 100.0 / validators.Count);
                    resultsList.AddRange(await validator.Validate(progress, ct));
                    progress.Report(new TaskReport(currentPercentage, $"Validating {validator.Name}"));
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    resultsList.Add(new MXFValidationResult(MXFValidationCategory.General, MXFValidationSeverity.Error, null, null, $"Validator {validator.Name} threw an exception: {ex.Message}"));
                }
            }
            sw.Stop();
            Log.ForContext<MXFValidatorInfo>().Information($"Validation completed in {sw.ElapsedMilliseconds} ms");
            return new MXFValidationReport(resultsList,exceptions, sw.ElapsedMilliseconds);
        }
    }
}
