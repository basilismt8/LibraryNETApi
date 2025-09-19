// wwwroot/js/books.js
// Adds row selection, filtering, and client-side pagination.

document.addEventListener("DOMContentLoaded", function () {
    const table = document.getElementById('booksTable');
    if (!table) return; // Safety: only run on Books page

    const tbody = table.querySelector('tbody');
    const selectAll = document.getElementById('selectAll');
    const btnEdit = document.getElementById('btnEdit');
    const btnDelete = document.getElementById('btnDelete');
    const status = document.getElementById('tableStatus');

    // Pagination controls (existing in markup)
    const pageSizeSelect = document.getElementById('pageSize');
    const prevPageBtn = document.getElementById('prevPage');
    const nextPageBtn = document.getElementById('nextPage');
    const pageDisplay = document.getElementById('currentPageDisplay');

    let currentPage = 1;
    let pageSize = parseInt(pageSizeSelect?.value || '10', 10);

    // Initialize filter match flag for all rows
    tbody.querySelectorAll('tr').forEach(r => r.dataset.filterMatch = '1');

    function rowCheckboxes() {
        return table.querySelectorAll('tbody .row-select');
    }

    function updateToolbar() {
        const selected = table.querySelectorAll('tbody .row-select:checked').length;
        btnEdit.disabled = selected !== 1; // enable edit only when single selected
        btnDelete.disabled = selected === 0;
    }

    function toggleRowActive(tr, active) {
        if (active) tr.classList.add('table-active'); else tr.classList.remove('table-active');
    }

    // Select all (applies to all rows, not just current row)
    if (selectAll) {
        selectAll.addEventListener('change', function () {
            rowCheckboxes().forEach(cb => {
                cb.checked = selectAll.checked;
                toggleRowActive(cb.closest('tr'), cb.checked);
            });
            updateToolbar();
        });
    }

    table.addEventListener('change', function (e) {
        if (e.target.classList.contains('row-select')) {
            const cb = e.target;
            toggleRowActive(cb.closest('tr'), cb.checked);
            const all = rowCheckboxes();
            selectAll.indeterminate = !selectAll.checked && [...all].some(c => c.checked);
            selectAll.checked = [...all].every(c => c.checked);
            updateToolbar();
        }
    });

    // Filtering
    const filterInputs = document.querySelectorAll('thead tr.filters input[data-field]');
    filterInputs.forEach(inp => inp.addEventListener('input', applyFilters));

    function applyFilters() {
        const filters = {};
        filterInputs.forEach(i => {
            if (i.value.trim()) filters[i.dataset.field] = i.value.trim().toLowerCase(); else delete filters[i.dataset.field];
        });
        tbody.querySelectorAll('tr').forEach(tr => {
            let match = true;
            for (const field in filters) {
                const cell = tr.querySelector('.col-' + field);
                if (!cell || !cell.textContent.toLowerCase().includes(filters[field])) { match = false; break; }
            }
            tr.dataset.filterMatch = match ? '1' : '0';
            if (!match) { // clear selection if filtered out
                const cb = tr.querySelector('.row-select');
                if (cb) { cb.checked = false; toggleRowActive(tr, false); }
            }
        });
        currentPage = 1; // Reset to first page after filter change
        render();
        updateToolbar();
    }

    function getMatchedRows() {
        return [...tbody.querySelectorAll('tr')].filter(r => r.dataset.filterMatch === '1');
    }

    function render() {
        const matched = getMatchedRows();
        const totalMatched = matched.length;
        const totalAll = tbody.querySelectorAll('tr').length;
        const totalPages = Math.max(1, Math.ceil(totalMatched / pageSize));
        if (currentPage > totalPages) currentPage = totalPages;

        // Hide all rows first
        tbody.querySelectorAll('tr').forEach(r => r.style.display = 'none');

        const start = (currentPage - 1) * pageSize;
        const end = start + pageSize;
        matched.forEach((row, idx) => {
            if (idx >= start && idx < end) row.style.display = '';
        });

        // Update status + pager controls
        if (status) status.textContent = `${totalMatched} of ${totalAll} item${totalMatched === 1 ? '' : 's'}`;
        if (pageDisplay) pageDisplay.textContent = `${totalMatched === 0 ? 0 : currentPage} / ${Math.max(1, totalPages)}`;
        if (prevPageBtn) prevPageBtn.disabled = currentPage <= 1;
        if (nextPageBtn) nextPageBtn.disabled = currentPage >= totalPages;
    }

    // Pagination events
    if (pageSizeSelect) {
        pageSizeSelect.addEventListener('change', () => {
            pageSize = parseInt(pageSizeSelect.value, 10);
            currentPage = 1;
            render();
        });
    }
    if (prevPageBtn) {
        prevPageBtn.addEventListener('click', () => {
            if (currentPage > 1) { currentPage--; render(); }
        });
    }
    if (nextPageBtn) {
        nextPageBtn.addEventListener('click', () => {
            const totalMatched = getMatchedRows().length;
            const totalPages = Math.max(1, Math.ceil(totalMatched / pageSize));
            if (currentPage < totalPages) { currentPage++; render(); }
        });
    }

    // Initial paint
    render();
});
