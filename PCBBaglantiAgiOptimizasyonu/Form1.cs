using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PCBBaglantiAgiOptimizasyonu;

// Proje Ekibi:
// Busra Dereli

namespace PCBBaglantiAgiOptimizasyonu
{
    public partial class Form1 : Form
    {
        // --- RENK PALETI ---
        static readonly Color BG_DARK = Color.FromArgb(242, 242, 247);
        static readonly Color BG_PANEL = Color.FromArgb(255, 255, 255);
        static readonly Color ACCENT = Color.FromArgb(52, 199, 89);
        static readonly Color EDGE_IDLE = Color.FromArgb(200, 200, 210);
        static readonly Color EDGE_MST = Color.FromArgb(52, 199, 89);
        static readonly Color NODE_DEFAULT = Color.FromArgb(255, 255, 255);
        static readonly Color NODE_START = Color.FromArgb(52, 199, 89);
        static readonly Color NODE_BORDER = Color.FromArgb(180, 180, 195);
        static readonly Color NODE_BDR_S = Color.FromArgb(52, 199, 89);
        static readonly Color TEXT_PRIMARY = Color.FromArgb(28, 28, 30);
        static readonly Color TEXT_DIM = Color.FromArgb(142, 142, 147);

        // --- VERI ---
        private Graph pcbGraph = new Graph(200);
        private List<Edge> mstEdges = new List<Edge>();
        private List<Edge> animationEdges = new List<Edge>();
        private int animationStep = 0;
        private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
        private Dictionary<string, PointF> nodePositions = new Dictionary<string, PointF>();

        // --- UI ELEMANLARI ---
        private Panel canvasPanel = new Panel();
        private Panel controlPanel = new Panel();
        private Button btnGenerate = new Button();
        private Button btnRunPrim = new Button();
        private Button btnReset = new Button();
        private Label lblNodeCount = new Label();
        private Label lblEdgeCount = new Label();
        private Label lblCost = new Label();
        private Label lblStatus = new Label();

        private List<Node> traversalOrder = new List<Node>();
        private int traversalStep = 0;
        private System.Windows.Forms.Timer traversalTimer = new System.Windows.Forms.Timer();
        private string traversalMode = "";
        private ToolTip nodeToolTip = new ToolTip();
        private string lastHoveredNode = "";
        private Button btnBFS = new Button();
        private Button btnDFS = new Button();

        public Form1()
        {
            InitializeComponent();
            BuildUI();
            SetupTimer();
            GenerateRandomGraph(18);
        }

        // -----------------------------------------
        // UI KURULUM
        // -----------------------------------------
        private void BuildUI()
        {
            // [DUZELTME 3] Baslik cok uzun kesiliyordu, kisaltildi
            this.Text = "PCB Baglanti Agi - Prim MST";
            this.Size = new Size(1280, 780);
            this.MinimumSize = new Size(960, 620);
            this.BackColor = BG_DARK;
            this.StartPosition = FormStartPosition.CenterScreen;

            // -- SOL PANEL --
            controlPanel.Dock = DockStyle.Left;
            controlPanel.Width = 210;
            controlPanel.BackColor = BG_PANEL;
            controlPanel.Padding = new Padding(0);

            controlPanel.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(35, 35, 42), 1),
                    controlPanel.Width - 1, 0,
                    controlPanel.Width - 1, controlPanel.Height);
            };

            int y = 24; // [DUZELTME 3] 36 yerine 24 - PCB yazisi icin daha fazla alan

            // [DUZELTME 3] PCB basligi -- daha buyuk font, daha fazla bosluk
            var lblPCB = new Label
            {
                Text = "PCB",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TEXT_PRIMARY,
                AutoSize = false,
                Width = 170,
                Height = 30,
                Left = 20,
                Top = y
            };
            controlPanel.Controls.Add(lblPCB);
            y += 32;

            AddLabel(controlPanel, "Baglanti Agi Optimizasyonu", ref y, 8, FontStyle.Regular, TEXT_DIM, 4);
            AddSeparator(controlPanel, ref y, 20);

            // Butonlar
            btnGenerate = AddButton(controlPanel, "Yeni Graf Urt", ref y);
            btnRunPrim = AddButton(controlPanel, "Prim Algoritmasi", ref y);
            btnReset = AddButton(controlPanel, "Sifirla", ref y);

            btnRunPrim.BackColor = Color.FromArgb(242, 242, 247);
            btnRunPrim.ForeColor = Color.FromArgb(52, 199, 89);
            btnRunPrim.FlatAppearance.BorderColor = Color.FromArgb(52, 199, 89);

            btnReset.ForeColor = Color.FromArgb(255, 59, 48);
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(220, 50, 40);

            AddSeparator(controlPanel, ref y, 20);

            // [DUZELTME 2] GRAF BILGISI -- dugum/kenar sayisi gosterimi
            AddLabel(controlPanel, "GRAF BILGISI", ref y, 7, FontStyle.Bold, TEXT_DIM, 0);
            y += 4;
            lblNodeCount = AddInfoRow(controlPanel, "Dugum", "--", ref y);
            lblEdgeCount = AddInfoRow(controlPanel, "Kenar", "--", ref y);

            AddSeparator(controlPanel, ref y, 20);

            // [DUZELTME 1] MST MALIYETI -- dinamik guncelleme icin lblCost
            AddLabel(controlPanel, "MST MALIYETI", ref y, 7, FontStyle.Bold, TEXT_DIM, 0);
            y += 6;
            lblCost = new Label
            {
                Text = "--",
                Font = new Font("Segoe UI", 22, FontStyle.Regular),
                ForeColor = TEXT_PRIMARY,
                AutoSize = false,
                Width = 180,
                Height = 40,
                Left = 20,
                Top = y,
                TextAlign = ContentAlignment.MiddleLeft
            };
            controlPanel.Controls.Add(lblCost);
            y += 48;

            AddSeparator(controlPanel, ref y, 20);

            AddLabel(controlPanel, "DOLASIM", ref y, 7, FontStyle.Bold, TEXT_DIM, 0);
            y += 6;
            btnBFS = AddButton(controlPanel, "BFS Goster", ref y);
            btnDFS = AddButton(controlPanel, "DFS Goster", ref y);
            btnBFS.ForeColor = Color.FromArgb(255, 149, 0);
            btnBFS.FlatAppearance.BorderColor = Color.FromArgb(255, 149, 0);
            btnDFS.ForeColor = Color.FromArgb(175, 82, 222);
            btnDFS.FlatAppearance.BorderColor = Color.FromArgb(175, 82, 222);

            AddSeparator(controlPanel, ref y, 20);

            AddLabel(controlPanel, "GOSTERIM", ref y, 7, FontStyle.Bold, TEXT_DIM, 0);
            y += 8;
            AddLegendRow(controlPanel, EDGE_IDLE, "Olasi baglanti", ref y);
            AddLegendRow(controlPanel, EDGE_MST, "MST kenari", ref y);
            AddLegendRow(controlPanel, NODE_START, "Baslangic dugumu", ref y);
            AddLegendRow(controlPanel, NODE_DEFAULT, "Bilesen", ref y);

            AddSeparator(controlPanel, ref y, 20);

            lblStatus = new Label
            {
                Text = "Canvas a tiklayarak yeni dugum ekleyebilirsiniz.",
                Font = new Font("Segoe UI", 7, FontStyle.Regular),
                ForeColor = TEXT_DIM,
                AutoSize = false,
                Width = 170,
                Height = 120,
                Left = 20,
                Top = y
            };
            controlPanel.Controls.Add(lblStatus);

            // -- CANVAS --
            canvasPanel.Dock = DockStyle.Fill;
            canvasPanel.BackColor = BG_DARK;
            canvasPanel.Cursor = Cursors.Cross;
            canvasPanel.Paint += CanvasPanel_Paint;
            canvasPanel.MouseClick += CanvasPanel_MouseClick;
            canvasPanel.MouseMove += CanvasPanel_MouseMove;

            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)!
                .SetValue(canvasPanel, true);

            this.Controls.Add(canvasPanel);
            this.Controls.Add(controlPanel);

            btnGenerate.Click += (s, e) => GenerateRandomGraph(new Random().Next(20, 101));
            btnRunPrim.Click += BtnRunPrim_Click;
            btnReset.Click += (s, e) => ResetAll();
            btnBFS.Click += (s, e) => StartTraversal("BFS");
            btnDFS.Click += (s, e) => StartTraversal("DFS");
        }

        private void AddLabel(Panel p, string text, ref int y, float size,
                              FontStyle style, Color color, int extraY)
        {
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, style),
                ForeColor = color,
                AutoSize = false,
                Width = 170,
                Height = 20,
                Left = 20,
                Top = y
            };
            p.Controls.Add(lbl);
            y += 20 + extraY;
        }

        private void AddSeparator(Panel p, ref int y, int margin)
        {
            y += margin / 2;
            var sep = new Panel
            {
                BackColor = Color.FromArgb(200, 200, 210),
                Height = 1,
                Width = 170,
                Left = 20,
                Top = y
            };
            p.Controls.Add(sep);
            y += 1 + margin / 2;
        }

        private Button AddButton(Panel p, string text, ref int y)
        {
            var btn = new Button
            {
                Text = text,
                Top = y,
                Left = 20,
                Width = 170,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(242, 242, 247),
                ForeColor = Color.FromArgb(28, 28, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 210);
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 235);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(210, 210, 220);
            p.Controls.Add(btn);
            y += 42;
            return btn;
        }

        private Label AddInfoRow(Panel p, string key, string val, ref int y)
        {
            var lKey = new Label
            {
                Text = key,
                Font = new Font("Segoe UI", 8),
                ForeColor = TEXT_DIM,
                AutoSize = false,
                Width = 80,
                Height = 22,
                Left = 20,
                Top = y
            };
            var lVal = new Label
            {
                Text = val,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = TEXT_PRIMARY,
                AutoSize = false,
                Width = 80,
                Height = 22,
                Left = 110,
                Top = y,
                TextAlign = ContentAlignment.MiddleRight
            };
            p.Controls.Add(lKey);
            p.Controls.Add(lVal);
            y += 24;
            return lVal;
        }

        private void AddLegendRow(Panel p, Color dotColor, string text, ref int y)
        {
            var dot = new Panel
            {
                BackColor = dotColor,
                Width = 6,
                Height = 6,
                Left = 22,
                Top = y + 6
            };
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = TEXT_DIM,
                AutoSize = false,
                Width = 150,
                Height = 18,
                Left = 36,
                Top = y
            };
            p.Controls.Add(dot);
            p.Controls.Add(lbl);
            y += 20;
        }

        // -----------------------------------------
        // TIMER
        // -----------------------------------------
        private void SetupTimer()
        {
            animationTimer.Interval = 800;
            animationTimer.Tick += (s, e) =>
            {
                if (animationStep < animationEdges.Count)
                {
                    mstEdges.Add(animationEdges[animationStep]);
                    animationStep++;
                    canvasPanel.Invalidate();

                    // [DUZELTME 1] Her adimda maliyet dinamik guncelleniyor
                    UpdateCostLabel();
                    lblStatus.Text = "Adim " + animationStep + " / " + animationEdges.Count;
                }
                else
                {
                    animationTimer.Stop();
                    btnRunPrim.Enabled = true;
                    UpdateCostLabel();
                    lblStatus.Text = "MST tamamlandi.";
                }
            };

            traversalTimer.Interval = 600;
            traversalTimer.Tick += (s, e) =>
            {
                if (traversalStep < traversalOrder.Count)
                {
                    traversalStep++;
                    string path = "";
                    for (int i = 0; i < traversalStep; i++)
                        path += (i == 0 ? "" : " -> ") + traversalOrder[i].Id;
                    lblStatus.Text = path;
                    canvasPanel.Invalidate();
                }
                else
                {
                    traversalTimer.Stop();
                    lblStatus.Text = traversalMode + " tamamlandi.";
                }
            };
        }

        // -----------------------------------------
        // BAGLANTILiLIK KONTROLu
        // -----------------------------------------
        private bool IsConnected()
        {
            if (pcbGraph.ComponentCount == 0) return true;
            ResetVisited();
            RunBFS(pcbGraph.Components[0]);
            for (int i = 0; i < pcbGraph.ComponentCount; i++)
                if (!pcbGraph.Components[i].IsVisited) return false;
            return true;
        }

        private void ResetVisited()
        {
            for (int i = 0; i < pcbGraph.ComponentCount; i++)
                pcbGraph.Components[i].IsVisited = false;
        }

        // -----------------------------------------
        // BFS / DFS
        // -----------------------------------------
        private void StartTraversal(string mode)
        {
            if (pcbGraph.ComponentCount == 0) return;

            ResetVisited();
            traversalOrder.Clear();
            traversalStep = 0;
            traversalMode = mode;
            traversalTimer.Stop();

            if (mode == "BFS")
                RunBFS(pcbGraph.Components[0]);
            else
                RunDFS(pcbGraph.Components[0]);

            ResetVisited();
            lblStatus.Text = mode + " baslatildi...";
            traversalTimer.Start();
        }

        private void RunBFS(Node start)
        {
            var queue = new CustomQueue();
            start.IsVisited = true;
            queue.Enqueue(start);

            while (!queue.IsEmpty())
            {
                var current = queue.Dequeue();
                traversalOrder.Add(current);

                var edge = current.HeadEdge;
                while (edge != null)
                {
                    if (!edge.Destination.IsVisited)
                    {
                        edge.Destination.IsVisited = true;
                        queue.Enqueue(edge.Destination);
                    }
                    edge = edge.Next;
                }
            }
        }

        private void RunDFS(Node start)
        {
            var stack = new CustomStack();
            stack.Push(start);

            while (!stack.IsEmpty())
            {
                var current = stack.Pop();
                if (current.IsVisited) continue;

                current.IsVisited = true;
                traversalOrder.Add(current);

                var edge = current.HeadEdge;
                while (edge != null)
                {
                    if (!edge.Destination.IsVisited)
                        stack.Push(edge.Destination);
                    edge = edge.Next;
                }
            }
        }

        // -----------------------------------------
        // GRAF uRETME
        // -----------------------------------------
        private void GenerateRandomGraph(int nodeCount)
        {
            ResetAll();
            var rand = new Random();
            int margin = 70;
            int w = Math.Max(canvasPanel.Width, 800);
            int h = Math.Max(canvasPanel.Height, 600);

            for (int i = 0; i < nodeCount; i++)
            {
                string id = "C" + (i + 1);
                var node = new Node(id);
                pcbGraph.AddNode(node);
                nodePositions[id] = new PointF(
                    rand.Next(margin, w - margin),
                    rand.Next(margin, h - margin));
            }

            for (int i = 0; i < pcbGraph.ComponentCount; i++)
            {
                int conns = rand.Next(2, 5);
                for (int j = 0; j < conns; j++)
                {
                    int t = rand.Next(0, pcbGraph.ComponentCount);
                    if (t == i) continue;
                    var from = pcbGraph.Components[i];
                    var to = pcbGraph.Components[t];
                    int weight = (int)Dist(nodePositions[from.Id], nodePositions[to.Id]);
                    from.AddEdge(to, weight);
                    to.AddEdge(from, weight);
                }
            }

            // [DUZELTME 2] Graf uretilince dugum/kenar sayisi hemen guncelleniyor
            UpdateStats();
            canvasPanel.Invalidate();
            lblStatus.Text = "Graf hazir.";
        }

        // -----------------------------------------
        // PRiM
        // -----------------------------------------
        private void BtnRunPrim_Click(object sender, EventArgs e)
        {
            if (pcbGraph.ComponentCount == 0) return;

            if (!IsConnected())
            {
                lblStatus.Text = "Graf bagli degil! Prim calistirilmiyor.";
                return;
            }

            ResetVisited();
            mstEdges.Clear();
            animationEdges.Clear();
            animationStep = 0;
            lblCost.Text = "0"; // [DUZELTME 1] Sifirdan basliyor

            var result = new PrimAlgorithm().Run(pcbGraph.Components[0], pcbGraph);
            animationEdges = result;

            btnRunPrim.Enabled = false;
            lblStatus.Text = "Hesaplaniyor...";
            animationTimer.Start();
        }

        // -----------------------------------------
        // CANVAS CiZiM
        // -----------------------------------------
        private void CanvasPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawIdleEdges(g);
            DrawMSTEdges(g);
            DrawNodes(g);
        }

        private void DrawIdleEdges(Graphics g)
        {
            var pen = new Pen(EDGE_IDLE, 1f);
            var drawn = new HashSet<string>();

            for (int i = 0; i < pcbGraph.ComponentCount; i++)
            {
                var node = pcbGraph.Components[i];
                var edge = node.HeadEdge;
                while (edge != null)
                {
                    string key = EdgeKey(node.Id, edge.Destination.Id);
                    if (!drawn.Contains(key))
                    {
                        drawn.Add(key);
                        if (nodePositions.ContainsKey(node.Id) &&
                            nodePositions.ContainsKey(edge.Destination.Id))
                            g.DrawLine(pen,
                                nodePositions[node.Id],
                                nodePositions[edge.Destination.Id]);
                    }
                    edge = edge.Next;
                }
            }
            pen.Dispose();
        }

        private void DrawMSTEdges(Graphics g)
        {
            var penThin = new Pen(Color.FromArgb(60, EDGE_MST), 4f);
            var penLine = new Pen(EDGE_MST, 1.2f);

            foreach (var edge in mstEdges)
            {
                string? srcId = FindSourceId(edge);
                if (srcId == null) continue;
                if (!nodePositions.ContainsKey(srcId) ||
                    !nodePositions.ContainsKey(edge.Destination.Id)) continue;

                var p1 = nodePositions[srcId];
                var p2 = nodePositions[edge.Destination.Id];

                g.DrawLine(penThin, p1, p2);
                g.DrawLine(penLine, p1, p2);

                var mid = new PointF((p1.X + p2.X) / 2f, (p1.Y + p2.Y) / 2f);
                var font = new Font("Segoe UI", 7f, FontStyle.Regular);
                g.DrawString(edge.Weight.ToString(), font,
                    new SolidBrush(Color.FromArgb(52, 199, 89)), mid);
                font.Dispose();
            }

            penThin.Dispose();
            penLine.Dispose();
        }

        private void DrawNodes(Graphics g)
        {
            int r = 16;
            for (int i = 0; i < pcbGraph.ComponentCount; i++)
            {
                var node = pcbGraph.Components[i];
                if (!nodePositions.ContainsKey(node.Id)) continue;

                var pos = nodePositions[node.Id];
                bool isStart = (i == 0);
                bool isVisited = traversalStep > 0 &&
                                 traversalOrder.Count > 0 &&
                                 traversalStep > traversalOrder.IndexOf(pcbGraph.Components[i]);

                Color traversalColor = traversalMode == "BFS"
                    ? Color.FromArgb(255, 149, 0)
                    : Color.FromArgb(175, 82, 222);

                Color fill = isStart ? NODE_START :
                                isVisited ? traversalColor : NODE_DEFAULT;
                Color border = isStart ? NODE_BDR_S :
                                isVisited ? traversalColor : NODE_BORDER;
                Color textCol = (isStart || isVisited) ? Color.White : TEXT_DIM;

                var rect = new RectangleF(pos.X - r, pos.Y - r, r * 2, r * 2);

                using var fb = new SolidBrush(fill);
                g.FillEllipse(fb, rect);
                g.DrawEllipse(new Pen(border, 1f), rect);

                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                var font = new Font("Segoe UI", 6.5f, FontStyle.Bold);
                g.DrawString(node.Id, font, new SolidBrush(textCol), rect, sf);
                font.Dispose();
            }
        }

        // -----------------------------------------
        // DuGuM EKLEME
        // -----------------------------------------
        private void CanvasPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (animationTimer.Enabled || traversalTimer.Enabled)
            {
                lblStatus.Text = "Algoritma calisirken dugum eklenemez.";
                return;
            }

            string newId = "C" + (pcbGraph.ComponentCount + 1);
            var newNode = new Node(newId);
            pcbGraph.AddNode(newNode);
            nodePositions[newId] = new PointF(e.X, e.Y);

            var rand = new Random();
            int conns = Math.Min(3, pcbGraph.ComponentCount - 1);
            for (int i = 0; i < conns; i++)
            {
                int t = rand.Next(0, pcbGraph.ComponentCount - 1);
                var target = pcbGraph.Components[t];
                int w = (int)Dist(new PointF(e.X, e.Y), nodePositions[target.Id]);
                newNode.AddEdge(target, w);
                target.AddEdge(newNode, w);
            }

            ResetMST();
            // [DUZELTME 2] Dugum eklenince istatistikler guncelleniyor
            UpdateStats();
            canvasPanel.Invalidate();
            lblStatus.Text = newId + " eklendi.";
        }

        private void CanvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            int r = 16;
            string hoveredId = "";

            for (int i = 0; i < pcbGraph.ComponentCount; i++)
            {
                var node = pcbGraph.Components[i];
                if (!nodePositions.ContainsKey(node.Id)) continue;
                var pos = nodePositions[node.Id];
                float dx = e.X - pos.X;
                float dy = e.Y - pos.Y;
                if (dx * dx + dy * dy <= r * r) { hoveredId = node.Id; break; }
            }

            if (hoveredId == lastHoveredNode) return;
            lastHoveredNode = hoveredId;

            if (hoveredId == "")
            {
                nodeToolTip.Hide(canvasPanel);
                return;
            }

            var n = pcbGraph.FindNode(hoveredId);
            if (n == null) return;

            int neighborCount = 0;
            var edge = n.HeadEdge;
            while (edge != null) { neighborCount++; edge = edge.Next; }

            bool inMST = mstEdges.Exists(ed => ed.Destination.Id == hoveredId);
            bool isStart = (pcbGraph.Components[0].Id == hoveredId);

            string tip = hoveredId + "\nKomsu: " + neighborCount +
                         "\nMST: " + (inMST || isStart ? "Evet" : "-") +
                         "\nBaslangic: " + (isStart ? "Evet" : "-");

            nodeToolTip.Show(tip, canvasPanel, e.X + 12, e.Y - 10, 3000);
        }

        // -----------------------------------------
        // YARDIMCI
        // -----------------------------------------
        private void ResetAll()
        {
            animationTimer.Stop();
            traversalTimer.Stop();
            pcbGraph = new Graph(200);
            nodePositions.Clear();
            mstEdges.Clear();
            animationEdges.Clear();
            traversalOrder.Clear();
            animationStep = 0;
            traversalStep = 0;
            traversalMode = "";
            btnRunPrim.Enabled = true;
            lblCost.Text = "--";
            lblNodeCount.Text = "--";
            lblEdgeCount.Text = "--";
            lblStatus.Text = "";
            canvasPanel.Invalidate();
        }

        private void ResetMST()
        {
            animationTimer.Stop();
            mstEdges.Clear();
            animationEdges.Clear();
            animationStep = 0;
            ResetVisited();
            btnRunPrim.Enabled = true;
            lblCost.Text = "--";
        }

        private string? FindSourceId(Edge target)
        {
            for (int i = 0; i < pcbGraph.ComponentCount; i++)
            {
                var e = pcbGraph.Components[i].HeadEdge;
                while (e != null)
                {
                    if (e == target) return pcbGraph.Components[i].Id;
                    e = e.Next;
                }
            }
            return null;
        }

        private float Dist(PointF a, PointF b)
        {
            float dx = a.X - b.X, dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        private string EdgeKey(string a, string b) =>
            string.Compare(a, b) < 0 ? (a + "|" + b) : (b + "|" + a);

        // [DUZELTME 2] Graf bilgisi - dugum ve kenar sayisini gunceller
        private void UpdateStats()
        {
            int ec = 0;
            for (int i = 0; i < pcbGraph.ComponentCount; i++)
            {
                var e = pcbGraph.Components[i].HeadEdge;
                while (e != null) { ec++; e = e.Next; }
            }
            lblNodeCount.Text = pcbGraph.ComponentCount.ToString();
            lblEdgeCount.Text = (ec / 2).ToString();
        }

        // [DUZELTME 1] MST maliyeti - her adimda toplam guncelleniyor
        private void UpdateCostLabel()
        {
            int total = 0;
            foreach (var e in mstEdges) total += e.Weight;
            lblCost.Text = total.ToString();
        }
    }
}
