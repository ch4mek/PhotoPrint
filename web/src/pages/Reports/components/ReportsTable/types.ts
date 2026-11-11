import { ReportsQueryParams } from '../../types';

import { ChartStackedResponse } from '@/components/ChartStacked/types';

export interface ReportsTableProps {
	reports: ChartStackedResponse | undefined;
	queryParams: ReportsQueryParams;
}
