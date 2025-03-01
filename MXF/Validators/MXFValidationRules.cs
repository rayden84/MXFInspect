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

using Myriadbits.MXF.Properties;
using System;
using System.Collections.Generic;

namespace Myriadbits.MXF.Validators
{
    public enum ValidationRuleIDs
    {
        ID_0000, // reserved, used for throwing exception when rule is not found
        ID_0051,
        ID_0052,
        ID_0053,
        ID_0054,
        ID_0055,
        ID_0056,
        ID_0057,
        ID_0058,
        ID_0060,
        ID_0062,
        ID_0063,
        ID_0064,
        ID_0065,
        ID_0066,
        ID_0067,
        ID_0068,
        ID_0069,
        ID_0070,
        ID_0071,
        ID_0072,
        ID_0073,
        ID_0074,
        ID_0075,
        ID_0084,
        ID_0085,
        ID_0090,
        ID_0091,
        ID_0092,
        ID_0093,
        ID_0094,
        ID_0095,
        ID_0096,
        ID_0097,
        ID_0098,
        ID_0099,
        ID_0101,
        ID_0102,
        ID_0103,
        ID_0106,
        ID_0107,
        ID_0108,
        ID_0110,
        ID_0111,
        ID_0114,
        ID_0707,
        ID_0708,
        ID_0710,
        ID_0711,
        ID_0712,
        ID_0713,
        ID_0714,
        ID_0715,
        ID_0716,
        ID_0717,
        ID_0718,
    }


    public static class ValidationRules
    {
        private static IReadOnlyDictionary<ValidationRuleIDs, (MXFValidationCategory category, MXFValidationSeverity severity, string templateMessage)> ValidationRulesDictionary { get; set; } = new
        Dictionary<ValidationRuleIDs, (MXFValidationCategory category, MXFValidationSeverity severity, string templateMessage)>()
        {
            {ValidationRuleIDs.ID_0051, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Success, ValidationMessages.ID_0051)},
            {ValidationRuleIDs.ID_0052, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Success, ValidationMessages.ID_0052)},
            {ValidationRuleIDs.ID_0053, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Success, ValidationMessages.ID_0053)},
            {ValidationRuleIDs.ID_0054, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Success, ValidationMessages.ID_0054)},
            {ValidationRuleIDs.ID_0055, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0055)},
            {ValidationRuleIDs.ID_0056, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0056)},
            {ValidationRuleIDs.ID_0057, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0057)},
            {ValidationRuleIDs.ID_0058, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0058)},
            {ValidationRuleIDs.ID_0060, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0060)},
            {ValidationRuleIDs.ID_0062, (MXFValidationCategory.RIP, MXFValidationSeverity.Error, ValidationMessages.ID_0062)},
            {ValidationRuleIDs.ID_0063, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0063)},
            {ValidationRuleIDs.ID_0064, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Success, ValidationMessages.ID_0064)},
            {ValidationRuleIDs.ID_0065, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0065)},
            {ValidationRuleIDs.ID_0066, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Error, ValidationMessages.ID_0066)},
            {ValidationRuleIDs.ID_0067, (MXFValidationCategory.RIP, MXFValidationSeverity.Warning, ValidationMessages.ID_0067)},
            {ValidationRuleIDs.ID_0068, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0068)},
            {ValidationRuleIDs.ID_0069, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0069)},
            {ValidationRuleIDs.ID_0070, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Error, ValidationMessages.ID_0070)},
            {ValidationRuleIDs.ID_0071, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0071)},
            {ValidationRuleIDs.ID_0072, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0072)},
            {ValidationRuleIDs.ID_0073, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Warning, ValidationMessages.ID_0073)},
            {ValidationRuleIDs.ID_0074, (MXFValidationCategory.RIP, MXFValidationSeverity.Success, ValidationMessages.ID_0074)},
            {ValidationRuleIDs.ID_0075, (MXFValidationCategory.RIP, MXFValidationSeverity.Success, ValidationMessages.ID_0075)},
            {ValidationRuleIDs.ID_0084, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0084)},
            {ValidationRuleIDs.ID_0085, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0085)},
            {ValidationRuleIDs.ID_0090, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0090)},
            {ValidationRuleIDs.ID_0091, (MXFValidationCategory.Metadata, MXFValidationSeverity.Warning, ValidationMessages.ID_0091)},
            {ValidationRuleIDs.ID_0092, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Success, ValidationMessages.ID_0092)},
            {ValidationRuleIDs.ID_0093, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0093)},
            {ValidationRuleIDs.ID_0094, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0094)},
            {ValidationRuleIDs.ID_0095, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0095)},
            {ValidationRuleIDs.ID_0096, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0096)},
            {ValidationRuleIDs.ID_0097, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0097)},
            {ValidationRuleIDs.ID_0098, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0098)},
            {ValidationRuleIDs.ID_0099, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0099)},
            {ValidationRuleIDs.ID_0101, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0101)},
            {ValidationRuleIDs.ID_0102, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0102)},
            {ValidationRuleIDs.ID_0103, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0103)},
            {ValidationRuleIDs.ID_0106, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Warning, ValidationMessages.ID_0106)},
            {ValidationRuleIDs.ID_0107, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Warning, ValidationMessages.ID_0107)},
            {ValidationRuleIDs.ID_0108, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Warning, ValidationMessages.ID_0108)},
            {ValidationRuleIDs.ID_0110, (MXFValidationCategory.Metadata, MXFValidationSeverity.Warning, ValidationMessages.ID_0110)},
            {ValidationRuleIDs.ID_0111, (MXFValidationCategory.Metadata, MXFValidationSeverity.Warning, ValidationMessages.ID_0111)},
            {ValidationRuleIDs.ID_0114, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0114)},
            {ValidationRuleIDs.ID_0707, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0707)},
            {ValidationRuleIDs.ID_0708, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Success, ValidationMessages.ID_0708)},
            {ValidationRuleIDs.ID_0710, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0710)},
            {ValidationRuleIDs.ID_0711, (MXFValidationCategory.PartitionStructure, MXFValidationSeverity.Error, ValidationMessages.ID_0711)},
            {ValidationRuleIDs.ID_0712, (MXFValidationCategory.RIP, MXFValidationSeverity.Error, ValidationMessages.ID_0712)},
            {ValidationRuleIDs.ID_0713, (MXFValidationCategory.RIP, MXFValidationSeverity.Error, ValidationMessages.ID_0713)},
            {ValidationRuleIDs.ID_0714, (MXFValidationCategory.RIP, MXFValidationSeverity.Error, ValidationMessages.ID_0714)},
            {ValidationRuleIDs.ID_0715, (MXFValidationCategory.RIP, MXFValidationSeverity.Error, ValidationMessages.ID_0715)},
            {ValidationRuleIDs.ID_0716, (MXFValidationCategory.KLVStream, MXFValidationSeverity.Error, ValidationMessages.ID_0716)},
            {ValidationRuleIDs.ID_0717, (MXFValidationCategory.SMPTERegisters, MXFValidationSeverity.Warning, ValidationMessages.ID_0717)},
            {ValidationRuleIDs.ID_0718, (MXFValidationCategory.SMPTERegisters, MXFValidationSeverity.Warning, ValidationMessages.ID_0718)},
        };


        // get validation rule from dictionary, compose validation message and finally create validation result
        public static MXFValidationResult CreateValidationResult(ValidationRuleIDs ruleId, MXFObject mxfObject, long? offset, params object[] messageParameters)
        {

            if (ValidationRulesDictionary.TryGetValue(ruleId, out var rule))
            {
                return new MXFValidationResult(rule.category, rule.severity, mxfObject, offset, string.Format(rule.templateMessage, messageParameters));
            }
            else
            {
                throw new ArgumentException($"Validation rule {ruleId} not found.");
            }
        }
    }
}