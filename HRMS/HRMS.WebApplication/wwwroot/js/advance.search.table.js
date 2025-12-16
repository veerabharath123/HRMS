const AdvanceSearchFilter = (function () {

    function create(options) {
        if (!options.tableSelector || !options.columns) {
            throw new Error("AdvanceSearchFilter requires containerSelector and columns");
        }

        const config = {
            operatorOptions: [
                { text: 'And', value: 'And' },
                { text: 'Or', value: 'Or' }
            ],
            textOptions: [
                { text: 'Contains', value: 'Contains' },
                { text: 'EndsWith', value: 'EndsWith' },
                { text: 'StartsWith', value: 'StartsWith' }
            ],
            dateOrNumOptions: [
                { text: 'GreaterThanOrEqual', value: 'GreaterThanOrEqual' },
                { text: 'LessThanOrEqual', value: 'LessThanOrEqual' }
            ],
            boolOptions: [
                { text: 'Yes', value: true },
                { text: 'No', value: false }
            ],
            baseOptions: [
                { text: 'Equals', value: 'Equals' },
                { text: 'NotEquals', value: 'NotEquals' }
            ],
            ...options
        };

        const $advanceSearchcontainer = $(`${config.tableSelector}-advance-search-container`);
        const $simpleSearchcontainer = $(`${config.tableSelector}-simple-search-container`);
        const $advanceSearchBox = $(`${config.tableSelector}-search-box`);

        initAdvanceSearch();
        bindAdvanceSearchEvents();

        /* ---------- init ---------- */
        function initAdvanceSearch() {
            addSearchRow($advanceSearchBox.find('.advance-search-row'));
            toggleOperatorField();
        }

        function bindAdvanceSearchEvents() {
            $advanceSearchBox.on('click', '.add-btn', onAdd);
            $advanceSearchBox.on('click', '.del-btn', onDelete);
            $advanceSearchBox.on('change', '.fields', onFieldChange);
            $advanceSearchBox.on('click', '.clear-btn', onClearRow);
            $simpleSearchcontainer.on('click', '.clear-btn', onClearSimpleRow)
        }

        /* ---------- handlers ---------- */
        function onAdd(e) {
            const row = $(e.target).closest('.advance-search-row');
            const newRow = row.clone();
            $advanceSearchBox.append(newRow);
            addSearchRow(newRow);
            toggleOperatorField();
        }

        function onDelete(e) {
            $(e.target).closest('.advance-search-row').remove();
            toggleDeleteButtons();
            toggleOperatorField();
        }

        function onClearRow() {
            $(e.target).closest('.advance-search-row')            
                .find('input, select').val('');
        }
        function onClearSimpleRow(e) {
            $simpleSearchcontainer
                .find('input, select').val('');
        }

        function onFieldChange(e) {
            const row = $(e.target).closest('.advance-search-row');
            const field = $(e.target).val();
            const type = config.columns.find(c => c.property === field)?.type || 'String';

            switch (type) {
                case 'Date':
                case 'Number':
                    loadNumeric(row);
                    break;
                case 'Bool':
                    loadBoolean(row);
                    break;
                default:
                    loadText(row);
            }
        }
        function onAdvanceSearchSubmit(fn) {
            if (typeof fn === 'function') {
                $advanceSearchcontainer
                    .find('.advance-serach-btn')
                    .off('click')
                    .on('click', fn);
            }
                
        }
        function onSimpleSearchSubmit(fn) {
            if (typeof fn === 'function') {
                $simpleSearchcontainer
                    .find('.simple-search-btn')
                    .off('click')
                    .on('click', fn);
            }

        }

        /* ---------- builders ---------- */
        function addSearchRow(row) {
            toggleDeleteButtons();

            row.find('.fields')
                .empty()
                .append('<option></option>')
                .append(config.columns
                    .filter(c => c.visible !== false)
                    .map(c => `<option value="${c.property}">${c.displayName || c.property}</option>`));

            loadOperator(row.find('.operator'));
            loadText(row);
        }

        function loadOperator($el) {
            $el.empty().append('<option></option>').prop('disabled', false);
            config.operatorOptions.forEach(o =>
                $el.append(`<option value="${o.value}">${o.text}</option>`));
        }

        function loadText(row) {
            loadComparators(row, config.baseOptions.concat(config.textOptions));
            row.find('.target').html('<input class="form-control form-control-sm">');
        }

        function loadNumeric(row) {
            loadComparators(row, config.baseOptions.concat(config.dateOrNumOptions));
            row.find('.target').html('<input type="number" class="form-control form-control-sm">');
        }

        function loadBoolean(row) {
            loadComparators(row, config.baseOptions);
            const select = $('<select class="form-select form-select-sm"><option></option></select>');
            config.boolOptions.forEach(o =>
                select.append(`<option value="${o.value}">${o.text}</option>`));
            row.find('.target').html(select);
        }

        function loadComparators(row, list) {
            const $cmp = row.find('.comparator').empty().append('<option></option>');
            list.forEach(o => $cmp.append(`<option value="${o.value}">${o.text}</option>`));
        }

        function toggleDeleteButtons() {
            const btns = $advanceSearchBox.find('.del-btn');
            btns.prop('disabled', btns.length === 1);
        }
        function toggleOperatorField() {
            const operators = $advanceSearchBox.find('.operator');
            if (operators.length)
                operators.first().prop('disabled', true).val('');
        }

        /* ---------- public ---------- */
        function getFilters() {
            const filters = [];

            if ($simpleSearchcontainer.hasClass('show')) {
                options.columns.filter(p => (p.type === 'String' || !p.type) && !(p.search === false)).forEach(p => {
                    if (p.property) {
                        filters.push({
                            PropertyName: p.property,
                            Comparator: 'Contains',
                            Value: $simpleSearchcontainer.find('input').val(),
                            Operator: 'Or',
                            PropertyType: 'String'
                        });
                    }
                })
            } else if ($advanceSearchcontainer.hasClass('show')) {
                $advanceSearchBox.find('.advance-search-row').each((_, row) => {
                    const $r = $(row);
                    const field = $r.find('.fields').val();
                    const comparator = $r.find('.comparator').val();
                    const value = $r.find('.target :input').val();
                    const operator = $r.find('.operator').val() || 'And';

                    if (field && comparator && value !== '') {
                        filters.push({
                            PropertyName: field,
                            Comparator: comparator,
                            Value: value,
                            Operator: operator,
                            PropertyType: 'String'
                        });
                    }
                });
            }

            return filters;
        }
        function clearAll() {
            $advanceSearchcontainer.find('input, select').val('').prop('checked', false);
            $simpleSearchcontainer.find('input, select').val('').prop('checked', false);
        } 

        return {
            getFilters,
            clearAll,
            onAdvanceSearchSubmit,
            onSimpleSearchSubmit
        };
    }

    return { create };

})();

const AdvanceSearchTable = (function () {

    function create(options) {

        if (!options.tableSelector || !options.ajaxUrl || !options.columns?.length) {
            throw new Error("AdvanceSearchTable requires tableSelector, ajaxUrl, columns");
        }

        const filter = AdvanceSearchFilter.create({
            tableSelector: options.tableSelector,
            columns: options.columns
        });
        let actionButtonRenderFn = () => '';

        const table = initTable();
        filter.onAdvanceSearchSubmit(reload)
        filter.onSimpleSearchSubmit(reload)

        if (typeof options.select === 'function') {
            debugger
            table.on('select.dt', function (e, dt, type, indexes) {
                
                if (type === 'row') {
                    const rowData = dt.rows(indexes).data().toArray();
                    console.log('Selected row:', rowData[0]);
                    options.select(rowData[0])
                }
            });
        }


        /* ---------- table ---------- */
        function initTable() {
            const headerRow = $(`${options.tableSelector} thead tr`).empty();
            options.columns.forEach(x => {
                const elem = document.createElement('th');
                if (!(x.visible === false))
                    elem.textContent = (x.displayName || x.property);
                headerRow.append(elem)
            })
            const table = $(options.tableSelector).DataTable({
                serverSide: true,
                processing: true,
                ajax: buildAjax(),
                columns: buildColumns(),
                ...options.dataTableOptions
            });

            if (options.hasActionButtons) {
                const elem = document.createElement('th');
                elem.textContent = 'Actions'
                headerRow.append(elem)
            }

            return table
        }

        function buildColumns() {
            const cols= options.columns.map(c => ({
                data: c.property,
                title: c.displayName,
                ...c
            }));

            if (options.hasActionButtons) {
                cols.push({
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (typeof options.renderActionButtons === 'function')
                            options.renderActionButtons(data, type, row)
                        else return''
                    }
                })
            }

            return cols;
        }
        
        function buildAjax() {
            return {
                url: options.ajaxUrl,
                type: 'POST',
                headers: {
                    'X-CSRF-TOKEN': getCsrfToken()
                },
                processData: false,
                contentType: 'application/json',
                data: d => JSON.stringify(buildRequest(d)),
                dataSrc: (response) => {
                    const dt = mapResponse(response);
                    return dt.data; // DataTables receives rows, but has access to root obj
                }
            };
        }
        function buildRequest(d) {
            const root = {
                Operator: "And",
                Filters: filter.getFilters(),
                Groups: []
            };
            const request = {
                Filter: {
                    Filters: []
                },
                FilterGroup: root,
                Sort: { SortOptions: buildSort(d) },
                Pagination: {
                    PageNumber: Math.floor(d.start / d.length) + 1,// this.getCurrentPage(d),
                    PageSize: d.length || 24,
                    MaxPages: 10
                }
            }
            return request
        }

        function buildSort(d) {
            return d.order.map(o => ({
                PropertyName: d.columns[o.column].data,
                Descending: o.dir === 'desc'
            }));
        }

        function mapResponse(apiResponse) {

            loadPagination({
                TotalItems: apiResponse.data.totalItems,
                PageNumber: apiResponse.data.pageNumber,
                TotalPages: apiResponse.data.totalPages,
                FirstPageToShow: apiResponse.data.firstPageToShow,
                LastPageToShow: apiResponse.data.lastPageToShow
            })

            // apiResponse = { success, data: { totalItems, items }, message }
            const dt = formatDataTablesResponse(apiResponse.data);

            // Attach totals to the root object so DataTables can read them
            apiResponse.recordsTotal = dt.recordsTotal;
            apiResponse.recordsFiltered = dt.recordsFiltered;
            apiResponse.data = dt.data;

            return apiResponse;
        }

        function formatDataTablesResponse(raw) {
            const r = typeof raw === "string" ? JSON.parse(raw) : raw;

            return {
                draw: r.draw ?? 1,
                recordsTotal: r.totalItems,
                recordsFiltered: r.totalItems,
                data: r.items
            };
        }

        function loadPagination(pRequest) {
            ajaxLoadHtml(
                resolveSafeUrl('/Home/GetPaginationHtml'),
                JSON.stringify(pRequest),
                `${options.tableSelector}-pagination`,
                {
                    afterLoad: function () {

                    },
                    contentType: "application/json",
                    processData: false
                }
            )

            if (!this.paginationEvent) {
                $(document).on('click', `${options.tableSelector}-pagination .page-item:not(.active) .page-link`, () => this.refresh())
                this.paginationEvent = true;
            }
        }
        function reload() {
            table.ajax.reload();
        }
        /* ---------- public ---------- */
        return {
            reload,
            clearAndReload() {
                filter.clearAll();
                table.ajax.reload();
            }
        };
    }

    return { create };

})();