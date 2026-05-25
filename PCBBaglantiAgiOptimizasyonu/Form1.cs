using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PCBBaglantiAgiOptimizasyonu;

namespace PCBBaglantiAgiOptimizasyonu
{
    public partial class Form1 : Form
    {
        // --- RENK PALETİ ---
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

        // --- VERİ ---
        private Graph pcbGraph = new Graph(200);
        private List<Edge> mstEdges = new List<Edge>();
        private List<Edge> animationEdges = new List<Edge>();
        private int animationStep = 0;
        private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
        private Dictionary<string, PointF> nodePositions = new Dictionary<string, PointF>();

        // --- UI ELEMANLARı ---
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

        // ─────────────────────────────────────────
        // UI KURULUM
        // ─────────────────────────────────────────
        private void BuildUI()
        {
            this.Text = "PCB Bağlantı Ağı — Prim MST";
            this.Size = new Size(1280, 780);
            this.MinimumSize = new Size(960, 620);
            this.BackColor = BG_DARK;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ── SOL PANEL ──────────────────────────
            controlPanel.Dock = DockStyle.Left;
            controlPanel.Width = 210;
            controlPanel.BackColor = BG_PANEL;
            controlPanel.Padding = new Padding(0);

            // İnce sağ kenarlık
            controlPanel.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(35, 35, 42), 1),
                    controlPanel.Width - 1, 0,
                    controlPanel.Width - 1, controlPanel.Height);
            };

            int y = 36;

            // Başlık
            AddLabel(controlPanel, "PCB", ref y, 13, FontStyle.Bold, TEXT_PRIMARY, 0);
            AddLabel(controlPanel, "Bağlantı Ağı Optimizasyonu", ref y, 8, FontStyle.Regular, TEXT_DIM, 4);
            AddSeparator(controlPanel, ref y, 24);

            // Butonlar
            btnGenerate = AddButton(controlPanel, "Yeni Graf Üret", ref y);
            btnRunPrim = AddButton(controlPanel, "Prim Algoritması", ref y);
            btnReset = AddButton(controlPanel, "Sıfırla", ref y);

            btnRunPrim.BackColor = Color.FromArgb(242, 242, 247);
            btnRunPrim.ForeColor = Color.FromArgb(52, 199, 89);
            btnRunPrim.FlatAppearance.BorderColor = Color.FromArgb(52, 199, 89);

            btnReset.ForeColor = Color.FromArgb(255, 59, 48);
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(220, 50, 40);

            AddSeparator(controlPanel, ref y, 24);

            // İstatistikler
            AddLabel(controlPanel, "GRAF BİLGİSİ", ref y, 7, FontStyle.Bold, TEXT_DIM, 0);
            y += 6;
            lblNodeCount = AddInfoRow(controlPanel, "Düğüm", "—", ref y);
            lblEdgeCount = AddInfoRow(controlPanel, "Kenar", "—", ref y);

            AddSeparator(controlPanel, ref y, 20);

            AddLabel(controlPanel, "MST MALİYETİ", ref y, 7, FontStyle.Bold, TEXT_DIM, 0);
            y += 6;
            lblCost = new Label
            {
                Text = "—",
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

            AddLabel(controlPanel, "DOLAŞIM", ref y, 7, FontStyle.Bold, TEXT_DIM, 0);
            y += 6;
            btnBFS = AddButton(controlPanel, "BFS Göster", ref y);
            btnDFS = AddButton(controlPanel, "DFS Göster", ref y);
            btnBFS.ForeColor = Color.FromArgb(255, 149, 0);
            btnBFS.FlatAppearance.BorderColor = Color.FromArgb(255, 149, 0);
            btnDFS.ForeColor = Color.FromArgb(175, 82, 222);
            btnDFS.FlatAppearance.BorderColor = Color.FromArgb(175, 82, 222);

            AddSeparator(controlPanel, ref y, 20);

            // Lejant
            AddLabel(controlPanel, "GÖSTERIM", ref y, 7, FontStyle.Bold, TEXT_DIM, 0);
            y += 8;
            AddLegendRow(controlPanel, EDGE_IDLE, "Olası bağlantı", ref y);
            AddLegendRow(controlPanel, EDGE_MST, "MST kenarı", ref y);
            AddLegendRow(controlPanel, NODE_START, "Başlangıç düğümü", ref y);
            AddLegendRow(controlPanel, NODE_DEFAULT, "Bileşen", ref y);

            AddSeparator(controlPanel, ref y, 20);

            // Durum etiketi
            lblStatus = new Label
            {
                Text = "Canvas'a tıklayarak yeni düğüm ekleyebilirsiniz.",
                Font = new Font("Segoe UI", 7, FontStyle.Regular),
                ForeColor = TEXT_DIM,
                AutoSize = false,
                Width = 170,
                Height = 120,
                Left = 20,
                Top = y
            };
            controlPanel.Controls.Add(lblStatus);

            // ── CANVAS ────────────────────────────
            canvasPanel.Dock = DockStyle.Fill;
            canvasPanel.BackColor = BG_DARK;
            canvasPanel.Cursor = Cursors.Cross;
            canvasPanel.Paint += CanvasPanel_Paint;
            canvasPanel.MouseClick += CanvasPanel_MouseClick;
            canvasPanel.MouseMove += CanvasPanel_MouseMove;

            // Double buffering — yanıp sönmeyi engeller
            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)!
                .SetValue(canvasPanel, true);

            this.Controls.Add(canvasPanel);
            this.Controls.Add(controlPanel);

            // Buton eventleri
            btnGenerate.Click += (s, e) => GenerateRandomGraph(new Random().Next(14, 28));
            btnRunPrim.Click += BtnRunPrim_Click;
            btnReset.Click += (s, e) => ResetAll();
            btnBFS.Click += (s, e) => StartTraversal("BFS");
            btnDFS.Click += (s, e) => StartTraversal("DFS");
        }

        // ─── Yardımcı UI builder metodları ───────
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
                BackColor = Color.FromArgb(32, 32, 38),
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

        // ─────────────────────────────────────────
        // TIMER
        // ─────────────────────────────────────────
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
                    lblStatus.Text = $"Adım {animationStep} / {animationEdges.Count}";
                }
                else
                {
                    animationTimer.Stop();
                    btnRunPrim.Enabled = true;
                    UpdateCostLabel();
                    lblStatus.Text = "MST tamamlandı.";
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
                        path += (i == 0 ? "" : " → ") + traversalOrder[i].Id;
                    lblStatus.Text = path;
                    canvasPanel.Invalidate();
                }
                else
                {
                    traversalTimer.Stop();
                    lblStatus.Text = lblStatus.Text + "\n\n✓ " + traversalMode + " tamamlandı.";
                }
            };
        }

        // ─────────────────────────────────────────
        // BFS / DFS
        // ─────────────────────────────────────────
        private void StartTraversal(string mode)
        {
            if (pcbGraph.ComponentCount == 0) return;

            for (int i = 0; i < pcbGraph.ComponentCount; i++)
                pcbGraph.Components[i].IsVisited = false;

            traversalOrder.Clear();
            traversalStep = 0;
            traversalMode = mode;
            traversalTimer.Stop();

            if (mode == "BFS")
                RunBFS(pcbGraph.Components[0]);
            else
                RunDFS(pcbGraph.Components[0]);

            for (int i = 0; i < pcbGraph.ComponentCount; i++)
                pcbGraph.Components[i].IsVisited = false;

            lblStatus.Text = $"{mode} başlatıldı...";
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
            var stack = new Stack<Node>();
            stack.Push(start);

            while (stack.Count > 0)
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

        // ─────────────────────────────────────────
        // GRAF ÜRETME
        // ─────────────────────────────────────────
        private void GenerateRandomGraph(int nodeCount)
        {
            ResetAll();
            var rand = new Random();
            int margin = 70;
            int w = Math.Max(canvasPanel.Width, 800);
            int h = Math.Max(canvasPanel.Height, 600);

            for (int i = 0; i < nodeCount; i++)
            {
                string id = $"C{i + 1}";
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

            UpdateStats();
            canvasPanel.Invalidate();
            lblStatus.Text = "Graf hazır.";
        }

        // ─────────────────────────────────────────
        // PRİM
        // ─────────────────────────────────────────
        private void BtnRunPrim_Click(object sender, EventArgs e)
        {
            if (pcbGraph.ComponentCount == 0) return;

            for (int i = 0; i < pcbGraph.ComponentCount; i++)
                pcbGraph.Components[i].IsVisited = false;

            mstEdges.Clear();
            animationEdges.Clear();
            animationStep = 0;
            lblCost.Text = "—";

            var result = new PrimAlgorithm().Run(pcbGraph.Components[0], pcbGraph);
            animationEdges = result;

            btnRunPrim.Enabled = false;
            lblStatus.Text = "Hesaplanıyor...";
            animationTimer.Start();
        }

        // ─────────────────────────────────────────
        // CANVAS ÇİZİM
        // ─────────────────────────────────────────
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

                // Ağırlık etiketi — MST kenarı ortası
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

        // ─────────────────────────────────────────
        // DÜĞÜM EKLEME
        // ─────────────────────────────────────────
        private void CanvasPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (animationTimer.Enabled || traversalTimer.Enabled)
            {
                lblStatus.Text = "⚠ Algoritma çalışırken düğüm eklenemez.";
                return;
            }

            string newId = $"C{pcbGraph.ComponentCount + 1}";
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
            UpdateStats();
            canvasPanel.Invalidate();
            lblStatus.Text = $"{newId} eklendi.";
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

            string tip = $"{hoveredId}\n" +
                         $"Komşu: {neighborCount}\n" +
                         $"MST: {(inMST || isStart ? "✓ Evet" : "—")}\n" +
                         $"Başlangıç: {(isStart ? "✓" : "—")}";

            nodeToolTip.Show(tip, canvasPanel, e.X + 12, e.Y - 10, 3000);
        }

        // ─────────────────────────────────────────
        // YARDIMCI
        // ─────────────────────────────────────────
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
            lblCost.Text = "—";
            lblNodeCount.Text = "—";
            lblEdgeCount.Text = "—";
            lblStatus.Text = "";
            canvasPanel.Invalidate();
        }

        private void ResetMST()
        {
            animationTimer.Stop();
            mstEdges.Clear();
            animationEdges.Clear();
            animationStep = 0;
            for (int i = 0; i < pcbGraph.ComponentCount; i++)
                pcbGraph.Components[i].IsVisited = false;
            btnRunPrim.Enabled = true;
            lblCost.Text = "—";
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
            string.Compare(a, b) < 0 ? $"{a}|{b}" : $"{b}|{a}";

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

        private void UpdateCostLabel()
        {
            int total = 0;
            foreach (var e in mstEdges) total += e.Weight;
            lblCost.Text = total.ToString();
        }
    }
}