import Navbar from './navbar.js';

console.log('Main.js carregado');

function bootstrap() {
  console.log('DOM pronto, carregando navbar');
  const navbar = new Navbar();
  console.log('Navbar criado:', navbar);
  navbar.mount('#navbar-container');

  initCarousel();
  initEntityModal();
  // updateCartCount();
  const cartPage = document.getElementById("cart-page");
  if (cartPage) {
    // renderCart();
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

function updateCartCount() {
  // TODO: Implementar lógica de atualização do carrinho
}

function renderCart() {
  // TODO: Implementar renderização do carrinho
}

function initEntityModal() {
  const modal = ensureEntityModal();

  document.addEventListener("click", async (event) => {
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
