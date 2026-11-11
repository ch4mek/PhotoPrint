import { PayslipRow } from '../types';

import { FULL_MONTH_WITHOUT_DATE_TYPE } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { UserPayslips } from '@/types/teamboard';

export const mapPayslipsResponseToRows = (
	items: Array<UserPayslips>
): Array<PayslipRow> =>
	items.map(({ fileId, reportDate, fileName }) => ({
		fileId,
		reportDate: dayjs(reportDate).format(FULL_MONTH_WITHOUT_DATE_TYPE),
		fileName,
	}));
