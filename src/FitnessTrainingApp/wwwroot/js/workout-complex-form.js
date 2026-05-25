(() => {
  const picker = document.querySelector("[data-exercise-picker]");

  if (!picker) {
    return;
  }

  const list = picker.querySelector("[data-exercise-list]");
  const template = picker.querySelector("[data-exercise-template]");
  const addButton = picker.querySelector("[data-add-exercise]");

  const getRows = () => Array.from(list.querySelectorAll("[data-exercise-row]"));

  const updateRows = () => {
    const rows = getRows();

    rows.forEach((row, index) => {
      const number = index + 1;
      const removeButton = row.querySelector("[data-remove-exercise]");
      const title = row.querySelector(".exercise-picker-title");

      if (title) {
        title.textContent = `Exercise ${number}`;
      }

      if (removeButton) {
        removeButton.disabled = rows.length === 1;
      }

      row.querySelectorAll("select, input").forEach((field) => {
        field.name = field.name.replace(/Exercises\[\d+\]/, `Exercises[${index}]`);
        field.id = field.id.replace(/Exercises_\d+__/, `Exercises_${index}__`);
      });

      row.querySelectorAll("label").forEach((label) => {
        const target = label.getAttribute("for");

        if (target) {
          label.setAttribute("for", target.replace(/Exercises_\d+__/, `Exercises_${index}__`));
        }
      });
    });
  };

  addButton.addEventListener("click", () => {
    const index = getRows().length;
    const html = template.innerHTML
      .replaceAll("__INDEX__", index)
      .replaceAll("__NUMBER__", index + 1);

    list.insertAdjacentHTML("beforeend", html);
    updateRows();
  });

  list.addEventListener("click", (event) => {
    const removeButton = event.target.closest("[data-remove-exercise]");

    if (!removeButton) {
      return;
    }

    const rows = getRows();

    if (rows.length === 1) {
      return;
    }

    removeButton.closest("[data-exercise-row]").remove();
    updateRows();
  });

  updateRows();
})();
