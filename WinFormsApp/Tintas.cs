using ClasesInnovaStock.Data;
using ClasesInnovaStock.Models;
using ClasesInnovaStock.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp
{
    public partial class Tintas : Form
    {
        private readonly IGenericRepository<Tinta> _repoTintas;
        private int? tintaIdSeleccionado = null;

        public Tintas()
        {
              using (var db = new AplicationDbContext())
            {
                // Esto borra la base vieja y la recrea con todas sus tablas al 100%
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            InitializeComponent();
            _repoTintas = new GenericRepository<Tinta>();

            this.Load += Tintas_Load;

            if (dataGridView1 != null)
            {
                dataGridView1.CellClick += dataGridView1_CellClick;
            }
        }

        private void Tintas_Load(object sender, EventArgs e)
        {
            if (comboBox1 != null)
            {
                comboBox1.Items.Clear();
                // Obliga a que el usuario elija obligatoriamente de la lista desplegable
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

                // Agregamos las opciones de colores solicitadas
                comboBox1.Items.Add("Cian");
                comboBox1.Items.Add("Magenta");
                comboBox1.Items.Add("Yellow");
                comboBox1.Items.Add("Black");
            }

            CargarGrilla(); // Carga los datos en la grilla al iniciar
        }

        private void CargarGrilla() // Método para cargar la DataGridView con los datos de la tabla Tintas
        {
            try
            {
                if (dataGridView1 != null)
                {
                    dataGridView1.DataSource = null;
                    var listaTintas = _repoTintas.ObtenerTodos();
                    if (listaTintas != null)
                    {
                        dataGridView1.DataSource = listaTintas.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                string errorReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error al cargar la grilla de tintas: {errorReal}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonMenu_Click(object sender, EventArgs e) // Botón volver al menú principal
        {
            MenuInicio menuPrincipal = new MenuInicio();
            menuPrincipal.Show();
            this.Close();
        }

        private void LimpiarCampos() // Método para limpiar campos e IDs seleccionados
        {
            tintaIdSeleccionado = null;
            if (comboBox1 != null) comboBox1.SelectedIndex = -1;
            if (textBox3 != null) textBox3.Clear(); // textBox3: Precio por Litro
            if (textBox4 != null) textBox4.Clear(); // textBox4: Stock
        }

        private void button1_Click_2(object sender, EventArgs e) // BOTÓN 1: AGREGAR
        {
            try
            {
                if (comboBox1 == null || textBox3 == null || textBox4 == null)
                {
                    MessageBox.Show("Faltan controles por inicializar en el formulario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (comboBox1.SelectedItem == null || string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("Por favor, selecciona un color y completa el precio y el stock.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(textBox3.Text, out decimal precio) || !decimal.TryParse(textBox4.Text, out decimal stock))
                {
                    MessageBox.Show("El precio y el stock deben ser valores numéricos válidos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var nuevaTinta = new Tinta
                {
                    Color = comboBox1.SelectedItem.ToString(),
                    PrecioPorLitro = precio,
                    Stock = stock
                };

                _repoTintas.Agregar(nuevaTinta);
                MessageBox.Show("Tinta agregada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarGrilla();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error detallado: {mensajeError}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) // DataGridView Click general de fila
        {
            if (e.RowIndex >= 0 && dataGridView1 != null)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
                try
                {
                    tintaIdSeleccionado = Convert.ToInt32(fila.Cells[0].Value);
                    if (comboBox1 != null) comboBox1.Text = fila.Cells[1].Value?.ToString();
                    if (textBox3 != null) textBox3.Text = fila.Cells[2].Value?.ToString(); // Precio
                    if (textBox4 != null) textBox4.Text = fila.Cells[3].Value?.ToString(); // Stock
                }
                catch
                {
                    // Evitar interrupciones por celdas vacías
                }
            }
        }

        private void button2_Click_2(object sender, EventArgs e) // BOTÓN 2: ELIMINAR
        {
            if (tintaIdSeleccionado == null)
            {
                MessageBox.Show("Selecciona una tinta de la grilla para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show("¿Estás seguro de eliminar esta tinta?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    _repoTintas.Eliminar(tintaIdSeleccionado.Value);
                    MessageBox.Show("Tinta eliminada correctamente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarGrilla();
                    LimpiarCampos();
                }
                catch (Exception ex)
                {
                    string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    MessageBox.Show($"Error al eliminar: {mensajeError}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click_2(object sender, EventArgs e) // BOTÓN 3: MODIFICAR
        {
            try
            {
                if (tintaIdSeleccionado == null)
                {
                    MessageBox.Show("Selecciona una tinta de la grilla para modificar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (comboBox1 == null || textBox3 == null || textBox4 == null || comboBox1.SelectedItem == null || !decimal.TryParse(textBox3.Text, out decimal precio) || !decimal.TryParse(textBox4.Text, out decimal stock))
                {
                    MessageBox.Show("Por favor, selecciona un color y asegúrate de que el precio y el stock sean numéricos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var tintaAEditar = _repoTintas.ObtenerPorId(tintaIdSeleccionado.Value);
                if (tintaAEditar != null)
                {
                    tintaAEditar.Color = comboBox1.SelectedItem.ToString();
                    tintaAEditar.PrecioPorLitro = precio;
                    tintaAEditar.Stock = stock;

                    _repoTintas.Modificar(tintaAEditar);
                    MessageBox.Show("Tinta modificada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
    }
}