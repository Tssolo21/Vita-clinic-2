// API Configuration
const API_BASE = window.location.origin;

// Authentication state
let isAuthenticated = false;
let currentUser = null;

// Default credentials for demo
const DEFAULT_CREDENTIALS = {
    username: 'admin',
    password: 'admin123'
};

// Data stores
let clients = [];
let animals = [];
let appointments = [];
let medicalRecords = [];
let settings = null;

// Initialize on load
document.addEventListener('DOMContentLoaded', async function() {
    // Setup login form handler
    setupLoginFormHandler();

    // Check if user is already authenticated
    checkStoredAuth();

    if (isAuthenticated) {
        await initializeApp();
    } else {
        showLoginScreen();
    }
});

// Check for stored authentication
function checkStoredAuth() {
    const storedUser = localStorage.getItem('vitaClinic_user');
    if (storedUser) {
        try {
            const user = JSON.parse(storedUser);
            currentUser = user;
            isAuthenticated = true;
        } catch (error) {
            console.error('Error parsing stored auth:', error);
            localStorage.removeItem('vitaClinic_user');
        }
    }
}

// Show login screen
function showLoginScreen() {
    document.getElementById('loginContainer').style.display = 'flex';
    document.querySelector('.main-container').classList.remove('authenticated');

    // Set focus on username field
    setTimeout(() => {
        document.getElementById('username').focus();
    }, 100);
}

// Initialize the main application
async function initializeApp() {
    document.getElementById('loginContainer').style.display = 'none';
    document.querySelector('.main-container').classList.add('authenticated');

    await loadAllData();
    updateDashboard();
    updateUserInfo();
}

// Login form handler
async function setupLoginFormHandler() {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;
            const loginBtn = document.getElementById('loginBtn');

            // Disable button during login
            loginBtn.disabled = true;
            loginBtn.textContent = 'Signing In...';

            try {
                // Simple authentication check (in a real app, this would call an API)
                if (authenticateUser(username, password)) {
                    // Store authentication
                    currentUser = { username, loginTime: new Date().toISOString() };
                    localStorage.setItem('vitaClinic_user', JSON.stringify(currentUser));
                    isAuthenticated = true;

                    // Initialize the main application
                    await initializeApp();
                    showNotification('Login successful! Welcome to VitaClinic.', 'success');
                } else {
                    showNotification('Invalid username or password. Please try again.', 'error');
                    document.getElementById('password').value = '';
                    document.getElementById('username').focus();
                }
            } catch (error) {
                console.error('Login error:', error);
                showNotification('Login failed. Please try again.', 'error');
            } finally {
                // Re-enable button
                loginBtn.disabled = false;
                loginBtn.textContent = 'Sign In';
            }
        });
    }
}

// Simple authentication function (replace with real API call in production)
function authenticateUser(username, password) {
    // For demo purposes, check against default credentials
    // In a real application, this would make an API call to authenticate
    return username === DEFAULT_CREDENTIALS.username && password === DEFAULT_CREDENTIALS.password;
}

// Update user info in UI
function updateUserInfo() {
    // Could add user info to the sidebar header if needed
    // For now, just ensure the main container is properly styled
    const mainContainer = document.querySelector('.main-container');
    if (isAuthenticated) {
        mainContainer.classList.add('authenticated');
    }
}

// Logout function
function logout() {
    if (confirm('Are you sure you want to logout?')) {
        // Clear authentication state
        isAuthenticated = false;
        currentUser = null;
        localStorage.removeItem('vitaClinic_user');

        // Clear all data
        clients = [];
        animals = [];
        appointments = [];
        medicalRecords = [];
        settings = null;

        // Show login screen
        showLoginScreen();
        showNotification('You have been logged out successfully.', 'success');
    }
}

// Navigation
function showPage(pageName) {
    // Hide all pages
    document.querySelectorAll('.page-section').forEach(section => {
        section.classList.remove('active');
    });
    
    // Remove active class from all menu items
    document.querySelectorAll('.sidebar-menu a').forEach(link => {
        link.classList.remove('active');
    });
    
    // Show selected page
    document.getElementById(`page-${pageName}`).classList.add('active');
    document.getElementById(`menu-${pageName}`).classList.add('active');
    
    // Update page title
    const titles = {
        'dashboard': 'Dashboard',
        'clients': 'Clients',
        'animals': 'Animals',
        'appointments': 'Appointments',
        'medical-records': 'Medical Records',
        'analytics': 'Analytics',
        'settings': 'Settings'
    };
    document.getElementById('pageTitle').textContent = `${titles[pageName]} - VitaClinic`;
    
    // Load page-specific data
    switch(pageName) {
        case 'dashboard':
            updateDashboard();
            break;
        case 'clients':
            loadClientsTable();
            break;
        case 'animals':
            loadAnimalsTable();
            break;
        case 'appointments':
            loadAppointmentsTable();
            break;
        case 'medical-records':
            loadMedicalRecordsTable();
            break;
        case 'analytics':
            loadAnalytics();
            break;
        case 'settings':
            loadSettings();
            break;
    }
}

// API Helper
async function apiCall(endpoint, options = {}) {
    try {
        const response = await fetch(`${API_BASE}${endpoint}`, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            },
            ...options
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        if (response.status === 204) {
            return null;
        }

        return await response.json();
    } catch (error) {
        console.error('API call failed:', error);
        throw error;
    }
}

// Load All Data
async function loadAllData() {
    try {
        [clients, animals, appointments, medicalRecords, settings] = await Promise.all([
            apiCall('/api/clients'),
            apiCall('/api/animals'),
            apiCall('/api/appointments'),
            apiCall('/api/medicalrecords'),
            apiCall('/api/settings')
        ]);
        
        if (settings?.clinicName) {
            document.getElementById('clinicNameDisplay').textContent = settings.clinicName;
        }
    } catch (error) {
        console.error('Failed to load data:', error);
    }
}

// Dashboard
async function updateDashboard() {
    const analyticsData = await apiCall('/api/analytics/dashboard');
    
    document.getElementById('dashTotalClients').textContent = analyticsData.totalClients || 0;
    document.getElementById('dashTotalAnimals').textContent = analyticsData.totalAnimals || 0;
    document.getElementById('dashTodayAppointments').textContent = analyticsData.todayAppointments || 0;
    document.getElementById('dashWeekAppointments').textContent = analyticsData.weekAppointments || 0;
    
    // Load today's appointments
    const today = new Date().toISOString().split('T')[0];
    const todayAppointments = appointments.filter(apt => 
        apt.appointmentDateTime.startsWith(today)
    );
    
    const listHtml = todayAppointments.length === 0
        ? '<div class="empty-state"><div class="empty-state-icon">📅</div><p>No appointments scheduled for today</p></div>'
        : `<table>
            <thead>
                <tr>
                    <th>Time</th>
                    <th>Pet</th>
                    <th>Owner</th>
                    <th>Type</th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
                ${todayAppointments.map(apt => {
                    const time = new Date(apt.appointmentDateTime).toLocaleTimeString('en-US', {hour: '2-digit', minute: '2-digit'});
                    return `<tr>
                        <td>${time}</td>
                        <td>${apt.petName || apt.animal?.name || 'N/A'}</td>
                        <td>${apt.ownerName || apt.animal?.client?.firstName + ' ' + apt.animal?.client?.lastName || 'N/A'}</td>
                        <td>${apt.appointmentType}</td>
                        <td><span style="padding: 4px 8px; background: #e6f7ff; border-radius: 4px; font-size: 12px;">${apt.status}</span></td>
                    </tr>`;
                }).join('')}
            </tbody>
        </table>`;
    
    document.getElementById('dashTodayAppointmentsList').innerHTML = listHtml;
}

// Clients
function loadClientsTable() {
    const tableHtml = clients.length === 0
        ? '<div class="empty-state"><div class="empty-state-icon">👥</div><p>No clients found. Add your first client to get started!</p></div>'
        : `<table>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Phone</th>
                    <th>Status</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                ${clients.map(client => `
                    <tr>
                        <td>${client.firstName} ${client.lastName}</td>
                        <td>${client.email}</td>
                        <td>${client.phone}</td>
                        <td><span style="padding: 4px 8px; background: #d4edda; border-radius: 4px; font-size: 12px;">Active</span></td>
                        <td>
                            <button class="btn btn-small btn-danger" onclick="deleteClient(${client.id})">Delete</button>
                        </td>
                    </tr>
                `).join('')}
            </tbody>
        </table>`;
    
    document.getElementById('clientsTableContainer').innerHTML = tableHtml;
}

function showAddClientModal() {
    document.getElementById('clientModalTitle').textContent = 'Add New Client';
    document.getElementById('clientForm').reset();
    document.getElementById('clientId').value = '';
    document.getElementById('clientModal').style.display = 'block';
}

async function saveClient() {
    const clientData = {
        firstName: document.getElementById('clientFirstName').value,
        lastName: document.getElementById('clientLastName').value,
        email: document.getElementById('clientEmail').value,
        phone: document.getElementById('clientPhone').value,
        address: document.getElementById('clientAddress').value,
        status: 0,
        joinDate: new Date().toISOString(),
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
    };

    try {
        await apiCall('/api/clients', {
            method: 'POST',
            body: JSON.stringify(clientData)
        });

        closeModal('clientModal');
        await loadAllData();
        loadClientsTable();
        showNotification('Client added successfully!', 'success');
    } catch (error) {
        showNotification('Failed to save client', 'error');
    }
}

async function deleteClient(id) {
    if (!confirm('Are you sure you want to delete this client?')) return;

    try {
        await apiCall(`/api/clients/${id}`, { method: 'DELETE' });
        await loadAllData();
        loadClientsTable();
        showNotification('Client deleted successfully!', 'success');
    } catch (error) {
        showNotification('Failed to delete client', 'error');
    }
}

// Animals
function loadAnimalsTable() {
    const tableHtml = animals.length === 0
        ? '<div class="empty-state"><div class="empty-state-icon">🐾</div><p>No animals found. Add your first animal to get started!</p></div>'
        : `<table>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Species</th>
                    <th>Breed</th>
                    <th>Owner</th>
                    <th>Weight</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                ${animals.map(animal => {
                    const client = clients.find(c => c.id === animal.clientId);
                    return `
                    <tr>
                        <td>${animal.name}</td>
                        <td>${animal.species}</td>
                        <td>${animal.breed || 'Mixed'}</td>
                        <td>${client ? client.firstName + ' ' + client.lastName : 'Unknown'}</td>
                        <td>${animal.weight} kg</td>
                        <td>
                            <button class="btn btn-small btn-danger" onclick="deleteAnimal(${animal.id})">Delete</button>
                        </td>
                    </tr>
                `}).join('')}
            </tbody>
        </table>`;
    
    document.getElementById('animalsTableContainer').innerHTML = tableHtml;
}

function showAddAnimalModal() {
    document.getElementById('animalModalTitle').textContent = 'Add New Animal';
    document.getElementById('animalForm').reset();
    document.getElementById('animalId').value = '';
    
    // Populate client dropdown
    const clientSelect = document.getElementById('animalClient');
    clientSelect.innerHTML = '<option value="">Select Client</option>' +
        clients.map(c => `<option value="${c.id}">${c.firstName} ${c.lastName}</option>`).join('');
    
    document.getElementById('animalModal').style.display = 'block';
}

async function saveAnimal() {
    const animalData = {
        clientId: parseInt(document.getElementById('animalClient').value),
        name: document.getElementById('animalName').value,
        species: document.getElementById('animalSpecies').value,
        breed: document.getElementById('animalBreed').value,
        gender: document.getElementById('animalGender').value,
        weight: parseFloat(document.getElementById('animalWeight').value) || 0,
        dateOfBirth: new Date().toISOString(),
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
    };

    try {
        await apiCall('/api/animals', {
            method: 'POST',
            body: JSON.stringify(animalData)
        });

        closeModal('animalModal');
        await loadAllData();
        // Update dashboard counts since we added a new animal
        updateDashboard();
        // Refresh the animals table with new data
        loadAnimalsTable();
        showNotification('Animal added successfully!', 'success');
    } catch (error) {
        showNotification('Failed to save animal', 'error');
    }
}

async function deleteAnimal(id) {
    if (!confirm('Are you sure you want to delete this animal?')) return;

    try {
        await apiCall(`/api/animals/${id}`, { method: 'DELETE' });
        await loadAllData();
        loadAnimalsTable();
        showNotification('Animal deleted successfully!', 'success');
    } catch (error) {
        showNotification('Failed to delete animal', 'error');
    }
}

// Appointments
function loadAppointmentsTable() {
    const tableHtml = appointments.length === 0
        ? '<div class="empty-state"><div class="empty-state-icon">📅</div><p>No appointments scheduled. Schedule your first appointment!</p></div>'
        : `<table>
            <thead>
                <tr>
                    <th>Date & Time</th>
                    <th>Pet</th>
                    <th>Owner</th>
                    <th>Type</th>
                    <th>Status</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                ${appointments.map(apt => {
                    const date = new Date(apt.appointmentDateTime);
                    const animal = animals.find(a => a.id === apt.animalId);
                    const client = animal ? clients.find(c => c.id === animal.clientId) : null;
                    
                    return `
                    <tr>
                        <td>${date.toLocaleDateString()} ${date.toLocaleTimeString('en-US', {hour: '2-digit', minute: '2-digit'})}</td>
                        <td>${animal?.name || apt.petName || 'Unknown'}</td>
                        <td>${client ? client.firstName + ' ' + client.lastName : apt.ownerName || 'Unknown'}</td>
                        <td>${apt.appointmentType}</td>
                        <td><span style="padding: 4px 8px; background: #e6f7ff; border-radius: 4px; font-size: 12px;">${apt.status}</span></td>
                        <td>
                            <button class="btn btn-small btn-danger" onclick="deleteAppointment(${apt.id})">Delete</button>
                        </td>
                    </tr>
                `}).join('')}
            </tbody>
        </table>`;
    
    document.getElementById('appointmentsTableContainer').innerHTML = tableHtml;
}

function showAddAppointmentModal() {
    document.getElementById('appointmentModalTitle').textContent = 'Schedule Appointment';
    document.getElementById('appointmentForm').reset();
    document.getElementById('appointmentId').value = '';
    
    // Populate animal dropdown
    const animalSelect = document.getElementById('appointmentAnimal');
    animalSelect.innerHTML = '<option value="">Select Animal</option>' +
        animals.map(a => {
            const client = clients.find(c => c.id === a.clientId);
            return `<option value="${a.id}">${a.name} (${client?.firstName || 'Unknown'})</option>`;
        }).join('');
    
    // Set default datetime to tomorrow at 10 AM
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    tomorrow.setHours(10, 0, 0, 0);
    document.getElementById('appointmentDateTime').value = tomorrow.toISOString().slice(0, 16);
    
    document.getElementById('appointmentModal').style.display = 'block';
}

async function saveAppointment() {
    const animalId = parseInt(document.getElementById('appointmentAnimal').value);
    const animal = animals.find(a => a.id === animalId);
    const client = animal ? clients.find(c => c.id === animal.clientId) : null;
    
    const appointmentData = {
        animalId: animalId,
        veterinarianId: 1,
        petName: animal?.name || '',
        ownerName: client ? `${client.firstName} ${client.lastName}` : '',
        appointmentType: document.getElementById('appointmentType').value,
        appointmentDateTime: new Date(document.getElementById('appointmentDateTime').value).toISOString(),
        veterinarianName: 'Dr. Smith',
        status: 0,
        description: document.getElementById('appointmentDescription').value,
        notes: '',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
    };

    try {
        await apiCall('/api/appointments', {
            method: 'POST',
            body: JSON.stringify(appointmentData)
        });

        closeModal('appointmentModal');
        await loadAllData();
        loadAppointmentsTable();
        updateDashboard();
        showNotification('Appointment scheduled successfully! Notifications will be sent if enabled in settings.', 'success');
    } catch (error) {
        showNotification('Failed to save appointment', 'error');
    }
}

async function deleteAppointment(id) {
    if (!confirm('Are you sure you want to delete this appointment?')) return;

    try {
        await apiCall(`/api/appointments/${id}`, { method: 'DELETE' });
        await loadAllData();
        loadAppointmentsTable();
        updateDashboard();
        showNotification('Appointment deleted successfully!', 'success');
    } catch (error) {
        showNotification('Failed to delete appointment', 'error');
    }
}

// Medical Records
function loadMedicalRecordsTable() {
    const tableHtml = medicalRecords.length === 0
        ? '<div class="empty-state"><div class="empty-state-icon">📋</div><p>No medical records found. Add your first medical record!</p></div>'
        : `<table>
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Animal</th>
                    <th>Owner</th>
                    <th>Diagnosis</th>
                    <th>Treatment</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                ${medicalRecords.map(record => {
                    const animal = animals.find(a => a.id === record.animalId);
                    const client = animal ? clients.find(c => c.id === animal.clientId) : null;
                    const date = new Date(record.recordDate);
                    
                    return `
                    <tr>
                        <td>${date.toLocaleDateString()}</td>
                        <td>${animal?.name || 'Unknown'}</td>
                        <td>${client ? client.firstName + ' ' + client.lastName : 'Unknown'}</td>
                        <td>${record.diagnosis || 'N/A'}</td>
                        <td>${record.treatment || 'N/A'}</td>
                        <td>
                            <button class="btn btn-small btn-danger" onclick="deleteMedicalRecord(${record.id})">Delete</button>
                        </td>
                    </tr>
                `}).join('')}
            </tbody>
        </table>`;
    
    document.getElementById('medicalRecordsTableContainer').innerHTML = tableHtml;
}

function showAddMedicalRecordModal() {
    document.getElementById('medicalRecordModalTitle').textContent = 'Add Medical Record';
    document.getElementById('medicalRecordForm').reset();
    document.getElementById('medicalRecordId').value = '';
    
    // Populate animal dropdown
    const animalSelect = document.getElementById('medicalRecordAnimal');
    animalSelect.innerHTML = '<option value="">Select Animal</option>' +
        animals.map(a => {
            const client = clients.find(c => c.id === a.clientId);
            return `<option value="${a.id}">${a.name} (${client?.firstName || 'Unknown'})</option>`;
        }).join('');
    
    // Set default date to today
    document.getElementById('medicalRecordDate').value = new Date().toISOString().split('T')[0];
    
    document.getElementById('medicalRecordModal').style.display = 'block';
}

async function saveMedicalRecord() {
    const recordData = {
        animalId: parseInt(document.getElementById('medicalRecordAnimal').value),
        diagnosis: document.getElementById('medicalRecordDiagnosis').value,
        treatment: document.getElementById('medicalRecordTreatment').value,
        medication: document.getElementById('medicalRecordMedication').value,
        notes: document.getElementById('medicalRecordNotes').value,
        recordDate: new Date(document.getElementById('medicalRecordDate').value).toISOString(),
        nextCheckupDate: document.getElementById('medicalRecordNextCheckup').value ? 
            new Date(document.getElementById('medicalRecordNextCheckup').value).toISOString() : null,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
    };

    try {
        await apiCall('/api/medicalrecords', {
            method: 'POST',
            body: JSON.stringify(recordData)
        });

        closeModal('medicalRecordModal');
        await loadAllData();
        loadMedicalRecordsTable();
        showNotification('Medical record added successfully!', 'success');
    } catch (error) {
        showNotification('Failed to save medical record', 'error');
    }
}

async function deleteMedicalRecord(id) {
    if (!confirm('Are you sure you want to delete this medical record?')) return;

    try {
        await apiCall(`/api/medicalrecords/${id}`, { method: 'DELETE' });
        await loadAllData();
        loadMedicalRecordsTable();
        showNotification('Medical record deleted successfully!', 'success');
    } catch (error) {
        showNotification('Failed to delete medical record', 'error');
    }
}

// Analytics
async function loadAnalytics() {
    try {
        const [dashboardData, appointmentsByType, speciesDistribution, appointmentsByStatus] = await Promise.all([
            apiCall('/api/analytics/dashboard'),
            apiCall('/api/analytics/appointments-by-type'),
            apiCall('/api/analytics/species-distribution'),
            apiCall('/api/analytics/appointments-by-status')
        ]);
        
        document.getElementById('analyticsClients').textContent = dashboardData.activeClients || 0;
        document.getElementById('analyticsMonthAppointments').textContent = dashboardData.monthAppointments || 0;
        document.getElementById('analyticsRevenue').textContent = '$' + (dashboardData.totalRevenue || 0).toFixed(2);
        document.getElementById('analyticsPending').textContent = '$' + (dashboardData.pendingRevenue || 0).toFixed(2);
        
        // Display charts as simple lists
        document.getElementById('chartAppointmentsByType').innerHTML = createSimpleChart(appointmentsByType, 'type', 'count');
        document.getElementById('chartSpeciesDistribution').innerHTML = createSimpleChart(speciesDistribution, 'species', 'count');
        document.getElementById('chartAppointmentsByStatus').innerHTML = createSimpleChart(appointmentsByStatus, 'status', 'count');
    } catch (error) {
        console.error('Failed to load analytics:', error);
    }
}

function createSimpleChart(data, labelKey, valueKey) {
    if (!data || data.length === 0) {
        return '<p style="color: #999;">No data available</p>';
    }
    
    const total = data.reduce((sum, item) => sum + item[valueKey], 0);
    
    return `<div>
        ${data.map(item => {
            const percentage = total > 0 ? (item[valueKey] / total * 100).toFixed(1) : 0;
            return `
                <div style="margin-bottom: 10px;">
                    <div style="display: flex; justify-content: space-between; margin-bottom: 5px;">
                        <span>${item[labelKey] || 'Unknown'}</span>
                        <span><strong>${item[valueKey]}</strong> (${percentage}%)</span>
                    </div>
                    <div style="background: #e0e0e0; height: 10px; border-radius: 5px;">
                        <div style="background: #667eea; height: 100%; width: ${percentage}%; border-radius: 5px;"></div>
                    </div>
                </div>
            `;
        }).join('')}
    </div>`;
}

// Settings
async function loadSettings() {
    try {
        settings = await apiCall('/api/settings');
        
        document.getElementById('settingsClinicName').value = settings.clinicName || '';
        document.getElementById('settingsPhone').value = settings.phone || '';
        document.getElementById('settingsEmail').value = settings.email || '';
        document.getElementById('settingsWebsite').value = settings.website || '';
        document.getElementById('settingsAddress').value = settings.address || '';
        document.getElementById('settingsBusinessHours').value = settings.businessHours || '';
        document.getElementById('settingsEmailNotifications').checked = settings.emailNotificationsEnabled || false;
        document.getElementById('settingsSmsNotifications').checked = settings.smsNotificationsEnabled || false;
        document.getElementById('settingsReminderHours').value = settings.appointmentReminderHours || 24;
    } catch (error) {
        console.error('Failed to load settings:', error);
    }
}

async function saveSettings() {
    const settingsData = {
        clinicName: document.getElementById('settingsClinicName').value,
        phone: document.getElementById('settingsPhone').value,
        email: document.getElementById('settingsEmail').value,
        website: document.getElementById('settingsWebsite').value,
        address: document.getElementById('settingsAddress').value,
        businessHours: document.getElementById('settingsBusinessHours').value,
        emailNotificationsEnabled: document.getElementById('settingsEmailNotifications').checked,
        smsNotificationsEnabled: document.getElementById('settingsSmsNotifications').checked,
        appointmentReminderHours: parseInt(document.getElementById('settingsReminderHours').value),
        updatedAt: new Date().toISOString()
    };

    try {
        await apiCall('/api/settings', {
            method: 'PUT',
            body: JSON.stringify(settingsData)
        });

        settings = settingsData;
        document.getElementById('clinicNameDisplay').textContent = settingsData.clinicName;
        showNotification('Settings saved successfully!', 'success');
    } catch (error) {
        showNotification('Failed to save settings', 'error');
    }
}

// Modals
function closeModal(modalId) {
    document.getElementById(modalId).style.display = 'none';
}

// Close modal when clicking outside
window.onclick = function(event) {
    if (event.target.classList.contains('modal')) {
        event.target.style.display = 'none';
    }
}

// Notifications
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = type === 'success' ? 'success' : 'error';
    notification.textContent = message;
    notification.style.position = 'fixed';
    notification.style.top = '20px';
    notification.style.right = '20px';
    notification.style.zIndex = '10000';
    notification.style.minWidth = '300px';
    notification.style.animation = 'slideIn 0.3s';
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.style.animation = 'fadeOut 0.3s';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}
