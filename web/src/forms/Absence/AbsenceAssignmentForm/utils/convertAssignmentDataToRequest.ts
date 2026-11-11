import { intersectById } from '../components/TransferList/utils/intersectById';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import {
	AbsenceAssignmentFormValues,
	TransferListItem,
} from '@/forms/Absence/AbsenceAssignmentForm/types';
import dayjs from '@/lib/dayjs';
import { AbsenceAssignmentValues } from '@/types/absences';

export const convertAssignmentDataToRequest = (
	values: AbsenceAssignmentFormValues,
	oldUnassignedUsers: TransferListItem[],
	oldAssignedUsers: TransferListItem[],
	newUnassignedUsers: TransferListItem[],
	newAssignedUsers: TransferListItem[]
): AbsenceAssignmentValues => ({
	assigneUserIds: intersectById(oldUnassignedUsers, newAssignedUsers).map(
		({ id }) => id
	),
	unassigneUserIds: intersectById(oldAssignedUsers, newUnassignedUsers).map(
		({ id }) => id
	),
	inquiryTemplateId: values.absence?.id || null,
	agreementId: values.agreement?.id || null,
	effectiveFrom: values.effectiveFromDate
		? dayjs(values.effectiveFromDate).format(YYYY_MM_DD_DAYJS_DATE_FORMAT)
		: null,
});
