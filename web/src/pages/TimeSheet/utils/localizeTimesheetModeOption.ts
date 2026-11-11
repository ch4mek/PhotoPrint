import { timesheetModesIds } from '../constants';
import { TimesheetModeFilterOption, TimesheetModeLabelKey } from '../types';

const { departmentModeId, projectsModeId, personalModeId, groupModeId } =
	timesheetModesIds;

const modeLabelKeys = {
	[departmentModeId]: 'departmentMode',
	[projectsModeId]: 'projectsMode',
	[personalModeId]: 'personalMode',
	[groupModeId]: 'groupMode',
} as const satisfies Record<string, TimesheetModeLabelKey>;

type TimesheetModeId = keyof typeof modeLabelKeys;

type LocalizableOption = {
	id: string | number;
	fullName: string;
	shortName?: string;
};

const isTimesheetModeId = (id: string | number): id is TimesheetModeId =>
	id in modeLabelKeys;

export const localizeTimesheetModeOption = <T extends LocalizableOption>(
	option: T,
	t: (key: TimesheetModeLabelKey) => string
): T => {
	if (!isTimesheetModeId(option.id)) {
		return option;
	}

	const label = t(modeLabelKeys[option.id]);

	if (option.shortName !== undefined) {
		return { ...option, fullName: label, shortName: label };
	}

	return { ...option, fullName: label };
};

export const localizeTimesheetModeOptions = <T extends LocalizableOption>(
	options: T[],
	t: (key: TimesheetModeLabelKey) => string
) => options.map((option) => localizeTimesheetModeOption(option, t));

export const resolveCanonicalTimesheetModeChange = (
	newRole: TimesheetModeFilterOption | null,
	canonicalOptions: TimesheetModeFilterOption[],
	fallback: TimesheetModeFilterOption
): TimesheetModeFilterOption => {
	if (!newRole) {
		return fallback;
	}

	return (
		canonicalOptions.find((option) => option.id === newRole.id) ?? fallback
	);
};
