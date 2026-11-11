import { InitialFormValues } from '../types';

import { AgreementItem } from '@/types/absences';

export const convertInitialDataToForm = (
	agreement: AgreementItem
): InitialFormValues => {
	const { name, daysPerContractYear, isDefault } = agreement;

	return {
		name,
		daysPerContractYear: daysPerContractYear?.toString() || null,
		isDefault,
	};
};
