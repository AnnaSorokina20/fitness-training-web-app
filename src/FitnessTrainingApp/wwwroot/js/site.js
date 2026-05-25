(() => {
    const fields = document.querySelectorAll("[data-autocomplete-url]");

    fields.forEach((field) => {
        const input = field.querySelector("[data-autocomplete-input]");
        const list = field.querySelector("[data-autocomplete-list]");

        if (!input || !list) {
            return;
        }

        let activeIndex = -1;
        let abortController;

        const hideList = () => {
            list.hidden = true;
            list.innerHTML = "";
            activeIndex = -1;
        };

        const getItems = () => Array.from(list.querySelectorAll("button"));

        const setActiveItem = (index) => {
            const items = getItems();
            activeIndex = index;

            items.forEach((item, itemIndex) => {
                item.classList.toggle("active", itemIndex === activeIndex);
            });
        };

        const chooseValue = (value) => {
            input.value = value;
            hideList();
            input.form?.requestSubmit();
        };

        const renderSuggestions = (suggestions) => {
            list.innerHTML = "";

            if (suggestions.length === 0) {
                hideList();
                return;
            }

            suggestions.forEach((suggestion) => {
                const button = document.createElement("button");
                button.type = "button";
                button.textContent = suggestion;
                button.addEventListener("mousedown", (event) => {
                    event.preventDefault();
                    chooseValue(suggestion);
                });
                list.appendChild(button);
            });

            list.hidden = false;
            setActiveItem(-1);
        };

        input.addEventListener("input", async () => {
            const query = input.value.trim();

            if (query.length < 2) {
                hideList();
                return;
            }

            abortController?.abort();
            abortController = new AbortController();

            try {
                const response = await fetch(`${field.dataset.autocompleteUrl}?query=${encodeURIComponent(query)}`, {
                    signal: abortController.signal,
                    headers: { "Accept": "application/json" }
                });

                if (!response.ok) {
                    hideList();
                    return;
                }

                renderSuggestions(await response.json());
            } catch (error) {
                if (error.name !== "AbortError") {
                    hideList();
                }
            }
        });

        input.addEventListener("keydown", (event) => {
            const items = getItems();

            if (list.hidden || items.length === 0) {
                return;
            }

            if (event.key === "ArrowDown") {
                event.preventDefault();
                setActiveItem((activeIndex + 1) % items.length);
            }

            if (event.key === "ArrowUp") {
                event.preventDefault();
                setActiveItem(activeIndex <= 0 ? items.length - 1 : activeIndex - 1);
            }

            if (event.key === "Enter" && activeIndex >= 0) {
                event.preventDefault();
                chooseValue(items[activeIndex].textContent);
            }

            if (event.key === "Escape") {
                hideList();
            }
        });

        input.addEventListener("blur", () => {
            window.setTimeout(hideList, 120);
        });
    });
})();
