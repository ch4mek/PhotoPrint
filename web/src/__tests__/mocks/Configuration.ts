import { Timezone } from '@/components/Timeline/types';
import { Roles } from '@/constants/roles';
import {
	Configuration as ConfigurationType,
	LocationTypeId,
	PermissionResourceTypeId,
	PermissionTypeId,
} from '@/types/configuration';

export const Configuration: ConfigurationType = {
	persistentValues: {
		locationTypes: [
			{
				id: 'Minsk',
				name: 'Minsk',
				description: 'Minsk',
				location: null,
			},
			{
				id: 'Vitebsk',
				name: 'Vitebsk',
				description: 'Vitebsk',
				location: null,
			},
			{
				id: 'Warsaw',
				name: 'Warsaw',
				description: 'Warsaw',
				location: null,
			},
			{
				id: 'Tbilisi',
				name: 'Tbilisi',
				description: 'Tbilisi',
				location: null,
			},
		],
		jsStackTypes: [
			{
				id: 'Node',
				name: 'Node',
				description: 'Node',
				location: null,
			},
			{
				id: 'React',
				name: 'React',
				description: 'React',
				location: null,
			},
			{
				id: 'ReactNative',
				name: 'ReactNative',
				description: 'React Native',
				location: null,
			},
		],
		equipmentLocationTypes: [
			{
				id: 'Belarus',
				name: 'Belarus',
				description: 'Belarus',
				location: null,
			},
			{
				id: 'Poland',
				name: 'Poland',
				description: 'Poland',
				location: null,
			},
		],
		equipmentAvailabilityTypes: [
			{
				id: 'Available',
				name: 'Available',
				description: 'Available',
				location: null,
			},
			{
				id: 'All',
				name: 'All',
				description: 'All',
				location: null,
			},
		],
		moneyCurrencyTypes: [
			{
				id: 'Byn',
				name: 'Byn',
				description: 'BYN',
				location: null,
			},
			{
				id: 'Eur',
				name: 'Eur',
				description: 'EUR',
				location: null,
			},
			{
				id: 'Pln',
				name: 'Pln',
				description: 'PLN',
				location: null,
			},
			{
				id: 'Usd',
				name: 'Usd',
				description: 'USD',
				location: null,
			},
		],
		permissionTypes: [
			{
				id: PermissionTypeId.TrackBillableWorkingHours,
				name: PermissionTypeId.TrackBillableWorkingHours,
				description: PermissionTypeId.TrackBillableWorkingHours,
				location: null,
			},
		],
		permissionResourceTypes: [
			{
				id: PermissionResourceTypeId.Project,
				name: PermissionResourceTypeId.Project,
				description: PermissionResourceTypeId.Project,
				location: null,
			},
		],
		equipmentBrokenStatuses: [],
		fireTypes: [],
		educationLevelTypes: [],
		studyFormTypes: [],
		merchItemStatuses: [],
		absenceTypes: [],
		activityTypes: [],
		trackingUnitTypes: [],
		merchStorageStatuses: [],
		accrualTypes: [],
		transferTypes: [],
	},
	configurableValues: {
		offices: [
			{
				id: 1,
				name: 'Minsk',
				location: LocationTypeId.Minsk,
				timeZone: Timezone.MINSK,
				timeZoneDisplayName: '',
				workspaces: [],
				meetingRooms: [],
			},
			{
				id: 3,
				name: 'Warsaw',
				location: LocationTypeId.Warsaw,
				timeZone: Timezone.MINSK,
				timeZoneDisplayName: '',
				workspaces: [],
				meetingRooms: [],
			},
		],
		roleTypes: [
			{
				id: Roles.CEO,
				name: 'CEO',
				description: 'CEO',
				location: null,
			},
			{
				id: Roles.CTO,
				name: 'CTO',
				description: 'CTO',
				location: null,
			},
			{
				id: Roles.HD,
				name: 'HeadOfDepartment',
				description: 'Head of Department',
				location: null,
			},
			{
				id: Roles.GM,
				name: 'GroupManager',
				description: 'Group Manager',
				location: null,
			},
			{
				id: Roles.SE,
				name: 'Employee',
				description: 'Employee',
				location: null,
			},
			{
				id: Roles.SA,
				name: 'SystemAdministrator',
				description: 'System Administrator',
				location: null,
			},
			{
				id: Roles.IN,
				name: 'Intern',
				description: 'Intern',
				location: null,
			},
		],
		departmentTypes: [
			{
				id: 'Java',
				shortName: 'Java',
				fullName: 'Java',
			},
			{
				id: 'Python',
				shortName: 'Python',
				fullName: 'Python',
			},
			{
				id: 'QA',
				shortName: 'QA',
				fullName: 'QA',
			},
			{
				id: 'BA',
				shortName: 'BA',
				fullName: 'BA',
			},
			{
				id: 'HR',
				shortName: 'HR',
				fullName: 'HR',
			},
			{
				id: 'Marketing',
				shortName: 'Marketing',
				fullName: 'Marketing',
			},
			{
				id: 'Sales',
				shortName: 'Sales',
				fullName: 'Sales',
			},
			{
				id: 'Mobile',
				shortName: 'Mobile',
				fullName: 'Mobile',
			},
			{
				id: 'PHP',
				shortName: 'PHP',
				fullName: 'PHP',
			},
			{
				id: 'DevOps',
				shortName: 'DevOps',
				fullName: 'DevOps',
			},
			{
				id: 'Administration',
				shortName: 'Administration',
				fullName: 'Administration',
			},
			{
				id: 'Js',
				shortName: 'Js',
				fullName: 'JavaScript',
			},
			{
				id: 'Net',
				shortName: 'Net',
				fullName: '.NET',
			},
			{
				id: 'Design',
				shortName: 'Design',
				fullName: 'Design',
			},
			{
				id: 'Ruby',
				shortName: 'Ruby',
				fullName: 'Ruby',
			},
			{
				id: 'PMO',
				shortName: 'PMO',
				fullName: 'PMO',
			},
			{
				id: 'LD',
				shortName: 'LD',
				fullName: 'LD',
			},
			{
				id: 'SysAdmin',
				shortName: 'System Administrators',
				fullName: 'System Administrators',
			},
		],
		equipmentTypes: [
			{
				id: 'AccessPoint',
				shortName: 'AccessPoint',
				fullName: 'Access point',
			},
			{
				id: 'DesktopComputer',
				shortName: 'DesktopComputer',
				fullName: 'Desktop computer',
			},
			{
				id: 'ExternalHardDrive',
				shortName: 'ExternalHardDrive',
				fullName: 'External hard drive',
			},
			{
				id: 'Firewall',
				shortName: 'Firewall',
				fullName: 'Firewall',
			},
			{
				id: 'FlashDrive',
				shortName: 'FlashDrive',
				fullName: 'Flash drive',
			},
			{
				id: 'Headset',
				shortName: 'Headset',
				fullName: 'Headset',
			},
			{
				id: 'Keyboard',
				shortName: 'Keyboard',
				fullName: 'Keyboard',
			},
			{
				id: 'Laptop',
				shortName: 'Laptop',
				fullName: 'Laptop',
			},
			{
				id: 'MacBook',
				shortName: 'MacBook',
				fullName: 'MacBook',
			},
			{
				id: 'MacBookMini',
				shortName: 'MacBookMini',
				fullName: 'MacBook mini',
			},
			{
				id: 'MobilePhone',
				shortName: 'MobilePhone',
				fullName: 'Mobile phone',
			},
			{
				id: 'Monitor',
				shortName: 'Monitor',
				fullName: 'Monitor',
			},
			{
				id: 'Mouse',
				shortName: 'Mouse',
				fullName: 'Mouse',
			},
			{
				id: 'MouseWithKeyboard',
				shortName: 'MouseWithKeyboard',
				fullName: 'Mouse + keyboard',
			},
			{
				id: 'MultiAdapter',
				shortName: 'MultiAdapter',
				fullName: 'Multi-adapter',
			},
			{
				id: 'NetworkAdapter',
				shortName: 'NetworkAdapter',
				fullName: 'Network adapter',
			},
			{
				id: 'PcAccessoriesHdd',
				shortName: 'PcAccessoriesHdd',
				fullName: 'PC accessory: HDD',
			},
			{
				id: 'PcAccessoriesRam',
				shortName: 'PcAccessoriesRam',
				fullName: 'PC accessory: RAM',
			},
			{
				id: 'PcAccessoriesSsd',
				shortName: 'PcAccessoriesSsd',
				fullName: 'PC accessory: SSD',
			},
			{
				id: 'Printer',
				shortName: 'Printer',
				fullName: 'Printer',
			},
			{
				id: 'Router',
				shortName: 'Router',
				fullName: 'Router',
			},
			{
				id: 'Switch',
				shortName: 'Switch',
				fullName: 'Switch',
			},
			{
				id: 'WebCamera',
				shortName: 'WebCamera',
				fullName: 'Web camera',
			},
		],
		hardSkillGroups: [
			{
				id: 'AP',
				name: 'API Protocols',
			},
			{
				id: 'ARCH',
				name: 'Architectures',
			},
			{
				id: 'CID',
				name: 'CI/CD',
			},
			{
				id: 'CP',
				name: 'Cloud Providers',
			},
			{
				id: 'DB',
				name: 'Databases',
			},
			{
				id: 'FW',
				name: 'Frameworks',
			},
			{
				id: 'GTE',
				name: 'Git Environments',
			},
			{
				id: 'IDE',
				name: 'Integrated Development Environments (IDE)',
			},
			{
				id: 'LIB',
				name: 'Libraries',
			},
			{
				id: 'MB',
				name: 'Message Brokers',
			},
			{
				id: 'OS',
				name: 'Operating Systems',
			},
			{
				id: 'PL',
				name: 'Programming Languages',
			},
			{
				id: 'TEST',
				name: 'Tests',
			},
			{
				id: 'VCS',
				name: 'Version Control Systems (VCS)',
			},
			{
				id: 'UN',
				name: 'Undefined',
			},
		],
		seniorityLevels: [
			{
				id: 'Trainee',
				name: 'Trainee',
			},
			{
				id: 'JuniorL1',
				name: 'Junior -',
			},
			{
				id: 'JuniorL2',
				name: 'Junior =',
			},
			{
				id: 'JuniorL3',
				name: 'Junior +',
			},
			{
				id: 'MiddleL1',
				name: 'Middle -',
			},
			{
				id: 'MiddleL2',
				name: 'Middle =',
			},
			{
				id: 'MiddleL3',
				name: 'Middle +',
			},
			{
				id: 'SeniorL1',
				name: 'Senior -',
			},
			{
				id: 'SeniorL2',
				name: 'Senior =',
			},
			{
				id: 'SeniorL3',
				name: 'Senior +',
			},
			{
				id: 'Lead',
				name: 'Lead',
			},
			{
				id: 'Architect',
				name: 'Architect',
			},
		],
		englishLevels: [
			{
				id: 'A1',
				name: 'A1',
			},
			{
				id: 'A2',
				name: 'A2',
			},
			{
				id: 'B1L1',
				name: 'B1',
			},
			{
				id: 'B1L2',
				name: 'B1+',
			},
			{
				id: 'B2L1',
				name: 'B2',
			},
			{
				id: 'C1',
				name: 'C1',
			},
			{
				id: 'C2',
				name: 'C2',
			},
		],
		generalSenioritySeniorityLevels: [],
		merchStorages: [],
		merchSizes: [],
		softSkillSeniorityLevels: [],
		countries: [],
	},
};
