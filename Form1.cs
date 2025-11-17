using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ina
{
    //Árbol 
    public class TreeNodeCustom
    {
        public string Value;
        public List<TreeNodeCustom> Children = new();
        public TreeNodeCustom(string value) => Value = value;
    }

    public class GeneralTree
    {
        public TreeNodeCustom Root;
        public GeneralTree(string rootValue) => Root = new TreeNodeCustom(rootValue);

        public void AddChild(string parent, string child)
        {
            var nodo = Search(Root, parent);
            if (nodo != null) nodo.Children.Add(new TreeNodeCustom(child));
        }

        public TreeNodeCustom Search(TreeNodeCustom node, string value)
        {
            if (node.Value == value) return node;
            foreach (var c in node.Children)
            {
                var encontrado = Search(c, value);
                if (encontrado != null) return encontrado;
            }
            return null;
        }

        public List<string> BFS()
        {
            var list = new List<string>();
            var queue = new Queue<TreeNodeCustom>();
            queue.Enqueue(Root);
            while (queue.Count > 0)
            {
                var n = queue.Dequeue();
                list.Add(n.Value);
                foreach (var c in n.Children)
                    queue.Enqueue(c);
            }
            return list;
        }

        public List<string> DFS()
        {
            var list = new List<string>();
            DFSHelper(Root, list);
            return list;
        }

        private void DFSHelper(TreeNodeCustom node, List<string> list)
        {
            list.Add(node.Value);
            foreach (var c in node.Children)
                DFSHelper(c, list);
        }
    }

    // Grafo
    public class Graph
    {
        public Dictionary<string, List<(string, double)>> Ady = new();
        public void AddVertex(string v) { if (!Ady.ContainsKey(v)) Ady[v] = new(); }

        public void AddEdge(string a, string b, double peso)
        {
            AddVertex(a); AddVertex(b);
            Ady[a].Add((b, peso));
            Ady[b].Add((a, peso));
        }

        public bool IsConnected()
        {
            if (Ady.Count == 0) return true;
            var visitados = new HashSet<string>();
            var queue = new Queue<string>();
            string inicio = Ady.Keys.First();
            queue.Enqueue(inicio);
            while (queue.Count > 0)
            {
                var actual = queue.Dequeue();
                if (!visitados.Add(actual)) continue;
                foreach (var (dest, _) in Ady[actual])
                    queue.Enqueue(dest);
            }
            return visitados.Count == Ady.Count;
        }

        public (double, List<string>) Dijkstra(string inicio, string fin)
        {
            var dist = new Dictionary<string, double>();
            var prev = new Dictionary<string, string>();
            var pq = new SortedSet<(double, string)>();
            foreach (var v in Ady.Keys)
            {
                dist[v] = double.PositiveInfinity;
                prev[v] = null;
            }
            dist[inicio] = 0;
            pq.Add((0, inicio));

            while (pq.Count > 0)
            {
                var (d, u) = pq.Min;
                pq.Remove(pq.Min);
                if (u == fin) break;

                foreach (var (v, peso) in Ady[u])
                {
                    double nuevo = d + peso;
                    if (nuevo < dist[v])
                    {
                        pq.Remove((dist[v], v));
                        dist[v] = nuevo;
                        prev[v] = u;
                        pq.Add((nuevo, v));
                    }
                }
            }

            var path = new List<string>();
            string actual = fin;
            while (actual != null)
            {
                path.Insert(0, actual);
                actual = prev[actual];
            }

            return (dist[fin], path);
        }
    }

    //Formulario
    public partial class Form1 : Form
    {
        GeneralTree arbol = new("Director");
        Graph grafo = new();

        // Controles
        TextBox txtNodo = new();
        TextBox txtA = new();
        TextBox txtB = new();
        TextBox txtPeso = new();

        Button btnAddNodo = new();
        Button btnAddConexion = new();
        Button btnDijkstra = new();
        Button btnBFS = new();
        Button btnDFS = new();
        Button btnCheckConex = new();

        Label lblNodo = new();
        Label lblA = new();
        Label lblB = new();
        Label lblPeso = new();

        TreeView treeViewEstructuras = new();

        public Form1()
        {
            InitializeComponent();
            ConfigFormulario();
            ConfigLabels();
            ConfigTextFields();
            ConfigBotones();
            ConfigTreeView();
            AddControlsToForm();
        }

        void ConfigFormulario()
        {
            this.Text = "Ina - Árbol y Grafo";
            this.Size = new Size(950, 700);
        }

        void ConfigLabels()
        {
            lblNodo.Text = "Nodo:"; lblNodo.Location = new Point(10, 15); lblNodo.Size = new Size(50, 20);
            lblA.Text = "A:"; lblA.Location = new Point(10, 60); lblA.Size = new Size(50, 20);
            lblB.Text = "B:"; lblB.Location = new Point(10, 100); lblB.Size = new Size(50, 20);
            lblPeso.Text = "Peso:"; lblPeso.Location = new Point(10, 140); lblPeso.Size = new Size(50, 20);
        }

        void ConfigTextFields()
        {
            txtNodo.Location = new Point(70, 10); txtNodo.Width = 150;
            txtA.Location = new Point(70, 55); txtA.Width = 150;
            txtB.Location = new Point(70, 95); txtB.Width = 150;
            txtPeso.Location = new Point(70, 135); txtPeso.Width = 80;
        }

        void ConfigBotones()
        {
            btnAddNodo.Text = "Agregar Nodo"; btnAddNodo.Size = new Size(120, 30); btnAddNodo.Location = new Point(250, 10);
            btnAddNodo.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtNodo.Text))
                {
                    arbol.AddChild("Director", txtNodo.Text);
                    grafo.AddVertex(txtNodo.Text);
                    ActualizarTreeView();
                    txtNodo.Clear();
                }
            };

            btnAddConexion.Text = "Conectar A-B"; btnAddConexion.Size = new Size(120, 30); btnAddConexion.Location = new Point(250, 55);
            btnAddConexion.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtA.Text) &&
                    !string.IsNullOrWhiteSpace(txtB.Text) &&
                    double.TryParse(txtPeso.Text, out double p))
                {
                    grafo.AddEdge(txtA.Text, txtB.Text, p);
                    ActualizarTreeView();
                    txtA.Clear(); txtB.Clear(); txtPeso.Clear();
                }
            };

            btnDijkstra.Text = "Ruta A → B"; btnDijkstra.Size = new Size(120, 30); btnDijkstra.Location = new Point(250, 135);
            btnDijkstra.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtA.Text) && !string.IsNullOrWhiteSpace(txtB.Text))
                {
                    var (dist, path) = grafo.Dijkstra(txtA.Text, txtB.Text);
                    MessageBox.Show($"Distancia: {dist}\nRuta: {string.Join(" → ", path)}");
                }
            };

            btnBFS.Text = "Recorrido BFS"; btnBFS.Size = new Size(120, 30); btnBFS.Location = new Point(400, 10);
            btnBFS.Click += (s, e) =>
            {
                var lista = arbol.BFS();
                MessageBox.Show("BFS Árbol: " + string.Join(", ", lista));
            };

            btnDFS.Text = "Recorrido DFS"; btnDFS.Size = new Size(120, 30); btnDFS.Location = new Point(400, 50);
            btnDFS.Click += (s, e) =>
            {
                var lista = arbol.DFS();
                MessageBox.Show("DFS Árbol: " + string.Join(", ", lista));
            };

            btnCheckConex.Text = "Grafo Conexo?"; btnCheckConex.Size = new Size(120, 30); btnCheckConex.Location = new Point(400, 90);
            btnCheckConex.Click += (s, e) =>
            {
                bool con = grafo.IsConnected();
                MessageBox.Show("Grafo Conexo: " + con);
            };
        }

        void ConfigTreeView()
        {
            treeViewEstructuras.Location = new Point(20, 200);
            treeViewEstructuras.Size = new Size(880, 440);
            treeViewEstructuras.Font = new Font("Segoe UI", 10);
        }

        void AddControlsToForm()
        {
            this.Controls.AddRange(new Control[]
            {
                lblNodo, txtNodo, btnAddNodo,
                lblA, txtA,
                lblB, txtB,
                lblPeso, txtPeso,
                btnAddConexion, btnDijkstra,
                btnBFS, btnDFS, btnCheckConex,
                treeViewEstructuras
            });
        }

        void ActualizarTreeView()
        {
            treeViewEstructuras.Nodes.Clear();

            // ÁRBOL
            TreeNode nodoArbol = new TreeNode("Árbol (Director)");
            treeViewEstructuras.Nodes.Add(nodoArbol);

            foreach (var hijo in arbol.Root.Children)
            {
                TreeNode nodoHijo = new TreeNode(hijo.Value);
                nodoArbol.Nodes.Add(nodoHijo);

                foreach (var sub in hijo.Children)
                    nodoHijo.Nodes.Add(new TreeNode(sub.Value));
            }

            // GRAFO
            TreeNode nodoGrafo = new TreeNode("Grafo");
            treeViewEstructuras.Nodes.Add(nodoGrafo);

            foreach (var v in grafo.Ady.Keys)
            {
                TreeNode nodoV = new TreeNode(v);
                nodoGrafo.Nodes.Add(nodoV);

                foreach (var (dest, peso) in grafo.Ady[v])
                    nodoV.Nodes.Add(new TreeNode($"{dest} (peso {peso})"));
            }

            treeViewEstructuras.ExpandAll();
        }
    }
}
