import { ReportsQueryParams } from '../../types';

import { ChartStackedResponse } from '@/components/ChartStacked/types';

export interface ReportListProps {
	reports: ChartStackedResponse | undefined;
	queryParams: ReportsQueryParams;
}
