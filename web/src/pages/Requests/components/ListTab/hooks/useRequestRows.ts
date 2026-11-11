import { useMemo } from 'react';

import { RequestGridRow } from '../types';

import { absencesType } from '@/constants/absence';
import { isChiefRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import {
	checkIsDocumentRequest,
	getAbsenceTrackingUnitTypeValue,
} from '@/helpers/absences';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { formatDateString } from '@/helpers/dateToISOString';
import { InquiryData } from '@/types/absences';
import { UserClaims } from '@/types/claims';
import { RoleType } from '@/types/configuration';

export function useRequestRows(
	rolesTypes: RoleType[],
	data?: InquiryData[]
): RequestGridRow[] {
	const { role, id: authId, claims } = useAuth();

	const hasHRModuleClaim = checkIsHasClaims(
		[UserClaims.ManageHRModule],
		claims
	);

	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const rows: RequestGridRow[] = useMemo(() => {
		if (!data || !data.length) {
			return [];
		}

		return data.map(
			({
				id,
				userId,
				totalHours,
				status,
				dateTo,
				dateFrom,
				createdAtUtc,
				userFullName,
				isUserActive,
				inquiryType,
				userDepartmentId,
				inquiryTemplateName,
				userPosition,
				notes,
				totalAccountantDays,
				managedByRoleRequest,
				userInquiryAccepts,
			}) => {
				const isDocumentRequest = checkIsDocumentRequest(inquiryType);

				const { quantity, quantityType } = getAbsenceTrackingUnitTypeValue(
					totalAccountantDays,
					totalHours
				);

				const position =
					rolesTypes.find(({ id: roleTypeId }) => roleTypeId === userPosition)
						?.description || '';

				const departmentFullName =
					departmentTypes?.find(
						(department) => department.id === userDepartmentId
					)?.fullName || '';

				const canManageByRole = isChiefRole(role)
					? !isChiefRole(userPosition)
					: userId !== authId && managedByRoleRequest;

				const canManageRequest =
					isUserActive && (hasHRModuleClaim || canManageByRole);

				return {
					id,
					userId,
					userPosition: position,
					status: {
						requestStatus: status,
						approvals: userInquiryAccepts,
					},
					quantity: isDocumentRequest ? '' : `${quantity} ${quantityType}`,
					dateFrom: isDocumentRequest ? '' : formatDateString(dateFrom, ''),
					dateTo: isDocumentRequest ? '' : formatDateString(dateTo, ''),
					createdAtUtc: formatDateString(createdAtUtc, ''),
					userFullName,
					userDepartmentId: departmentFullName,
					inquiryType: absencesType[inquiryType],
					inquiryTemplateName,
					notes,
					managedByRoleRequest: canManageRequest,
				};
			}
		);
	}, [data]);

	return rows;
}
