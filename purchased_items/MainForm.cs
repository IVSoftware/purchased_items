using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace purchased_items
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if (!DesignMode)
			{
				// Bind the lists to the UI element to display them.
				dataGridView1.DataSource = PurchasesByUser;
				dataGridView2.DataSource = PurchasedByItem;
				initList1();
				dataGridView1.Columns[nameof(UserPurchase.Name)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				dataGridView2.Columns[nameof(PurchasedItem.Description)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				dataGridView2.Columns[nameof(PurchasedItem.Count)].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
				dataGridView1.AllowUserToAddRows = false;
				dataGridView2.AllowUserToAddRows = false;
			}
		}
		internal BindingList<UserPurchase> PurchasesByUser { get; } = new BindingList<UserPurchase>();
		internal BindingList<PurchasedItem> PurchasedByItem { get; } = new BindingList<PurchasedItem>();

		private void initList1()
		{
			XElement xsource = XElement.Parse(source);
			XElement[] xusers = xsource.Elements("user").ToArray();
            foreach (XElement xuser in xusers)
            {
				XElement[] xpurchases = xuser.Element("items").Elements("item").ToArray();
                foreach (XElement xpurchase in xpurchases)
                {
					var xname = xuser.Element("name");
					// Every time you add a record to the list, it appears in the View automatically.
					PurchasesByUser.Add(new UserPurchase { Name = (string)xname, Purchase = (string)xpurchase });
                }
            }
			// Make what is essentially a list *of* lists using System.Linq.GroupBy
			var groups = 
				PurchasesByUser.GroupBy(purchase => purchase.Purchase);
            foreach (var group in groups)
            {
				// Decoding the Group:
                Debug.WriteLine(
					$"The group '{group.Key}' holds {group.Count()} items");
				Debug.WriteLine(
					$"The customers are { string.Join(",", group.ToList().Select(item=>item.Name))}");
				var byItem = new PurchasedItem { Description = group.Key, Count = group.Count() };
				PurchasedByItem.Add(byItem);
			}
		}

        const string source =
			@"<purchases>
				<user>
					<name>John Doe</name>
					<items>
						<item>Shoes</item>
						<item>T-Shirts</item>
						<item>Jeans</item>
					</items>
				</user>
				<user>
					<name>Rick Astley</name>
					<items>
						<item>Baseball bat</item>
						<item>Sandwich</item>
						<item>T-Shirts</item>
						<item>Shoes</item>
					</items>
				</user>
				<user>
					<name>Jane Doe</name>
					<items>
						<item>Shoes</item>
						<item>T-Shirts</item>
					</items>
				</user>
				<user>
					<name>Anonymous</name>
					<items>
						<item>Laptop</item>
					</items>
				</user>
				<user>
					<name>Joe Mm.</name>
					<items>
						<item>Milk</item>
						<item>Shoes</item>
					</items>
				</user>
			</purchases>";
	}
	internal class UserPurchase
	{		
		public string Name { get; internal set; } // Internal makes item ReadOnly in view
		public string Purchase { get; internal set; }
	}


	internal class PurchasedItem
    {
        public string Description { get; internal set; }
        public int Count { get; internal set; }
    }
}
