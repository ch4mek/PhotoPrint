import { initialAcceptPolicesValue } from '../components/ApprovalsTab/constants';
import { mapScope } from '../components/ApprovalsTab/utils/mapScope';
import { mapUser } from '../components/ApprovalsTab/utils/mapUser';
import { AbsenceFormValues } from '../types';

import { Absence } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export const convertInitialDataToForm = (
	data: Absence,
	absenceTypesOptions: SimpleConfigurableConfigurationItem[],
	activityTypesOptions: SimpleConfigurableConfigurationItem[],
	trackingUnitTypesOptions: SimpleConfigurableConfigurationItem[],
	accrualTypeOptions: SimpleConfigurableConfigurationItem[],
	transferTypeOptions: SimpleConfigurableConfigurationItem[]
): AbsenceFormValues => {
	const acceptPolicesArray = Array.isArray(data?.acceptPolicies)
		? data.acceptPolicies
		: [];

	const acceptPolicies =
		acceptPolicesArray.length > 0
			? acceptPolicesArray.map(
					({
						userId,
						substituteUserId,
						substituteUserFullName,
						userFullName,
						scope,
						substituteScope,
						...rest
					}) => ({
						user: mapUser(userId, userFullName),
						substituteUser: mapUser(substituteUserId, substituteUserFullName),
						scope: mapScope(scope),
						substituteScope: mapScope(substituteScope),
						userId,
						substituteUserId,
						...rest,
					})
			  )
			: initialAcceptPolicesValue;

	return {
		details: {
			...data,
			type: absenceTypesOptions.find(({ id }) => data.type === id) || null,
			trackingUnitType:
				trackingUnitTypesOptions.find(
					({ id }) => data.trackingUnitType === id
				) || null,
			activityType:
				activityTypesOptions.find(({ id }) => data.activityType === id) || null,
			accrualType:
				accrualTypeOptions.find(({ id }) => data.accrualType === id) || null,
			transferType:
				transferTypeOptions.find(({ id }) => data.transferType === id) || null,
		},
		approvals: {
			isSubstitutesToApproveAllowed: data.isSubstitutesToApproveAllowed,
			acceptPolicies,
		},
	};
};
