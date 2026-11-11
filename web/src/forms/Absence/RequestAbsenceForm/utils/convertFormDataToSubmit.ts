import { RequestDataValues } from '../types';

import { DateFormat } from '@/constants/date';
import { formatDate } from '@/helpers/formatDate';
import { RequestAddData } from '@/queries/inquiries/types';

export const convertFormDataToSubmit = (
	values: RequestDataValues & { userId: number | undefined }
): RequestAddData => {
	const {
		notes,
		absenceName,
		dateFrom,
		dateTo,
		hours,
		documentsIds,
		userId,
		inquiryTemplateId,
		agreementTemplateId,
	} = values;

	if (!absenceName || !dateFrom || !dateTo || !userId) {
		return {} as RequestAddData;
	}

	const submitData = {
		notes,
		inquiryTemplateId,
		agreementTemplateId,
		dateFrom: formatDate(dateFrom, DateFormat.YYYY_MM_DD),
		dateTo: formatDate(dateTo, DateFormat.YYYY_MM_DD),
		hours,
		files: documentsIds || [],
		userId,
	};

	return submitData;
};
