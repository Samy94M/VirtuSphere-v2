-- VM Tabelle
CREATE TABLE IF NOT EXISTS deploy_vms (
    id INT AUTO_INCREMENT PRIMARY KEY,
    mission_id INT NOT NULL,
    vm_name VARCHAR(255) NOT NULL,
    vm_hostname VARCHAR(255) NOT NULL,
    vm_domain VARCHAR(255),
    vm_os VARCHAR(255),
    vm_ram VARCHAR(255),
    vm_disk VARCHAR(255),
    vm_datastore VARCHAR(255),
    vm_datacenter VARCHAR(255),
    vm_guest_id VARCHAR(255),
    vm_creator VARCHAR(255),
    vm_status INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    vm_notes TEXT
);

-- Interface Tabelle
CREATE TABLE IF NOT EXISTS deploy_interfaces (
    id INT AUTO_INCREMENT PRIMARY KEY,
    vm_id INT NOT NULL,
    ip VARCHAR(255) NOT NULL,
    subnet VARCHAR(255) NOT NULL,
    gateway VARCHAR(255) NOT NULL,
    dns1 VARCHAR(255),
    dns2 VARCHAR(255),
    vlan VARCHAR(255),
    mac VARCHAR(255),
    FOREIGN KEY (vm_id) REFERENCES deploy_vms(id) ON DELETE CASCADE
);

-- Packages Tabelle (bereits von Ihnen bereitgestellt)
CREATE TABLE IF NOT EXISTS deploy_packages (
    id INT AUTO_INCREMENT PRIMARY KEY,
    package_name VARCHAR(255) NOT NULL,
    package_version VARCHAR(255) NOT NULL,
    package_status VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS deploy_vm_packages (
    vm_id INT NOT NULL,
    package_id INT NOT NULL,
    PRIMARY KEY (vm_id, package_id),
    FOREIGN KEY (vm_id) REFERENCES deploy_vms (id) ON DELETE CASCADE,
    FOREIGN KEY (package_id) REFERENCES deploy_packages (id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
