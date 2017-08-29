using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ukadc.Diagnostics.Tests
{
    public static class StreamHelper
    {
        private const int SIZE = 4096;

        /// <summary>
        /// Creates a copy of the passed stream and returns the clone to position 0.
        /// </summary>
        /// <param name="source">The Stream to be copied</param>
        /// <returns>A copy of the source stream</returns>
        public static Stream CloneStreamAndReturnToStart(Stream source)
        {
            Stream clone = new MemoryStream();
            CloneStream(source, clone);
            clone.Seek(0, SeekOrigin.Begin);
            return clone;
        }

        /// <summary>
        /// Copies the contents of one stream to another. Note the output stream will be left at the last
        /// position and may require a seek before it is used.
        /// </summary>
        /// <param name="input">The stream to copy</param>
        /// <param name="output">The target stream</param>
        public static void CloneStream(Stream input, Stream output)
        {
            byte[] bytes = new byte[SIZE];
            int numBytes;
            while ((numBytes = input.Read(bytes, 0, SIZE)) > 0)
            {
                output.Write(bytes, 0, numBytes);
            }
        }

        /// <summary>
        /// Reads all bytes from a stream and returns a byte array
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        public static byte[] ReadAllBytes(Stream stream)
        {
            byte[] buffer = new byte[SIZE];
            int read = 0;

            int numBytes;
            while ((numBytes = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += numBytes;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }
    }
}
