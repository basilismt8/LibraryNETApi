(function(){
    document.addEventListener('DOMContentLoaded', () => {
        const modal = document.getElementById('bookModal');
        if(!modal) return; // safety
        const openNew = document.getElementById('btnNew');
        const openEdit = document.getElementById('btnEdit');
        const closeBtns = [document.getElementById('bookModalClose'), document.getElementById('bookModalCancel')];
        const form = document.getElementById('bookForm');
        const titleEl = document.getElementById('bookModalTitle');
        const saveBtn = document.getElementById('bookModalSave');
        const table = document.getElementById('booksTable');

        // Form fields
        const fldId = document.getElementById('bookId');
        const fldTitle = document.getElementById('bookTitle');
        const fldCopiesAvail = document.getElementById('copiesAvailable');
        const fldTotalCopies = document.getElementById('totalCopies');

        function populateFromRow(row){
            if(!row) return;
            fldId.value = row.dataset.id || '';
            fldTitle.value = row.dataset.title || '';
            fldCopiesAvail.value = row.dataset.copiesAvailable || '';
            fldTotalCopies.value = row.dataset.totalCopies || '';
        }

        function open(mode){
            titleEl.textContent = mode === 'edit' ? 'Edit Book' : 'Add New Book';
            if(mode === 'new') { form.reset(); fldId.value=''; }
            if(mode === 'edit') {
                const selected = table.querySelector('tbody .row-select:checked');
                const row = selected ? selected.closest('tr') : null;
                form.reset();
                populateFromRow(row);
            }
            modal.classList.add('show');
            document.body.style.overflow='hidden';
        }
        function close(){
            modal.classList.remove('show');
            document.body.style.overflow='';
        }
        openNew?.addEventListener('click',()=>open('new'));
        openEdit?.addEventListener('click',()=>open('edit'));
        closeBtns.forEach(b=>b?.addEventListener('click',close));
        modal.addEventListener('click',e=>{ if(e.target===modal) close(); });
        saveBtn.addEventListener('click',()=>{
            // placeholder: gather data example
            const payload = {
                id: fldId.value || null,
                title: fldTitle.value.trim(),
                copiesAvailable: parseInt(fldCopiesAvail.value||'0',10),
                totalCopies: parseInt(fldTotalCopies.value||'0',10)
            };
            console.log('Would submit', payload);
            close();
        });
    });
})();
