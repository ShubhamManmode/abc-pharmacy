import { useEffect, useState } from "react";
import {
  addMedicine,
  createSale,
  getMedicines,
  getSales
} from "./api";

function getDaysToExpiry(expiryDate) {
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const expiry = new Date(expiryDate);
  expiry.setHours(0, 0, 0, 0);

  return Math.ceil((expiry - today) / (1000 * 60 * 60 * 24));
}

function getMedicineStatus(medicine) {
  const days = getDaysToExpiry(medicine.expiryDate);

  if (days < 30) {
    return "expiry-warning";
  }

  if (medicine.quantity < 10) {
    return "low-stock";
  }

  return "";
}

function formatDate(value) {
  return new Date(value).toLocaleDateString();
}

function MedicineList({ medicines, onSell }) {
  return (
    <section className="card">
      <div className="section-header">
        <h2>Medicines</h2>
        <span>{medicines.length} item(s)</span>
      </div>

      {medicines.length === 0 ? (
        <div className="empty">No medicines found.</div>
      ) : (
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Brand</th>
                <th>Expiry Date</th>
                <th>Quantity</th>
                <th>Price</th>
                <th>Action</th>
              </tr>
            </thead>

            <tbody>
              {medicines.map((medicine) => (
                <tr
                  key={medicine.id}
                  className={getMedicineStatus(medicine)}
                >
                  <td>{medicine.fullName}</td>
                  <td>{medicine.brand}</td>
                  <td>{formatDate(medicine.expiryDate)}</td>
                  <td>{medicine.quantity}</td>
                  <td>₹{Number(medicine.price).toFixed(2)}</td>
                  <td>
                    <button
                      className="button small"
                      onClick={() => onSell(medicine)}
                      disabled={medicine.quantity === 0}
                    >
                      Sell
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <div className="legend">
        <span className="legend-item">
          <span className="legend-color red" />
          Expiry less than 30 days
        </span>

        <span className="legend-item">
          <span className="legend-color yellow" />
          Quantity less than 10
        </span>
      </div>
    </section>
  );
}

function AddMedicine({ onAdded }) {
  const initialForm = {
    fullName: "",
    notes: "",
    expiryDate: "",
    quantity: "",
    price: "",
    brand: ""
  };

  const [form, setForm] = useState(initialForm);
  const [saving, setSaving] = useState(false);

  function updateField(event) {
    setForm({
      ...form,
      [event.target.name]: event.target.value
    });
  }

  async function submit(event) {
    event.preventDefault();

    try {
      setSaving(true);

      await addMedicine({
        ...form,
        quantity: Number(form.quantity),
        price: Number(form.price),
        expiryDate: form.expiryDate
      });

      setForm(initialForm);
      alert("Medicine added successfully.");
      onAdded();
    } catch (error) {
      alert(error.message);
    } finally {
      setSaving(false);
    }
  }

  return (
    <section className="card">
      <h2>Add Medicine</h2>

      <form className="form-grid" onSubmit={submit}>
        <label>
          Full Name
          <input
            name="fullName"
            value={form.fullName}
            onChange={updateField}
            required
            placeholder="Paracetamol 500mg"
          />
        </label>

        <label>
          Brand
          <input
            name="brand"
            value={form.brand}
            onChange={updateField}
            required
            placeholder="ABC Pharma"
          />
        </label>

        <label>
          Expiry Date
          <input
            type="date"
            name="expiryDate"
            value={form.expiryDate}
            onChange={updateField}
            required
          />
        </label>

        <label>
          Quantity
          <input
            type="number"
            min="0"
            name="quantity"
            value={form.quantity}
            onChange={updateField}
            required
          />
        </label>

        <label>
          Price
          <input
            type="number"
            min="0"
            step="0.01"
            name="price"
            value={form.price}
            onChange={updateField}
            required
          />
        </label>

        <label className="full-width">
          Notes
          <textarea
            name="notes"
            value={form.notes}
            onChange={updateField}
            rows="3"
            placeholder="Optional notes"
          />
        </label>

        <button className="button" disabled={saving}>
          {saving ? "Saving..." : "Add Medicine"}
        </button>
      </form>
    </section>
  );
}

function SellMedicine({ medicine, onSold, onCancel }) {
  const [quantity, setQuantity] = useState(1);
  const [saving, setSaving] = useState(false);

  async function submit(event) {
    event.preventDefault();

    if (quantity <= 0 || quantity > medicine.quantity) {
      alert(`Enter a quantity between 1 and ${medicine.quantity}.`);
      return;
    }

    try {
      setSaving(true);

      await createSale(medicine.id, Number(quantity));

      alert("Sale recorded successfully.");
      onSold();
    } catch (error) {
      alert(error.message);
    } finally {
      setSaving(false);
    }
  }

  return (
    <section className="card">
      <h2>Sell Medicine</h2>

      <div className="sale-summary">
        <strong>{medicine.fullName}</strong>
        <span>Available: {medicine.quantity}</span>
        <span>Price: ₹{Number(medicine.price).toFixed(2)}</span>
      </div>

      <form className="sale-form" onSubmit={submit}>
        <label>
          Quantity to sell
          <input
            type="number"
            min="1"
            max={medicine.quantity}
            value={quantity}
            onChange={(event) => setQuantity(event.target.value)}
          />
        </label>

        <div className="actions">
          <button className="button" disabled={saving}>
            {saving ? "Processing..." : "Confirm Sale"}
          </button>

          <button
            type="button"
            className="button secondary"
            onClick={onCancel}
          >
            Cancel
          </button>
        </div>
      </form>
    </section>
  );
}

function Sales({ sales }) {
  return (
    <section className="card">
      <div className="section-header">
        <h2>Sales History</h2>
        <span>{sales.length} sale(s)</span>
      </div>

      {sales.length === 0 ? (
        <div className="empty">No sales recorded.</div>
      ) : (
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Medicine</th>
                <th>Quantity</th>
                <th>Unit Price</th>
                <th>Total</th>
                <th>Sale Date</th>
              </tr>
            </thead>

            <tbody>
              {sales.map((sale) => (
                <tr key={sale.id}>
                  <td>{sale.medicineName}</td>
                  <td>{sale.quantitySold}</td>
                  <td>₹{Number(sale.unitPrice).toFixed(2)}</td>
                  <td>₹{Number(sale.totalAmount).toFixed(2)}</td>
                  <td>{new Date(sale.saleDate).toLocaleString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}

export default function App() {
  const [activeTab, setActiveTab] = useState("medicines");
  const [medicines, setMedicines] = useState([]);
  const [sales, setSales] = useState([]);
  const [search, setSearch] = useState("");
  const [selectedMedicine, setSelectedMedicine] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  async function loadMedicines(searchValue = search) {
    try {
      setLoading(true);
      setError("");
      const data = await getMedicines(searchValue);
      setMedicines(data);
    } catch (error) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  }

  async function loadSales() {
    try {
      const data = await getSales();
      setSales(data);
    } catch (error) {
      setError(error.message);
    }
  }

  useEffect(() => {
    loadMedicines("");
    loadSales();
  }, []);

  function handleSearch(event) {
    const value = event.target.value;
    setSearch(value);
    loadMedicines(value);
  }

  async function handleAdded() {
    await loadMedicines("");
    setSearch("");
    setActiveTab("medicines");
  }

  async function handleSold() {
    setSelectedMedicine(null);
    await loadMedicines(search);
    await loadSales();
    setActiveTab("medicines");
  }

  return (
    <div className="app">
      <header className="header">
        <div>
          <h1>ABC Pharmacy</h1>
          <p>Medicine & Sales Management</p>
        </div>
      </header>

      <nav className="nav">
        <button
          className={activeTab === "medicines" ? "active" : ""}
          onClick={() => setActiveTab("medicines")}
        >
          Medicines
        </button>

        <button
          className={activeTab === "add" ? "active" : ""}
          onClick={() => setActiveTab("add")}
        >
          Add Medicine
        </button>

        <button
          className={activeTab === "sales" ? "active" : ""}
          onClick={() => {
            setActiveTab("sales");
            loadSales();
          }}
        >
          Sales
        </button>
      </nav>

      <main>
        {error && <div className="error">{error}</div>}

        {activeTab === "medicines" && (
          <>
            <section className="search-box">
              <label htmlFor="search">Search medicine by name</label>
              <input
                id="search"
                value={search}
                onChange={handleSearch}
                placeholder="e.g. Paracetamol"
              />
            </section>

            {loading ? (
              <div className="card">Loading medicines...</div>
            ) : (
              <MedicineList
                medicines={medicines}
                onSell={(medicine) => setSelectedMedicine(medicine)}
              />
            )}

            {selectedMedicine && (
              <SellMedicine
                medicine={selectedMedicine}
                onSold={handleSold}
                onCancel={() => setSelectedMedicine(null)}
              />
            )}
          </>
        )}

        {activeTab === "add" && (
          <AddMedicine onAdded={handleAdded} />
        )}

        {activeTab === "sales" && <Sales sales={sales} />}
      </main>
    </div>
  );
}
