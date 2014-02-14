using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBall.Infrastructure
{
    public enum SplittingStreamMode
    {
        Reading_1 = 1,
        Writing_2 = 2
    }

    public class SplittingStream : Stream
    {
        private const int DEFAULT_BUFFER_SIZE = 1024*1024;
        private byte[] Buffer;
        private int BufferSize;
        private long CurrentPosition = 0;
        private int SplitSize;
        private StreamProvider GetStreamFromName;
        private string PositionFormatName;
        private SplittingStreamMode UseMode;

        private int CurrentSplitIndex;
        private Stream CurrentStream;
        private string CurrentStreamName;
        private StreamOperationHandler CloseHandler;
        private StreamOperationHandler FlushHandler;
        private bool isClosed;

        public delegate Stream StreamProvider(string streamName);

        public delegate void StreamOperationHandler(string streamName, Stream stream);

        public static StreamOperationHandler DefaultCloseHandler = (name, stream) => stream.Close();
        public static StreamOperationHandler DefaultFlushHandler = (name, stream) => stream.Flush();

        public SplittingStream(SplittingStreamMode useMode, int splitSize, string positionFormatName, StreamProvider getStreamFromName, StreamOperationHandler closeHandler = null, StreamOperationHandler flushHandler = null)
        {
            
            PositionFormatName = positionFormatName;
            GetStreamFromName = getStreamFromName;
            SplitSize = splitSize;
            BufferSize = getBufferSize(SplitSize);
            Buffer = new byte[BufferSize];
            UseMode = useMode;
            CloseHandler = closeHandler;
            FlushHandler = flushHandler; 
            isClosed = false;
            CurrentSplitIndex = -1;
        }

        public override void Close()
        {
            if(CloseHandler != null)
                CloseHandler(CurrentStreamName, CurrentStream);
            base.Close();
        }

        private static int getBufferSize(int splitSize)
        {
            if (splitSize < DEFAULT_BUFFER_SIZE)
                return splitSize;
            return DEFAULT_BUFFER_SIZE;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long afterPosition = CurrentPosition + count;
            long currSplitIndex = CurrentPosition/SplitSize;
            long afterSplitIndex = afterPosition/SplitSize;
            if (currSplitIndex == afterSplitIndex)
            {
                //ensureCurrentStream(currSplitIndex);
                CurrentPosition += count;
                return CurrentStream.Read(buffer, offset, count);
            }
            else
            {
                for (long executingIX = currSplitIndex; executingIX <= afterSplitIndex; executingIX++)
                {
                    
                }
            }
            return 0;
        }

        private void ensureCurrentStream(int currSplitIndex)
        {
            if (CurrentSplitIndex == currSplitIndex)
                return;
            if (CurrentStream != null && CloseHandler != null)
                CloseHandler(CurrentStreamName, CurrentStream);
            CurrentSplitIndex = currSplitIndex;
            string streamName = string.Format(PositionFormatName, CurrentSplitIndex);
            CurrentStreamName = streamName;
            CurrentStream = GetStreamFromName(streamName);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            long afterPosition = CurrentPosition + count;
            long currSplitIndex = CurrentPosition / SplitSize;
            long afterSplitIndex = afterPosition / SplitSize;
            if (currSplitIndex == afterSplitIndex)
            {
                //ensureCurrentStream(currSplitIndex);
                CurrentPosition += count;
                CurrentStream.Write(buffer, offset, count);
            }
            else
            {
                for (long executingIX = currSplitIndex; executingIX <= afterSplitIndex; executingIX++)
                {

                }
            }
        }

        public override bool CanRead
        {
            get { return isClosed == false && UseMode == SplittingStreamMode.Reading_1; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return isClosed == false && UseMode == SplittingStreamMode.Writing_2; }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get { return CurrentPosition; }
            set { throw new NotSupportedException();}
        }

        public override void Flush()
        {
            if (FlushHandler != null)
                FlushHandler(CurrentStreamName, CurrentStream);
        }
    }
}
