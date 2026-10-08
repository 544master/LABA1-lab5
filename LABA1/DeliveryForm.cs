using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using laba1.Models;
using laba1.Services;

namespace laba1.Forms
{
    public class DeliveryForm : Form
    {
        private DeliveryManager deliveryManager;
        private TextBox customerNameTextBox;
        private TextBox addressTextBox;
        private DateTimePicker deliveryDatePicker;
        private ComboBox statusComboBox;
        private Button addDeliveryButton;
        private Button removeDeliveryButton;
        private Button updateStatusButton;
        private Button editDeliveryButton;
        private ListBox deliveriesListBox;

        private static readonly Dictionary<string, DeliveryStatus> StatusByDisplayText = new()
        {
            { "Новый", DeliveryStatus.Новый },
            { "В пути", DeliveryStatus.В_пути },
            { "Доставлен", DeliveryStatus.Доставлен }
        };

        public DeliveryForm()
        {
            this.Text = "Управление доставкой";
            this.Width = 600;
            this.Height = 500;

            customerNameTextBox = new TextBox
            {
                Location = new Point(10, 10),
                Width = 150,
                PlaceholderText = "Имя клиента"
            };

            addressTextBox = new TextBox
            {
                Location = new Point(170, 10),
                Width = 200,
                PlaceholderText = "Адрес"
            };

            deliveryDatePicker = new DateTimePicker
            {
                Location = new Point(380, 10)
            };

            statusComboBox = new ComboBox
            {
                Location = new Point(10, 40),
                Width = 100,
                Items = { "Новый", "В пути", "Доставлен" }
            };

            addDeliveryButton = new Button
            {
                Location = new Point(10, 70),
                Text = "Добавить",
                Width = 100
            };
            addDeliveryButton.Click += AddDeliveryButton_Click;

            removeDeliveryButton = new Button
            {
                Location = new Point(120, 70),
                Text = "Удалить",
                Width = 100
            };
            removeDeliveryButton.Click += RemoveDeliveryButton_Click;

            updateStatusButton = new Button
            {
                Location = new Point(220, 70),
                Text = "Обновить статус",
                Width = 120
            };
            updateStatusButton.Click += UpdateStatusButton_Click;

            editDeliveryButton = new Button
            {
                Location = new Point(350, 70),
                Text = "Редактировать",
                Width = 110
            };
            editDeliveryButton.Click += EditDeliveryButton_Click;

            deliveriesListBox = new ListBox
            {
                Location = new Point(10, 100),
                Width = 560,
                Height = 250
            };
            deliveriesListBox.SelectedIndexChanged += DeliveriesListBox_SelectedIndexChanged;

            this.Controls.Add(customerNameTextBox);
            this.Controls.Add(addressTextBox);
            this.Controls.Add(deliveryDatePicker);
            this.Controls.Add(statusComboBox);
            this.Controls.Add(addDeliveryButton);
            this.Controls.Add(removeDeliveryButton);
            this.Controls.Add(updateStatusButton);
            this.Controls.Add(editDeliveryButton);
            this.Controls.Add(deliveriesListBox);

            deliveryManager = new DeliveryManager();
            UpdateDeliveriesList();
        }

        private void UpdateDeliveriesList()
        {
            deliveriesListBox.Items.Clear();
            foreach (var delivery in deliveryManager.Deliveries)
            {
                deliveriesListBox.Items.Add($"{delivery.CustomerName} - {delivery.Address} ({delivery.Status})");
            }
        }

        private void AddDeliveryButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(customerNameTextBox.Text) ||
                string.IsNullOrEmpty(addressTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (deliveryDatePicker.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Дата доставки не может быть в прошлом!");
                return;
            }

            DateTime deliveryDate = deliveryDatePicker.Value;
            Delivery newDelivery = new Delivery(customerNameTextBox.Text, addressTextBox.Text, deliveryDate);

            try
            {
                deliveryManager.AddDelivery(newDelivery);
                customerNameTextBox.Clear();
                addressTextBox.Clear();
                UpdateDeliveriesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveDeliveryButton_Click(object sender, EventArgs e)
        {
            int index = deliveriesListBox.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("Выберите доставку для удаления!");
                return;
            }

            var deliveryToRemove = deliveryManager.Deliveries[index];

            try
            {
                deliveryManager.RemoveDelivery(deliveryToRemove);
                UpdateDeliveriesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateStatusButton_Click(object sender, EventArgs e)
        {
            int index = deliveriesListBox.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("Выберите доставку для обновления статуса!");
                return;
            }

            if (statusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите новый статус!");
                return;
            }

            var deliveryToUpdate = deliveryManager.Deliveries[index];
            var newStatus = StatusByDisplayText[statusComboBox.SelectedItem.ToString()];

            try
            {
                deliveryManager.UpdateDeliveryStatus(deliveryToUpdate, newStatus);
                UpdateDeliveriesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeliveriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = deliveriesListBox.SelectedIndex;
            if (index == -1) return;

            var delivery = deliveryManager.Deliveries[index];
            customerNameTextBox.Text = delivery.CustomerName;
            addressTextBox.Text = delivery.Address;
            deliveryDatePicker.Value = delivery.DeliveryDate;
        }

        private void EditDeliveryButton_Click(object sender, EventArgs e)
        {
            int index = deliveriesListBox.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("Выберите доставку для редактирования!");
                return;
            }

            if (string.IsNullOrEmpty(customerNameTextBox.Text) ||
                string.IsNullOrEmpty(addressTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (deliveryDatePicker.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Дата доставки не может быть в прошлом!");
                return;
            }

            var delivery = deliveryManager.Deliveries[index];
            delivery.CustomerName = customerNameTextBox.Text;
            delivery.Address = addressTextBox.Text;
            delivery.DeliveryDate = deliveryDatePicker.Value;

            deliveryManager.Save();
            UpdateDeliveriesList();
            MessageBox.Show("Доставка отредактирована!", "Успех",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
