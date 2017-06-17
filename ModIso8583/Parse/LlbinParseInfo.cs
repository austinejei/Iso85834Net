﻿using System;
using System.Text;
using ModIso8583.Util;

namespace ModIso8583.Parse
{
    public class LlbinParseInfo : FieldParseInfo
    {
        public LlbinParseInfo() : base(IsoType.LLBIN,
            0)
        { }

        public override IsoValue Parse(int field,
            byte[] buf,
            int pos,
            ICustomField custom)
        {
            if (pos < 0) throw new Exception($"Invalid LLBIN field {field} position {pos}");
            if (pos + 2 > buf.Length) throw new Exception($"Invalid LLBIN field {field} position {pos}");
            var len = DecodeLength(buf,
                pos,
                2);
            if (len < 0) throw new Exception($"Invalid LLBIN field {field} length {len} pos {pos}");
            if (len + pos + 2 > buf.Length) throw new Exception($"Insufficient data for LLBIN field {field}, pos {pos} (LEN states '{Encoding.ASCII.GetString(buf, pos, 2)}')");
            var binval = len == 0 ? new byte[0] : HexCodec.HexDecode(Encoding.ASCII.GetString(buf,
                pos + 2,
                len));
            if (custom == null)
                return new IsoValue(IsoType,
                    binval,
                    binval.Length);
            if (custom is ICustomBinaryField)
                try
                {
                    var dec = ((ICustomBinaryField) custom).DecodeBinaryField(buf,
                        pos + 2,
                        len);
                    if (dec == null)
                        return new IsoValue(IsoType,
                            binval,
                            binval.Length);
                    return new IsoValue(IsoType,
                        dec,
                        0,
                        custom);
                }
                catch (Exception) { throw new Exception($"Insufficient data for LLBIN field {field}, pos {pos} (LEN states '{Encoding.ASCII.GetString(buf, pos, 2)}')"); }
            try
            {
                var dec = custom.DecodeField(Encoding.ASCII.GetString(buf,
                    pos + 2,
                    len));
                return dec == null ? new IsoValue(IsoType,
                    binval,
                    binval.Length) : new IsoValue(IsoType,
                    dec,
                    binval.Length,
                    custom);
            }
            catch (Exception) { throw new Exception($"Insufficient data for LLBIN field {field}, pos {pos} (LEN states '{Encoding.ASCII.GetString(buf, pos, 2)}')"); }
        }

        public override IsoValue ParseBinary(int field,
            byte[] buf,
            int pos,
            ICustomField custom)
        {
            if (pos < 0) throw new Exception($"Invalid bin LLBIN field {field} position {pos}");
            if (pos + 1 > buf.Length) throw new Exception($"Insufficient bin LLBIN header field {field}");

            var l = ((buf[pos] & 0xf0) >> 4) * 10 + (buf[pos] & 0x0f);
            if (l < 0)
                throw new Exception($"Invalid bin LLBIN length {l} pos {pos}");
            if (l + pos + 1 > buf.Length)
                throw new Exception($"Insufficient data for bin LLBIN field {field}, pos {pos}: need {l}, only {buf.Length} available");
            var v = new byte[l];
            Array.Copy(buf,
                pos + 1,
                v,
                0,
                l);
            if (custom == null)
                return new IsoValue(IsoType,
                    v);
            if (custom is ICustomBinaryField)
            {
                try
                {
                    var dec = ((ICustomBinaryField) custom).DecodeBinaryField(buf,
                        pos + 1,
                        l);
                    return dec == null ? new IsoValue(IsoType,
                        v,
                        v.Length) : new IsoValue(IsoType,
                        dec,
                        l,
                        custom);
                }
                catch (Exception)
                {
                    throw new Exception($"Insufficient data for LLBIN field {field}, pos {pos} length {l}");
                }
            }
            {
                var dec = custom.DecodeField(HexCodec.HexEncode(v,
                    0,
                    v.Length));
                return dec == null ? new IsoValue(IsoType,
                    v) : new IsoValue(IsoType,
                    dec,
                    custom);
            }
        }
    }
}