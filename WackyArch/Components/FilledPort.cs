﻿namespace WackyArch.Components
{
    public class FilledPort : Port, ICyclable
    {
        public List<Word> BacklogData { get; set; }
        public Word CurrentData { get; set; }
        private List<Word> loadedData { get; set; }

        public FilledPort(List<Word> data, Pipe pipe, string name) : base(pipe, name)
        {
            BacklogData = data;
            loadedData = data.Select(w => new Word { Value = w.Value }).ToList();
            Cycle();
        }

        public void SetLoadedData(List<Word> data)
        {
            loadedData = data.Select(w => new Word { Value = w.Value }).ToList();
        }

        public override void Cycle()
        {
            if (BacklogData.Count > 0 && (Pipe.Status == PipeStatus.Idle || Pipe.Status == PipeStatus.AwaitingWrite))
            {
                // Write the next value from the list, then shrink the list by one
                CurrentData = BacklogData[0];
                Pipe.Write(BacklogData[0].Value);
                BacklogData = BacklogData.Skip(1).ToList();
            }
            else if (Pipe.Status == PipeStatus.Idle || Pipe.Status == PipeStatus.AwaitingWrite)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                CurrentData = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                Pipe.Status = PipeStatus.Idle;
            }
        }

        public override void Reset()
        {
            BacklogData = loadedData.Select(w => new Word { Value = w.Value }).ToList();
            Pipe.Status = PipeStatus.Idle;
            Cycle();
        }
    }
}
