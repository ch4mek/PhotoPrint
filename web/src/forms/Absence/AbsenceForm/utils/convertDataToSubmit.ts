import { AbsenceFormValues } from '../types';

import { absencesType } from '@/constants/absence';
import { AbsencesFormValues } from '@/types/absences';

export const convertDataToSubmit = (
	values: AbsenceFormValues
): AbsencesFormValues => {
	const isDocumentsType = values.details.type?.name === absencesType.Documents;

	return {
		...values.details,
		...values.approvals,
		comment: values.details.comment || null,
		type: values.details.type?.id || '',
		trackingUnitType: values.details.trackingUnitType?.id || '',
		activityType: values.details.activityType?.id || '',
		accrualType: values.details.accrualType?.id || null,
		transferType: values.details.transferType?.id || null,
		isIncludeWeekends: isDocumentsType
			? true
			: values.details.isIncludeWeekends,
		isIncludeHolidays: isDocumentsType
			? true
			: values.details.isIncludeHolidays,
		acceptPolicies: values.approvals.acceptPolicies.map((p) => ({
			...p,
			scope: p.scope?.scope || '',
			userId: p.userId ?? null,
			substituteUserId: values.approvals.isSubstitutesToApproveAllowed
				? p.substituteUserId || null
				: null,
			substituteScope: values.approvals.isSubstitutesToApproveAllowed
				? p.substituteScope?.scope || null
				: null,
		})),
	};
};
