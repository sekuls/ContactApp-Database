// frontend 
var token           = localStorage.getItem('token') || null; // keep token in llocal storage
var currentContactId = null;
var categories      = [];   //cached categories

//init
window.onload = function () {
    if (token) showLoggedIn();
    fetchCategories();
    showList();
};

//nav
function showList() {
    hideAll();
    show('view-list');
    fetchContacts();
}

function showLogin() {
    hideAll();
    show('view-login');
    hide('error-login');
}

function showForm(id) {
    hideAll();
    show('view-form');
    hide('error-form');
    if (id) {
        setText('form-title', 'Edit Contact');
        fetchContactDetails(id, true);
    } else {
        setText('form-title', 'Add Contact');
        clearForm();
    }
}

function hideAll() {
    ['view-list', 'view-details', 'view-login', 'view-form'].forEach(hide);
}

//authentication
async function login() {
    hide('error-login');
    try {
        var resp = await fetch('/api/auth/login', {
            method:  'POST',
            headers: { 'Content-Type': 'application/json' },
            body:    JSON.stringify({ login: val('inp-login'), password: val('inp-password') })
        });
        if (resp.ok) {
            token = (await resp.json()).token;
            localStorage.setItem('token', token);
            showLoggedIn();
            showList();
        } else {
            show('error-login');
        }
    } catch (e) {
        alert('Connection error');
    }
}

function logout() {
    token = null;
    localStorage.removeItem('token');
    hide('btn-add');
    hide('action-buttons');
    hide('info-user');
    document.getElementById('auth-buttons').innerHTML ='<button onclick="showLogin()" class="btn btn-primary btn-sm">Login</button>';
    showList();
}

function showLoggedIn() {
    setText('info-user', 'Logged in as: admin');
    show('info-user');
    show('btn-add');
    document.getElementById('auth-buttons').innerHTML ='<button onclick="logout()" class="btn btn-outline-danger btn-sm">Logout</button>';
}

// contact list
async function fetchContacts() {
    var tbody = document.getElementById('table-body');
    tbody.innerHTML = '<tr><td colspan="5" class="text-center">Loading...</td></tr>';
    try {
        var contacts = await (await fetch('/api/contacts')).json();
        if (!contacts.length) {
            tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No contacts found</td></tr>';
            return;
        }
        tbody.innerHTML = contacts.map(c => `
            <tr class="contact-row" onclick="showDetails(${c.id})">
                <td>${c.firstName}</td>
                <td>${c.lastName}</td>
                <td>${c.email}</td>
                <td>${c.phone || '-'}</td>
                <td><span class="badge bg-secondary">${c.category}</span></td>
            </tr>`
        ).join('');
    } catch (e) {
        tbody.innerHTML = '<tr><td colspan="5" class="text-danger text-center">Error loading data!</td></tr>';
    }
}

// contact details
async function showDetails(id) {
    currentContactId = id;
    await fetchContactDetails(id, false);
    hideAll();
    show('view-details');
    if (token)
        show('action-buttons');
    else
        hide('action-buttons');
}

async function fetchContactDetails(id, toForm) {
    var c = await (await fetch('/api/contacts/' + id)).json();
    if (toForm) {
        setVal('f-id', c.id);
        setVal('f-firstname',c.firstName);
        setVal('f-lastname', c.lastName);
        setVal('f-email', c.email);
        setVal('f-phone',c.phone || '');
        if (c.dateOfBirth) setVal('f-dob', c.dateOfBirth.split('T')[0]);
        setVal('f-category', c.categoryId);
        await onCategoryChange();
        if (c.subcategoryId) setVal('f-subcategory', c.subcategoryId);
        if (c.customSubcategory) setVal('f-custom-subcategory', c.customSubcategory);
    } else {
        setText('det-name',c.firstName + ' ' + c.lastName);
        setText('det-email', c.email);
        setText('det-phone', c.phone || 'not provided');
        setText('det-category', c.category);
        setText('det-subcategory', c.subcategory || 'not provided');
        setText('det-dob',c.dateOfBirth ? new Date(c.dateOfBirth).toLocaleDateString('en-GB'): 'not provided');
    }
}


// crud
async function saveContact() {
    hide('error-form');
    var id = val('f-id');
    var payload = {
        firstName:val('f-firstname'),
        lastName:val('f-lastname'),
        email: val('f-email'),
        password: val('f-password'),
        phone:val('f-phone'),
        dateOfBirth: val('f-dob') || null,
        categoryId: parseInt(val('f-category')),
        subcategoryId:val('f-subcategory') ? parseInt(val('f-subcategory')) : null,
        customSubcategory: val('f-custom-subcategory') || null
    };

    // client validation
    if (!payload.firstName || !payload.lastName || !payload.email || !payload.categoryId) {
        showFormError('Please fill in all required fields!!!');
        return;
    }

    try {
        var resp = await fetch(id ? '/api/contacts/' +id :'/api/contacts', {
            method:  id ? 'PUT':'POST',
            headers: {'Content-Type':'application/json','Authorization':'Bearer ' + token},
            body: JSON.stringify(payload)
        });
        if (resp.ok) {alert(id ? 'Contact updated!' :'Contact added!');
            showList();
        } else {
            var err = await resp.json();
            showFormError('Error:' + (err.message || '???'));
        }
    } catch (e) {
        showFormError('Connection error');
    }
}

async function deleteContact() {
    if (!confirm('Are you sure you want to delete this contact?')) return;
    try {
        var resp = await fetch('/api/contacts/' + currentContactId, {method:'DELETE',headers: { 'Authorization': 'Bearer ' + token } });
        if (resp.ok) { alert('Contact deleted!'); showList(); }
        else  alert('Error while deleting');
    } catch (e) {
        alert('Connection error');
    }
}

function editContact() { showForm(currentContactId); }

// dictionary

async function fetchCategories() {
    try {
        categories = await (await fetch('/api/dictionary/categories')).json();
        var sel = document.getElementById('f-category');
        sel.innerHTML = '<option value="">-- select --</option>';
        categories.forEach(c => { sel.innerHTML += `<option value="${c.id}">${c.name}</option>`;});
    } catch (e) {
        console.error('Failed to fetch categories', e);
    }
}

async function onCategoryChange() {
    var categoryId = val('f-category');
    var category   = categories.find(c => c.id == categoryId);
    hide('div-subcategory-dict');
    hide('div-subcategory-custom');
    setVal('f-subcategory',        '');
    setVal('f-custom-subcategory', '');

    if (!category) return;

    if (category.name === 'work') {
        // load subcategories from dictionary
        var subcategories = await (await fetch('/api/dictionary/subcategories/' + categoryId)).json();
        var sel  = document.getElementById('f-subcategory');
        sel.innerHTML = '<option value="">-- select --</option>';
        subcategories.forEach(s => {
            sel.innerHTML += `<option value="${s.id}">${s.name}</option>`;
        });
        show('div-subcategory-dict');
    } else if (category.name === 'other') {
        show('div-subcategory-custom');
    }
}

// helpers tools

function val(id)        { return document.getElementById(id).value;     }
function setVal(id, v)  { document.getElementById(id).value     = v;    }
function setText(id, v) { document.getElementById(id).innerText = v;    }
function show(id)       { document.getElementById(id).classList.remove('hidden'); }
function hide(id)       { document.getElementById(id).classList.add('hidden');    }

function clearForm() {
    ['f-id', 'f-firstname', 'f-lastname', 'f-email', 'f-password',
     'f-phone', 'f-dob', 'f-custom-subcategory'].forEach(id => setVal(id, ''));
    setVal('f-category', '');
    hide('div-subcategory-dict');
    hide('div-subcategory-custom');
}

function showFormError(msg) {
    setText('error-form', msg);
    show('error-form');
}
