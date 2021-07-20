using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAPService.Utils
{
    public class IdGenerator
    {
        static IdGenerator()
        {
            _workId = (ushort)(DateTime.Now.Ticks % 100);
            _centerId = (ushort)(DateTime.Now.Ticks % 100);
        }

        private static ushort _workId;
        private static ushort _centerId;
        private static readonly object _lock = new ();
        private static IdWorker _idWorker;

        public static void Init(ushort workId, ushort centerId)
        {
            _workId = workId;
            _centerId = centerId;
        }

        public static ulong GetSnowflakeId()
        {
            if (_idWorker == null)
            {
                lock (_lock) { _idWorker ??= new IdWorker(_workId, _centerId); }
            }
            return Convert.ToUInt64(_idWorker.NextId());
        }
    }
}