(function () {
    const KLJUC = 'lageros-tema';

    function primijeni(tema) {
        const sustavTamni = window.matchMedia('(prefers-color-scheme: dark)').matches;
        const stvarna = tema === 'system' ? (sustavTamni ? 'dark' : 'light') : tema;
        document.documentElement.setAttribute('data-bs-theme', stvarna);
        document.documentElement.setAttribute('data-tema', tema);
    }

    // Odmah, prije iscrtavanja stranice (nema "bljeska" krive teme)
    primijeni(localStorage.getItem(KLJUC) || 'system');

    // Ako je odabran "sustav" i korisnik promijeni temu u OS-u
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function () {
        if ((localStorage.getItem(KLJUC) || 'system') === 'system') primijeni('system');
    });

    document.addEventListener('DOMContentLoaded', function () {
        const gumbi = document.querySelectorAll('[data-tema-btn]');

        function oznaci() {
            const trenutna = localStorage.getItem(KLJUC) || 'system';
            gumbi.forEach(g => g.classList.toggle('active', g.dataset.temaBtn === trenutna));
        }

        gumbi.forEach(g => g.addEventListener('click', function () {
            localStorage.setItem(KLJUC, g.dataset.temaBtn);
            primijeni(g.dataset.temaBtn);
            oznaci();
        }));

        oznaci();
    });
    document.addEventListener('submit', function (e) {
        const f = e.target.closest('.js-potvrda');
        if (f && !confirm(f.dataset.poruka)) e.preventDefault();
    });
})();