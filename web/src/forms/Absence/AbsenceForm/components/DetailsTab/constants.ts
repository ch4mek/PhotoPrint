import { AbsenceDetailsFields } from '@/types/absences';

export const fieldNames: Record<AbsenceDetailsFields, AbsenceDetailsFields> = {
	name: 'name',
	type: 'type',
	trackingUnitType: 'trackingUnitType',
	activityType: 'activityType',
	comment: 'comment',
	isProhibitOnProbationPeriod: 'isProhibitOnProbationPeriod',
	isIncludeWeekends: 'isIncludeWeekends',
	isIncludeHolidays: 'isIncludeHolidays',
	isRequireAttachments: 'isRequireAttachments',
	accrualType: 'accrualType',
	transferType: 'transferType',
};

export const formSections = {
	details: 'Details',
	limits: 'Limits',
};

export const absenceDetailsFieldData: Record<
	string,
	{
		name: AbsenceDetailsFields;
		label: string;
		valueField?: string;
		description?: string;
	}
> = {
	name: {
		name: fieldNames.name,
		label: 'Name',
	},
	type: {
		name: fieldNames.type,
		label: 'Absence type',
		valueField: 'name',
	},
	trackingUnitType: {
		name: fieldNames.trackingUnitType,
		label: 'Tracking unit ',
		valueField: 'name',
	},
	activityType: {
		name: fieldNames.activityType,
		label: 'Activity type',
		valueField: 'name',
		description: `How should the employee's requested time off be considered as working time or paid time? For example, 
    “working from home” would be considered working time, paid time`,
	},
	accrualType: {
		name: fieldNames.accrualType,
		label: 'Accrual type',
		valueField: 'name',
	},
	transferType: {
		name: fieldNames.transferType,
		label: 'Transfer type',
		valueField: 'name',
	},
	comment: {
		name: fieldNames.comment,
		label: 'Comment',
	},
};

export const detailsCheckBoxesFieldData = [
	{
		name: fieldNames.isProhibitOnProbationPeriod,
		label: 'Prohibit requesting during probation period',
		description:
			'If this option is checked, employees on probation will not be able to request this absence',
	},
	{
		name: fieldNames.isIncludeWeekends,
		label: 'Include weekends',
		description:
			'If this option is checked, weekends will be included in the absence calculation',
	},
	{
		name: fieldNames.isIncludeHolidays,
		label: 'Include holidays',
		description:
			'If this option is checked, holidays will be included in the absence calculation',
	},
	{
		name: fieldNames.isRequireAttachments,
		label: 'Require attachment of documents',
		description:
			'If this option is checked, documents must be attached to the request',
	},
];

export const MIN_COMMENT_COUNT_OF_ROWS = 3;

export const wrapperStyles = {
	alignItems: 'flex-start',
	padding: 0,
};
