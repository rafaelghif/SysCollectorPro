# 🛡️ SysCollectoPro – Enterprise IT Asset & Configuration Audit Tool

**SysCollectoPro** is a robust, enterprise-grade application for automated IT asset discovery, configuration auditing, and compliance documentation across Windows-based environments. This application empowers IT departments, infrastructure teams, and compliance auditors to perform comprehensive diagnostics and generate structured, actionable CSV logs per host—enabling centralized, standardized, and repeatable audits at scale.

---

## 🚀 Key Capabilities

- **Comprehensive System Inventory**
  - Collects detailed information on CPU, RAM, GPU, storage devices, and motherboard
  - Detects physical or virtual machine status via chassis type
  - Reports real-time device health and capacity metrics

- **Operating System Intelligence**
  - Gathers OS name, version (e.g., "22H2"), and full build number (e.g., "26100.4770")
  - Captures install date, architecture, and last boot time
  - Audits Windows activation and license status

- **Security & Compliance Reporting**
  - Audits Secure Boot, TPM version, and BitLocker encryption state
  - Reports Windows Defender status (enabled/disabled, real-time protection)
  - Verifies domain membership and administrative privileges

- **Network Adapter Audit**
  - Enumerates adapter names, MAC addresses, and IP configuration (DHCP/static)
  - Reports connection status (up/down), speed (Mbps), and interface type

- **Site Metadata & Annotation**
  - Supports configurable fields: `Site`, `Zone`, `Position`, and `Notes`
  - Facilitates physical location mapping and audit traceability

- **Structured CSV Export**
  - Each run of this application generates a `.csv` log with complete audit data
  - Output is ready for ingestion into Excel, Power BI, CMDBs, or ITSM platforms

---

## ⚙️ Configuration

SysCollectoPro uses an INI configuration file to control application behavior and output.  
The config file is located at:

``` plaintext
build directory/Configs/SysCollectorPro.config.ini
```

### Example Configuration

``` ini
[General] 
AppVersion=1.0
[Output] 
OutputPath=C:\SysCollectorPro
FileName=SysCollectorPro 
FileFormat=csv
```

#### **Configuration Sections**

- **[General]**
  - `AppVersion`: The application version.

- **[Output]**
  - `OutputPath`: Directory where audit logs are saved.
  - `FileName`: Base name for the exported log files.
  - `FileFormat`: Export format (e.g., `csv`).

> **Tip:** Edit these values to customize where and how your audit logs are generated.

---

## 📤 Sample Export Headers

```csv
Hostname,ChassisType,SerialNumber,Manufacturer,Model,BIOSVersion,BIOSDate, CPUs,TotalCPUCores, RAMModules,RAMTotalGB, StorageDevices,StorageTotalGB,TotalStorageCount, GPUs,TotalGpuCount, OSName,OSVersion,InstallDate,Architecture,SecureBoot,TPMVersion, BitLockerStatus,Domain,LoggedInUser,LastBootTime, NetworkAdapters, AdminUser,WindowsActivated,AntivirusStatus, Site,Zone,Position,Notes,AuditDate
```

---
