import { absencesType } from '@/constants/absence';
import { DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { RequestsWidgetRow, RequestsWidgetResponse } from '@/types/dashboard';

export const mapRequestToRow = ({
	inquiryId,
	userId,
	userFullName,
	inquiryType,
	inquiryTemplateName,
	dateFrom,
	canUserInfoBeRetrievedByCurrentUser,
}: RequestsWidgetResponse): RequestsWidgetRow => ({
	id: `${userId}-${inquiryId}`,
	inquiryId,
	userId,
	userFullName,
	inquiryType: absencesType[inquiryType],
	inquiryTemplateName,
	dateFrom: dayjs(dateFrom).format(DATE_FORMAT),
	canUserBeRetrieved: canUserInfoBeRetrievedByCurrentUser,
});
