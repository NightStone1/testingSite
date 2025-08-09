document.addEventListener("DOMContentLoaded", function () {
    addTestsEventListeners();
    const overlayBackdrop = document.getElementById("overlayBackdrop");
    const overlayTitle = document.getElementById("overlay-title");
    const overlayContent = document.querySelector(".overlay-body");
    const questionoverlayBackdrop = document.getElementById("questionoverlayBackdrop");
    const questionoverlayTitle = document.getElementById("question-overlay-title");
    const questionoverlayContent = document.querySelector(".question-overlay-body");

    const addCategoryBtn = document.getElementById("addTestCategoryBtn");
    if (addCategoryBtn) {
        addCategoryBtn.addEventListener("click", function () {
            fetch('/Teacher/CreateTestCategory').then(response => response.text()).then(html => {
                    overlayTitle.textContent = "Добавить категорию";
                    overlayContent.innerHTML = html;
                    overlayBackdrop.classList.add("active");
                    const form = document.getElementById("OverlayForm");
                    if (form) {
                        form.addEventListener("submit", function (e) {
                            e.preventDefault();
                            updateDisciplineName();
                    
                            const formData = new FormData(form);
                    
                            fetch('/Teacher/CreateTestCategory', {
                                method: 'POST',
                                body: formData
                            })
                            .then(response => response.text().then(html => ({ ok: response.ok, html })))
                            .then(({ ok, html }) => {
                                if (ok) {
                                    reloadTestTable();
                                    window.closeOverlay();
                                } else {
                                    overlayContent.innerHTML = html;
                                }
                            })
                            .catch(error => {
                                console.error("Ошибка при создании категории:", error);
                            });
                        });
                    }                    
                });
        });
    }

    document.addEventListener("click", function (e) {
        const addTestBtn = e.target.closest(".add-test-btn");
        if (!addTestBtn) return;
    
        const categoryId = addTestBtn.dataset.categoryId;
        const row = addTestBtn.closest('tr');
        let categoryName = '';
        if (row) {
            const nameCell = row.querySelector('.category-cell');
            if (nameCell && nameCell.textContent) {
                categoryName = nameCell.textContent.replace(/^Категория:\s*/i, '').trim();
            }
        }
        fetch('/Teacher/CreateTest').then(response => response.text()).then(html => {
            overlayTitle.textContent = "Добавить тест";
            overlayContent.innerHTML = html;
            overlayBackdrop.classList.add("active");
            const form = document.getElementById("OverlayForm");
            if (form) {
                const hiddenInput = form.querySelector("input[name='categoryId']");
                if (hiddenInput) {
                    hiddenInput.value = categoryId;
                }
                const categoryNameInput = form.querySelector("input[name='categoryName']");
                if (categoryNameInput) {
                    categoryNameInput.value = categoryName;
                }
                form.addEventListener("submit", function (e) {
                    e.preventDefault();
                    const formData = new FormData(form);
                    fetch('/Teacher/CreateTest', {
                        method: 'POST',
                        body: formData
                    })
                    .then(response => response.text().then(html => ({ ok: response.ok, html })))
                    .then(({ ok, html }) => {
                        if (ok) {
                            reloadTestTable();
                            window.closeOverlay();
                        } else {
                            overlayContent.innerHTML = html;
                        }
                    });
                });
            }
        });
    });
    
    function reloadTestTable() {
        fetch('/Teacher/GetTestTableBody')
            .then(response => response.text())
            .then(newTbodyHtml => {
                const currentTbody = document.getElementById("testTableBody");
                if (currentTbody) {
                    currentTbody.innerHTML = newTbodyHtml;
                    addTestsEventListeners();
                }
            })
            .catch(err => console.error("Ошибка при обновлении таблицы:", err));
    }

    function reloadQuestionTable(testId) {
        fetch(`/Teacher/GetQuestionTableBody?id=${testId}`)
            .then(response => response.text())
            .then(newTbodyHtml => {
                const currentTbody = document.getElementById("questionTableBody");
                if (currentTbody) {
                    currentTbody.innerHTML = newTbodyHtml;
                    addQuestionEventListeners(testId);
                }
            })
            .catch(err => console.error("Ошибка при обновлении таблицы:", err));
    }

    function addQuestionEventListeners(testId) {
        document.querySelectorAll(".question-cell").forEach(cell => {
            cell.addEventListener("click", function () {
                const questionId = this.dataset.questionId;        
                fetch(`/Teacher/EditQuestion?id=${questionId}`)
                    .then(response => response.text())
                    .then(html => {
                        questionoverlayTitle.textContent = "Редактировать вопрос";
                        questionoverlayContent.innerHTML = html;
                        questionoverlayBackdrop.classList.add("active");
                        const form = document.getElementById("questionOverlayForm");
                        if (form) {
                            const hiddenInput = form.querySelector("input[name='questionId']");
                            if (hiddenInput) {
                                hiddenInput.value = questionId;
                            }
                            form.addEventListener("submit", function (e) {
                                e.preventDefault();
                                const formData = new FormData(form);
                                fetch('/Teacher/EditQuestion', {
                                    method: 'POST',
                                    body: formData
                                })
                                .then(response => response.text().then(html => ({ ok: response.ok, html })))
                                .then(({ ok, html }) => {
                                    if (ok) {
                                        reloadQuestionTable(testId);
                                        window.questioncloseOverlay();
                                    } else {
                                        questionoverlayContent.innerHTML = html;
                                    }
                                });
                            });
                        }
                    })
                    .catch(err => console.error("Ошибка при загрузке категории:", err));
            });
        });
    }

    function addTestsEventListeners() {
        document.querySelectorAll(".category-cell").forEach(cell => {
            cell.addEventListener("click", function () {
                const categoryId = this.dataset.categoryId;
        
                fetch(`/Teacher/EditTestCategory?id=${categoryId}`)
                    .then(response => response.text())
                    .then(html => {
                        overlayTitle.textContent = "Редактировать категорию";
                        overlayContent.innerHTML = html;
                        overlayBackdrop.classList.add("active");
                        const form = document.getElementById("OverlayForm");
                        if (form) {
                            const hiddenInput = form.querySelector("input[name='categoryId']");
                            if (hiddenInput) {
                                hiddenInput.value = categoryId;
                            }
                            form.addEventListener("submit", function (e) {
                                e.preventDefault();
                                const formData = new FormData(form);
                                fetch('/Teacher/EditTestCategory', {
                                    method: 'POST',
                                    body: formData
                                })
                                .then(response => response.text().then(html => ({ ok: response.ok, html })))
                                .then(({ ok, html }) => {
                                    if (ok) {
                                        reloadTestTable();
                                        window.closeOverlay();
                                    } else {
                                        overlayContent.innerHTML = html;
                                    }
                                });
                            });
                        }
                    })
                    .catch(err => console.error("Ошибка при загрузке категории:", err));
            });
        });
        document.querySelectorAll(".test-cell").forEach(cell => {
            cell.addEventListener("click", function () {
                const testId = this.dataset.testId;
        
                fetch(`/Teacher/EditTest?id=${testId}`)
                    .then(response => response.text())
                    .then(html => {
                        overlayTitle.textContent = "Редактировать тест";
                        const header = document.querySelector('.overlay-header');
                        if (header && !header.querySelector('.overlay-tabs')) {
                            const tabs = document.createElement('div');
                            tabs.className = 'overlay-tabs';
                            tabs.innerHTML = `
                                <span class="overlay-tab active" data-tab="info">Основная информация</span>
                                <span class="overlay-tab" data-tab="questions">Вопросы</span>
                            `;
                            header.insertBefore(tabs, header.querySelector('#closeOverlayBtn'));
                        }
                        overlayContent.innerHTML = html;
                        overlayBackdrop.classList.add("active");
                        const addQuestionBtn = document.getElementById("addQuestionBtn");
                        addQuestionEventListeners(testId);
                        if (addQuestionBtn) {
                            addQuestionBtn.addEventListener("click", function () {
                                fetch(`/Teacher/Questions?testId=${testId}`).then(response => response.text()).then(html => {
                                    questionoverlayTitle.textContent = "Добавить вопрос";                                    
                                    questionoverlayContent.innerHTML = html;
                                    questionoverlayBackdrop.classList.add("active");
                                    const form = document.getElementById("questionOverlayForm");
                                    if (form) {
                                        form.addEventListener("submit", function (e) {
                                            e.preventDefault();
                                            const formData = new FormData(form);
                                            fetch('/Teacher/Questions', {
                                                method: 'POST',
                                                body: formData
                                            })
                                            .then(response => response.text().then(html => ({ ok: response.ok, html })))
                                            .then(({ ok, html }) => {
                                                if (ok) {
                                                    window.questioncloseOverlay();                                                    
                                                    reloadQuestionTable(testId);
                                                } else {
                                                    questionoverlayContent.innerHTML = html;
                                                }
                                            })
                                            .catch(error => {
                                                console.error("Ошибка при создании категории:", error);
                                            });
                                        });
                                    }
                                });
                            });
                        }
                        const form = document.getElementById("OverlayForm");
                        if (form) {
                            const hiddenInput = form.querySelector("input[name='testId']");
                            if (hiddenInput) {
                                hiddenInput.value = testId;
                            }
                            form.addEventListener("submit", function (e) {
                                e.preventDefault();
                                const formData = new FormData(form);
                                fetch('/Teacher/EditTest', {
                                    method: 'POST',
                                    body: formData
                                })
                                .then(response => response.text().then(html => ({ ok: response.ok, html })))
                                .then(({ ok, html }) => {
                                    if (ok) {
                                        reloadTestTable();                                        
                                        window.closeOverlay();
                                    } else {
                                        overlayContent.innerHTML = html;
                                    }
                                });

                            });
                            const tabs = document.querySelectorAll('.overlay-tab');
                            tabs.forEach(tab => {
                                tab.addEventListener('click', () => {
                                    tabs.forEach(t => t.classList.remove('active'));
                                    tab.classList.add('active');
                                    const isInfo = tab.dataset.tab === 'info';
                                    const infoSection = document.getElementById('testInfoSection');
                                    const questionsSection = document.getElementById('testQuestionsSection');
                                    if (infoSection && questionsSection) {
                                        infoSection.style.display = isInfo ? '' : 'none';
                                        questionsSection.style.display = isInfo ? 'none' : '';
                                    }
                                });
                            });
                        }
                    })
                    .catch(err => console.error("Ошибка при загрузке категории:", err));
            });
        });
    }

    overlayBackdrop?.addEventListener("click", function () {
        window.closeOverlay();
    });

    overlayContent?.addEventListener("click", function (e) {
        e.stopPropagation();
    });

    questionoverlayBackdrop?.addEventListener("click", function () {
        window.questioncloseOverlay();
    });

    questionoverlayContent?.addEventListener("click", function (e) {
        e.stopPropagation();
    });

    window.closeOverlay = function () {
        overlayBackdrop.classList.remove("active");
        setTimeout(() => {
            overlayContent.innerHTML = "";
            const tabs = document.querySelector('.overlay-tabs');
            if (tabs && tabs.parentElement) {
                tabs.parentElement.removeChild(tabs);
            }
        }, 300);
    };

    window.questioncloseOverlay = function () {
        questionoverlayBackdrop.classList.remove("active");
        setTimeout(() => {
            if (questionoverlayContent) {
                questionoverlayContent.innerHTML = "";
            }
        }, 300);
    };

    window.updateDisciplineName = function () {
        const select = document.getElementById("discipline");
        const selectedText = select?.options[select.selectedIndex]?.text;
        if (selectedText) {
            const nameInput = document.getElementById("disciplineName");
            if (nameInput) nameInput.value = selectedText;
        }
    };

    window.updateDisciplineName();

    window.submitCategoryForm = function () {
        const form = document.getElementById("OverlayForm");
        if (form) {
            form.requestSubmit();
        }
    };    
    
    window.submitQuestionForm = function () {
        const form = document.getElementById("questionOverlayForm");
        if (form) {
            form.requestSubmit();
        }
    };    
});
