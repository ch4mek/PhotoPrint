import { JiraImportFormValues } from '../types';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { JiraImportTimesheetsPayload } from '@/types/jiraImport';

export const convertFormValues = ({
	dateFrom,
	dateTo,
	project,
}: JiraImportFormValues): JiraImportTimesheetsPayload => ({
	startDate: dayjs(dateFrom).format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
	endDate: dayjs(dateTo).format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
	projectId: project.id,
});
