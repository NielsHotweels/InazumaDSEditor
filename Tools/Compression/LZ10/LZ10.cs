using System;
using System.IO;
using System.Collections.Generic;

//Compression code from dsdecmp

namespace ScriptInazumaElevenDS.Level5.Compression.LZ10
{
    public class LZ10 : ICompression
    {
        public unsafe byte[] Compress(byte[] indata)
        {
            long inLength = indata.Length;
            MemoryStream outstream = new MemoryStream();

            outstream.WriteByte(16);
            outstream.WriteByte((byte)(inLength & 0xFF));
            outstream.WriteByte((byte)((inLength >> 8) & 0xFF));
            outstream.WriteByte((byte)((inLength >> 16) & 0xFF));

            int compressedLength = 4;

            fixed (byte* instart = &indata[0])
            {
                byte[] outbuffer = new byte[8 * 2 + 1];
                outbuffer[0] = 0;
                int bufferlength = 1, bufferedBlocks = 0;
                int readBytes = 0;

                int[] lengths, disps;
                GetOptimalCompressionLengths(instart, indata.Length, out lengths, out disps);

                while (readBytes < inLength)
                {
                    if (bufferedBlocks == 8)
                    {
                        outstream.Write(outbuffer, 0, bufferlength);
                        compressedLength += bufferlength;
                        outbuffer[0] = 0;
                        bufferlength = 1;
                        bufferedBlocks = 0;
                    }

                    if (lengths[readBytes] == 1)
                    {
                        outbuffer[bufferlength++] = *(instart + (readBytes++));
                    }
                    else
                    {
                        outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

                        outbuffer[bufferlength] = (byte)(((lengths[readBytes] - 3) << 4) & 0xF0);
                        outbuffer[bufferlength] |= (byte)(((disps[readBytes] - 1) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disps[readBytes] - 1) & 0xFF);
                        bufferlength++;

                        readBytes += lengths[readBytes];
                    }
                    bufferedBlocks++;
                }

                if (bufferedBlocks > 0)
                {
                    outstream.Write(outbuffer, 0, bufferlength);
                    compressedLength += bufferlength;
                }
            }

            outstream.Position = 0;
            return ToArray(outstream);
        }

        public byte[] Decompress(byte[] data)
        {
            List<byte> o = new List<byte>();

            int p = 4;
            int op = 0;

            int mask = 0;
            int flag = 0;

            while (p < data.Length)
            {
                if (mask == 0)
                {
                    flag = (data[p++] & 0xFF);
                    //					System.out.println(Integer.toHexString(flag));
                    mask = 0x80;
                }

                if ((flag & mask) == 0)
                {
                    if (p + 1 > data.Length) break;
                    o.Add(data[p++]);
                    op++;
                }
                else
                {
                    if (p + 2 > data.Length) break;
                    int dat = ((data[p++] & 0xFF) << 8) | (data[p++] & 0xFF);
                    int pos = (dat & 0x0FFF) + 1;
                    int length = (dat >> 12) + 3;

                    //				System.out.println(Integer.toHexString(dat) + "\t" + pos + "\t" + length);

                    for (int i = 0; i < length; i++)
                    {
                        if (op - pos >= 0)
                        {
                            o.Add(o[op - pos >= o.Count ? 0 : op - pos]);
                            op++;
                        }
                    }
                }
                mask >>= 1;
            }


            //Console.WriteLine("decompress " + p.ToString("x") + " " + data.Length.ToString("x"));
            return o.ToArray();
        }

        public static unsafe byte[] ToArray(Stream s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (!s.CanRead)
                throw new ArgumentException("Stream cannot be read");

            MemoryStream ms = s as MemoryStream;
            if (ms != null)
                return ms.ToArray();

            long pos = s.CanSeek ? s.Position : 0L;
            if (pos != 0L)
                s.Seek(0, SeekOrigin.Begin);

            byte[] result = new byte[s.Length];
            s.Read(result, 0, result.Length);
            if (s.CanSeek)
                s.Seek(pos, SeekOrigin.Begin);
            return result;
        }

        public static unsafe int GetOccurrenceLength(byte* newPtr, int newLength, byte* oldPtr, int oldLength, out int disp, int minDisp = 1)
        {
            disp = 0;
            if (newLength == 0)
                return 0;
            int maxLength = 0;
            // try every possible 'disp' value (disp = oldLength - i)
            for (int i = 0; i < oldLength - minDisp; i++)
            {
                // work from the start of the old data to the end, to mimic the original implementation's behaviour
                // (and going from start to end or from end to start does not influence the compression ratio anyway)
                byte* currentOldStart = oldPtr + i;
                int currentLength = 0;
                // determine the length we can copy if we go back (oldLength - i) bytes
                // always check the next 'newLength' bytes, and not just the available 'old' bytes,
                // as the copied data can also originate from what we're currently trying to compress.
                for (int j = 0; j < newLength; j++)
                {
                    // stop when the bytes are no longer the same
                    if (*(currentOldStart + j) != *(newPtr + j))
                        break;
                    currentLength++;
                }

                // update the optimal value
                if (currentLength > maxLength)
                {
                    maxLength = currentLength;
                    disp = oldLength - i;

                    // if we cannot do better anyway, stop trying.
                    if (maxLength == newLength)
                        break;
                }
            }
            return maxLength;
        }

        private static unsafe void GetOptimalCompressionLengths(byte* indata, int inLength, out int[] lengths, out int[] disps)
        {
            lengths = new int[inLength];
            disps = new int[inLength];
            int[] minLengths = new int[inLength];

            for (int i = inLength - 1; i >= 0; i--)
            {
                // first get the compression length when the next byte is not compressed
                minLengths[i] = int.MaxValue;
                lengths[i] = 1;
                if (i + 1 >= inLength)
                    minLengths[i] = 1;
                else
                    minLengths[i] = 1 + minLengths[i + 1];
                // then the optimal compressed length
                int oldLength = Math.Min(0x1000, i);
                // get the appropriate disp while at it. Takes at most O(n) time if oldLength is considered O(n)
                // be sure to bound the input length with 0x12, as that's the maximum length for LZ-10 compressed blocks.

                int maxLen = GetOccurrenceLength(indata + i, Math.Min(inLength - i, 0x12),
                                                 indata + i - oldLength, oldLength, out disps[i]);
                if (disps[i] > i)
                    throw new Exception("Input too short!");
                for (int j = 3; j <= maxLen; j++)
                {
                    int newCompLen;
                    if (i + j >= inLength)
                        newCompLen = 2;
                    else
                        newCompLen = 2 + minLengths[i + j];
                    if (newCompLen < minLengths[i])
                    {
                        lengths[i] = j;
                        minLengths[i] = newCompLen;
                    }
                }
            }
        }
    }
}
