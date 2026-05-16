import Navbar from './navbar.js';

console.log('Main.js carregado');

document.addEventListener('DOMContentLoaded', () => {
  console.log('DOM pronto, carregando navbar');
  const navbar = new Navbar();
  console.log('Navbar criado:', navbar);
  navbar.mount('#navbar-container');

  initCarousel();
  // updateCartCount();
  const cartPage = document.getElementById("cart-page");
  if (cartPage) {
    // renderCart();
  }
});

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