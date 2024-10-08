using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace ACT4
{
    public partial class Form1 : Form
    {
        int side;
        int n = 6;
        int k = 3;
        List<SixState> states;
        SixState bestState;
        int moveCounter;

        public Form1()
        {
            InitializeComponent();

            side = pictureBox1.Width / n;

            states = initializeRandomStates(k);
            bestState = getBestState(states);

            updateUI();
            label1.Text = "Attacking pairs: " + getAttackingPairs(bestState);
        }

        private List<SixState> initializeRandomStates(int k)
        {
            List<SixState> randomStates = new List<SixState>();
            for (int i = 0; i < k; i++)
            {
                randomStates.Add(randomSixState());
            }
            return randomStates;
        }

        private void updateUI()
        {
            pictureBox2.Refresh();
            label3.Text = "Attacking pairs: " + getAttackingPairs(bestState);
            label4.Text = "Moves: " + moveCounter;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        e.Graphics.FillRectangle(Brushes.Blue, i * side, j * side, side, side);
                    }
                    if (j == bestState.Y[i])
                        e.Graphics.FillEllipse(Brushes.Fuchsia, i * side, j * side, side, side);
                }
            }
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        e.Graphics.FillRectangle(Brushes.Black, i * side, j * side, side, side);
                    }
                    if (j == bestState.Y[i])
                        e.Graphics.FillEllipse(Brushes.Fuchsia, i * side, j * side, side, side);
                }
            }
        }

        private SixState randomSixState()
        {
            Random r = new Random();
            SixState random = new SixState(r.Next(n),
                                             r.Next(n),
                                             r.Next(n),
                                             r.Next(n),
                                             r.Next(n),
                                             r.Next(n));
            return random;
        }

        private int getAttackingPairs(SixState state)
        {
            int attackers = 0;
            for (int rf = 0; rf < n; rf++)
            {
                for (int tar = rf + 1; tar < n; tar++)
                {
                    if (state.Y[rf] == state.Y[tar])
                        attackers++;
                    if (state.Y[tar] == state.Y[rf] + tar - rf)
                        attackers++;
                    if (state.Y[rf] == state.Y[tar] + tar - rf)
                        attackers++;
                }
            }
            return attackers;
        }

        private List<SixState> getSuccessors(SixState state)
        {
            List<SixState> successors = new List<SixState>();
            for (int i = 0; i < n; i++) 
            {
                for (int j = 0; j < n; j++)
                {
                    if (state.Y[i] != j)
                    {
                        SixState successor = new SixState(state);
                        successor.Y[i] = j; 
                        successors.Add(successor);
                    }
                }
            }
            return successors;
        }

        private List<SixState> getBestKSuccessors(List<SixState> currentStates, int k)
        {
            List<SixState> allSuccessors = new List<SixState>();

            foreach (SixState state in currentStates)
            {
                List<SixState> successors = getSuccessors(state);
                allSuccessors.AddRange(successors);
            }

            allSuccessors.Sort((s1, s2) => getAttackingPairs(s1).CompareTo(getAttackingPairs(s2)));
            return allSuccessors.GetRange(0, Math.Min(k, allSuccessors.Count));
        }

        private SixState getBestState(List<SixState> states)
        {
            return states.OrderBy(s => getAttackingPairs(s)).First();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (getAttackingPairs(bestState) > 0)
            {
                states = getBestKSuccessors(states, k);
                bestState = getBestState(states);
                moveCounter++;
                updateUI();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            states = initializeRandomStates(k);
            bestState = getBestState(states);
            moveCounter = 0;
            updateUI();
            pictureBox1.Refresh();
            label1.Text = "Attacking pairs: " + getAttackingPairs(bestState);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while (getAttackingPairs(bestState) > 0)
            {
                states = getBestKSuccessors(states, k);
                bestState = getBestState(states); 
                moveCounter++;
                updateUI();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }

    public class SixState
    {
        public int[] Y;

        public SixState(params int[] positions)
        {
            Y = new int[positions.Length];
            Array.Copy(positions, Y, positions.Length);
        }

        public SixState(SixState other)
        {
            Y = new int[other.Y.Length];
            Array.Copy(other.Y, Y, other.Y.Length);
        }
    }
}
