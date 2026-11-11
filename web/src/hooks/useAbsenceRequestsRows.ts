import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { absencesType } from '@/constants/absence';
import { checkIsDocumentRequest } from '@/helpers/absences';
import { formatDateString } from '@/helpers/dateToISOString';
import { AbsenceRequest, AbsenceRequestGridRow } from '@/types/absences';

export function useAbsenceRequestsRows(
	data?: AbsenceRequest[]
): AbsenceRequestGridRow[] {
	const { t } = useTranslation('common');
	const rows: AbsenceRequestGridRow[] = useMemo(() => {
		if (!data || !data.length) {
			return [];
		}

		return data.map(
			({
				totalAccountantDays,
				totalHours,
				status,
				userInquiryAccepts,
				dateTo,
				dateFrom,
				inquiryType,
				canCurrentUserApprove,
				canCurrentUserReject,
				...request
			}) => {
				const isDocumentRequest = checkIsDocumentRequest(inquiryType);
				const isDays = totalAccountantDays !== null;
				const count = isDays ? totalAccountantDays! : totalHours ?? 0;

				const quantityString = isDocumentRequest
					? ''
					: t(isDays ? 'days' : 'hours', {
							count,
							keyPrefix: 'absenceRequests',
					  });

				return {
					status: {
						requestStatus: status,
						approvals: userInquiryAccepts,
						canCurrentUserApprove,
						canCurrentUserReject,
					},
					quantity: quantityString,
					dateFrom: isDocumentRequest ? '' : formatDateString(dateFrom, ''),
					dateTo: isDocumentRequest ? '' : formatDateString(dateTo, ''),
					inquiryType: absencesType[inquiryType] || '',
					...request,
				};
			}
		);
	}, [data, t]);

	return rows;
}
