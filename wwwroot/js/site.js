document.addEventListener("DOMContentLoaded", function () {
    addTestsEventListeners();
    const overlayBackdrop = document.getElementById("overlayBackdrop");
    const overlayTitle = document.getElementById("overlay-title");
    const overlayContent = document.querySelector(".overlay-body");
    const questionoverlayBackdrop = document.getElementById("questionoverlayBackdrop");
    const questionoverlayTitle = document.getElementById("question-overlay-title");
    const questionoverlayContent = document.querySelector(".question-overlay-body");

    const typeSelect = document.getElementById('assignmentType'); // single/group
    const groupSelect = document.getElementById('groupSelect'); // группы
    const studentSelect = document.getElementById('studentSelect'); // студенты    
    const categorySelect = document.getElementById('testCategory'); // категории
    const testsSelect = document.getElementById('test'); // тесты
     
    if (typeSelect && groupSelect && studentSelect && categorySelect && testsSelect) {
        loadStudents();
        loadTests();
        typeSelect.addEventListener('change', function () {
            if (this.value === 'single') {
                studentSelect.disabled = false;
                loadStudents(groupSelect.value);
            } else {
                studentSelect.disabled = true;
                studentSelect.innerHTML = '<option value="">---</option>';
            }
        });

        groupSelect.addEventListener('change', function () {
            if (typeSelect.value === 'single') {
                loadStudents(this.value);
            }
        });

        categorySelect.addEventListener('change', function () {
            loadTests(this.value);
        });
        
    }

    // Ограничение ввода для количества попыток (0–10, только цифры)
    const countAttempts = document.getElementById('countAttempts');
    if (countAttempts) {
        countAttempts.addEventListener('input', () => {
            // оставляем только цифры
            let v = countAttempts.value.replace(/[^0-9]/g, '');
            // обрезаем до 2 символов
            if (v.length > 2) v = v.slice(0, 2);
            // приводим к числу и ограничиваем диапазон
            if (v !== '') {
                let n = parseInt(v, 10);
                if (Number.isNaN(n)) n = 0;
                if (n > 10) n = 10;
                if (n < 0) n = 0;
                v = String(n);
            }
            countAttempts.value = v;
        });
        countAttempts.addEventListener('blur', () => {
            // пустое значение → 0
            if (countAttempts.value === '') countAttempts.value = '0';
        });
    }

    function loadStudents(groupId) {
        if (!groupId) {
            studentSelect.innerHTML = '<option value="">---</option>';
            return;
        }

        fetch(`/Teacher/GetStudentsByGroup?groupId=${groupId}`)
            .then(r => r.json())
            .then(data => {
                studentSelect.innerHTML = '<option value="">---</option>';
                data.forEach(student => {
                    const opt = document.createElement('option');
                    opt.value = student.id;
                    opt.textContent = student.name;
                    studentSelect.appendChild(opt);
                });
            });
    }

    function loadTests(categoryId) {
        if (!categoryId) {
            testsSelect.innerHTML = '<option value="">---</option>';
            return;
        }

        fetch(`/Teacher/GetTestsByCategories?categoryId=${categoryId}`)
            .then(r => r.json())
            .then(data => {
                testsSelect.innerHTML = '<option value="">---</option>';
                data.forEach(category => {
                    const opt = document.createElement('option');
                    opt.value = category.id;
                    opt.textContent = category.name;
                    testsSelect.appendChild(opt);
                });
            });
    }

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
        const btn = e.target.closest(".delete-test-btn");
        if (!btn) return;
        if (!confirm("Удалить тест?")) return;

        const testId = btn.dataset.testId;
        fetch(`/Teacher/DeleteTest?id=${testId}`, {
            method: "POST"
        })
        .then(response => {
            if (response.redirected) {
                window.location.href = response.url;
            } else {
                reloadTestTable();
            }
        });
    });

    document.addEventListener("click", function (e) {
        const btn = e.target.closest(".delete-category-btn");
        if (!btn) return;
        if (!confirm("Удалить категорию?")) return;

        const categoryId = btn.dataset.categoryId;
        fetch(`/Teacher/DeleteCategory?id=${categoryId}`, {
            method: "POST"
        })
        .then(response => {
            if (response.redirected) {
                window.location.href = response.url;
            } else {
                reloadTestTable();
            }
        });
    });

    document.addEventListener('submit', (e) => {
        const form = e.target.closest('#registerForm');
        if (!form) return;
        e.preventDefault();
        fetch('/Admin/Register', { method: 'POST', body: new FormData(form) })
          .then(r => r.text())
          .then(html => {
            const container = document.getElementById('registerFormContainer');
            if (container) container.innerHTML = html; // заменяем весь блок, ничего не “накладывается”
          })
          .catch(console.error);
    });

    document.addEventListener('submit', (e) => {
        const form = e.target.closest('#assignmentForm');
        if (!form) return;
        e.preventDefault();
        fetch('/Teacher/TestsAssignments', { method: 'POST', body: new FormData(form) })
            .then(response => response.text().then(html => ({ ok: response.ok, html })))
            .then(({ ok, html }) => {
                if (ok) {
                    reloadGroupTestAssignmentsTable();
                    reloadSingleTestAssignmentsTable();                    
                } else {
                    overlayContent.innerHTML = html;
                }
            });
    });

    window.addListeners = function (){
        document.querySelectorAll('.group-row').forEach(row => {
            row.addEventListener('click', () => {
                const next = row.nextElementSibling;
                if (next && next.classList.contains('assignment-subrow')) {
                    next.style.display = next.style.display === 'none' ? 'table-row' : 'none';
                }
            });
        });
    }
    window.addListeners();

    window.addListenersStudentTestRow = function (){
        document.querySelectorAll('.student-test-row').forEach(row => {
            row.addEventListener('click', () => {
                const aId = row.getAttribute('data-id');
                window.location.href = `/Student/TakeTest/${aId}`;
            });
        });
    }
    window.addListenersStudentTestRow();

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

    function reloadStudentTestTable() {
        fetch('/Student/GetTestTable')
            .then(response => response.text())
            .then(newTbodyHtml => {
                const currentTbody = document.getElementById("testTable");
                if (currentTbody) {
                    currentTbody.innerHTML = newTbodyHtml;
                }
            })
            .catch(err => console.error("Ошибка при обновлении таблицы:", err));
    }

    function reloadGroupTestAssignmentsTable() {
        fetch('/Teacher/GetGroupAssignmentsTable')
            .then(response => response.text())
            .then(newTbodyHtml => {
                const currentTbody = document.getElementById("testGroupAssignmentsTableBody");
                if (currentTbody) {
                    currentTbody.innerHTML = newTbodyHtml;
                    window.addListeners();
                }
            })
            .catch(err => console.error("Ошибка при обновлении таблицы:", err));
    }

    function reloadSingleTestAssignmentsTable() {
        fetch('/Teacher/GetSingleAssignmentsTable')
            .then(response => response.text())
            .then(newTbodyHtml => {
                const currentTbody = document.getElementById("testSingleAssignmentsTableBody");
                if (currentTbody) {
                    currentTbody.innerHTML = newTbodyHtml;
                    window.addListeners();
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
                window.currentTestId = testId;
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
                                    window.answerIndex = 0;
                                    initAnswerButtons();
                                    const form = document.getElementById("questionOverlayForm");
                                    if (form) {
                                        form.addEventListener('click', function (e) {
                                            if (e.target.classList.contains('delete-answer')) {
                                                e.preventDefault();
                                                e.target.closest('tr').remove();
                                            }
                                        })
                                        form.addEventListener("submit", function (e) {
                                            e.preventDefault();  
                                            let questionText = document.getElementById("questionText").value;                             
                                            let answers = [];
                                            let isCorrect = [];
                                            document.querySelectorAll(".answer-row").forEach(row => {
                                                let answerInput = row.querySelector(".answer-text");
                                                let checkbox = row.querySelector(".answer-correct");
        
                                                if (answerInput && answerInput.value.trim() !== "") {
                                                    answers.push(answerInput.value.trim());
                                                    isCorrect.push(checkbox.checked);
                                                }
                                            });
                                            const fd = new FormData();
                                            fd.append('testId', String(testId));
                                            fd.append('questionText', questionText);
                                            answers.forEach(a => fd.append('answers', a));
                                            isCorrect.forEach(v => fd.append('isCorrect', v ? 'true' : 'false'));
                                            fetch('/Teacher/Questions', {
                                                method: 'POST',
                                                body: fd
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

    function initAnswerButtons() {        
        const addAnswerBtn = document.getElementById("addAnswerBtn");
        if (!addAnswerBtn) return;
        
        addAnswerBtn.addEventListener("click", function () {
            const answersContainer = document.getElementById("answersContainer");
            if (!answersContainer) return;
            
            const index = window.answerIndex ?? 0;
            const answerBlock = document.createElement("tr");
            answerBlock.classList.add("answer-row");
            answerBlock.innerHTML = `
                <td>
                    <textarea name="Answers[${index}].Text" class="answer-text form-control" rows="1"> </textarea>
                </td>
                <td>
                    <input type="checkbox" class="answer-correct" name="Answers[${index}].IsCorrect">
                </td>
                <td class="actions">
                    <button type="button" class="delete-btn delete-answer">Удалить</button>
                </td>
            `;
            answersContainer.appendChild(answerBlock);
            
            window.answerIndex = index + 1;
        });
    }
    

    overlayBackdrop?.addEventListener("click", function () {
        window.closeOverlay();
    });

    overlayContent?.addEventListener("click", function (e) {
        e.stopPropagation();
    });

    questionoverlayBackdrop?.addEventListener("click", function () {
        const form = document.getElementById("questionOverlayForm");
        const textareas = form.querySelectorAll('textarea[name^="Answers"]');
        const someFilled = Array.from(textareas).some(ta => ta.value.trim() !== '');
        const questionText = document.getElementById("questionText");
        if(someFilled || questionText.value.trim() !== '')
        {
            if(confirm("Вы уверены, что хотите выйти?"))
            {
                window.questioncloseOverlay();
            }  
        }
        else
        {
            window.questioncloseOverlay();
        }
        
     
    });

    questionoverlayContent?.addEventListener("click", function (e) {
        e.stopPropagation();
    });

    // Делегирование кликов внутри overlayContent для удаления вопросов
    overlayContent?.addEventListener("click", function (e) {
        const qDeleteBtn = e.target.closest('.delete-question-btn');
        if (qDeleteBtn) {
            e.preventDefault();
            if (!confirm('Удалить вопрос?')) return;
            const questionId = qDeleteBtn.dataset.questionId;
            fetch(`/Teacher/DeleteQuestion?id=${questionId}`, { method: 'POST' })
                .then(res => { if (res.ok && window.currentTestId) { reloadQuestionTable(window.currentTestId); } });
            return; // не пускаем дальше
        }
        // остальные клики не всплывают выше overlayContent
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
        const form = document.getElementById('questionOverlayForm');
        if (!form) return;
      
        const checkboxes = form.querySelectorAll('input[type="checkbox"][name^="Answers"]');
        const textareas  = form.querySelectorAll('textarea[name^="Answers"]');
      
        // если это форма добавления (есть ответы/чекбоксы) — валидируем
        if (checkboxes.length > 0 || textareas.length > 0) {
          const atLeastOneChecked = Array.from(checkboxes).some(cb => cb.checked);
          const allFilled = Array.from(textareas).every(ta => ta.value.trim() !== '');
          if (!allFilled) { alert('Заполните все варианты ответа!'); return; }
          if (!atLeastOneChecked) { alert('Отметьте хотя бы один правильный ответ!'); return; }
        }
      
        form.requestSubmit();
    };
});