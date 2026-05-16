import Navbar from './navbar.js';

console.log('Main.js carregado');

const CART_KEY = "pi_cart_v1";
const PAYMENT_KEY = "pi_payment_method_v1";
const CLIENT_KEY = "pi_cliente_v1";
let cartPageInitialized = false;
let checkoutPageInitialized = false;

function safeCall(fn) {
  try {
    fn();
  } catch (error) {
    console.error(error);
  }
}

function bootstrap() {
  console.log('DOM pronto, carregando navbar');
  safeCall(() => {
    const navbar = new Navbar();
    console.log('Navbar criado:', navbar);
    navbar.mount('#navbar-container');
  });

  safeCall(initCarousel);
  safeCall(initEntityModal);
  safeCall(initCart);

  const cartPage = document.getElementById("cart-page");
  const cartItems = document.getElementById("cart-items");
  if (cartPage || cartItems) safeCall(initCartPage);

  const checkoutPage = document.getElementById("checkout-page");
  const checkoutForm = document.getElementById("checkout-form");
  if (checkoutPage || checkoutForm) safeCall(initCheckoutPage);

  const params = new URLSearchParams(window.location.search);
  if (params.has("pedido")) {
    clearCart();
    clearPaymentMethod();
    updateCartCount();
  }
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", bootstrap);
} else {
  bootstrap();
}

function initCarousel() {
  const carousels = document.querySelectorAll(".carousel");

  carousels.forEach((carousel) => {
    const viewport = carousel.querySelector(".carousel__viewport");
    const prevBtn = carousel.querySelector(".carousel__control--prev");
    const nextBtn = carousel.querySelector(".carousel__control--next");

    if (!viewport || !prevBtn || !nextBtn) return;

    const scrollAmount = 260;

    prevBtn.addEventListener("click", () => {
      viewport.scrollBy({
        left: -scrollAmount,
        behavior: "smooth",
      });
    });

    nextBtn.addEventListener("click", () => {
      viewport.scrollBy({
        left: scrollAmount,
        behavior: "smooth",
      });
    });
  });
}

function safeJsonParse(value, fallback) {
  try {
    return JSON.parse(value);
  } catch {
    return fallback;
  }
}

function readCart() {
  try {
    const raw = localStorage.getItem(CART_KEY);
    const parsed = safeJsonParse(raw || "[]", []);
    let list = parsed;
    if (typeof list === "string") list = safeJsonParse(list, []);
    if (list && typeof list === "object" && !Array.isArray(list) && Array.isArray(list.items)) list = list.items;
    if (!Array.isArray(list)) return [];

    return list
      .map((i) => {
        const entity = String(i?.entity ?? i?.Entity ?? "").trim().toLowerCase();
        const id = Number(i?.id ?? i?.Id);
        const quantity = Number(i?.quantity ?? i?.Quantity);
        const name = String(i?.name ?? i?.Name ?? "").trim();
        const summary = String(i?.summary ?? i?.Summary ?? "").trim();
        const image = String(i?.image ?? i?.Image ?? "").trim();
        const price = Number(i?.price ?? i?.Price);

        return {
          entity,
          id: Number.isFinite(id) ? id : 0,
          quantity: Number.isFinite(quantity) ? quantity : 1,
          name,
          summary,
          image,
          price: Number.isFinite(price) ? price : 0,
        };
      })
      .filter((i) => Boolean(i.name || i.image || i.id))
      .map((i) => ({
        ...i,
        quantity: Number.isFinite(i.quantity) && i.quantity > 0 ? Math.min(99, Math.floor(i.quantity)) : 1,
        price: Number.isFinite(i.price) && i.price >= 0 ? i.price : 0,
      }));
  } catch {
    return [];
  }
}

function writeCart(cart) {
  try {
    localStorage.setItem(CART_KEY, JSON.stringify(cart || []));
  } catch { }
}

function clearCart() {
  try {
    localStorage.removeItem(CART_KEY);
  } catch { }
}

function readPaymentMethod() {
  try {
    return String(localStorage.getItem(PAYMENT_KEY) || "").trim();
  } catch {
    return "";
  }
}

function writePaymentMethod(value) {
  try {
    localStorage.setItem(PAYMENT_KEY, String(value || "").trim());
  } catch { }
}

function clearPaymentMethod() {
  try {
    localStorage.removeItem(PAYMENT_KEY);
  } catch { }
}

function normalizeDocumento(value) {
  return String(value || "").replace(/\D+/g, "").trim();
}

function writeClient(client) {
  try {
    localStorage.setItem(CLIENT_KEY, JSON.stringify(client || null));
  } catch { }
}

function cartCount(cart) {
  return (cart || []).reduce((acc, i) => acc + (Number(i.quantity) || 0), 0);
}

function updateCartCount() {
  const badge = document.getElementById("cart-count");
  if (!badge) return;
  const count = cartCount(readCart());
  badge.textContent = String(count);
  badge.style.display = count > 0 ? "flex" : "none";
}

function addToCart(item) {
  const entity = String(item?.entity ?? "").trim().toLowerCase();
  const id = Number(item?.id);
  const quantity = Number(item?.quantity);
  if (!(entity === "pizza" || entity === "bebida")) return;
  if (!Number.isFinite(id) || id <= 0) return;
  const qty = Number.isFinite(quantity) && quantity > 0 ? Math.min(99, Math.floor(quantity)) : 1;

  const cart = readCart();
  const existing = cart.find((x) => x.entity === entity && Number(x.id) === id);
  if (existing) {
    existing.quantity = Math.min(99, (Number(existing.quantity) || 0) + qty);
  } else {
    cart.push({
      entity,
      id,
      quantity: qty,
      name: String(item?.name ?? "").trim(),
      summary: String(item?.summary ?? "").trim(),
      image: String(item?.image ?? "").trim(),
      price: Number.isFinite(Number(item?.price)) ? Number(item.price) : 0,
    });
  }
  writeCart(cart);
}

function setItemQuantity(entity, id, quantity) {
  const cart = readCart();
  const e = String(entity || "").trim().toLowerCase();
  const i = Number(id);
  const q = Math.floor(Number(quantity));
  const idx = cart.findIndex((x) => x.entity === e && Number(x.id) === i);
  if (idx === -1) return;
  if (!Number.isFinite(q) || q <= 0) {
    cart.splice(idx, 1);
  } else {
    cart[idx].quantity = Math.min(99, q);
  }
  writeCart(cart);
}

function removeItem(entity, id) {
  const cart = readCart();
  const e = String(entity || "").trim().toLowerCase();
  const i = Number(id);
  const next = cart.filter((x) => !(x.entity === e && Number(x.id) === i));
  writeCart(next);
}

function cartTotals(cart) {
  const subtotal = (cart || []).reduce((acc, i) => acc + (Number(i.price) || 0) * (Number(i.quantity) || 0), 0);
  const fee = 0;
  const total = subtotal + fee;
  return { subtotal, fee, total };
}

function initCart() {
  updateCartCount();

  document.addEventListener("click", (event) => {
    const btn = event.target.closest(".js-add-to-cart");
    if (!btn) return;
    event.preventDefault();
    event.stopPropagation();

    addToCart({
      entity: btn.dataset.entity,
      id: btn.dataset.id,
      quantity: 1,
      name: btn.dataset.name,
      summary: btn.dataset.summary,
      image: btn.dataset.image,
      price: btn.dataset.price,
    });
    updateCartCount();
  }, true);
}

function initCartPage() {
  if (cartPageInitialized) {
    renderCart();
    return;
  }
  cartPageInitialized = true;

  const paymentSelect = document.getElementById("cart-payment");
  if (paymentSelect) paymentSelect.value = readPaymentMethod();

  document.addEventListener("click", (event) => {
    const clearBtn = event.target.closest(".js-cart-clear");
    if (clearBtn) {
      event.preventDefault();
      clearCart();
      clearPaymentMethod();
      if (paymentSelect) paymentSelect.value = "";
      updateCartCount();
      renderCart();
      return;
    }

    const qtyBtn = event.target.closest(".js-cart-qty");
    if (qtyBtn) {
      event.preventDefault();
      const entity = qtyBtn.dataset.entity;
      const id = qtyBtn.dataset.id;
      const delta = Number(qtyBtn.dataset.delta);
      const cart = readCart();
      const existing = cart.find((x) => x.entity === String(entity || "").toLowerCase() && String(x.id) === String(id));
      const nextQty = (Number(existing?.quantity) || 0) + (Number.isFinite(delta) ? delta : 0);
      setItemQuantity(entity, id, nextQty);
      updateCartCount();
      renderCart();
      return;
    }

    const removeBtn = event.target.closest(".js-cart-remove");
    if (removeBtn) {
      event.preventDefault();
      removeItem(removeBtn.dataset.entity, removeBtn.dataset.id);
      updateCartCount();
      renderCart();
      return;
    }

    const checkoutBtn = event.target.closest(".js-go-checkout");
    if (checkoutBtn) {
      event.preventDefault();
      const cart = readCart();
      if (cart.length === 0) return;
      const payment = paymentSelect ? String(paymentSelect.value || "").trim() : "";
      if (!payment) {
        if (paymentSelect) paymentSelect.focus();
        return;
      }
      writePaymentMethod(payment);
      const url = String(checkoutBtn.dataset.checkoutUrl || "").trim();
      if (url) window.location.href = url;
    }
  }, true);

  if (paymentSelect) {
    paymentSelect.addEventListener("change", () => {
      writePaymentMethod(paymentSelect.value);
    });
  }

  renderCart();
}

function renderCart() {
  const container = document.getElementById("cart-items");
  const empty = document.getElementById("cart-empty");
  const subtotalEl = document.getElementById("cart-subtotal");
  const feeEl = document.getElementById("cart-fee");
  const totalEl = document.getElementById("cart-total");

  if (!container) return;

  const cart = readCart();
  if (empty) empty.style.display = cart.length === 0 ? "flex" : "none";
  container.innerHTML = "";

  const totals = cartTotals(cart);
  if (subtotalEl) subtotalEl.textContent = formatCurrency(totals.subtotal) || "R$ 0,00";
  if (feeEl) feeEl.textContent = formatCurrency(totals.fee) || "R$ 0,00";
  if (totalEl) totalEl.textContent = formatCurrency(totals.total) || "R$ 0,00";

  if (cart.length === 0) return;

  container.innerHTML = cart
    .map((item) => {
      const name = escapeHtml(item.name || "Item");
      const qty = Number(item.quantity) || 1;
      const image = String(item.image || "").trim();
      const imgHtml = image
        ? `<img src="${escapeHtml(image)}" alt="${name}" style="width:56px;height:56px;border-radius:12px;object-fit:cover;flex:0 0 auto;" />`
        : `<div style="width:56px;height:56px;border-radius:12px;background:#f3f4f6;display:flex;align-items:center;justify-content:center;flex:0 0 auto;">🛒</div>`;

      return `
        <div class="cart-item">
          <div style="display:flex;align-items:center;gap:12px;min-width:0;">
            ${imgHtml}
            <div class="cart-item__info" style="min-width:0;">
              <div class="cart-item__name">${name}</div>
              <div class="cart-item__price-unit">Qtd: ${escapeHtml(qty)}</div>
            </div>
          </div>
        </div>
      `;
    })
    .join("");
}

function initCheckoutPage() {
  if (checkoutPageInitialized) return;
  checkoutPageInitialized = true;

  const form = document.getElementById("checkout-form");
  const cartInput = document.getElementById("checkout-cartJson");
  const paymentInput = document.getElementById("checkout-formaPagamento");

  const cart = readCart();
  if (cart.length === 0) {
    window.location.href = "/Cart";
    return;
  }

  const payment = readPaymentMethod();
  if (!payment) {
    window.location.href = "/Cart";
    return;
  }

  if (paymentInput) paymentInput.value = payment;

  if (form) {
    form.addEventListener("submit", () => {
      const nomeEl = document.getElementById("cliente-nome");
      const telefoneEl = document.getElementById("cliente-telefone");
      const emailEl = document.getElementById("cliente-email");
      const cpfEl = document.getElementById("cliente-cpf");

      const cpfRaw = String(cpfEl?.value || "").trim();
      const cpf = normalizeDocumento(cpfRaw);
      if (cpf) {
        writeClient({
          nome: String(nomeEl?.value || "").trim(),
          telefone: String(telefoneEl?.value || "").trim(),
          email: String(emailEl?.value || "").trim(),
          cpfCnpj: cpf,
          cpfCnpjRaw: cpfRaw,
          updatedAt: new Date().toISOString(),
        });
      }

      if (cartInput) cartInput.value = JSON.stringify(readCart().map((i) => ({
        entity: i.entity,
        id: i.id,
        quantity: i.quantity,
      })));
      if (paymentInput) paymentInput.value = readPaymentMethod();
    });
  }
}

function initEntityModal() {
  const modal = ensureEntityModal();

  document.addEventListener("click", async (event) => {
    if (event.target.closest(".js-add-to-cart")) return;
    const trigger = event.target.closest(".js-view-entity");
    if (!trigger) return;

    const entity = (trigger.dataset.entity || "").trim();
    const url = (trigger.dataset.url || "").trim();

    if (!entity || !url) return;

    event.preventDefault();

    modal.open();
    modal.setTitle("Carregando...");
    modal.setBody(renderLoading());

    try {
      const response = await fetch(url, {
        method: "GET",
        headers: { Accept: "application/json" },
      });

      if (!response.ok) throw new Error(`HTTP ${response.status}`);

      const data = await response.json();
      const content = renderEntity(entity, data);
      modal.setTitle(content.title);
      modal.setBody(content.bodyHtml);
    } catch {
      modal.setTitle("Erro");
      modal.setBody(renderError());
    }
  }, true);

  document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") return;
    if (!modal.isOpen()) return;
    modal.close();
  });
}

function ensureEntityModal() {
  let root = document.getElementById("entity-modal");
  if (!root) {
    root = document.createElement("div");
    root.id = "entity-modal";
    root.className = "modal";
    root.setAttribute("aria-hidden", "true");
    root.innerHTML = `
      <div class="modal__backdrop" data-modal-close></div>
      <div class="modal__panel" role="dialog" aria-modal="true" aria-labelledby="entity-modal-title">
        <div class="modal__header">
          <h3 id="entity-modal-title" class="modal__title"></h3>
          <button type="button" class="btn btn--ghost btn--sm" data-modal-close>Fechar</button>
        </div>
        <div class="modal__body" id="entity-modal-body"></div>
      </div>
    `;
    document.body.appendChild(root);
  }

  const titleEl = root.querySelector(".modal__title");
  const bodyEl = root.querySelector("#entity-modal-body");

  const closeButtons = root.querySelectorAll("[data-modal-close]");
  closeButtons.forEach((btn) => {
    btn.addEventListener("click", () => {
      root.classList.remove("modal--open");
      root.setAttribute("aria-hidden", "true");
    });
  });

  root.addEventListener("click", (event) => {
    const panel = root.querySelector(".modal__panel");
    if (!panel) return;
    if (panel.contains(event.target)) return;
    root.classList.remove("modal--open");
    root.setAttribute("aria-hidden", "true");
  });

  return {
    open() {
      root.classList.add("modal--open");
      root.setAttribute("aria-hidden", "false");
    },
    close() {
      root.classList.remove("modal--open");
      root.setAttribute("aria-hidden", "true");
    },
    isOpen() {
      return root.classList.contains("modal--open");
    },
    setTitle(text) {
      if (titleEl) titleEl.textContent = text || "";
    },
    setBody(html) {
      if (bodyEl) bodyEl.innerHTML = html || "";
    },
  };
}

function renderEntity(entity, data) {
  const entityKey = (entity || "").toLowerCase();

  if (entityKey === "pizza") {
    return {
      title: safeText(data?.nome || "Pizza"),
      bodyHtml: renderKeyValueBody({
        image: data?.image,
        rows: [
          ["Nome", data?.nome],
          ["Categoria", data?.categoria],
          ["Sabor", data?.sabor],
          ["Descrição", data?.descricao],
          ["Preço", formatCurrency(data?.preco)],
        ],
      }),
    };
  }

  if (entityKey === "bebida") {
    return {
      title: safeText(data?.nome || "Bebida"),
      bodyHtml: renderKeyValueBody({
        image: data?.image,
        rows: [
          ["Nome", data?.nome],
          ["Categoria", data?.categoria],
          ["Sabor", data?.sabor],
          ["Descrição", data?.descricao],
          ["Preço", formatCurrency(data?.preco)],
        ],
      }),
    };
  }

  if (entityKey === "evento") {
    const dateText = formatDate(data?.dataEvento);
    const subtitleParts = [];
    if (dateText) subtitleParts.push(dateText);
    if (data?.local) subtitleParts.push(data.local);
    const subtitle = subtitleParts.join(" • ");

    return {
      title: safeText(data?.nome || "Evento"),
      bodyHtml: renderKeyValueBody({
        image: data?.image,
        subtitle,
        rows: [
          ["Nome", data?.nome],
          ["Data", dateText],
          ["Local", data?.local],
          ["Descrição", data?.descricao],
        ],
      }),
    };
  }

  if (entityKey === "usuario") {
    return {
      title: safeText(data?.nome || "Usuário"),
      bodyHtml: renderKeyValueBody({
        rows: [
          ["Nome", data?.nome],
          ["Email", data?.email],
          ["Tipo", data?.tipo],
          ["Data Criação", formatDateTime(data?.dataCriacao)],
        ],
      }),
    };
  }

  return { title: "Detalhes", bodyHtml: renderError() };
}

function renderKeyValueBody({ image, subtitle, rows }) {
  const safeRows = (rows || [])
    .map(([k, v]) => [safeText(k), safeText(v)])
    .filter(([, v]) => v.length > 0);

  const imageHtml = image
    ? `<img class="entity-modal__image" src="${escapeHtml(image)}" alt="" />`
    : "";

  const subtitleHtml = subtitle
    ? `<div class="entity-modal__subtitle">${escapeHtml(subtitle)}</div>`
    : "";

  const rowsHtml = safeRows
    .map(([k, v]) => `<div class="kv__k">${escapeHtml(k)}</div><div class="kv__v">${escapeHtml(v)}</div>`)
    .join("");

  return `
    ${imageHtml}
    ${subtitleHtml}
    <div class="kv">
      ${rowsHtml}
    </div>
  `;
}

function renderLoading() {
  return `<div class="empty-state"><h3>Carregando...</h3><p>Aguarde um instante.</p></div>`;
}

function renderError() {
  return `<div class="empty-state"><h3>Não foi possível carregar</h3><p>Tente novamente.</p></div>`;
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function safeText(value) {
  return String(value ?? "").trim();
}

function formatCurrency(value) {
  const number = Number(value);
  if (!Number.isFinite(number)) return "";
  return number.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

function formatDate(value) {
  if (!value) return "";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "";
  return date.toLocaleDateString("pt-BR");
}

function formatDateTime(value) {
  if (!value) return "";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "";
  return date.toLocaleString("pt-BR");
}
