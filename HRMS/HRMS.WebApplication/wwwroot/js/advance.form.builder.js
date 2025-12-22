const FormBuilder = (function () {

    /* ==========================
       SYNC VALIDATORS REGISTRY
    ========================== */
    const syncValidators = {
        required(value) {
            if (Array.isArray(value)) return value.length > 0;
            if (typeof value === 'boolean') return value === true;
            return value !== null && value !== undefined && value.toString().trim() !== '';
        },
        minLength(value, len) {
            return value && value.length >= len;
        },
        maxLength(value, len) {
            return value && value.length <= len;
        },
        match(value, otherField, state) {
            return value === state.controls[otherField]?.getValue();
        },
        email(value) {
            if (!value) return true; // allow empty unless required

            const emailRegex =
                /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

            return emailRegex.test(value);
        }
    };

    /* ==========================
       ASYNC VALIDATORS REGISTRY
    ========================== */
    const asyncValidators = {};    

    /* ==========================
       DEBOUNCE
    ========================== */
    function debounce(fn, delay = 400) {
        let timer;
        return (...args) => {
            clearTimeout(timer);
            return new Promise(resolve => {
                timer = setTimeout(async () => {
                    resolve(await fn(...args));
                }, delay);
            });
        };
    }

    /* ==========================
       FACTORY
    ========================== */
    function create(formSelector, options = {}) {

        const $form = $(formSelector);
        const state = {
            isInvalid: false,
            isDirty: false,
            controls: {}
        };

        const localSyncValidators = {
            ...syncValidators,
            ...(options.validators || {})
        };

        const localAsyncValidators = {
            ...asyncValidators,
            ...(options.asyncValidators || {})
        };

        /* ==========================
           INIT CONTROLS
        ========================== */
        Object.keys(options.builder).forEach(name => {
            const $input = $form.find(`[name="${name}"]`);
            if (!$input.length) return;

            const $feedback = resolveFeedbackElement($input);

            state.controls[name] = {
                input: $input,
                feedback: $feedback,
                rules: options.builder[name],
                messages: options.messages?.[name] || {},
                touched: false,
                dirty: false,
                notMapped: options.builder[name].notMapped === true,
                live: typeof options.builder[name].live === 'boolean' ? options.builder[name].live : options.live,
                getValue: () => getValue($input),
                validate: () => validate(name),
                setValue: (value) => setControlValue($input, value),
            };

            $input.on('blur', () => state.controls[name].touched = true);
            $input.on('change', () => {
                state.controls[name].dirty = true;
                state.isDirty = true;
                if (state.controls[name].live)
                    validate(name);
            });
        });

        function patchValue(values) {
            if (!values || typeof values !== 'object') return;

            for (const name in values) {
                if (!state.controls[name]) continue;

                setControlValue(state.controls[name].input, values[name]);
                clearError(name);
            }
        }

        /* ==========================
           FEEDBACK RESOLVER
        ========================== */
        function resolveFeedbackElement($input) {
            const $section = $input.closest('.form-control-section');

            if (!$section.length) {
                console.warn('form-control-section not found for input:', $input[0]);
                return null;
            }

            let $feedback = $section.find('.invalid-feedback');
            if ($feedback.length) return $feedback;

            const elem = document.createElement('div');
            elem.className = 'invalid-feedback';

            $feedback = $(elem);
            $section.append($feedback);

            return $feedback;
        }

        function setControlValue($input, value) {
            if ($input.is(':checkbox')) {
                $input.prop('checked', !!value);
            }
            else if ($input.is(':radio')) {
                $form
                    .find(`input[name="${$input.attr('name')}"][value="${value}"]`)
                    .prop('checked', true);
            }
            else {
                $input.val(value);
            }

            control.dirty = false;
        }


        /* ==========================
           SYNC FIELD VALIDATION
        ========================== */
        function validateField(name) {
            const c = state.controls[name];
            if (!c) return true;

            clearError(name);
            const value = c.getValue();

            for (const rule in c.rules) {
                if (rule === 'async') continue;

                const validator = localSyncValidators[rule];
                if (!validator) continue;

                const ruleValue = c.rules[rule];
                const valid = validator(value, ruleValue, state);

                if (!valid) {
                    showError(
                        name,
                        c.messages[rule] || defaultMessage(rule, ruleValue)
                    );
                    return false;
                }
            }
            return true;
        }

        /* ==========================
           ASYNC FIELD VALIDATION
        ========================== */
        async function validateFieldAsync(name) {
            const c = state.controls[name];
            if (!c || !c.rules.async) return true;

            for (const key of c.rules.async) {
                const validator = localAsyncValidators[key];
                if (!validator) continue;

                const result = await validator(c.getValue(), state);
                if (result !== true) {
                    showError(name, result);
                    return false;
                }
            }
            return true;
        }

        /* ==========================
           FORM VALIDATION
        ========================== */
        function validate(name) {
            return validateField(name);
        }
        function validateAll() {
            state.isInvalid = false;

            for (const name in state.controls) {
                const ok = validateField(name);
                if (!ok) {
                    state.isInvalid = true;
                }
            }

            if (state.isInvalid) {
                focusFirstInvalid();
            }

            return !state.isInvalid;
        }
        async function validateAllAsync() {
            state.isInvalid = false;          // 🔥 reset here
            let firstInvalid = null;

            for (const name in state.controls) {

                const syncOk = validateField(name);

                if (!syncOk) {
                    if (!firstInvalid) {
                        firstInvalid = state.controls[name].input;
                    }
                    state.isInvalid = true;
                    continue; // ❗ do not run async if sync fails
                }

                const asyncOk = await validateFieldAsync(name);

                if (!asyncOk) {
                    if (!firstInvalid) {
                        firstInvalid = state.controls[name].input;
                    }
                    state.isInvalid = true;
                }
            }

            if (firstInvalid) {
                firstInvalid.focus();
            }

            return !state.isInvalid;
        }


        /* ==========================
           ERROR HELPERS
        ========================== */
        function showError(name, message) {
            const c = state.controls[name];

            c.input.addClass('is-invalid');
            c.feedback.text(message).show();
            c.input.closest('.form-control-section').addClass('has-error');

            state.isInvalid = true;
        }

        function clearError(name) {
            const c = state.controls[name];
            if (!c) return;

            c.input.removeClass('is-invalid');
            c.feedback.text('').hide();
            c.input.closest('.form-control-section').removeClass('has-error');
        }

        function defaultMessage(rule, val) {
            switch (rule) {
                case 'required': return 'This field is required';
                case 'minLength': return `Minimum ${val} characters required`;
                case 'match': return 'Values do not match';
                case 'email': return 'Invalid email address';
                default: return 'Invalid value';
            }
        }

        function focusFirstInvalid() {
            const $firstInvalid = $form
                .find('.is-invalid:visible')
                .first();

            if ($firstInvalid.length) {
                $firstInvalid.trigger('focus');
            }
        }

        function getFormValue() {
            const result = {};

            for (const name in state.controls) {
                const control = state.controls[name];
                if (control.notMapped) continue;
                result[name] = control.getValue();
            }

            return result;
        }

        function getFormValueJson() {
            return JSON.stringify(getFormValue());
        }
        let submitHandler = null;
        function runSubmit() {
            if (typeof submitHandler !== 'function') return;

            const isValid = validateAll();
            submitHandler(isValid);
        }

        /* ==========================
           PUBLIC API
        ========================== */
        return {
            validateAll,
            validateAllAsync,
            validateField,
            patchValue,
            getValue: getFormValue,
            getValueJson: getFormValueJson,
            reset() {
                Object.values(state.controls).forEach(c => {
                    c.input.val('').prop('checked', false).removeClass('is-invalid');
                    c.feedback.text('').hide();
                    c.touched = false;
                    c.dirty = false;
                });
                state.isInvalid = false;
                state.isDirty = false;
            },
            disable(name) {
                state.controls[name]?.input.prop('disabled', true);
            },
            enable(name) {
                state.controls[name]?.input.prop('disabled', false);
            },
            applyServerErrors(errors) {
                if (!errors) return;
                Object.keys(errors).forEach(name => {
                    if (state.controls[name]) {
                        showError(name, errors[name]);
                    }
                });
            },
            setValidator(name, fn) {
                if (!localSyncValidators[name] && typeof fn === 'function')
                    localSyncValidators[name] = fn;
            },
            setAsyncValidator(name, fn) {
                if (!localAsyncValidators[name] && typeof fn === 'function')
                    localAsyncValidators[name] = fn;
            },
            get isInvalid() { return state.isInvalid; },
            get isDirty() { return state.isDirty; },
            controls: state.controls,
            onSubmit(fn) {
                if (typeof fn === 'function') {
                    submitHandler = fn;

                    $form.on('submit', function (e) {
                        e.preventDefault()
                        runSubmit()
                        return false;
                    })
                }
            },
            addManualSubmitTrigger(selector) {
                const $trigger = $(selector);

                if (!$trigger.length) return;

                $trigger.on('click', function (e) {
                    e.preventDefault();
                    runSubmit();
                });
            }
        };
    }

    /* ==========================
       VALUE RESOLUTION
    ========================== */
    function getValue($input) {
        if ($input.is(':checkbox')) return $input.is(':checked');
        if ($input.is(':radio'))
            return $(`input[name="${$input.attr('name')}"]:checked`).val();
        return $input.val();
    }

    /* ==========================
       PUBLIC REGISTRATION API
    ========================== */
    function registerValidator(name, fn) {
        syncValidators[name] = fn;
    }

    function registerAsyncValidator(name, fn) {
        asyncValidators[name] = fn;
    }

    /* ==========================
       EXPORT
    ========================== */
    return {
        create,
        registerValidator,
        registerAsyncValidator,
        debounce,
        
    };

})();