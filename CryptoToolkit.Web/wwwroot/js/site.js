// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Copy to clipboard functionality
function copyToClipboard(button, text) {
    navigator.clipboard.writeText(text).then(
        function() {
            // Change button text and icon temporarily
            const originalContent = button.innerHTML;
            button.innerHTML = '<i class="bi bi-check-circle-fill me-1"></i> Kopyalandı';
            button.classList.add('btn-success');
            button.classList.remove('btn-outline-primary');
            
            // Show a toast notification
            showToast('Panoya kopyalandı!');
            
            // Reset button after 2 seconds
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
    // Create toast container if it doesn't exist
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'position-fixed bottom-0 end-0 p-3';
        toastContainer.style.zIndex = '1080';
        document.body.appendChild(toastContainer);
    }
    
    // Create toast element
    const toastId = 'toast-' + Date.now();
    const toast = document.createElement('div');
    toast.id = toastId;
    toast.className = `toast align-items-center ${isError ? 'bg-danger' : 'bg-success'} text-white border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    
    // Toast content
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                <i class="bi ${isError ? 'bi-exclamation-circle' : 'bi-check-circle'} me-2"></i>${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    `;
    
    toastContainer.appendChild(toast);
    
    // Initialize and show toast
    const bsToast = new bootstrap.Toast(toast, {
        autohide: true,
        delay: 3000
    });
    bsToast.show();
    
    // Remove toast after it's hidden
    toast.addEventListener('hidden.bs.toast', function () {
        toast.remove();
    });
}

// Initialize tab controls and modals
document.addEventListener('DOMContentLoaded', function() {
    // Initialize tabs
    const triggerTabList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tab"]'));
    triggerTabList.forEach(function(triggerEl) {
        const tabTrigger = new bootstrap.Tab(triggerEl);
        triggerEl.addEventListener('click', function(event) {
            event.preventDefault();
            tabTrigger.show();
        });
    });    // Handle form validation with conditional fields
    const forms = document.querySelectorAll('form.needs-validation');
    forms.forEach(form => {
        // Disable HTML5 validation on forms with conditional fields to prevent conflicts
        const methodRadios = form.querySelectorAll('input[name="method"]');
        if (methodRadios.length > 0) {
            form.setAttribute('novalidate', 'true');
        }
        
        form.addEventListener('submit', function(event) {
            // Handle conditional validation for crypto forms
            if (methodRadios.length > 0) {
                // Get selected method
                const selectedMethod = form.querySelector('input[name="method"]:checked')?.value;
                
                // Get field containers and fields
                const aesFields = form.querySelectorAll('#aesFields input, #aesFields textarea');
                const rsaFields = form.querySelectorAll('#rsaFields input, #rsaFields textarea');
                
                if (selectedMethod === 'AES') {
                    // Enable AES field validation, disable RSA
                    aesFields.forEach(field => {
                        field.required = true;
                        field.disabled = false;
                        // Restore name attribute if it was stored
                        if (field.dataset.originalName) {
                            field.name = field.dataset.originalName;
                        }
                        field.classList.remove('is-invalid');
                        // Remove any validation data attributes
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                    rsaFields.forEach(field => {
                        field.required = false;
                        // Store original name and remove it to prevent submission
                        if (field.name && !field.dataset.originalName) {
                            field.dataset.originalName = field.name;
                        }
                        field.removeAttribute('name');
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        // Remove any validation data attributes
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                } else if (selectedMethod === 'RSA') {
                    // Enable RSA field validation, disable AES
                    rsaFields.forEach(field => {
                        field.required = true;
                        field.disabled = false;
                        // Restore name attribute if it was stored
                        if (field.dataset.originalName) {
                            field.name = field.dataset.originalName;
                        }
                        field.classList.remove('is-invalid');
                        // Remove any validation data attributes
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                    aesFields.forEach(field => {
                        field.required = false;
                        // Store original name and remove it to prevent submission
                        if (field.name && !field.dataset.originalName) {
                            field.dataset.originalName = field.name;
                        }
                        field.removeAttribute('name');
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        // Remove any validation data attributes
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                }
                
                // Custom validation check for required fields in active method
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
                // For forms without conditional fields, use normal validation
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                    showToast('Lütfen tüm gerekli alanları doldurun.', true);
                }
            }
            form.classList.add('was-validated');
        });
    });    // Handle method change for conditional field validation
    const methodRadios = document.querySelectorAll('input[name="method"]');
    methodRadios.forEach(radio => {
        radio.addEventListener('change', function() {
            const form = this.closest('form');
            if (form) {
                const aesFields = form.querySelectorAll('#aesFields input, #aesFields textarea');
                const rsaFields = form.querySelectorAll('#rsaFields input, #rsaFields textarea');
                const aesContainer = form.querySelector('#aesFields');
                const rsaContainer = form.querySelector('#rsaFields');
                  // Clear any existing validation messages
                window.clearValidationErrors();
                
                if (this.value === 'AES') {
                    // Show AES fields and hide RSA fields
                    if (aesContainer) aesContainer.style.display = 'block';
                    if (rsaContainer) rsaContainer.style.display = 'none';
                    
                    // Reset and enable AES fields
                    aesFields.forEach(field => {
                        field.required = true;
                        field.disabled = false;
                        // Restore name attribute if it was stored
                        if (field.dataset.originalName) {
                            field.name = field.dataset.originalName;
                            delete field.dataset.originalName;
                        }
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        // Remove validation data attributes
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                    // Disable RSA fields
                    rsaFields.forEach(field => {
                        field.required = false;
                        // Store original name and remove it
                        if (field.name) {
                            field.dataset.originalName = field.name;
                            field.removeAttribute('name');
                        }
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        // Remove validation data attributes
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                } else if (this.value === 'RSA') {
                    // Show RSA fields and hide AES fields
                    if (rsaContainer) rsaContainer.style.display = 'block';
                    if (aesContainer) aesContainer.style.display = 'none';
                    
                    // Reset and enable RSA fields
                    rsaFields.forEach(field => {
                        field.required = true;
                        field.disabled = false;
                        // Restore name attribute if it was stored
                        if (field.dataset.originalName) {
                            field.name = field.dataset.originalName;
                            delete field.dataset.originalName;
                        }
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        // Remove validation data attributes
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                    // Disable AES fields
                    aesFields.forEach(field => {
                        field.required = false;
                        // Store original name and remove it
                        if (field.name) {
                            field.dataset.originalName = field.name;
                            field.removeAttribute('name');
                        }
                        field.value = '';
                        field.classList.remove('is-invalid', 'is-valid');
                        // Remove validation data attributes
                        field.removeAttribute('data-val');
                        field.removeAttribute('data-val-required');
                    });
                }
                
                // Remove form validation state when method changes
                form.classList.remove('was-validated');
                
                // Clear any toast notifications
                const toasts = document.querySelectorAll('.toast');
                toasts.forEach(toast => {
                    const bsToast = bootstrap.Toast.getInstance(toast);
                    if (bsToast) bsToast.hide();
                });
            }
        });
    });    // Initialize field states on page load
    const checkedMethod = document.querySelector('input[name="method"]:checked');
    if (checkedMethod) {
        // Clear any potential validation errors first
        window.clearValidationErrors();
        
        // Trigger the change event to set up initial field states
        checkedMethod.dispatchEvent(new Event('change'));
    }
      // Initialize and show result modal if exists
    const resultModal = document.getElementById('resultModal');
    if (resultModal) {
        const bsModal = new bootstrap.Modal(resultModal, {
            backdrop: true,
            keyboard: true,
            focus: true
        });
        
        // Show success toast when modal is displayed
        const modalTitle = resultModal.querySelector('.modal-title');
        if (modalTitle) {
            const titleText = modalTitle.textContent || modalTitle.innerText;
            if (titleText.includes('Başarılı')) {
                showToast('İşlem başarıyla tamamlandı!');
            }
        }
        
        // Clean up backdrop on hide
        resultModal.addEventListener('hidden.bs.modal', function () {
            // Remove any remaining backdrops
            const backdrops = document.querySelectorAll('.modal-backdrop');
            backdrops.forEach(backdrop => backdrop.remove());
            
            // Ensure body classes are cleaned up
            document.body.classList.remove('modal-open');
            document.body.style.removeProperty('overflow');
            document.body.style.removeProperty('padding-right');
        });
        
        // Additional cleanup on modal close button click
        const closeButtons = resultModal.querySelectorAll('[data-bs-dismiss="modal"]');
        closeButtons.forEach(button => {
            button.addEventListener('click', function() {
                setTimeout(() => {
                    // Force cleanup after a short delay
                    const backdrops = document.querySelectorAll('.modal-backdrop');
                    backdrops.forEach(backdrop => backdrop.remove());
                    document.body.classList.remove('modal-open');
                    document.body.style.removeProperty('overflow');
                    document.body.style.removeProperty('padding-right');
                }, 300);
            });
        });
        
        // Show the modal
        bsModal.show();
    }
      // Check if there are any ModelState errors displayed on the page
    // Only show error toast if there are actual server-side validation errors AND no success modal
    const errorAlerts = document.querySelectorAll('.alert-danger');
    const hasResultModal = document.getElementById('resultModal');
    
    if (errorAlerts.length > 0 && !hasResultModal) {
        // Only show toast for actual validation errors when there's no success modal
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
    
    // Initialize tooltips if they exist
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    if (tooltipTriggerList.length > 0) {
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
});

// Global modal cleanup function - can be called manually if needed
window.cleanupModalBackdrop = function() {
    const backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach(backdrop => backdrop.remove());
    document.body.classList.remove('modal-open');
    document.body.style.removeProperty('overflow');
    document.body.style.removeProperty('padding-right');
};

// Global function to clear all validation errors
window.clearValidationErrors = function() {
    // Clear HTML5 validation errors
    document.querySelectorAll('.is-invalid, .is-valid').forEach(field => {
        field.classList.remove('is-invalid', 'is-valid');
    });
    
    // Clear validation messages
    document.querySelectorAll('.field-validation-error, .validation-summary-errors').forEach(msg => {
        msg.style.display = 'none';
        msg.innerHTML = '';
        msg.classList.remove('field-validation-error', 'validation-summary-errors');
        msg.classList.add('field-validation-valid', 'validation-summary-valid');
    });
    
    // Clear form validation states
    document.querySelectorAll('form.was-validated').forEach(form => {
        form.classList.remove('was-validated');
    });
    
    // Clear jQuery validation if present
    if (typeof $ !== 'undefined' && $.validator) {
        $('form').each(function() {
            const validator = $(this).data('validator');
            if (validator) {
                validator.resetForm();
            }
        });
    }
    
    // Hide any visible toasts with error messages
    const toasts = document.querySelectorAll('.toast.bg-danger');
    toasts.forEach(toast => {
        const bsToast = bootstrap.Toast.getInstance(toast);
        if (bsToast) bsToast.hide();
    });
};
