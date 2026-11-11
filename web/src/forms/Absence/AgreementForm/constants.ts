import { AgreementDetailsFields } from '@/types/absences';

import { InitialFormValues } from './types';

export const AGREEMENT_ENTITY_NAME = 'Agreement';
export const FORM_NAME = 'Absence agreement';

const fieldNames: Record<AgreementDetailsFields, AgreementDetailsFields> = {
	name: 'name',
	daysPerContractYear: 'daysPerContractYear',
	isDefault: 'isDefault',
};

export const initialFormValues: InitialFormValues = {
	name: '',
	daysPerContractYear: null,
	isDefault: false,
};

export const agreementsFieldData: Record<
	string,
	{
		name: AgreementDetailsFields;
		label: string;
		valueField?: string;
		description?: string;
	}
> = {
	name: {
		name: fieldNames.name,
		label: 'Agreement name',
	},
	daysPerContractYear: {
		name: fieldNames.daysPerContractYear,
		label: 'Number of accrual days per year',
	},
	isDefault: {
		name: fieldNames.isDefault,
		label: 'Set as default agreement',
		valueField: 'name',
		description: `If this option is checked, this agreement will automatically be assigned to any new user added`,
	},
};
