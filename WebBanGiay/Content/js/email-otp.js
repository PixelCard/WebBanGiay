// File: WebBanGiay/Content/js/email-otp.js
$(document).ready(function () {
    // Email OTP functionality
    var otpTimer = null;
    var countdown = 0;

    // Send OTP button click
    $('#sendOTP').click(function () {
        var newEmail = $('#NewEmail').val().trim();

        // Validate email
        if (!newEmail) {
            showAlert('Vui lòng nhập email mới', 'error');
            return;
        }

        if (!isValidEmail(newEmail)) {
            showAlert('Email không hợp lệ', 'error');
            return;
        }

        // Disable button and show loading
        var $btn = $(this);
        var originalText = $btn.text();
        $btn.prop('disabled', true).text('Đang gửi...');

        // Send AJAX request
        $.ajax({
            url: '/Account/SendEmailOTP',
            type: 'POST',
            data: { newEmail: newEmail },
            success: function (response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    $('#otpSection').slideDown();
                    startOTPTimer();
                } else {
                    showAlert(response.message, 'error');
                }
            },
            error: function () {
                showAlert('Có lỗi xảy ra, vui lòng thử lại', 'error');
            },
            complete: function () {
                $btn.prop('disabled', false).text(originalText);
            }
        });
    });

    // Verify OTP button click
    $('#verifyOTP').click(function () {
        var otp = $('#OTP').val().trim();

        if (!otp) {
            showAlert('Vui lòng nhập mã OTP', 'error');
            return;
        }

        if (otp.length !== 6) {
            showAlert('Mã OTP phải có 6 số', 'error');
            return;
        }

        // Disable button and show loading
        var $btn = $(this);
        var originalText = $btn.text();
        $btn.prop('disabled', true).text('Đang xác thực...');

        // Send AJAX request
        $.ajax({
            url: '/Account/VerifyEmailOTP',
            type: 'POST',
            data: { otp: otp },
            success: function (response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                } else {
                    showAlert(response.message, 'error');
                }
            },
            error: function () {
                showAlert('Có lỗi xảy ra, vui lòng thử lại', 'error');
            },
            complete: function () {
                $btn.prop('disabled', false).text(originalText);
            }
        });
    });

    // Resend OTP button click
    $('#resendOTP').click(function () {
        var newEmail = $('#NewEmail').val().trim();

        if (!newEmail) {
            showAlert('Vui lòng nhập email mới', 'error');
            return;
        }

        // Disable button and show loading
        var $btn = $(this);
        var originalText = $btn.text();
        $btn.prop('disabled', true).text('Đang gửi lại...');

        // Send AJAX request
        $.ajax({
            url: '/Account/SendEmailOTP',
            type: 'POST',
            data: { newEmail: newEmail },
            success: function (response) {
                if (response.success) {
                    showAlert('Mã OTP mới đã được gửi', 'success');
                    startOTPTimer();
                } else {
                    showAlert(response.message, 'error');
                }
            },
            error: function () {
                showAlert('Có lỗi xảy ra, vui lòng thử lại', 'error');
            },
            complete: function () {
                $btn.prop('disabled', false).text(originalText);
            }
        });
    });

    // Start OTP countdown timer
    function startOTPTimer() {
        countdown = 300; // 5 minutes
        updateTimerDisplay();

        otpTimer = setInterval(function () {
            countdown--;
            updateTimerDisplay();

            if (countdown <= 0) {
                clearInterval(otpTimer);
                $('#otpTimer').text('Mã OTP đã hết hạn');
                $('#resendOTP').prop('disabled', false).text('Gửi lại OTP');
            }
        }, 1000);
    }

    // Update timer display
    function updateTimerDisplay() {
        var minutes = Math.floor(countdown / 60);
        var seconds = countdown % 60;
        $('#otpTimer').text('Mã OTP hết hạn sau: ' +
            (minutes < 10 ? '0' : '') + minutes + ':' +
            (seconds < 10 ? '0' : '') + seconds);
    }

    // Email validation
    function isValidEmail(email) {
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    // Show alert message
    function showAlert(message, type) {
        var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        var alertHtml = '<div class="alert ' + alertClass + ' alert-dismissible fade show" role="alert">' +
            message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>';

        // Remove existing alerts
        $('.alert').remove();

        // Add new alert
        $('.edit-header').after(alertHtml);

        // Auto hide after 5 seconds
        setTimeout(function () {
            $('.alert').fadeOut();
        }, 5000);
    }

    // OTP input formatting (only numbers, max 6 digits)
    $('#OTP').on('input', function () {
        var value = $(this).val();
        value = value.replace(/[^0-9]/g, ''); // Remove non-numeric characters
        value = value.substring(0, 6); // Limit to 6 digits
        $(this).val(value);
    });

    // Auto-submit when OTP is complete
    $('#OTP').on('keyup', function () {
        if ($(this).val().length === 6) {
            $('#verifyOTP').click();
        }
    });

    // Enter key support for email input
    $('#NewEmail').on('keypress', function (e) {
        if (e.which === 13) { // Enter key
            $('#sendOTP').click();
        }
    });

    // Enter key support for OTP input
    $('#OTP').on('keypress', function (e) {
        if (e.which === 13) { // Enter key
            $('#verifyOTP').click();
        }
    });

    // Clear OTP section when email changes
    $('#NewEmail').on('input', function () {
        if ($('#otpSection').is(':visible')) {
            $('#otpSection').slideUp();
            if (otpTimer) {
                clearInterval(otpTimer);
            }
            $('#OTP').val('');
        }
    });

    // Add loading spinner to buttons
    function addLoadingSpinner($button, text) {
        $button.html('<i class="fa fa-spinner fa-spin"></i> ' + text);
    }

    function removeLoadingSpinner($button, originalText) {
        $button.html(originalText);
    }

    // Enhanced error handling
    function handleAjaxError(xhr, status, error) {
        console.error('AJAX Error:', status, error);

        if (xhr.status === 0) {
            showAlert('Không thể kết nối đến máy chủ', 'error');
        } else if (xhr.status === 500) {
            showAlert('Lỗi máy chủ, vui lòng thử lại sau', 'error');
        } else {
            showAlert('Có lỗi xảy ra, vui lòng thử lại', 'error');
        }
    }

    // Add visual feedback for OTP input
    $('#OTP').on('input', function () {
        var value = $(this).val();
        var $input = $(this);

        // Add visual feedback based on length
        if (value.length === 6) {
            $input.addClass('is-valid').removeClass('is-invalid');
        } else if (value.length > 0) {
            $input.removeClass('is-valid is-invalid');
        } else {
            $input.removeClass('is-valid is-invalid');
        }
    });

    // Add success animation
    function showSuccessAnimation() {
        var $otpInput = $('#OTP');
        $otpInput.addClass('success-pulse');
        setTimeout(function () {
            $otpInput.removeClass('success-pulse');
        }, 1000);
    }

    // Add CSS for animations
    $('<style>')
        .prop('type', 'text/css')
        .html(`
            .success-pulse {
                animation: pulse 0.5s ease-in-out;
            }
            
            @keyframes pulse {
                0% { transform: scale(1); }
                50% { transform: scale(1.05); }
                100% { transform: scale(1); }
            }
            
            .is-valid {
                border-color: #28a745 !important;
                box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25) !important;
            }
            
            .is-invalid {
                border-color: #dc3545 !important;
                box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25) !important;
            }
            
            .btn:disabled {
                opacity: 0.6;
                cursor: not-allowed;
            }
            
            .alert {
                animation: slideInDown 0.3s ease-out;
            }
            
            @keyframes slideInDown {
                from {
                    transform: translateY(-20px);
                    opacity: 0;
                }
                to {
                    transform: translateY(0);
                    opacity: 1;
                }
            }
        `)
        .appendTo('head');
});