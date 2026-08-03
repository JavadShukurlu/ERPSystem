function initializeAdvancedFilter(config) {
    const quickSearchInput = document.getElementById(config.quickSearchInputId);
    const advancedFilterPopup = document.getElementById(config.advancedFilterPopupId);
    const filterFields = document.getElementById(config.filterFieldsId);
    const addFieldButton = document.getElementById(config.addFieldButtonId);
    const restoreDefaultFields = document.getElementById(config.restoreDefaultFieldsId);
    const applyButton = document.getElementById(config.applyButtonId);
    const resetButton = document.getElementById(config.resetButtonId);
    const saveFilterButton = document.getElementById(config.saveFilterButtonId);
    const savedFiltersList = document.getElementById(config.savedFiltersListId);
    const activeFilterChip = document.getElementById(config.activeFilterChipId);
    const activeFilterName = document.getElementById(config.activeFilterNameId);
    const clearActiveFilter = document.getElementById(config.clearActiveFilterId);

    const saveFilterBox = document.getElementById(config.saveFilterBoxId);
    const saveFilterNameInput = document.getElementById(config.saveFilterNameId);
    const confirmSaveFilterButton = document.getElementById(config.saveFilterConfirmId);
    const cancelSaveFilterButton = document.getElementById(config.saveFilterCancelId);

    const defaultFilterName = config.defaultFilterName || "All records";
    const fieldSettingsGroupName = config.fieldSettingsGroupName || "Fields";

    let activeFields = [...config.defaultFields];
    let currentSavedFilterName = null;

    function getSavedFilters() {
        try {
            return JSON.parse(localStorage.getItem(config.storageKey)) || [];
        } catch {
            return [];
        }
    }

    function setSavedFilters(filters) {
        localStorage.setItem(config.storageKey, JSON.stringify(filters));
    }

    function getPinnedFilterName() {
        return localStorage.getItem(config.pinnedStorageKey);
    }

    function setPinnedFilterName(name) {
        if (!name) {
            localStorage.removeItem(config.pinnedStorageKey);
            return;
        }

        localStorage.setItem(config.pinnedStorageKey, name);
    }

    function getField(key) {
        return config.availableFields.find(field => field.key === key);
    }

    function getInput(key) {
        return filterFields.querySelector(`[data-filter-input="${key}"]`);
    }

    function getItemValue(item, key) {
        return (item.dataset[key] || "").toLowerCase();
    }

    function getAllItemValues(item) {
        return config.availableFields
            .map(field => getItemValue(item, field.key))
            .join(" ");
    }

    function getCurrentValues() {
        const values = {};

        activeFields.forEach(key => {
            const input = getInput(key);
            values[key] = input ? input.value.trim() : "";
        });

        return values;
    }

    function renderFilterFields(values = {}) {
        filterFields.innerHTML = "";

        activeFields.forEach(key => {
            const field = getField(key);

            if (!field) {
                return;
            }

            const wrapper = document.createElement("div");
            wrapper.className = "filter-field";

            const header = document.createElement("div");
            header.className = "filter-field-header";

            const label = document.createElement("label");
            label.textContent = field.label;

            const removeButton = document.createElement("button");
            removeButton.type = "button";
            removeButton.textContent = "x";
            removeButton.className = "filter-field-remove";

            removeButton.addEventListener("click", function () {
                const valuesBeforeRemove = getCurrentValues();

                activeFields = activeFields.filter(item => item !== key);
                delete valuesBeforeRemove[key];

                renderFilterFields(valuesBeforeRemove);
                currentSavedFilterName = null;
                updateActiveChip();
                applyFilter();
            });

            let input;

            if (field.type === "select") {
                input = document.createElement("select");

                const emptyOption = document.createElement("option");
                emptyOption.value = "";
                emptyOption.textContent = "Select";
                input.appendChild(emptyOption);

                (field.options || []).forEach(option => {
                    const selectOption = document.createElement("option");
                    selectOption.value = option.value;
                    selectOption.textContent = option.text;
                    input.appendChild(selectOption);
                });

                input.addEventListener("change", function () {
                    currentSavedFilterName = null;
                    updateActiveChip();
                    applyFilter();
                });
            } else {
                input = document.createElement("input");
                input.type = field.type || "text";

                input.addEventListener("input", function () {
                    currentSavedFilterName = null;
                    updateActiveChip();
                    applyFilter();
                });
            }

            input.dataset.filterInput = key;
            input.value = values[key] || "";

            header.appendChild(label);

            if (!config.defaultFields.includes(key)) {
                header.appendChild(removeButton);
            }

            wrapper.appendChild(header);
            wrapper.appendChild(input);
            filterFields.appendChild(wrapper);
        });
    }

    function applyFilter() {
        const quickValue = quickSearchInput.value.trim().toLowerCase();
        const values = getCurrentValues();
        const items = document.querySelectorAll(config.itemSelector);

        items.forEach(item => {
            const quickMatch =
                !quickValue ||
                getAllItemValues(item).includes(quickValue);

            const fieldMatch = activeFields.every(key => {
                const value = (values[key] || "").toLowerCase();

                if (!value) {
                    return true;
                }

                return getItemValue(item, key).includes(value);
            });

            item.style.display = quickMatch && fieldMatch ? "" : "none";
        });
    }

    function updateActiveChip() {
        if (!currentSavedFilterName) {
            activeFilterChip?.classList.add("d-none");

            if (activeFilterName) {
                activeFilterName.textContent = "";
            }

            return;
        }

        activeFilterChip?.classList.remove("d-none");

        if (activeFilterName) {
            activeFilterName.textContent = currentSavedFilterName;
        }
    }

    function renderSavedFilters() {
        const filters = getSavedFilters();
        const pinnedName = getPinnedFilterName();

        savedFiltersList.innerHTML = "";

        const allButton = document.createElement("button");
        allButton.type = "button";
        allButton.className = "saved-filter-main";
        allButton.textContent = defaultFilterName;

        allButton.addEventListener("click", function () {
            currentSavedFilterName = null;
            activeFields = [...config.defaultFields];
            quickSearchInput.value = "";

            renderFilterFields();
            updateActiveChip();
            applyFilter();
            advancedFilterPopup.classList.remove("is-open");
            advancedFilterPopup.classList.add("d-none");
        });

        savedFiltersList.appendChild(allButton);

        filters.forEach(filter => {
            const row = document.createElement("div");
            row.className = "saved-filter-row";

            const filterButton = document.createElement("button");
            filterButton.type = "button";
            filterButton.className = "saved-filter-item";
            filterButton.textContent = filter.name;

            if (filter.name === pinnedName) {
                filterButton.classList.add("is-pinned");
            }

            filterButton.addEventListener("click", function () {
                activeFields = [...filter.fields];
                currentSavedFilterName = filter.name;

                renderFilterFields(filter.values);
                updateActiveChip();
                applyFilter();
                advancedFilterPopup.classList.remove("is-open");
                advancedFilterPopup.classList.add("d-none");
            });

            const pinButton = document.createElement("button");
            pinButton.type = "button";
            pinButton.className = "saved-filter-pin";
            pinButton.textContent = filter.name === pinnedName ? "Unpin" : "Pin";

            pinButton.addEventListener("click", function (event) {
                event.stopPropagation();

                if (getPinnedFilterName() === filter.name) {
                    setPinnedFilterName(null);
                } else {
                    setPinnedFilterName(filter.name);
                }

                renderSavedFilters();
                loadPinnedFilter();
            });

            const deleteButton = document.createElement("button");
            deleteButton.type = "button";
            deleteButton.className = "saved-filter-delete";
            deleteButton.textContent = "Delete";

            deleteButton.addEventListener("click", function (event) {
                event.stopPropagation();

                const updated = getSavedFilters()
                    .filter(item => item.name !== filter.name);

                setSavedFilters(updated);

                if (getPinnedFilterName() === filter.name) {
                    setPinnedFilterName(null);
                }

                if (currentSavedFilterName === filter.name) {
                    currentSavedFilterName = null;
                    updateActiveChip();
                }

                renderSavedFilters();
                applyFilter();
            });

            row.appendChild(filterButton);
            row.appendChild(pinButton);
            row.appendChild(deleteButton);
            savedFiltersList.appendChild(row);
        });
    }

    function openFieldSettings() {
        const oldModal = document.getElementById("filterFieldSettingsModal");

        if (oldModal) {
            oldModal.remove();
        }

        const valuesBeforeModal = getCurrentValues();

        const modal = document.createElement("div");
        modal.id = "filterFieldSettingsModal";
        modal.className = "field-settings-overlay is-open";

        modal.innerHTML = `
            <div class="field-settings-modal">
                <div class="field-settings-header">
                    <h3>Filter field settings</h3>
                    <button type="button" class="field-settings-close">x</button>
                </div>

                <div class="field-settings-search-wrap">
                    <input type="text" class="field-settings-search" placeholder="Find field" />
                </div>

                <div class="field-settings-group-title">${fieldSettingsGroupName}</div>

                <div class="field-settings-list"></div>

                <div class="field-settings-footer">
                    <button type="button" class="field-settings-select-all">select all</button>

                    <div>
                        <button type="button" class="field-settings-apply">APPLY</button>
                        <button type="button" class="field-settings-cancel">CANCEL</button>
                    </div>

                    <button type="button" class="field-settings-default">default</button>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        const list = modal.querySelector(".field-settings-list");
        const search = modal.querySelector(".field-settings-search");

        function renderList(searchValue = "") {
            list.innerHTML = "";

            config.availableFields
                .filter(field => field.label.toLowerCase().includes(searchValue.toLowerCase()))
                .forEach(field => {
                    const item = document.createElement("label");
                    item.className = "field-settings-item";

                    const checkbox = document.createElement("input");
                    checkbox.type = "checkbox";
                    checkbox.value = field.key;
                    checkbox.checked = activeFields.includes(field.key);

                    const span = document.createElement("span");
                    span.textContent = field.label;

                    item.appendChild(checkbox);
                    item.appendChild(span);
                    list.appendChild(item);
                });
        }

        renderList();

        search.addEventListener("input", function () {
            renderList(this.value);
        });

        modal.querySelector(".field-settings-close").addEventListener("click", function () {
            modal.remove();
        });

        modal.querySelector(".field-settings-cancel").addEventListener("click", function () {
            modal.remove();
        });

        modal.querySelector(".field-settings-select-all").addEventListener("click", function () {
            modal.querySelectorAll(".field-settings-item input").forEach(input => {
                input.checked = true;
            });
        });

        modal.querySelector(".field-settings-default").addEventListener("click", function () {
            modal.querySelectorAll(".field-settings-item input").forEach(input => {
                input.checked = config.defaultFields.includes(input.value);
            });
        });

        modal.querySelector(".field-settings-apply").addEventListener("click", function () {
            const selected = Array.from(modal.querySelectorAll(".field-settings-item input:checked"))
                .map(input => input.value);

            activeFields = selected.length > 0 ? selected : [...config.defaultFields];

            renderFilterFields(valuesBeforeModal);
            currentSavedFilterName = null;
            updateActiveChip();
            applyFilter();
            modal.remove();
        });
    }

    function saveCurrentFilterFromInlineInput() {
        const name = saveFilterNameInput.value;

        if (!name || !name.trim()) {
            saveFilterNameInput.focus();
            return;
        }

        const cleanName = name.trim();
        const filters = getSavedFilters();

        const newFilter = {
            name: cleanName,
            fields: [...activeFields],
            values: getCurrentValues()
        };

        const index = filters.findIndex(filter => filter.name === cleanName);

        if (index >= 0) {
            filters[index] = newFilter;
        } else {
            filters.push(newFilter);
        }

        setSavedFilters(filters);

        currentSavedFilterName = cleanName;
        updateActiveChip();
        renderSavedFilters();
        applyFilter();

        saveFilterBox.classList.add("d-none");
        saveFilterButton.classList.remove("d-none");
        saveFilterNameInput.value = "";
    }

    function loadPinnedFilter() {
        const pinnedName = getPinnedFilterName();

        if (!pinnedName) {
            return;
        }

        const filter = getSavedFilters()
            .find(item => item.name === pinnedName);

        if (!filter) {
            return;
        }

        activeFields = [...filter.fields];
        currentSavedFilterName = filter.name;

        renderFilterFields(filter.values);
        updateActiveChip();
        applyFilter();
    }

    function openSearchPopup() {
        advancedFilterPopup.classList.remove("d-none");
        advancedFilterPopup.classList.add("is-open");
        renderSavedFilters();
    }

    quickSearchInput?.addEventListener("focus", function (event) {
        event.stopPropagation();
        openSearchPopup();
    });

    quickSearchInput?.addEventListener("click", function (event) {
        event.stopPropagation();
        openSearchPopup();
    });

    advancedFilterPopup?.addEventListener("click", function (event) {
        event.stopPropagation();
    });

    document.addEventListener("click", function () {
        advancedFilterPopup?.classList.remove("is-open");
        advancedFilterPopup?.classList.add("d-none");
    });

    quickSearchInput?.addEventListener("input", function () {
        currentSavedFilterName = null;
        updateActiveChip();
        applyFilter();
    });

    addFieldButton?.addEventListener("click", function (event) {
        event.stopPropagation();
        openFieldSettings();
    });

    restoreDefaultFields?.addEventListener("click", function () {
        activeFields = [...config.defaultFields];
        currentSavedFilterName = null;
        quickSearchInput.value = "";

        renderFilterFields();
        updateActiveChip();
        applyFilter();
    });

    applyButton?.addEventListener("click", function () {
        applyFilter();
        advancedFilterPopup.classList.remove("is-open");
        advancedFilterPopup.classList.add("d-none");
    });

    resetButton?.addEventListener("click", function () {
        activeFields = [...config.defaultFields];
        currentSavedFilterName = null;
        quickSearchInput.value = "";

        renderFilterFields();
        updateActiveChip();
        applyFilter();
    });

    saveFilterButton?.addEventListener("click", function (event) {
        event.stopPropagation();

        saveFilterBox.classList.remove("d-none");
        saveFilterButton.classList.add("d-none");
        saveFilterNameInput.value = "";
        saveFilterNameInput.focus();
    });

    cancelSaveFilterButton?.addEventListener("click", function () {
        saveFilterBox.classList.add("d-none");
        saveFilterButton.classList.remove("d-none");
        saveFilterNameInput.value = "";
    });

    confirmSaveFilterButton?.addEventListener("click", function () {
        saveCurrentFilterFromInlineInput();
    });

    saveFilterNameInput?.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            saveCurrentFilterFromInlineInput();
        }
    });

    clearActiveFilter?.addEventListener("click", function () {
        currentSavedFilterName = null;
        setPinnedFilterName(null);
        quickSearchInput.value = "";

        activeFields = [...config.defaultFields];

        renderFilterFields();
        renderSavedFilters();
        updateActiveChip();
        applyFilter();
    });

    renderFilterFields();
    renderSavedFilters();
    loadPinnedFilter();
    applyFilter();
}