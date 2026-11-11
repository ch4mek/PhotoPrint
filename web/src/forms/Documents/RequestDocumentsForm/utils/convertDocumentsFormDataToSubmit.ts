import { RequestDocumentsValues } from '../types';

import { DateFormat } from '@/constants/date';
import { formatDate } from '@/helpers/formatDate';
import dayjs from '@/lib/dayjs';
import { RequestAddData } from '@/queries/inquiries/types';

export const convertDocumentsFormDataToSubmit = (
	values: RequestDocumentsValues & { userId: number | undefined }
): RequestAddData => {
	const {
		notes,
		inquiryTemplateId,
		agreementTemplateId,
		documentsIds,
		userId,
	} = values;

	if (!inquiryTemplateId || !userId) {
		return {} as RequestAddData;
	}

	const today = dayjs();
	const formattedToday = formatDate(today, DateFormat.YYYY_MM_DD);

	const submitData = {
		notes: notes || '',
		inquiryTemplateId,
		agreementTemplateId: agreementTemplateId || null,
		dateFrom: formattedToday,
		dateTo: formattedToday,
		hours: null,
		files: documentsIds || [],
		userId,
	};

	return submitData;
};
