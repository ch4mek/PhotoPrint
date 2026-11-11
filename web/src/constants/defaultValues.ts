import { FromToProps } from '@/components/Grid/modals/types';
import { Configuration } from '@/types/configuration';

export const fromToDateObject: FromToProps<Date> = {
	from: null,
	to: null,
};

export const fromToCurrencyObject: FromToProps<string> = {
	from: null,
	to: null,
};

export const defaultConfiguration: Configuration = {
	persistentValues: {
		equipmentAvailabilityTypes: [],
		equipmentLocationTypes: [],
		jsStackTypes: [],
		locationTypes: [],
		moneyCurrencyTypes: [],
		permissionResourceTypes: [],
		permissionTypes: [],
		equipmentBrokenStatuses: [],
		fireTypes: [],
		educationLevelTypes: [],
		studyFormTypes: [],
		merchItemStatuses: [],
		merchTransactionTypes: [],
		absenceTypes: [],
		activityTypes: [],
		trackingUnitTypes: [],
		merchStorageStatuses: [],
		accrualTypes: [],
		transferTypes: [],
	},
	configurableValues: {
		merchSizes: [],
		merchStorages: [],
		roleTypes: [],
		departmentTypes: [],
		englishLevels: [],
		equipmentTypes: [],
		hardSkillGroups: [],
		offices: [],
		seniorityLevels: [],
		softSkillSeniorityLevels: [],
		countries: [],
		generalSenioritySeniorityLevels: [],
	},
};

export const DEFAULT_LOADER_COLUMN_SPAN_AMOUNT = 100;
