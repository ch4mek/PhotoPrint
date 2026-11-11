import dayjs from 'dayjs';
import { TFunction } from 'i18next';

import {
	absenceRequestColumns,
	absenceTransactionDescriptionKey,
	orderValuesForAbsenceRequest,
} from '@/constants/absence';
import { SortDirection } from '@/constants/sort';
import { pluralize } from '@/helpers/pluralize';
import { GetCalendarExclusionsItem } from '@/queries/calendar/types';
import {
	AbsenceTrackingUnitType,
	AbsenceTransactionHistory,
	AbsenceTransactionHistoryGridRow,
	AbsenceTransactionHistoryType,
	AbsenceType,
	TrackingUnitType,
	UserInquiryApprover,
	UserInquiryStatusByApprover,
} from '@/types/absences';

import { formatDateString } from './dateToISOString';

const { Day, Hour } = AbsenceTrackingUnitType;
const { status: absenceRequestColumnsStatus } = absenceRequestColumns;
const { dateFrom: orderValuesForAbsenceRequestDateFrom, orderWeight } =
	orderValuesForAbsenceRequest;

function formatAbsenceQuantity(
	quantity: number | null,
	trackingUnitType: string,
	t: TFunction<'common', 'absenceRequests'>
): string {
	if (quantity == null) {
		return '';
	}

	const isDays = trackingUnitType === TrackingUnitType.Days;

	return t(isDays ? 'days' : 'hours', { count: quantity });
}

export const getAbsenceTrackingUnitTypeValue = (
	countOfDays: number | null,
	countOfHours: number | null
) => {
	const type = countOfDays !== null ? Day : Hour;
	const quantity = countOfDays !== null ? countOfDays : countOfHours ?? 0;
	const quantityType = pluralize(type, quantity).toLowerCase();

	return {
		quantityType,
		quantity,
	};
};

export const checkIsDocumentRequest = (inquiryType: string) =>
	inquiryType === AbsenceType.Documents;

const { Rejected, Accepted } = UserInquiryStatusByApprover;

export const getApproverData = (
	status: UserInquiryStatusByApprover,
	acceptUser: UserInquiryApprover,
	acceptedByUser: UserInquiryApprover | null,
	rejectedByUser: UserInquiryApprover | null
) => {
	switch (status) {
		case Accepted:
			return {
				approver: acceptedByUser ?? acceptUser,
				isApproverNotAccepter: acceptedByUser?.id !== acceptUser?.id,
			};
		case Rejected:
			return {
				approver: rejectedByUser ?? acceptUser,
				isApproverNotAccepter: rejectedByUser?.id !== acceptUser?.id,
			};
		default:
			return {
				approver: acceptUser,
				isApproverNotAccepter: false,
			};
	}
};

export const convertDataToHolidays = (
	data: GetCalendarExclusionsItem[] | undefined
) => {
	if (!data) {
		return [];
	}

	return data.map(({ date, name }) => ({
		date: dayjs(date),
		title: name || '',
	}));
};

export function convertToAbsenceTransactionHistoryRows(
	tAbsenceRequests: TFunction<'common', 'absenceRequests'>,
	tAbsenceDescriptions: TFunction<'profile', 'absence'>,
	data?: AbsenceTransactionHistory[]
): AbsenceTransactionHistoryGridRow[] {
	if (!data || !data.length) {
		return [];
	}

	return data.map(
		(
			{
				id,
				transactionDate,
				documents,
				transactionType,
				used,
				accrued,
				remainder,
				trackingUnitType,
				inquiryTemplateName,
				inquiryId,
				inquiryType,
				description,
				userAgreementId,
			},
			index
		) => {
			const isCorrectional =
				userAgreementId === null &&
				transactionType === AbsenceTransactionHistoryType.Accrual;

			const isDocumentRequest = checkIsDocumentRequest(inquiryType);

			return {
				id: id ?? index,
				transactionDate: formatDateString(transactionDate, ''),
				transactionType,
				description: isCorrectional
					? tAbsenceDescriptions('correction')
					: tAbsenceDescriptions(
							absenceTransactionDescriptionKey[transactionType]
					  ),
				correctionDescription: isCorrectional ? description : null,
				remainder: isDocumentRequest
					? ''
					: formatAbsenceQuantity(
							remainder,
							trackingUnitType,
							tAbsenceRequests
					  ),
				files: documents,
				inquiryTemplateName,
				used: isDocumentRequest
					? ''
					: formatAbsenceQuantity(used, trackingUnitType, tAbsenceRequests),
				accrued: isDocumentRequest
					? ''
					: formatAbsenceQuantity(accrued, trackingUnitType, tAbsenceRequests),
				inquiryId,
			};
		}
	);
}

export const getOrderValueForAbsenceRequest = (orderBy?: string) => {
	if (!orderBy) {
		return orderBy;
	}

	const lower = orderBy.toLowerCase();

	if (lower.includes(absenceRequestColumnsStatus.key)) {
		return orderBy
			.replace(new RegExp(absenceRequestColumnsStatus.key, 'i'), orderWeight)
			.concat(`,${orderValuesForAbsenceRequestDateFrom} ${SortDirection.DESC}`);
	}

	return orderBy;
};

export const getCountLabelWithUnit = (
	trackingUnitType: string | undefined,
	firstPart?: string
) =>
	`${firstPart ?? ''} ${
		trackingUnitType ? trackingUnitType.toLowerCase() : ''
	}`;
