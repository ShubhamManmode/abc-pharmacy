// Change this URL to the URL printed by the ASP.NET Core API.
export const API_BASE_URL = "https://localhost:7001/api";

async function request(url, options = {}) {
  const response = await fetch(`${API_BASE_URL}${url}`, {
    headers: {
      "Content-Type": "application/json",
      ...(options.headers || {})
    },
    ...options
  });

  let data = null;

  try {
    data = await response.json();
  } catch {
    // Empty response body.
  }

  if (!response.ok) {
    throw new Error(data?.message || "Something went wrong.");
  }

  return data;
}

export function getMedicines(search = "") {
  const query = search.trim()
    ? `?search=${encodeURIComponent(search.trim())}`
    : "";

  return request(`/medicines${query}`);
}

export function addMedicine(medicine) {
  return request("/medicines", {
    method: "POST",
    body: JSON.stringify(medicine)
  });
}

export function getSales() {
  return request("/sales");
}

export function createSale(medicineId, quantitySold) {
  return request("/sales", {
    method: "POST",
    body: JSON.stringify({
      medicineId,
      quantitySold
    })
  });
}
