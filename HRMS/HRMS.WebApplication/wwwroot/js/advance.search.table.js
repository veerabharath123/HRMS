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
                { text: 'GreaterThanOrEqual', value: 'GreaterThanOrEqual' },
                { text: 'LessThanOrEqual', value: 'LessThanOrEqual' }
            ],
            textOptions: options.textOptions || [
                { text: 'Contains', value: 'Contains' },
                { text: 'EndsWith', value: 'EndsWith' },
                { text: 'StartsWith', value: 'StartsWith' }
            ],
            boolOptions: options.boolOptions || [
                { text: 'Yes', value: 'Yes' },
                { text: 'No', value: 'No' },
            ],
            baseOptions: options.baseOptions || [
                { text: 'Equals', value: 'Equals' },
                { text: 'NotEquals', value: 'NotEquals' }
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
        const prop = this.config.columns.find(x => x.property == $(event.target).val())?.type || 'String'

        switch (prop) {
            case 'Date':
            case 'Time':
            case 'Number':
                this.loadNumOrDateOrTime(prop, compField, valField)
                break;
            case 'Bool':
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
                    if (x.visible === false) return;
                    input.append(`<option value="${x.property}">${(x.displayName || x.property)}</option>`)
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

        valField.html('<input type="text" class="form-control form-control-sm" />')
    }
    loadNumOrDateOrTime(type, compField, valField) {
        compField.empty().append(`<option></option>`)
        valField.empty();
        this.config.baseOptions.concat(this.config.dateOrNumOrTimeOptions).forEach(x => {
            compField.append(`<option value="${x.value}">${x.text}</option>`)
        })

        valField.html(`<input type="${type}" class="form-control form-control-sm " />`)
    }
    loadBoolean(compField, valField) {
        compField.empty().append(`<option></option>`)
        valField.empty();

        this.config.baseOptions.forEach(x => {
            compField.append(`<option value="${x.value}">${x.text}</option>`)
        })

        const select = $('<select class="form-select form-select-sm "></select>')
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
                    PropertyName: field,
                    Comparator: comparator,            // enum: FilterComparator.Equals
                    Value: value,
                    Operator: operator,                 // enum: FilterOperator.And
                    PropertyType: "String"           // enum: FilterPropertyType.String
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

        this.columnProps = [
            "name",
            "className",
            "width",
            "visible",
            "orderable",
            "searchable",
            "type",
            "render",
            "defaultContent",
            "createdCell",
            "orderData",
            "orderDataType",
            "orderSequence"
        ];

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

    buildColumns(cols) {
        return cols.map(c => {
            const col = {
                data: c.property,
                title: c.displayName
            };

            for (const key of this.columnProps) {
                if (c[key] !== undefined) {
                    col[key] = c[key];
                }
            }

            return col;
        });
    }

    initTable() {
        const tblCols = this.buildColumns(this.config.columns)//.map(c => ({ data: c.property, render: c.render, visible: c.visible }));
        const finalConfig = this.getFinalConfig(tblCols);
        this.table = $(this.config.tableSelector).DataTable(finalConfig);
    }

    getFinalConfig(tblCols) {
        const defaults = {
            processing: true,
            serverSide: true,
            ajax: this.buildAjaxConfig(),
            //drawCallback: () => { },
            //recordsTotal: () => this.lastRecordsTotal,
            //recordsFiltered: () => this.lastRecordsFiltered,
            columns: tblCols,
            lengthMenu: [20, 30, 40, 50]
        };
        return $.extend(true, {}, defaults, this.config.dataTableOptions);
    }

    buildAjaxConfig() {
        return {
            url: this.config.ajaxUrl,
            type: 'POST',
            headers: {
                'X-CSRF-TOKEN': getCsrfToken()
            },
            contentType: 'application/json',
            processData: false,
            data: d => this.buildRequestPayload(d),
            dataSrc: (response) => {
                const dt = this.mapResponse(response);
                
                return dt.data; // DataTables receives rows, but has access to root obj
            }
            
        };
    }

    buildRequestPayload(d) {
        return JSON.stringify({
            Filter: {
                Filters: []
            },
            FilterGroup: this.buildFilters(d),
            Sort: { SortOptions: this.buildSort(d) },
            Pagination: {
                PageNumber: this.getCurrentPage(d),
                PageSize: d.length || 24,
                MaxPages: 10
            }
        });
    }

    getCurrentPage(d) {
        return parseInt($(`${this.config.tableSelector}-pagination page-item.active page-nums`).text()) || Math.floor(d.start / d.length) + 1;
    }

    buildFilters(d) {
        const root = {
            Operator: "And",    
            Filters: [],
            Groups: []
        };

        const filters = this.filter.getFilters();
        if (d.search && d.search.value) filters.push(this.buildGlobalFilter(d.search.value));

        root.Filters = filters
        return root;
    }

    buildGlobalFilter(value) {
        return this.config.columns.map(x =>
        {
            return { PropertyName: x.property, Comparator: "Contains", Operator: "And", PropertyType: 'String', Value: value };
        })
    }

    buildSort(d) {
        if (!(d.order && d.order.length)) return [];
        return d.order.map(ord => ({
            PropertyName: d.columns[ord.column].data,
            Descending: ord.dir === "desc"
        }));
    }

    mapResponse(apiResponse) {

        this.loadPagination({
            TotalItems: apiResponse.data.totalItems,
            PageNumber: apiResponse.data.pageNumber,
            TotalPages: apiResponse.data.totalPages,
            FirstPageToShow: apiResponse.data.firstPageToShow,
            LastPageToShow: apiResponse.data.lastPageToShow
        })

        // apiResponse = { success, data: { totalItems, items }, message }
        const dt = this.formatDataTablesResponse(apiResponse.data);

        // Attach totals to the root object so DataTables can read them
        apiResponse.recordsTotal = dt.recordsTotal;
        apiResponse.recordsFiltered = dt.recordsFiltered;
        apiResponse.data = dt.data;

        return apiResponse;
    }

    formatDataTablesResponse(raw) {
        const r = typeof raw === "string" ? JSON.parse(raw) : raw;

        return {
            draw: r.draw ?? 1,
            recordsTotal: r.totalItems,
            recordsFiltered: r.totalItems,
            data: r.items
        };
    }

    refresh() {
        this.table.ajax.reload();
    }

    loadPagination(pRequest) {
        ajaxLoadHtml(
            resolveSafeUrl('/Home/GetPaginationHtml'),
            JSON.stringify(pRequest),
            `${this.config.tableSelector}-pagination`,
            {
                afterLoad: function () {
                    
                },
                contentType: "application/json",
                processData: false
            }
        )

        if (!this.paginationEvent) {
            $(document).on('click', `${this.config.tableSelector}-pagination .page-item:not(.active) .page-link`, () => this.refresh())
            this.paginationEvent = true;
        }
    }
}