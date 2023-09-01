using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SP_HW_4
{
    public partial class Form1 : Form
    {
        private readonly List<ProgressBar> _dancers;

        private readonly List<ProgressBar> _horses;

        private readonly List<Thread> _threads = new List<Thread>();

        private readonly SynchronizationContext _context;

        private Thread _thread;

        public Form1()
        {
            InitializeComponent();
            _context = SynchronizationContext.Current;
            _dancers = new List<ProgressBar>() { pB_1, pB_2, pB_3, pB_4, pB_5 };
            _horses = new List<ProgressBar>() { pB_6, pB_7, pB_8, pB_9, pB_10 };
            foreach (var item in _horses)
            {
                item.Value = 0;
                item.Maximum = 1000;
                item.Minimum = 0;
            }
        }

        private void DancePb()
        {
            while (true)
            {
                foreach (var item in _dancers)
                {
                    _context.Send(d => item.Minimum = 1, null);
                    _context.Send(d => item.Maximum = 250, null);
                    _context.Send(d => item.Value = new Random().Next(item.Minimum, item.Maximum), null);
                    Thread.Sleep(100);
                }
            }
        }

        private void HorseRacing()
        {
            int currentThreadIndex = int.Parse(Thread.CurrentThread.Name.Substring(Thread.CurrentThread.Name.Length - 1).ToString());

            while (_horses.ElementAt(currentThreadIndex).Value != _horses.ElementAt(currentThreadIndex).Maximum)
            {
                int currentDistance = _horses.ElementAt(currentThreadIndex).Value;
                int takeDistance = new Random().Next(0, 70);

                if (currentDistance + takeDistance > _horses.ElementAt(currentThreadIndex).Maximum)
                {
                    _context.Send(d => _horses.ElementAt(currentThreadIndex).Value +=
                                      _horses.ElementAt(currentThreadIndex).Maximum - currentDistance, null);
                }
                else if (currentDistance + takeDistance <= _horses.ElementAt(currentThreadIndex).Maximum)
                {
                    _context.Send(d => _horses.ElementAt(currentThreadIndex).Value += takeDistance, null);
                }

                Thread.Sleep(250);
            }

            if (_horses.ElementAt(currentThreadIndex).Value == _horses.ElementAt(currentThreadIndex).Maximum)
                _context.Send(d => listBox1.Items.Add($"Horse number: {currentThreadIndex + 1}"), null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _horses.Count; i++)
            {
                Thread thread = new Thread(HorseRacing);
                thread.Name = $"thread{i}";
                _threads.Add(thread);
            }

            foreach (var item in _threads)
            {
                item.Start();
                Thread.Sleep(50);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _thread = new Thread(new ThreadStart(DancePb));
            _thread.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_thread != null)
                _thread.Abort();

            foreach (var item in _threads)
                if (item != null)
                    item.Abort();
        }


    }
}
