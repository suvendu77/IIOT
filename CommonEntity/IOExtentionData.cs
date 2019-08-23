using System;
using System.Collections.Generic;
using System.Text;

namespace CommonEntity
{
    public enum IOType
    {
        Read,
        ReadWrite,
        Write
    }
    public class IOExtentionData
    {
        public IOType IOType { get; set; }

        public string ReadFrom { get; set; }

        public string WriteTo { get; set; }

        public bool IsWriteToDifferentFromRead { get; set; }

        public bool IsBuffered { get; set; }

        public double Deadband { get; set; }

        public bool IsReflectInputToOutput { get; set; }

        public bool IsIOScalingEnable { get; set; }

        public IOScalingData IOScalingData { get; set; }

    }
}
