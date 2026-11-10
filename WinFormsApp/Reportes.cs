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
    public partial class Reportes : Form
    {
        private readonly IGenericRepository<Material> _repoMateriales;
        private readonly IGenericRepository<Proveedor> _repoProveedores;

        public Reportes()
        {
            InitializeComponent();
            _repoMateriales = new GenericRepository<Material>();
            _repoProveedores = new GenericRepository<Proveedor>();

            this.Load += Reportes_Load;
        }

        private void Reportes_Load(object sender, EventArgs e)
        {
        }

        // --- BOTÓN 1: MATERIAL MÁS UTILIZADO ---
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var materiales = _repoMateriales.ObtenerTodos();
                if (materiales != null && materiales.Any())
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = materiales;
                    MessageBox.Show("Mostrando el listado de materiales registrados.", "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No hay materiales registrados en el sistema.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            MenuInicio menuPrincipal = new MenuInicio();
            menuPrincipal.Show();
            this.Close();
        }

        // --- BOTÓN 2: PROVEEDOR MÁS UTILIZADO ---
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var proveedores = _repoProveedores.ObtenerTodos();
                if (proveedores != null && proveedores.Any())
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = proveedores;
                    MessageBox.Show("Mostrando el reporte de proveedores.", "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No hay proveedores registrados en el sistema.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        // --- BOTÓN 3: COLOR MÁS UTILIZADO ---
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var materiales = _repoMateriales.ObtenerTodos();
                if (materiales != null && materiales.Any())
                {
                    var reporteColores = materiales
                        .GroupBy(m => string.IsNullOrWhiteSpace(m.Color) ? "Sin Especificar" : m.Color)
                        .Select(g => new
                        {
                            Color = g.Key,
                            CantidadMateriales = g.Count()
                        })
                        .OrderByDescending(c => c.CantidadMateriales)
                        .ToList();

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = reporteColores;

                    MessageBox.Show("Reporte de colores generado exitosamente.", "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No hay datos suficientes para calcular los colores.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }

        // --- BOTÓN MENÚ (Abre MenuInicio y cierra Reportes) ---
       
    }
}