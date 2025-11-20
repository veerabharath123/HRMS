// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const LOGOUT_PATH = '/Account/Logout';
class Spinner {
    static show() {
        const spinner = document.getElementById('spinner');
        if (spinner) spinner.classList.add('show');
    }

    static hide() {
        const spinner = document.getElementById('spinner');
        if (spinner) spinner.classList.remove('show');
    }
}

function setImageSrc(element, base64Data) {
    const img = $(element);

    if (!base64Data) {
        console.warn("No image data provided for element:", element);
        return;
    }

    // Check if it already starts with a proper base64 data URI
    if (!base64Data.startsWith("data:image")) {
        base64Data = `data:image/png;base64,${base64Data}`;
    }

    img.attr("src", base64Data);
}

// Get CSRF token
function getCsrfToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

// Toaster helper
function showNotification(options) {
    const {
        type = 'info',          // 'error', 'success', 'warning', 'info'
        title = '',             // title of the message
        message = 'Something went wrong',           // message text
        timer = 3000,           // default duration (ms)
        customOptions = {},      // additional toastr or custom options
        onComplete = null,     // <-- add optional callback
        onShow = null 
    } = options || {};

    if (typeof toastr === 'undefined') {
        alert(`${title ? title + ':\n' : ''}${message}`);
        return;
    }
        
    const defaultOptions = {
        timeOut: timer,
        closeButton: true,
        progressBar: true,
        preventDuplicates: true,
        onShown: function () {
            if (typeof onShow === 'function') onShow();
        },
        onHidden: function () {
            if (typeof onComplete === 'function') onComplete();
        }

    };

    const finalOptions = Object.assign({}, defaultOptions, customOptions);

    switch (type.toLowerCase()) {
        case 'error':
            toastr.error(message, title || 'Error', finalOptions);
            break;
        case 'success':
            toastr.success(message, title || 'Success', finalOptions);
            break;
        case 'warning':
            toastr.warning(message, title || 'Warning', finalOptions);
            break;
        default:
            toastr.info(message, title || 'Info', finalOptions);
            break;
    }
}

// ----------------------------
// Core AJAX Utility
// ----------------------------
function ajaxRequest(options) {
    const config = buildAjaxConfig(options);
    const method = config.method.toUpperCase();

    $.ajax({
        url: config.url,
        method,
        data: prepareRequestData(config),
        contentType: config.contentType,
        dataType: config.dataType,
        processData: config.processData,
        headers: prepareHeaders(config, method),
        beforeSend: config.beforeSend || (() => Spinner.show()),
        afterSend: () => Spinner.hide(),
        complete: () => Spinner.hide(),
        success: (res) => handleAjaxSuccess(res, config),
        error: config.errorCallback
    });
}

/* ----------------------- Helper Functions ----------------------- */

function buildAjaxConfig(options) {
    const defaults = {
        method: 'GET',
        url: '',
        data: {},
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        processData: true,
        headers: {},
        useDefaultSuccessCallBack: true,
        successCallback: null,
        errorCallback: handleAjaxError,
        beforeSend: null
    };

    const config = { ...defaults, ...options };
    config.url = resolveSafeUrl(config.url);
    return config;
}

function prepareHeaders(config, method) {
    const headers = { ...config.headers };
    if (['POST', 'PUT', 'DELETE'].includes(method)) {
        headers['X-CSRF-TOKEN'] = getCsrfToken();
    }
    return headers;
}

function prepareRequestData(config) {
    if (
        config.contentType !== false &&
        config.contentType?.includes('application/json') &&
        config.processData !== false
    ) {
        return JSON.stringify(config.data || {});
    }
    return config.data;
}

function handleAjaxSuccess(res, config) {

    const useDefaultSuccessCallBack = typeof config?.useDefaultSuccessCallBack === 'boolean'
        ? config?.useDefaultSuccessCallBack 
        : true;

    const callback = typeof config?.successCallback === 'function'
        ? config.successCallback : null;

    if (callback) {
        if (!useDefaultSuccessCallBack)
            return callback(res);

        if (useDefaultSuccessCallBack && res?.success) {
            callback(res.data, res.message);
        }
    }

    if (res && !res.success) showNotification({
        type: 'error',
        message: res?.message || 'Something went wrong!'
    });

    if (res?.logout) {
        window.location.href = res.logoutUrl || LOGOUT_PATH;
        return;
    }

    if (res?.redirectTo) {
        window.location.href = res.redirectTo;
        return;
    }
}

/**
 * ✅ Safely combine window.basePath + relative URL
 * Prevents injection or external redirects
 */
function resolveSafeUrl(url) {
    if (!url) return '';

    // If absolute URL (http:// or https://), block unless same origin
    const isAbsolute = /^https?:\/\//i.test(url);
    if (isAbsolute) {
        const link = document.createElement('a');
        link.href = url;
        if (link.origin !== window.location.origin) {
            console.warn('Blocked external AJAX request:', url);
            throw new Error('External AJAX URLs are not allowed for security reasons.');
        }
        return url;
    }

    // Use window.basePath if defined
    let base = (typeof window !== 'undefined' && window.basePath ? window.basePath?.trim() : '') || '';
    if (base && !base.endsWith('/')) base += '/';

    // Normalize and remove accidental double slashes
    const safeUrl = (base + url).replace(/([^:]\/)\/+/g, '$1');
    return safeUrl;
}

// ----------------------------
// GET Request
// ----------------------------
function ajaxGet(url, successCallback, options = {}) {
    ajaxRequest({
        method: 'GET',
        url: url,
        successCallback: successCallback,
        ...options
    });
}

// ----------------------------
// POST JSON Request
// ----------------------------
function ajaxPost(url, data = {}, successCallback, options = {}) {
    ajaxRequest({
        method: 'POST',
        url: url,
        data: data,
        successCallback: successCallback,
        ...options
    });
}

// ----------------------------
// POST File Upload
// ----------------------------
function ajaxPostFile(url, formData, successCallback, options = {}) {
    const csrfToken = getCsrfToken();
    if (!csrfToken) {
        showNotification({ type: 'error', message: 'CSRF token not found. Please refresh the page and try again.' });
        return;
    }
    formData.append('__RequestVerificationToken', csrfToken);


    ajaxRequest({
        method: 'POST',
        url: url,
        data: formData,
        processData: false,
        contentType: false,
        successCallback: successCallback,
        ...options
    });
}

// ----------------------------
// LOAD HTML Partial View
// ----------------------------
function ajaxLoadHtml(url, data = {}, targetSelector, options = {}) {
    ajaxRequest({
        method: 'POST',
        url: url,
        dataType: 'html',
        data: data,
        useDefaultSuccessCallBack: false,
        successCallback: function (html) {
            // Inject HTML into target container
            if (targetSelector) {
                const $target = $(targetSelector);
                if ($target.length) {
                    $target.html(html);
                } else {
                    console.warn(`ajaxLoadHtml: Target selector "${targetSelector}" not found.`);
                }
            }

            // Optional per-request callback
            if (typeof options.afterLoad === 'function') {
                options.afterLoad(html);
            }
        },
        errorCallback: options.errorCallback || handleAjaxError,
        ...options
    });
}



// --------------------
// Main AJAX Error Handler
// --------------------
function handleAjaxError(xhr) {
    Spinner.hide();

    const response = parseAjaxResponse(xhr);
    const message = getErrorMessage(response, xhr.status);

    // Handle custom actions from server response
    handleServerActions(response);

    // Display appropriate message based on status
    handleStatusCode(xhr.status, message);
}

// --------------------
// Helper: Parse JSON safely
// --------------------
function parseAjaxResponse(xhr) {
    if (xhr.responseJSON) return xhr.responseJSON;

    if (xhr.responseText) {
        try {
            return JSON.parse(xhr.responseText);
        } catch (e) {
            console.warn("Invalid JSON in response:", e);
        }
    }

    return {};
}

// --------------------
// Helper: Extract user-friendly message
// --------------------
function getErrorMessage(response, status) {
    if (response.message) return response.message;
    if (response.error) return response.error;

    switch (status) {
        case 400: return "Bad request. Please check your input.";
        case 401: return "Your session has expired. Please log in again.";
        case 403: return "Access denied. You do not have permission.";
        case 404: return "The requested resource was not found.";
        case 500: return "A server error occurred. Please try again later.";
        default: return "An unknown error occurred.";
    }
}

// --------------------
// Helper: Handle different status codes
// --------------------
function handleStatusCode(status, message) {
    switch (status) {
        case 400:
        case 401:
        case 403:
        case 404:
        case 500:
        default:
            showNotification({ type: 'error', message: message });
            break;
    }
}

// --------------------
// Helper: Handle custom actions from backend (logout, redirect, etc.)
// --------------------
function handleServerActions(response) {
    if (response.logout) {
        setTimeout(() => {
            window.location.href = LOGOUT_PATH;
        }, 1500);
    }

    if (response.redirect) {
        setTimeout(() => {
            window.location.href = response.redirect;
        }, 1000);
    }
}
