export enum ProjectFormGridArea {
	InputFields = 'inputFields',
	RadioFields = 'radioFields',
	ColorsFields = 'colorFields',
}

export const PROJECT_FORM_FIELDS = {
	name: {
		name: 'name',
		label: 'Name',
	},
	client: {
		name: 'client',
		label: 'Client',
	},
	manager: {
		name: 'manager',
		label: 'Project manager',
	},
	isPublic: {
		title: 'Project type:',
		name: 'isPublic',
		positiveLabel: 'Public',
		negativeLabel: 'Private',
	},
	isBillable: {
		title: 'Are hours billable?',
		name: 'isBillable',
		positiveLabel: 'Yes',
		negativeLabel: 'No',
	},
	color: {
		name: 'color',
	},
};

export const IS_PUBLIC_GAP = 80;
export const IS_BILLABLE_GAP = 100;
