using ClasesInnovaStock.Models;
using ClasesInnovaStock.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp
{
    public partial class Proveedores : Form
    {
        private readonly IGenericRepository<Proveedor> _repoProveedores;
        private int? proveedorIdSeleccionado = null;

        public Proveedores()
        {
            InitializeComponent();
            _repoProveedores = new GenericRepository<Proveedor>();

            // Vinculamos los eventos por código para asegurar que funcionen siempre
            this.Load += Proveedores_Load;
            dataGridView1.CellClick += dataGridView1_CellClick;

            // Opciones visuales para el ComboBox
            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Items.Add("General");
                comboBox1.Items.Add("Materias Primas");
                comboBox1.Items.Add("Servicios");
            }
        }

        private void Proveedores_Load(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        private void CargarGrilla()
        {
            try
            {
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = _repoProveedores.ObtenerTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la grilla de proveedores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- BUTTON1: AGREGAR ---
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Por favor, completa al menos el nombre y el teléfono.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var nuevoProveedor = new Proveedor
                {
                    Nombre = textBox1.Text,
                    Telefono = textBox2.Text,
                    Email = string.IsNullOrWhiteSpace(textBox3.Text) ? "N/A" : textBox3.Text
                };

                _repoProveedores.Agregar(nuevoProveedor);
                MessageBox.Show("Proveedor agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarGrilla();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error detallado: {mensajeError}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- BUTTON2: QUITAR ---
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (proveedorIdSeleccionado == null)
            {
                MessageBox.Show("Selecciona un proveedor de la grilla para quitar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show("¿Estás seguro de eliminar este proveedor?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.Yes)
            {
                _repoProveedores.Eliminar(proveedorIdSeleccionado.Value);
                MessageBox.Show("Proveedor eliminado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarGrilla();
                LimpiarCampos();
            }
        }

        // --- BUTTON3: MODIFICAR ---
        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (proveedorIdSeleccionado == null)
                {
                    MessageBox.Show("Selecciona un proveedor de la grilla para modificar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Por favor, completa al menos el nombre y el teléfono.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var proveedorAEditar = _repoProveedores.ObtenerPorId(proveedorIdSeleccionado.Value);
                if (proveedorAEditar != null)
                {
                    proveedorAEditar.Nombre = textBox1.Text;
                    proveedorAEditar.Telefono = textBox2.Text;
                    proveedorAEditar.Email = string.IsNullOrWhiteSpace(textBox3.Text) ? "N/A" : textBox3.Text;

                    _repoProveedores.Modificar(proveedorAEditar);
                    MessageBox.Show("Proveedor modificado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarGrilla();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error detallado: {mensajeError}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- SELECCIONAR FILA DE LA GRILLA ---
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                try
                {
                    proveedorIdSeleccionado = Convert.ToInt32(fila.Cells[0].Value);
                    textBox1.Text = fila.Cells[1].Value?.ToString();
                    textBox2.Text = fila.Cells[2].Value?.ToString();
                    textBox3.Text = fila.Cells[3].Value?.ToString();
                }
                catch
                {
                    foreach (DataGridViewCell celda in fila.Cells)
                    {
                        string nombreColumna = dataGridView1.Columns[celda.ColumnIndex].Name.ToLower();

                        if (nombreColumna.Contains("id"))
                            proveedorIdSeleccionado = Convert.ToInt32(celda.Value);
                        if (nombreColumna.Contains("nom"))
                            textBox1.Text = celda.Value?.ToString();
                        if (nombreColumna.Contains("tel"))
                            textBox2.Text = celda.Value?.ToString();
                        if (nombreColumna.Contains("mail") || nombreColumna.Contains("correo"))
                            textBox3.Text = celda.Value?.ToString();
                    }
                }
            }
        }

        // --- BUTTON4: MENÚ ---
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LimpiarCampos()
        {
            proveedorIdSeleccionado = null;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = -1;
        }
    }
}