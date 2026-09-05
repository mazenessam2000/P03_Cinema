(function () {
    'use strict';

    // Navbar scroll
    const navbar = document.querySelector('.cv-navbar');
    if (navbar) {
        const onScroll = () =>
            navbar.classList.toggle('scrolled', window.scrollY > 20);
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll();
    }

    // Password toggle
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-eye]');
        if (!btn) return;

        const input = document.getElementById(btn.dataset.eye);
        if (!input) return;

        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';

        const icon = btn.querySelector('i');
        if (icon) {
            icon.classList.toggle('fa-eye');
            icon.classList.toggle('fa-eye-slash');
        }
    });

    // Collapsible
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-toggle]');
        if (!btn) return;

        const section = document.getElementById(btn.dataset.toggle);
        if (!section) return;

        const isOpen = section.classList.toggle('open');

        if (btn.dataset.toggleOpenText && btn.dataset.toggleCloseText) {
            btn.textContent = isOpen
                ? btn.dataset.toggleCloseText
                : btn.dataset.toggleOpenText;
        }

        if (!isOpen) {
            section.querySelectorAll('input[data-clear-on-close]')
                .forEach(inp => inp.value = '');
            resetStrengthBar(section);
        }
    });

    // Strength meter
    document.addEventListener('input', function (e) {
        const input = e.target;
        if (!input.dataset.strengthFill) return;

        const fill = document.getElementById(input.dataset.strengthFill);
        const hint = document.getElementById(input.dataset.strengthHint);
        if (!fill) return;

        const val = input.value;
        let score = 0;

        if (val.length >= 8) score++;
        if (/[A-Z]/.test(val) && /[a-z]/.test(val)) score++;
        if (/[0-9]/.test(val)) score++;
        if (/[^A-Za-z0-9]/.test(val)) score++;

        const levels = [
            { pct: '0%', color: 'transparent', text: '' },
            { pct: '25%', color: '#e57309', text: 'Weak' },
            { pct: '50%', color: '#e5c409', text: 'Fair' },
            { pct: '75%', color: '#6be509', text: 'Good' },
            { pct: '100%', color: '#09e550', text: 'Strong 💪' },
        ];

        const level = val.length === 0 ? levels[0] : (levels[score] || levels[1]);

        fill.style.width = level.pct;
        fill.style.background = level.color;

        if (hint) {
            hint.textContent = level.text;
            hint.style.color = level.color;
        }
    });

    function resetStrengthBar(container) {
        container.querySelectorAll('[data-strength-fill]').forEach(inp => {
            const fill = document.getElementById(inp.dataset.strengthFill);
            const hint = document.getElementById(inp.dataset.strengthHint);

            if (fill) {
                fill.style.width = '0%';
                fill.style.background = 'transparent';
            }

            if (hint) {
                hint.textContent = '';
            }
        });
    }

    // Active nav
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.cv-nav-links .nav-link').forEach(link => {
        const href = (link.getAttribute('href') || '').toLowerCase();

        if (
            href &&
            href !== '/' &&
            (currentPath === href || currentPath.startsWith(href + '/'))
        ) {
            link.classList.add('active');
        }
    });

})();