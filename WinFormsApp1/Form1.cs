using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    // ---------------- ÁRBOL ----------------
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
            if (nodo != null)
                nodo.Children.Add(new TreeNodeCustom(child));
        }

        public TreeNodeCustom Search(TreeNodeCustom node, string value)
        {
            if (node.Value == value) return node;

            foreach (var c in node.Children)
            {
                var result = Search(c, value);
                if (result != null) return result;
            }
            return null;
        }
    }

    // ---------------- GRAFO ----------------
    public class Graph
    {
        public Dictionary<string, List<(string, double)>> Ady = new();

        public void AddVertex(string v)
        {
            if (!Ady.ContainsKey(v))
                Ady[v] = new();
        }

        public void AddEdge(string a, string b, double peso)
        {
            AddVertex(a);
            AddVertex(b);
            Ady[a].Add((b, peso));
            Ady[b].Add((a, peso));
        }
    }

    // ---------------- FORMULARIO ----------------
    public partial class Form1 : Form
    {
        GeneralTree arbol = new("Director");
        Graph grafo = new();

        TextBox txtNodo = new();
        TextBox txtA = new();
        TextBox txtB = new();
        TextBox txtPeso = new();

        Button btnAddNodo = new();
        Button btnAddConexion = new();
        TreeView treeViewEstructuras = new();

        public Form1()
        {
            InitializeComponent();
            CrearInterfaz();
        }

  
        private void Form1_Load(object sender, EventArgs e)
        {
            // Mensaje para verificar que funciona
            // Puedes quitarlo
            MessageBox.Show("Formulario cargado correctamente");
        }

        private void CrearInterfaz()
        {
            this.Text = "Árbol y Grafo WinForms";
            this.Size = new Size(950, 700);

            // ---------------- Labels ----------------
            var lblNodo = new Label() { Text = "Nodo:", Location = new Point(10, 15), Size = new Size(50, 20) };
            var lblA = new Label() { Text = "A:", Location = new Point(10, 60), Size = new Size(50, 20) };
            var lblB = new Label() { Text = "B:", Location = new Point(10, 100), Size = new Size(50, 20) };
            var lblPeso = new Label() { Text = "Peso:", Location = new Point(10, 140), Size = new Size(50, 20) };

            // ---------------- TextBox ----------------
            txtNodo.Location = new Point(70, 10);
            txtA.Location = new Point(70, 55);
            txtB.Location = new Point(70, 95);
            txtPeso.Location = new Point(70, 135);

            // ---------------- Botón agregar nodo ----------------
            btnAddNodo.Text = "Agregar Nodo";
            btnAddNodo.Location = new Point(250, 10);
            btnAddNodo.Size = new Size(120, 30);
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

            // ---------------- Botón conectar ----------------
            btnAddConexion.Text = "Conectar A-B";
            btnAddConexion.Location = new Point(250, 55);
            btnAddConexion.Size = new Size(120, 30);
            btnAddConexion.Click += (s, e) =>
            {
                if (double.TryParse(txtPeso.Text, out double p))
                {
                    grafo.AddEdge(txtA.Text, txtB.Text, p);
                    ActualizarTreeView();
                }
                txtA.Clear();
                txtB.Clear();
                txtPeso.Clear();
            };

            // ---------------- TreeView ----------------
            treeViewEstructuras.Location = new Point(20, 200);
            treeViewEstructuras.Size = new Size(880, 440);
            treeViewEstructuras.Font = new Font("Segoe UI", 10);

            // ---------------- Agregar controles ----------------
            this.Controls.AddRange(new Control[] {
                lblNodo, txtNodo, btnAddNodo,
                lblA, txtA,
                lblB, txtB,
                lblPeso, txtPeso,
                btnAddConexion, treeViewEstructuras
            });
        }

        private void ActualizarTreeView()
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
