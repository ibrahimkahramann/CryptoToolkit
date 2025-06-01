// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function copyToClipboard(button, text) {
    navigator.clipboard.writeText(text).then(
        function() {
            const originalContent = button.innerHTML;
            button.innerHTML = '<i class="bi bi-check-circle-fill me-1"></i> Kopyalandı';
            button.classList.add('btn-success');
            button.classList.remove('btn-outline-primary');
            
            showToast('Panoya kopyalandı!');
            
            setTimeout(function() {
                button.innerHTML = originalContent;
                button.classList.remove('btn-success');
                button.classList.add('btn-outline-primary');
            }, 2000);
        },
        function() {
            showToast('Kopyalama başarısız oldu!', true);
        }
    );
}

// Show toast notification
function showToast(message, isError = false) {
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'position-fixed bottom-0 end-0 p-3';
        toastContainer.style.zIndex = '1080';
        document.body.appendChild(toastContainer);
    }
    
    const toastId = 'toast-' + Date.now();
    const toast = document.createElement('div');
    toast.id = toastId;
    toast.className = `toast align-items-center ${isError ? 'bg-danger' : 'bg-success'} text-white border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                <i class="bi ${isError ? 'bi-exclamation-circle' : 'bi-check-circle'} me-2"></i>${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    `;
    
    toastContainer.appendChild(toast);
    
    const bsToast = new bootstrap.Toast(toast, {
        autohide: true,
        delay: 3000
    });
    bsToast.show();
    
    toast.addEventListener('hidden.bs.toast', function () {
        toast.remove();
    });
}

document.addEventListener('DOMContentLoaded', function() {
    const triggerTabList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tab"]'));
    triggerTabList.forEach(function(triggerEl) {
        const tabTrigger = new bootstrap.Tab(triggerEl);
        triggerEl.addEventListener('click', function(event) {
            event.preventDefault();
            tabTrigger.show();
        });
    });   
    const forms = document.querySelectorAll('form.needs-validation');
    forms.forEach(form => {
        const methodRadios = form.querySelectorAll('input[name="method"]');
        if (methodRadios.length > 0) {
            form.setAttribute('novalidate', 'true');
        }
        
        form.addEventListener('submit', function(event) {
            if (methodRadios.length > 0) {
                const selectedMethod = form.querySelector('input[name="method"]:checked')?.value;
                
                const aesFields = form.querySelectorAll('#aesFields input, #aesFields textarea');
                const rsaFields = form.querySelectorAll('#rsaFields input, #rsaFields textarea');
                
                if (selectedMethod === 'AES') {
                    aesFields.forEach(field => {
                        field.required = true;
                        field.disabled = false;
                        if (field.dataset.originalName) {
                            field.name = field.dataset.originalName;
                        }
                        field.classList.remove('is-invalid');
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                    rsaFields.forEach(field => {
                        field.required = false;
                        if (field.name && !field.dataset.originalName) {
                            field.dataset.originalName = field.name;
                        }
                        field.removeAttribute('name');
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                } else if (selectedMethod === 'RSA') {
                    rsaFields.forEach(field => {
                        field.required = true;
                        field.disabled = false;
                        if (field.dataset.originalName) {
                            field.name = field.dataset.originalName;
                        }
                        field.classList.remove('is-invalid');
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                    aesFields.forEach(field => {
                        field.required = false;
                        if (field.name && !field.dataset.originalName) {
                            field.dataset.originalName = field.name;
                        }
                        field.removeAttribute('name');
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                }
                
                let isValid = true;
                const visibleFields = form.querySelectorAll('input:not([style*="display: none"]), textarea:not([style*="display: none"])');
                
                visibleFields.forEach(field => {
                    if (field.hasAttribute('name') && field.required && !field.value.trim()) {
                        isValid = false;
                        field.classList.add('is-invalid');
                    } else if (field.hasAttribute('name')) {
                        field.classList.remove('is-invalid');
                    }
                });
                
                if (!isValid) {
                    event.preventDefault();
                    event.stopPropagation();
                    showToast('Lütfen tüm gerekli alanları doldurun.', true);
                    return false;
                }
            } else {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                    showToast('Lütfen tüm gerekli alanları doldurun.', true);
                }
            }
            form.classList.add('was-validated');
        });
    });   
    const methodRadios = document.querySelectorAll('input[name="method"]');
    methodRadios.forEach(radio => {
        radio.addEventListener('change', function() {
            const form = this.closest('form');
            if (form) {
                const aesFields = form.querySelectorAll('#aesFields input, #aesFields textarea');
                const rsaFields = form.querySelectorAll('#rsaFields input, #rsaFields textarea');
                const aesContainer = form.querySelector('#aesFields');
                const rsaContainer = form.querySelector('#rsaFields');
                window.clearValidationErrors();
                
                if (this.value === 'AES') {
                    if (aesContainer) aesContainer.style.display = 'block';
                    if (rsaContainer) rsaContainer.style.display = 'none';
                    
                    aesFields.forEach(field => {
                        field.required = true;
                        field.disabled = false;
                        if (field.dataset.originalName) {
                            field.name = field.dataset.originalName;
                            delete field.dataset.originalName;
                        }
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                    rsaFields.forEach(field => {
                        field.required = false;
                        if (field.name) {
                            field.dataset.originalName = field.name;
                            field.removeAttribute('name');
                        }
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                } else if (this.value === 'RSA') {
                    if (rsaContainer) rsaContainer.style.display = 'block';
                    if (aesContainer) aesContainer.style.display = 'none';
                    
          
                    rsaFields.forEach(field => {
                        field.required = true;
                        field.disabled = false;
               
                        if (field.dataset.originalName) {
                            field.name = field.dataset.originalName;
                            delete field.dataset.originalName;
                        }
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
           
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
          
                    aesFields.forEach(field => {
                        field.required = false;
              
                        if (field.name) {
                            field.dataset.originalName = field.name;
                            field.removeAttribute('name');
                        }
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                 
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                }
                
            
                form.classList.remove('was-validated');
                
               
                const toasts = document.querySelectorAll('.toast');
                toasts.forEach(toast => {
                    const bsToast = bootstrap.Toast.getInstance(toast);
                    if (bsToast) bsToast.hide();
                });
            }
        });
    });    
    const checkedMethod = document.querySelector('input[name="method"]:checked');
    if (checkedMethod) {
      
        window.clearValidationErrors();
        
       
        checkedMethod.dispatchEvent(new Event('change'));
    }
      
    const resultModal = document.getElementById('resultModal');
    if (resultModal) {
        const bsModal = new bootstrap.Modal(resultModal, {
            backdrop: true,
            keyboard: true,
            focus: true
        });
        
        
        const modalTitle = resultModal.querySelector('.modal-title');
        if (modalTitle) {
            const titleText = modalTitle.textContent || modalTitle.innerText;
            if (titleText.includes('Başarılı')) {
                showToast('İşlem başarıyla tamamlandı!');
            }
        }
        
        
        resultModal.addEventListener('hidden.bs.modal', function () {
            // Remove any remaining backdrops
            const backdrops = document.querySelectorAll('.modal-backdrop');
            backdrops.forEach(backdrop => backdrop.remove());
            
        
            document.body.classList.remove('modal-open');
            document.body.style.removeProperty('overflow');
            document.body.style.removeProperty('padding-right');
        });
        
        
        const closeButtons = resultModal.querySelectorAll('[data-bs-dismiss="modal"]');
        closeButtons.forEach(button => {
            button.addEventListener('click', function() {
                setTimeout(() => {
                    
                    const backdrops = document.querySelectorAll('.modal-backdrop');
                    backdrops.forEach(backdrop => backdrop.remove());
                    document.body.classList.remove('modal-open');
                    document.body.style.removeProperty('overflow');
                    document.body.style.removeProperty('padding-right');
                }, 300);
            });
        });
        
      
        bsModal.show();
    }
    const errorAlerts = document.querySelectorAll('.alert-danger');
    const hasResultModal = document.getElementById('resultModal');
    
    if (errorAlerts.length > 0 && !hasResultModal) {
        let hasActualErrors = false;
        errorAlerts.forEach(alert => {
            if (alert.style.display !== 'none' && alert.textContent.trim() !== '') {
                hasActualErrors = true;
            }
        });
        
        if (hasActualErrors) {
            showToast('İşlem sırasında hata oluştu. Lütfen kontrol edin.', true);
        }
    }
    
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    if (tooltipTriggerList.length > 0) {
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
});

window.cleanupModalBackdrop = function() {
    const backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach(backdrop => backdrop.remove());
    document.body.classList.remove('modal-open');
    document.body.style.removeProperty('overflow');
    document.body.style.removeProperty('padding-right');
};

window.clearValidationErrors = function() {
    document.querySelectorAll('.is-invalid, .is-valid').forEach(field => {
        field.classList.remove('is-invalid', 'is-valid');
    });
    

    document.querySelectorAll('.field-validation-error, .validation-summary-errors').forEach(msg => {
        msg.style.display = 'none';
        msg.innerHTML = '';
        msg.classList.remove('field-validation-error', 'validation-summary-errors');
        msg.classList.add('field-validation-valid', 'validation-summary-valid');
    });
    

    document.querySelectorAll('form.was-validated').forEach(form => {
        form.classList.remove('was-validated');
    });
    

    if (typeof $ !== 'undefined' && $.validator) {
        $('form').each(function() {
            const validator = $(this).data('validator');
            if (validator) {
                validator.resetForm();
            }
        });
    }
    
    const toasts = document.querySelectorAll('.toast.bg-danger');
    toasts.forEach(toast => {
        const bsToast = bootstrap.Toast.getInstance(toast);
        if (bsToast) bsToast.hide();
    });
};
