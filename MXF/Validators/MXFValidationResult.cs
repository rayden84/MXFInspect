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


namespace Myriadbits.MXF.Validators
{
    public enum MXFValidationSeverity
    {
        Success = 1,
        Warning = 2,
        Error = 3,
        Info = 4,
        Question = 5
    };

    public enum MXFValidationCategory
    {
        General,
        KLVStream,
        PartitionStructure,
                RIP,
        IndexTables,
        Metadata,
        SMPTERegisters,
        Essences,
        TrackInfo,
    }

    public class MXFValidationResult
    {
        public MXFValidationCategory Category { get; }
        public MXFObject Object { get; }
        public long? Offset { get; }
        public MXFValidationSeverity Severity { get; }
        public string Message { get; }

        public MXFValidationResult(MXFValidationCategory category, MXFValidationSeverity severity, MXFObject mxfObject, long? offset, string message)
        {
            Category = category;
            Severity = severity;
            Object = mxfObject;
            Offset = offset;
            Message = message;
        }
    }
}
