﻿using System;
using System.Text;
using ModIso8583.Util;

namespace ModIso8583.Parse
{
    /// <summary>
    ///     This class is used to parse fields of type BINARY.
    /// </summary>
    public class BinaryParseInfo : FieldParseInfo
    {
        public BinaryParseInfo(int len) : base(IsoType.BINARY,
            len)
        { }

        public override IsoValue Parse(int field,
            byte[] buf,
            int pos,
            ICustomField custom)
        {
            if (pos < 0) throw new Exception($"Invalid BINARY field {field} position {pos}");
            if (pos + Length * 2 > buf.Length) throw new Exception($"Insufficient data for BINARY field {field} of length {Length}, pos {pos}");
            var binval = HexCodec.HexDecode(Encoding.ASCII.GetString(buf,
                pos,
                Length * 2));
            if (custom == null)
                return new IsoValue(IsoType,
                    binval,
                    binval.Length);
            var dec = custom.DecodeField(Encoding.GetString(buf,
                pos,
                Length * 2));
            return dec == null ? new IsoValue(IsoType,
                binval,
                binval.Length) : new IsoValue(IsoType,
                dec,
                Length,
                custom);
        }

        public override IsoValue ParseBinary(int field,
            byte[] buf,
            int pos,
            ICustomField custom)
        {
            if (pos < 0) throw new Exception($"Invalid BINARY field {field} position {pos}");
            if (pos + Length > buf.Length) throw new Exception($"Insufficient data for BINARY field {field} of length {Length}, pos {pos}");
            var v = new byte[Length];
            Array.Copy(buf,
                pos,
                v,
                0,
                Length);
            if (custom == null)
                return new IsoValue(IsoType,
                    v,
                    Length);
            var dec = custom.DecodeField(HexCodec.HexEncode(v,
                0,
                v.Length));
            return dec == null ? new IsoValue(IsoType,
                v,
                Length) : new IsoValue(IsoType,
                dec,
                Length,
                custom);
        }
    }
}