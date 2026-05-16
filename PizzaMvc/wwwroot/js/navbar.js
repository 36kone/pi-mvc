class Navbar {
    constructor(options = {}) {
        this.options = {
            logoSrc: options.logoSrc || '/images/MENDE_LOGO.jpeg_page-0001.jpg',
            logoAlt: options.logoAlt || 'Logo Mendê Pizzaria',
            logoText: options.logoText || 'Mendê Pizzaria',
            navLinks: options.navLinks || [
                { label: 'Início', href: '/#inicio', active: true },
                { label: 'Em destaque', href: '/#destaque' },
                { label: 'Cardápio', href: '/#cardapio' },
                { label: 'Eventos', href: '/#eventos' },
                { label: 'Ajuda', href: '/#ajuda' }
            ],
            cartUrl: options.cartUrl || '/Cart'
        };
        this.element = null;
    }

    render() {
        this.element = document.createElement('header');
        this.element.className = 'topbar';
        this.element.innerHTML = `
            <div class="container topbar__content">
                <div class="logo">
                    <img class="logo__mark" src="${this.options.logoSrc}" alt="${this.options.logoAlt}">
                    <span class="logo__text">${this.options.logoText}</span>
                </div>

                <nav class="nav">
                    ${this.options.navLinks.map(link => `
                        <a href="${link.href}" class="nav__link ${link.active ? 'nav__link--active' : ''}">
                            ${link.label}
                        </a>
                    `).join('')}
                </nav>

                <div class="topbar__actions">
                    <a href="${this.options.cartUrl}" class="cart-btn">
                        <span class="cart-btn__icon">🛒</span>
                        <span class="cart-btn__badge" id="cart-count">0</span>
                    </a>

                </div>
            </div>
        `;

        return this.element;
    }

    mount(selector) {
        const container = document.querySelector(selector);
        if (container) {
            container.appendChild(this.render());
        }
    }

    unmount() {
        if (this.element && this.element.parentNode) {
            this.element.parentNode.removeChild(this.element);
        }
    }
}

export default Navbar;