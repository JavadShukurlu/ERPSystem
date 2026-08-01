function initializeAdvancedFilter(config) {
    const storageKey = config.storageKey;
    const pinnedKey = config.pinnedKey;
    const itemSelector = config.itemSelector;
    const pageSelector = config.pageSelector;
    const defaultPlaceholder = config.defaultPlaceholder;

    const page = document.querySelector(pageSelector);
    const quickSearch = document.getElementById("quickSearch");
    const filterPopup = document.getElementById("filterPopup");
    const addFieldButton = document.getElementById("addFieldButton");
    const addFieldMenu = document.getElementById("addFieldMenu");
    const savedFiltersList = document.getElementById("savedFiltersList");
    const saveFilterButton = document.getElementById("saveFilterButton");
    const restoreFiltersButton = document.getElementById("restoreFiltersButton");
    const searchButton = document.getElementById("searchButton");
    const resetButton = document.getElementById("resetButton");
    const activeFilterChip = document.getElementById("activeFilterChip");
    const activeFilterName = document.getElementById("activeFilterName");
    const clearActiveFilterButton = document.getElementById("clearActiveFilterButton");

    function openFilterPopup() {
        filterPopup.style.display = "grid";
    }

    function closeFilterPopup() {
        filterPopup.style.display = "none";

        if (addFieldMenu) {
            addFieldMenu.style.display = "none";
        }
    }

    function showActiveFilterChip(filterName) {
        activeFilterName.textContent = filterName;
        activeFilterChip.style.display = "inline-flex";
        quickSearch.placeholder = "search";
    }

    function hideActiveFilterChip() {
        activeFilterName.textContent = "";
        activeFilterChip.style.display = "none";
        quickSearch.placeholder = defaultPlaceholder;
    }

    function getFilters() {
        const values = {};

        config.fields.forEach(field => {
            const input = document.getElementById(field.inputId);

            if (!input) {
                values[field.key] = "";
                return;
            }

            values[field.key] = input.value.toLowerCase();
        });

        return values;
    }

    function applyFilters() {
        const filters = getFilters();
        const items = document.querySelectorAll(itemSelector);

        items.forEach(item => {
            let isVisible = true;

            config.fields.forEach(field => {
                const filterValue = filters[field.key];

                if (!filterValue) {
                    return;
                }

                const dataValue = item.dataset[field.dataKey]?.toLowerCase() || "";

                if (!dataValue.includes(filterValue)) {
                    isVisible = false;
                }
            });

            item.style.display = isVisible ? "" : "none";
        });
    }

    function getSavedFilters() {
        return JSON.parse(localStorage.getItem(storageKey) || "[]");
    }

    function setSavedFilters(filters) {
        localStorage.setItem(storageKey, JSON.stringify(filters));
    }

    function getVisibleOptionalFields() {
        return [...document.querySelectorAll(".optional-filter")]
            .filter(item => item.style.display !== "none")
            .map(item => item.dataset.filterField);
    }

    function showOptionalFilter(fieldName) {
        const field = document.querySelector(`[data-filter-field="${fieldName}"]`);

        if (field) {
            field.style.display = "";
        }

        const checkbox = document.querySelector(`.field-choice[value="${fieldName}"]`);

        if (checkbox) {
            checkbox.checked = true;
        }
    }

    function resetFilters(runFilter = true) {
        document.querySelectorAll(".filter-input").forEach(input => {
            input.value = "";
        });

        document.querySelectorAll(".optional-filter").forEach(item => {
            item.style.display = "none";
        });

        document.querySelectorAll(".field-choice").forEach(choice => {
            choice.checked = false;
        });

        hideActiveFilterChip();

        if (runFilter) {
            applyFilters();
        }
    }

    function saveCurrentFilter() {
        const name = prompt("Filter name:");

        if (!name || !name.trim()) {
            return;
        }

        const filters = getSavedFilters();

        filters.push({
            id: Date.now().toString(),
            name: name.trim(),
            values: getFilters(),
            optionalFields: getVisibleOptionalFields()
        });

        setSavedFilters(filters);
        renderSavedFilters();
    }

    function applySavedFilter(filterId, shouldOpenPopup = true) {
        const filters = getSavedFilters();
        const filter = filters.find(item => item.id === filterId);

        if (!filter) {
            return;
        }

        resetFilters(false);

        if (shouldOpenPopup) {
            openFilterPopup();
        }

        showActiveFilterChip(filter.name);

        filter.optionalFields.forEach(field => {
            showOptionalFilter(field);
        });

        config.fields.forEach(field => {
            const input = document.getElementById(field.inputId);

            if (input) {
                input.value = filter.values[field.key] || "";
            }
        });

        applyFilters();
    }

    function pinFilter(filterId) {
        localStorage.setItem(pinnedKey, filterId);
        renderSavedFilters();
        applySavedFilter(filterId);
    }

    function unpinFilter() {
        localStorage.removeItem(pinnedKey);
        renderSavedFilters();
    }

    function deleteSavedFilter(filterId) {
        const filters = getSavedFilters().filter(item => item.id !== filterId);

        setSavedFilters(filters);

        if (localStorage.getItem(pinnedKey) === filterId) {
            localStorage.removeItem(pinnedKey);
            hideActiveFilterChip();
            resetFilters();
        }

        renderSavedFilters();
        applyFilters();
    }

    function renderSavedFilters() {
        const filters = getSavedFilters();
        const pinnedId = localStorage.getItem(pinnedKey);

        savedFiltersList.innerHTML = "";

        if (filters.length === 0) {
            savedFiltersList.innerHTML = `<div class="text-muted small">No saved filters.</div>`;
            return;
        }

        filters.forEach(filter => {
            const item = document.createElement("div");
            item.className = "saved-filter-item";

            item.innerHTML = `
                <button type="button" class="saved-filter-name">${filter.name}</button>
                <button type="button" class="saved-filter-pin">${pinnedId === filter.id ? "Pinned" : "Pin"}</button>
                <button type="button" class="saved-filter-delete">Delete</button>
            `;

            item.querySelector(".saved-filter-name").addEventListener("click", () => {
                applySavedFilter(filter.id);
            });

            item.querySelector(".saved-filter-pin").addEventListener("click", () => {
                if (pinnedId === filter.id) {
                    unpinFilter();
                } else {
                    pinFilter(filter.id);
                }
            });

            item.querySelector(".saved-filter-delete").addEventListener("click", () => {
                deleteSavedFilter(filter.id);
            });

            savedFiltersList.appendChild(item);
        });
    }

    quickSearch?.addEventListener("focus", openFilterPopup);
    quickSearch?.addEventListener("click", openFilterPopup);

    searchButton?.addEventListener("click", () => {
        applyFilters();
        closeFilterPopup();
    });

    resetButton?.addEventListener("click", () => {
        localStorage.removeItem(pinnedKey);
        resetFilters();
        renderSavedFilters();
    });

    restoreFiltersButton?.addEventListener("click", () => {
        resetFilters();
    });

    addFieldButton?.addEventListener("click", () => {
        addFieldMenu.style.display = addFieldMenu.style.display === "block" ? "none" : "block";
    });

    document.querySelectorAll(".field-choice").forEach(choice => {
        choice.addEventListener("change", function () {
            if (this.checked) {
                showOptionalFilter(this.value);
            } else {
                const field = document.querySelector(`[data-filter-field="${this.value}"]`);

                if (field) {
                    field.style.display = "none";

                    const input = field.querySelector("input");

                    if (input) {
                        input.value = "";
                    }
                }
            }
        });
    });

    saveFilterButton?.addEventListener("click", saveCurrentFilter);

    activeFilterChip?.addEventListener("click", function () {
        openFilterPopup();
    });

    clearActiveFilterButton?.addEventListener("click", function (event) {
        event.stopPropagation();

        localStorage.removeItem(pinnedKey);
        hideActiveFilterChip();
        resetFilters();
        renderSavedFilters();
    });

    document.addEventListener("click", function (event) {
        const searchArea = document.querySelector(".search-area");

        if (!searchArea.contains(event.target)) {
            closeFilterPopup();
        }
    });

    renderSavedFilters();

    const pinnedFilterId = localStorage.getItem(pinnedKey);

    if (pinnedFilterId) {
        applySavedFilter(pinnedFilterId, false);
    }
}