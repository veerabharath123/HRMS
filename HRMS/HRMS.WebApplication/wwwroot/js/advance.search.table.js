class AdvanceSearchFilter {
    constructor(options) {
        // Ensure required options are provided
        if (!options.containerSelector || !options.columns) {
            console.error("AdvancedSearchFilter requires containerSelector and columns options.");
            return;
        }

        // Store configuration
        this.config = {
            containerSelector: options.containerSelector,
            columns: options.columns,
            operatorOptions: options.operatorOptions || [
                { text: 'And', value: 'And' },
                { text: 'Or', value: 'Or' }
            ],
            dateOrNumOrTimeOptions: options.dateOrNumOrTimeOptions || [
                { text: '>=', value: '>=' },
                { text: '<=', value: '<=' }
            ],
            textOptions: options.textOptions || [
                { text: 'contains', value: 'contains' },
                { text: 'ends', value: 'ends' },
                { text: 'starts', value: 'starts' }
            ],
            boolOptions: options.boolOptions || [
                { text: 'Yes', value: 'Yes' },
                { text: 'No', value: 'No' },
            ],
            baseOptions: options.baseOptions || [
                { text: '=', value: '=' },
                { text: '!=', value: '!=' }
            ]
        };

        // Store jQuery object for the main container
        this.$container = $(this.config.containerSelector);

        // Set up initial UI and event listeners
        this.addSearchRow(this.$container.find('.advance-search-row'));
        this.setupEventListeners();
    }

    setupEventListeners() {
        this.$container.on('click', '.add-btn', (e) => this.handleAddRow(e));
        this.$container.on('click', '.del-btn', (e) => this.handleDeleteRow(e));
        this.$container.on('change', '.fields', (e) => this.handleFieldChange(e));
    }
    setSearchCallBack(callback) {
        const $searchButton = this.$container.closest('.collapse').find('.advance-serach-btn')

        if (typeof callback === 'function') {
            $searchButton.on('click', callback)
        }

    }
    handleAddRow(event) {
        const row = $(event.target).closest('.advance-search-row')
        const newRow = row.clone();
        this.$container.append(newRow)
        this.addSearchRow(newRow)
    }
    handleDeleteRow(event) {
        $(event.target).closest('.advance-search-row').remove();
        this.checkDeleteButtons();
    }
    handleFieldChange(event) {
        const input = $(event.target).closest('.advance-search-row')
        const compField = input.find('.comparator')
        const valField = input.find('.target')
        const prop = this.config.columns.find(x => x.property == $(event.target).val())?.type || 'text'

        switch (prop) {
            case 'date':
            case 'time':
            case 'number':
                this.loadNumOrDateOrTime(prop, compField, valField)
                break;
            case 'bool':
                this.loadBoolean(compField, valField)
                break;
            default:
                this.loadText(compField, valField);
                break;
        }
    }
    addSearchRow(row) {
        this.checkDeleteButtons()
        const columns = this.config.columns;

        row.find('input, select').each((i, elem) => {
            const input = $(elem)
            input.val('');
            if (input.closest('.fields').length) {
                input.empty().append(`<option></option>`)
                columns.forEach(x => {
                    input.append(`<option value="${x.property}">${x.property}</option>`)

                })
            }
            if (input.hasClass('operator')) {
                this.loadOperator(input)
            }
            if (input.hasClass('comparator')) {
                this.loadText(input, input.closest('.advance-search-row').find('.target'))
            }
        })
    }
    checkDeleteButtons() {
        const delBtns = this.$container.find('.advance-search-row .del-btn')
        delBtns.prop('disabled', delBtns.length == 1)
    }

    loadOperator(valField) {
        valField.empty().append(`<option></option>`)
        this.config.operatorOptions.forEach(x => {
            valField.append(`<option value="${x.value}">${x.text}</option>`)
        })
    }
    loadText(compField, valField) {
        compField.empty().append(`<option></option>`)
        valField.empty();
        this.config.baseOptions.concat(this.config.textOptions).forEach(x => {
            compField.append(`<option value="${x.value}">${x.text}</option>`)
        })

        valField.html('<input type="text" class="form-control form-control-sm border border-secondary" />')
    }
    loadNumOrDateOrTime(type, compField, valField) {
        compField.empty().append(`<option></option>`)
        valField.empty();
        this.config.baseOptions.concat(this.config.dateOrNumOrTimeOptions).forEach(x => {
            compField.append(`<option value="${x.value}">${x.text}</option>`)
        })

        valField.html(`<input type="${type}" class="form-control form-control-sm border border-secondary" />`)
    }
    loadBoolean(compField, valField) {
        compField.empty().append(`<option></option>`)
        valField.empty();

        this.config.baseOptions.forEach(x => {
            compField.append(`<option value="${x.value}">${x.text}</option>`)
        })

        const select = $('<select class="form-select form-select-sm border border-secondary"></select>')
            .append(`<option></option>`);

        this.config.boolOptions.forEach(x => {
            select.append(`<option value="${x.value}">${x.text}</option>`)
        })

        valField.html(select);
    }

    getFilters() {
        const filters = [];
        const rows = this.$container.find('.advance-search-row');

        rows.each(function (index, row) {
            const $row = $(row);
            const field = $row.find('.fields').val();
            const operator = $row.find('.operator').val();
            const comparator = $row.find('.comparator').val();
            const value = $row.find('.target input, .target select').val();

            // Only add valid filter rows
            if (field && comparator && value) {
                filters.push({
                    property: field,
                    comparator: comparator,
                    value: value,
                    operator: index > 0 ? (operator || 'And') : '' // Operator is 'And' by default for subsequent rows
                });
            }
        });

        return filters;
    }
}

class AdvanceSearchTable {
    constructor(options) {
        const defaults = {
            tableSelector: null,
            filterSelector: null,
            ajaxUrl: null,
            columns: [],
            dataTableOptions: {}
        };

        this.config = $.extend(true, {}, defaults, options);

        if (!this.isValidConfig()) {
            throw new Error("AdvanceSearchTable requires tableSelector, ajaxUrl and columns.");
        }

        this.filter = new AdvanceSearchFilter({
            containerSelector: this.config.filterSelector,
            columns: this.config.columns
        });

        this.initTable();
        this.filter.setSearchCallBack(() => this.table.draw());
    }

    isValidConfig() {
        return this.config.tableSelector && this.config.ajaxUrl && this.config.columns.length;
    }

    initTable() {
        const tblCols = this.config.columns.map(c => ({ data: c.property }));
        const finalConfig = this.getFinalConfig(tblCols);
        this.table = $(this.config.tableSelector).DataTable(finalConfig);
    }

    getFinalConfig(tblCols) {
        const defaults = {
            processing: true,
            serverSide: true,
            ajax: this.buildAjaxConfig(),
            columns: tblCols
        };
        return $.extend(true, {}, defaults, this.config.dataTableOptions);
    }

    buildAjaxConfig() {
        return {
            url: this.config.ajaxUrl,
            type: 'POST',
            contentType: 'application/json',
            data: d => this.buildRequestPayload(d),
            dataSrc: data => this.mapResponse(data)
        };
    }

    buildRequestPayload(d) {
        return JSON.stringify({
            currentPage: this.getCurrentPage(d),
            maxPages: 0,
            pageLength: d.length,
            filter: this.buildFilters(d),
            sort: this.buildSort(d)
        });
    }

    getCurrentPage(d) {
        return Math.floor(d.start / d.length) + 1;
    }

    buildFilters(d) {
        const filters = this.filter.getFilters();
        if (d.search && d.search.value) filters.push(this.buildGlobalFilter(d.search.value));
        return filters;
    }

    buildGlobalFilter(value) {
        return { property: "Name", comparator: "like", operator: "AND", value };
    }

    buildSort(d) {
        if (!(d.order && d.order.length)) return [];
        return d.order.map(ord => ({
            property: d.columns[ord.column].data,
            desc: ord.dir === "desc"
        }));
    }

    mapResponse(data) {
        console.log("Server response:", data);
        return data;
    }
}